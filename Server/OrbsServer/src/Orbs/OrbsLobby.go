package Orbs

import (
	"OrbsCommandTypes"
	"Packets"
	"fmt"
	"net"
)

type OrbsLobby struct {
	Name        string
	LobbyID     int
	PlayerLimit int

	connections map[string]*OrbsConnection

	objToOwner map[float32]*OrbsConnection
}

// Packet Processor
func (this *OrbsLobby) ProcessPacket(sender *net.UDPAddr, reader *Packets.PacketReader, firstCommand int) {
	fmt.Printf("\n	L:%v Receiving PACKET from %v\n", this.LobbyID, sender)

	var command int = firstCommand
	var ip string = sender.IP.String()

	for command != OrbsCommandTypes.EndPacket {
		fmt.Printf("	L:%v COMMAND: '%v' from %v\n", this.LobbyID, command, sender)

		switch command {

		// 2010
		case OrbsCommandTypes.RacerRegister:
			this.racerRegister(this.connections[ip], reader)

		// 2004
		case OrbsCommandTypes.RacerUpdate:
			this.racerUpdate(this.connections[ip], reader)

		// 9999
		case OrbsCommandTypes.CloseConnection:
			if this.connectionExists(ip) {
				for _, value := range this.connections[ip].OwnedObjects {
					delete(this.objToOwner, value)
				}
				delete(this.connections, ip)
			}
		}

		command = reader.ReadInt32()
	}

	this.broadcastPacket()
}

// Command Processors

func (this *OrbsLobby) racerRegister(connection *OrbsConnection, reader *Packets.PacketReader) {
	racerID := reader.ReadFloat32()
	var returnPacket = new(Packets.PacketWriter)
	returnPacket.InitPacket()
	if !this.objExists(racerID) { // Success
		this.objToOwner[racerID] = connection
		returnPacket.WriteInt(OrbsCommandTypes.RacerRegisterOK)
		returnPacket.WriteFloat32(racerID)
		fmt.Printf("	S	L:%v Racer %v registered\n", this.LobbyID, racerID)
	} else if this.objToOwner[racerID].IPAddress != connection.IPAddress { // Racer already registered
		returnPacket.WriteInt(OrbsCommandTypes.RacerAlreadyRegistered)
		returnPacket.WriteFloat32(racerID)
		fmt.Printf("	X	L:%v Racer %v already registered\n", this.LobbyID, racerID)
	}
	connection.WriteData(returnPacket.GetMinimalData())
}

func (this *OrbsLobby) racerUpdate(connection *OrbsConnection, reader *Packets.PacketReader) {
	racerID := reader.ReadFloat32()

	if owner, exists := this.objToOwner[racerID]; exists && connection.IPAddress == owner.IPAddress {

		// Pull all the packet data
		position, rotation, speed := new(Vector3), new(Vector3), new(Vector3)
		var vInput, hInput float32

		position.Set(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32())
		rotation.Set(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32())
		speed.Set(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32())

		vInput, hInput = reader.ReadFloat32(), reader.ReadFloat32()

		// broadcast the data
		var broadcastPacket = new(Packets.PacketWriter)
		broadcastPacket.InitPacket()
		broadcastPacket.WriteUInt32(OrbsCommandTypes.RacerUpdate)
		broadcastPacket.WriteFloat32(racerID)
		broadcastPacket.WriteVector3(position.X, position.Y, position.Z)
		broadcastPacket.WriteVector3(rotation.X, rotation.Y, rotation.Z)
		broadcastPacket.WriteVector3(speed.X, speed.Y, speed.Z)
		broadcastPacket.WriteFloat32(vInput)
		broadcastPacket.WriteFloat32(hInput)

		this.writeBroadcastData(broadcastPacket.GetMinimalData())

	} else {
		// Read the necessary number of bytes
		reader.EmptyReadBytes(Packets.SIZEOF_FLOAT32 * 3 * 3) // Vector3
		reader.EmptyReadBytes(Packets.SIZEOF_FLOAT32 * 2)     // Inputs

		var returnPacket = new(Packets.PacketWriter)
		returnPacket.InitPacket()
		if !exists { // Racer is not registered
			returnPacket.WriteInt(OrbsCommandTypes.RacerIsNotRegistered)
		} else if connection != owner { // Someone else owns the racer
			returnPacket.WriteInt(OrbsCommandTypes.RacerAlreadyRegistered)
		}
		returnPacket.WriteFloat32(racerID)

		connection.WriteData(returnPacket.GetMinimalData())
	}
}

// Helpers

func (this *OrbsLobby) writeBroadcastData(data []byte) {
	for _, value := range this.connections {
		value.WriteData(data)
	}
}

func (this *OrbsLobby) broadcastPacket() {
	for _, value := range this.connections {
		value.SendPacket()
	}
}

// Initialization functions

func (this *OrbsLobby) Init(index int, name string) {
	this.LobbyID = index
	this.Name = name
	this.PlayerLimit = 6
	this.connections = make(map[string]*OrbsConnection)
	this.objToOwner = make(map[float32]*OrbsConnection)
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
	newConnection.SendPacket()

	fmt.Printf("	S	%v registered to lobby %v\n", newConnection.IPAddress, this.LobbyID)
	return true
}

func (this *OrbsLobby) connectionExists(ipAddress string) bool {
	_, exists := this.connections[ipAddress]
	return exists
}

func (this *OrbsLobby) objExists(objID float32) bool {
	_, exists := this.objToOwner[objID]
	return exists
}
