package Orbs

import (
	"OrbsCommandTypes"
	"Packets"
	"fmt"
	"math/rand"
	"net"
	"time"
)

var NumLevels int = 5

type OrbsLobby struct {
	// Config
	Name        string
	LobbyID     int
	PlayerLimit int
	BotsEnabled bool
	Level       int

	debugMode bool

	// Countdown
	CountdownTimeLeft  float32
	CountdownLength    float32
	CountdownStart     time.Time
	isCountdownStarted bool

	// Status
	InGame bool

	// Objects
	connections map[string]*OrbsConnection
	objToOwner  map[float32]*OrbsConnection
	racers      map[float32]*OrbsRacer

	// Race
	Race *OrbsRace
}

// Packet Processor
func (this *OrbsLobby) ProcessPacket(sender *net.UDPAddr, reader *Packets.PacketReader, firstCommand int) {
	if this.debugMode {
		fmt.Printf("\n	L:%v Receiving PACKET from %v\n", this.LobbyID, sender)
	}

	var command int = firstCommand
	var ip string = sender.IP.String()

	for command != OrbsCommandTypes.EndPacket {
		if this.debugMode {
			fmt.Printf("	L:%v COMMAND: '%v' from %v\n", this.LobbyID, command, sender)
		}

		switch command {

		// 2010
		case OrbsCommandTypes.RacerRegister:
			this.racerRegister(this.connections[ip], reader)

		// 2020
		case OrbsCommandTypes.RacerReadyState:
			this.racerReady(this.connections[ip], reader)

		// 9999
		case OrbsCommandTypes.CloseConnection:
			if this.connectionExists(ip) {
				fmt.Printf("	%v disconnected.\n", ip)

				if len(this.connections) == 1 {
					this.Reset()
				} else {
					for _, value := range this.connections[ip].OwnedObjects {

						if _, exists := this.racers[value]; exists {
							// Remove racers
							this.racerDeregister(this.connections[ip], value)
						} else if _, exists := this.Race.PowerupPlatforms[value]; exists {
							// Get rid of the platforms here
							this.Race.powerupPlatformDeregister(value, this.connections[ip])
						}
					}
					delete(this.connections, ip)

					if len(this.connections) <= 1 {
						this.stopCountdown()
					}
				}

			}

		default:
			if this.InGame {
				this.Race.ProcessPacket(sender, reader, command)
			}

		}

		command = reader.ReadInt32()
	}

	this.BroadcastPacket()
}

// Command Processors

func (this *OrbsLobby) racerRegister(connection *OrbsConnection, reader *Packets.PacketReader) {
	if !this.connectionExists(connection.IPAddress) {
		return
	}

	var racerID float32 = reader.ReadFloat32()
	reader.EmptyRead4Bytes() // Ignore the racer index
	var racerIndex int = len(this.racers)
	var racerRig int = reader.ReadInt32()
	var racerPerk1 int = reader.ReadInt32()
	var racerPerk2 int = reader.ReadInt32()
	var racerCharacter int = reader.ReadInt32()
	var racerName string = reader.Read16String()
	var racerControlType int = reader.ReadInt32()

	var returnPacket = new(Packets.PacketWriter)
	returnPacket.InitPacket()

	if !this.objExists(racerID) { // Success
		// Add the racer to the connection
		connection.AddObject(racerID)

		racer := new(OrbsRacer)
		this.racers[racerID] = racer
		racer.Init(racerID, racerIndex, racerRig, racerPerk1, racerPerk2, racerCharacter, racerName, racerControlType, connection)

		// Return success command to the sender
		this.objToOwner[racerID] = connection
		returnPacket.WriteInt(OrbsCommandTypes.RacerRegisterOK)
		returnPacket.WriteFloat32(racerID)
		returnPacket.WriteInt(racer.Index)
		fmt.Printf("	S	L:%v Racer %v '%v' registered\n", this.LobbyID, racerID, racerName)

		// Broadcast to everyone else
		this.writeBroadcastDataExceptSender(this.racerRegisterBytes(racer), connection)
		this.BroadcastPacket()

	} else if this.objToOwner[racerID].IPAddress != connection.IPAddress { // Racer already registered
		returnPacket.WriteInt(OrbsCommandTypes.RacerAlreadyRegistered)
		returnPacket.WriteFloat32(racerID)
		fmt.Printf("	X	L:%v Racer %v already registered\n", this.LobbyID, racerID)
	}

	connection.WriteData(returnPacket.GetMinimalData())
}

func (this *OrbsLobby) racerDeregister(connection *OrbsConnection, racerID float32) {
	if this.debugMode {
		fmt.Printf("	S	L:%v Racer %v deregistered\n", this.LobbyID, racerID)
	}

	packet := new(Packets.PacketWriter)
	packet.InitPacket()

	packet.WriteInt(OrbsCommandTypes.RacerDeregister)
	packet.WriteFloat32(racerID)

	this.writeBroadcastDataExceptSender(packet.GetMinimalData(), connection)

	delete(this.racers, racerID)
	delete(this.objToOwner, racerID)
}

func (this *OrbsLobby) racerReady(connection *OrbsConnection, reader *Packets.PacketReader) {
	racerID := reader.ReadFloat32()
	isReady := reader.ReadBool()

	this.racers[racerID].isReady = isReady

	// Check if all racers are ready.
}

// Lobby race start/countdown

func (this *OrbsLobby) startRace() {
	var packet = new(Packets.PacketWriter)
	packet.InitPacket()
	packet.WriteInt(OrbsCommandTypes.LobbyStartGame)
	packet.WriteInt(this.Level)

	this.writeBroadcastData(packet.GetMinimalData())
	this.BroadcastPacket()
}

