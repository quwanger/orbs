package Orbs

import (
	"OrbsCommandTypes"
	"Packets"
	"net"
)

type OrbsRace struct {
	Lobby *OrbsLobby

	Connections map[string]*OrbsConnection
	ObjToOwner  map[float32]*OrbsConnection
	Racers      map[float32]*OrbsRacer
}

func (this *OrbsRace) InitRace(connections map[string]*OrbsConnection, objToOwner map[float32]*OrbsConnection, racers map[float32]*OrbsRacer) {
	this.Connections = connections
	this.ObjToOwner = objToOwner
	this.Racers = racers
}

func (this *OrbsRace) SpawnRacer(racerID float32, connection *OrbsConnection) {
	racer := this.Racers[racerID]

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
	this.writeBroadcastDataExceptSender(returnPacket.GetMinimalData(), connection)
	// this.writeBroadcastData(returnPacket.GetMinimalData())

}

func (this *OrbsRace) ProcessPacket(sender *net.UDPAddr, reader *Packets.PacketReader, firstCommand int) {
	var command int = firstCommand
	var ip string = sender.IP.String()

	for command != OrbsCommandTypes.EndPacket {
		switch command {

		// 2004
		case OrbsCommandTypes.RacerUpdate:
			this.racerUpdate(this.Connections[ip], reader)

		// 5001
		case OrbsCommandTypes.PowerupRegister:
			this.powerupDeploy(this.Connections[ip], reader)

		// 5002
		case OrbsCommandTypes.PowerupStaticRegister:
			println(">	Powerup Received from ", ip)
			this.staticPowerupDeploy(this.Connections[ip], reader)

		// 5004
		case OrbsCommandTypes.PowerupUpdate:
			this.powerupUpdate(this.Connections[ip], reader)
		}

		command = reader.ReadInt32()
	}

	reader.Rewind4Bytes()
}

// Command Processors

func (this *OrbsRace) powerupUpdate(connection *OrbsConnection, reader *Packets.PacketReader) {
	powerupID := reader.ReadFloat32()

	println("#	Powerup Update Received ", powerupID)

	// Pull all the packet data
	position, rotation, speed := new(Vector3), new(Vector3), new(Vector3)

	position.Set(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32())
	rotation.Set(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32())
	speed.Set(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32())

	var broadcastPacket = new(Packets.PacketWriter)
	broadcastPacket.InitPacket()
	broadcastPacket.WriteUInt32(OrbsCommandTypes.PowerupUpdate)
	broadcastPacket.WriteFloat32(powerupID)
	broadcastPacket.WriteVector3(position.X, position.Y, position.Z)
	broadcastPacket.WriteVector3(rotation.X, rotation.Y, rotation.Z)
	broadcastPacket.WriteVector3(speed.X, speed.Y, speed.Z)

	this.writeBroadcastData(broadcastPacket.GetMinimalData())
}

func (this *OrbsRace) powerupDeploy(connection *OrbsConnection, reader *Packets.PacketReader) {
	racerID := reader.ReadFloat32()

	if owner, exists := this.ObjToOwner[racerID]; exists && connection.IPAddress == owner.IPAddress {
		powerupID := reader.ReadFloat32()
		powerupType := reader.ReadInt32()

		println("#	Powerup Received ", powerupID, powerupType)

		var broadcastPacket = new(Packets.PacketWriter)
		broadcastPacket.InitPacket()
		broadcastPacket.WriteInt(OrbsCommandTypes.PowerupRegister)
		broadcastPacket.WriteFloat32(racerID)
		broadcastPacket.WriteFloat32(powerupID)
		broadcastPacket.WriteInt(powerupType)
		this.writeBroadcastDataExceptSender(broadcastPacket.GetMinimalData(), connection)

		var returnPacket = new(Packets.PacketWriter)
		returnPacket.InitPacket()
		returnPacket.WriteInt(OrbsCommandTypes.PowerupRegisterOK)
		returnPacket.WriteFloat32(powerupID)
		connection.WriteData(returnPacket.GetMinimalData())
	} else {
		reader.EmptyReadBytes(Packets.SIZEOF_FLOAT32 * 2)
		// Just leave it.
	}

}

func (this *OrbsRace) staticPowerupDeploy(connection *OrbsConnection, reader *Packets.PacketReader) {
	racerID := reader.ReadFloat32()
	powerupType, powerupTier := reader.ReadInt32(), reader.ReadFloat32()
	// println("#	Powerup Received ", powerupType, powerupTier)

	if owner, exists := this.ObjToOwner[racerID]; exists && connection.IPAddress == owner.IPAddress {

		var broadcastPacket = new(Packets.PacketWriter)
		broadcastPacket.InitPacket()
		broadcastPacket.WriteInt(OrbsCommandTypes.PowerupStaticRegister)
		broadcastPacket.WriteFloat32(racerID)
		broadcastPacket.WriteInt(powerupType)
		broadcastPacket.WriteFloat32(powerupTier)
		this.writeBroadcastDataExceptSender(broadcastPacket.GetMinimalData(), connection)
	} else {
		// Just leave it.
	}
}

func (this *OrbsRace) racerUpdate(connection *OrbsConnection, reader *Packets.PacketReader) {
	racerID := reader.ReadFloat32()

	if owner, exists := this.ObjToOwner[racerID]; exists && connection.IPAddress == owner.IPAddress {

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
func (this *OrbsRace) writeBroadcastData(data []byte) {
	for _, value := range this.Connections {
		value.WriteData(data)
	}
}

func (this *OrbsRace) writeBroadcastDataExceptSender(data []byte, sender *OrbsConnection) {
	for _, value := range this.Connections {
		if value.IPAddress != sender.IPAddress {
			value.WriteData(data)
		}
	}
}

func (this *OrbsRace) connectionExists(ipAddress string) bool {
	_, exists := this.Connections[ipAddress]
	return exists
}

func (this *OrbsRace) objExists(objID float32) bool {
	_, exists := this.ObjToOwner[objID]
	return exists
}
