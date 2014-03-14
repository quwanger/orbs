package Orbs

import (
	"OrbsCommandTypes"
	"Packets"
	"fmt"
	"net"
)

type OrbsLobby struct {
	// Config
	Name        string
	LobbyID     int
	PlayerLimit int
	BotsEnabled bool
	Level       int

	debugMode bool

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
				for _, value := range this.connections[ip].OwnedObjects {
					this.racerDeregister(this.connections[ip], value)
					delete(this.objToOwner, value)
					delete(this.racers, value)
				}
				delete(this.connections, ip)
			}
			if len(this.racers) == 0 {
				this.Reset()
			}

		default:
			if this.InGame {
				this.Race.ProcessPacket(sender, reader, command)
			}

		}

		command = reader.ReadInt32()
	}

	this.broadcastPacket()
}

// Command Processors

func (this *OrbsLobby) racerRegister(connection *OrbsConnection, reader *Packets.PacketReader) {
	var racerID float32 = reader.ReadFloat32()
	reader.EmptyRead4Bytes()
	var racerIndex int = len(this.racers)
	var racerRig int = reader.ReadInt32()
	var racerPerk1 int = reader.ReadInt32()
	var racerPerk2 int = reader.ReadInt32()
	var racerName string = reader.Read16String()
	var racerControlType int = reader.ReadInt32()

	var returnPacket = new(Packets.PacketWriter)
	returnPacket.InitPacket()
	if !this.objExists(racerID) { // Success

		this.racers[racerID] = new(OrbsRacer)
		this.racers[racerID].Init(racerID, racerIndex, racerRig, racerPerk1, racerPerk2, racerName, racerControlType, connection)

		// if this.InGame { // Broadcast to everyone else
		// 	this.Race.SpawnRacer(racerID, connection)
		// }

		// Return success command to the sender
		this.objToOwner[racerID] = connection
		connection.AddObject(racerID)
		returnPacket.WriteInt(OrbsCommandTypes.RacerRegisterOK)
		returnPacket.WriteFloat32(racerID)
		fmt.Printf("	S	L:%v Racer %v registered\n", this.LobbyID, racerID)

		// Broadcast to everyone else
		racer := this.racers[racerID]
		var returnPacket = new(Packets.PacketWriter)
		returnPacket.InitPacket()
		returnPacket.WriteInt(OrbsCommandTypes.RacerRegister)
		returnPacket.WriteFloat32(racer.ID)
		returnPacket.WriteInt(racer.Index)
		returnPacket.WriteInt(racer.RigType)
		returnPacket.WriteInt(racer.Perk1Type)
		returnPacket.WriteInt(racer.Perk2Type)
		returnPacket.WriteString(racer.Name)
		returnPacket.WriteInt(racer.ControlType)
		this.writeBroadcastDataExceptSender(returnPacket.GetMinimalData(), connection)

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
}

func (this *OrbsLobby) racerReady(connection *OrbsConnection, reader *Packets.PacketReader) {
	racerID := reader.ReadFloat32()
	isReady := reader.ReadBool()

	this.racers[racerID].isReady = isReady

	// Check if all racers are ready.
}

func (this *OrbsLobby) startRace() {

}

// Initialization functions

func (this *OrbsLobby) Init(index int, name string) {
	this.LobbyID = index
	this.Name = name
	this.PlayerLimit = 6
	this.connections = make(map[string]*OrbsConnection)
	this.objToOwner = make(map[float32]*OrbsConnection)
	this.racers = make(map[float32]*OrbsRacer)

	this.Race = new(OrbsRace)
	this.Race.InitRace(this.connections, this.objToOwner, this.racers)

	this.InGame = true

	this.debugMode = false
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
	} else {
		fmt.Printf("	W	Key %v already exists, but continue anways\n", newConnection.IPAddress)
	}
	newConnection.OutPacket.WriteInt(OrbsCommandTypes.LobbyRegisterOK)
	newConnection.OutPacket.WriteInt(this.LobbyID)

	// Send the registered racers
	for _, value := range this.racers {
		fmt.Printf("	D	Sending racer %v back to %v", value.ID, newConnection.IPAddress)
		this.SendRacer(value, newConnection)
	}

	newConnection.SendPacket()

	fmt.Printf("	S	%v registered to lobby %v\n", newConnection.IPAddress, this.LobbyID)
	return true
}

func (this *OrbsLobby) SendRacer(racer *OrbsRacer, connection *OrbsConnection) {
	var returnPacket = new(Packets.PacketWriter)
	returnPacket.InitPacket()
	returnPacket.WriteInt(OrbsCommandTypes.RacerRegister)
	returnPacket.WriteFloat32(racer.ID)
	// returnPacket.WriteFloat32(22)
	returnPacket.WriteInt(racer.Index)
	// returnPacket.WriteInt(len(this.Racers))
	returnPacket.WriteInt(racer.RigType)
	returnPacket.WriteInt(racer.Perk1Type)
	returnPacket.WriteInt(racer.Perk2Type)
	returnPacket.WriteString(racer.Name)
	returnPacket.WriteInt(racer.ControlType)
	connection.OutPacket.WriteBytes(returnPacket.GetMinimalData())
	// this.writeBroadcastData(returnPacket.GetMinimalData())
}

func (this *OrbsLobby) Reset() {
	fmt.Printf("	St	L:%v Reset\n", this.LobbyID)
	this.InGame = true
	this.connections = make(map[string]*OrbsConnection)
	this.objToOwner = make(map[float32]*OrbsConnection)
	this.racers = make(map[float32]*OrbsRacer)
	this.Race.InitRace(this.connections, this.objToOwner, this.racers)
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

func (this *OrbsLobby) broadcastPacket() {
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