func (this *OrbsLobby) endRace() {

}

func (this *OrbsLobby) startCountdown() {
	this.isCountdownStarted = true
	this.CountdownStart = time.Now()

	go this.runCountdown()
}

func (this *OrbsLobby) runCountdown() {
	var timeSinceStart time.Duration = time.Now().Sub(this.CountdownStart)
	var CountdownTimeLeft float32 = this.CountdownLength - float32(timeSinceStart.Seconds())

	if CountdownTimeLeft <= 0 && this.isCountdownStarted {
		this.isCountdownStarted = false
		this.startRace()
	} else if this.isCountdownStarted {
		var packet = new(Packets.PacketWriter)
		packet.InitPacket()
		packet.WriteInt(OrbsCommandTypes.LobbyCountdownUpdate)
		packet.WriteFloat32(CountdownTimeLeft)

		this.writeBroadcastData(packet.GetMinimalData())
		this.BroadcastPacket()
		time.Sleep(500 * time.Millisecond)
		this.runCountdown()
	}
}

func (this *OrbsLobby) stopCountdown() {
	this.isCountdownStarted = false
	var packet = new(Packets.PacketWriter)
	packet.InitPacket()
	packet.WriteInt(OrbsCommandTypes.LobbyStopCountdown)

	this.writeBroadcastData(packet.GetMinimalData())
	this.BroadcastPacket()
}

// Initialization functions

func (this *OrbsLobby) Init(index int, name string) {

	this.LobbyID = index
	this.Name = name
	this.PlayerLimit = 6
	this.CountdownLength = 30
	this.Level = rand.Int() % NumLevels
	this.connections = make(map[string]*OrbsConnection)
	this.objToOwner = make(map[float32]*OrbsConnection)
	this.racers = make(map[float32]*OrbsRacer)

	this.Race = new(OrbsRace)
	this.Race.InitRace(this.connections, this.objToOwner, this.racers, this)

	this.InGame = true

	this.debugMode = false

	if this.debugMode {
		println("Initialized", this.Name, "on level", this.Level)
	}
}

func (this *OrbsLobby) AddConnection(newConnection *OrbsConnection) bool {
	// Check player limit
	if len(this.connections) >= this.PlayerLimit {
		newConnection.OutPacket.WriteInt(OrbsCommandTypes.LobbyRegisterFail)
		newConnection.OutPacket.WriteInt(this.LobbyID)
		newConnection.SendPacket()
		return false
	}

	// Add connection if connection not already established, send back success packet
	if !this.connectionExists(newConnection.IPAddress) {
		this.connections[newConnection.IPAddress] = newConnection

		// Check countdown
		if len(this.connections) == 1 {
			this.startCountdown()
		}

	} else {
		fmt.Printf("	W	Key %v already exists, but continue anways\n", newConnection.IPAddress)
	}
	newConnection.OutPacket.WriteInt(OrbsCommandTypes.LobbyRegisterOK)
	newConnection.OutPacket.WriteInt(this.LobbyID)

	// Send the registered racers to the new connection
	for _, value := range this.racers {
		fmt.Printf("	D	Sending racer %v back to %v\n", value.ID, newConnection.IPAddress)
		this.SendRacer(value, newConnection)
	}
	newConnection.SendPacket()

	fmt.Printf("	S	%v registered to lobby %v\n", newConnection.IPAddress, this.LobbyID)
	return true
}

func (this *OrbsLobby) racerRegisterBytes(racer *OrbsRacer) []byte {
	var returnPacket = new(Packets.PacketWriter)
	returnPacket.InitPacket()
	returnPacket.WriteInt(OrbsCommandTypes.RacerRegister)
	returnPacket.WriteFloat32(racer.ID)
	returnPacket.WriteInt(racer.Index)
	returnPacket.WriteInt(racer.RigType)
	returnPacket.WriteInt(racer.PerkAType)
	returnPacket.WriteInt(racer.PerkBType)
	returnPacket.WriteInt(racer.CharacterType)
	returnPacket.WriteString(racer.Name)
	returnPacket.WriteInt(racer.ControlType)
	return returnPacket.GetMinimalData()
}

func (this *OrbsLobby) SendRacer(racer *OrbsRacer, connection *OrbsConnection) {
	connection.OutPacket.WriteBytes(this.racerRegisterBytes(racer))
}

func (this *OrbsLobby) Reset() {
	fmt.Printf("	St	L:%v Reset\n", this.LobbyID)
	this.InGame = true
	this.connections = make(map[string]*OrbsConnection)
	this.objToOwner = make(map[float32]*OrbsConnection)
	this.racers = make(map[float32]*OrbsRacer)
	this.Race.InitRace(this.connections, this.objToOwner, this.racers, this)
	this.Level = rand.Int() % NumLevels
}

// Helpers

func (this *OrbsLobby) writeBroadcastData(data []byte) {
	for _, value := range this.connections {
		value.WriteData(data)
	}
}

func (this *OrbsLobby) writeBroadcastDataExceptSender(data []byte, sender *OrbsConnection) {
	for _, value := range this.connections {
		if value.IPAddress != sender.IPAddress {
			value.WriteData(data)
		}
	}
}

func (this *OrbsLobby) BroadcastPacket() {
	for _, value := range this.connections {
		value.SendPacket()
	}
}

func (this *OrbsLobby) connectionExists(ipAddress string) bool {
	_, exists := this.connections[ipAddress]
	return exists
}

func (this *OrbsLobby) objExists(objID float32) bool {
	_, exists := this.objToOwner[objID]
	return exists
}

func (this *OrbsLobby) GetNumRacers() int {
	return len(this.racers)
}
