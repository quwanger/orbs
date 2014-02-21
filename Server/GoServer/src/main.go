package main

import (
	"OrbsCommandTypes"
	"OrbsGo"
	"Packets"
	"UnityGo"
	"errors"
	"fmt"
	"net"
	"time"
)

func chk(err error) {
	if err != nil {
		panic(err)
	}
}

// Constants
const INIT_CONNECTION_TIMEOUT = 5000

// Connections/Objects
var connections map[string]*OrbsGo.OrbsConn // Mapped using the IP string
var objs map[float32]*UnityGo.UnityGameObject
var racers map[float32]*OrbsGo.OrbsRacer
var inConn, outConn *net.UDPConn

func InitServer() {
	println("")
	println("  ▒█████   ██▀███   ▄▄▄▄     ██████ ")
	println(" ▒██▒  ██▒▓██ ▒ ██▒▓█████▄ ▒██    ▒ ")
	println(" ▒██░  ██▒▓██ ░▄█ ▒▒██▒ ▄██░ ▓██▄   ")
	println(" ▒██   ██░▒██▀▀█▄  ▒██░█▀    ▒   ██▒")
	println(" ░ ████▓▒░░██▓ ▒██▒░▓█  ▀█▓▒██████▒▒")
	println(" ░ ▒░▒░▒░ ░ ▒▓ ░▒▓░░▒▓███▀▒▒ ▒▓▒ ▒ ░")
	println("   ░ ▒ ▒░   ░▒ ░ ▒░▒░▒   ░ ░ ░▒  ░ ░")
	println(" ░ ░ ░ ▒    ░░   ░  ░    ░ ░  ░  ░  ")
	println("     ░ ░     ░      ░            ░  ")
	println("                         ░          ")
	println("                                2014")
	println("____________________________________")

	connections = make(map[string]*OrbsGo.OrbsConn)
	objs = make(map[float32]*UnityGo.UnityGameObject)
	racers = make(map[float32]*OrbsGo.OrbsRacer)

	var err error

	inConn, err = net.ListenUDP("udp", &net.UDPAddr{net.IPv4zero, 6666, ""})
	chk(err)

	outConn, err = net.ListenUDP("udp", &net.UDPAddr{IP: net.IPv4zero, Port: 0})
	chk(err)
}

func main() {
	InitServer()

	for {
		packetReader := new(Packets.PacketReader)
		packetReader.Data = make([]byte, 1024)
		_, address, err := inConn.ReadFromUDP(packetReader.Data)

		if err != nil {
			fmt.Println(err)
		}

		go ProcessPacket(address, packetReader)
	}
}

func ProcessPacket(sender *net.UDPAddr, reader *Packets.PacketReader) {
	var command int = int(reader.ReadUInt32())

	var outPacket *Packets.PacketWriter

	if _, keyExists := connections[sender.IP.String()]; keyExists {
		outPacket = connections[sender.IP.String()].OutPacket
	} else {
		outPacket = new(Packets.PacketWriter)
		outPacket.InitPacket()
	}

	for command != OrbsCommandTypes.EndPacket {

		fmt.Printf("Received command: '%v' from %v\n", command, sender)

		switch command {

		// 1
		case OrbsCommandTypes.HandshakeStart:
			// Establish connection
			connection, keyExists := connections[sender.IP.String()]

			if !keyExists {
				fmt.Printf("X	STARTING CONNECTION TO: %v\n\n", sender.IP)
				connection = new(OrbsGo.OrbsConn)
				connection.Init(sender, outPacket, outConn)
				connections[sender.IP.String()] = connection
			}

			StartHandshake(connection)

			go TimeoutConnection(sender.IP.String())

		// 3
		case OrbsCommandTypes.HandshakeComplete:
			HandshakeComplete(sender)

		// 1010
		case OrbsCommandTypes.ObjectRegister:
			RegisterObject(reader, connections[sender.IP.String()])

		// 1004
		case OrbsCommandTypes.ObjectUpdate:
			ObjectUpdate(reader, connections[sender.IP.String()])

		// 2004
		case OrbsCommandTypes.RacerUpdate:
			RacerUpdate(reader, connections[sender.IP.String()])

		// 2010
		case OrbsCommandTypes.RacerRegister:
			RegisterRacer(reader, connections[sender.IP.String()])

		// 8001
		case OrbsCommandTypes.RequestNumRacers:
			// Return number of racers

		// 9999
		case OrbsCommandTypes.CloseConnection:
			CloseConnection(sender.IP.String())
		}

		command = reader.ReadInt32()
	}

	for _, value := range connections {
		value.CommandSender.SendPacket()
		value.CommandSender.ClearData()
	}

	/*
		SendPacket(outPacket.Data[0:outPacket.Size()], returnAddress)
		outPacket.Clear()

		var pos = new(UnityGo.UnityGo.Vector3)
		pos.X, pos.Y, pos.Z = reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32()

		fmt.Printf("%v: %v, %v\n", command, pos, sender)
	*/
}

// case 1: reply with 2
func StartHandshake(connection *OrbsGo.OrbsConn) {
	connection.CommandSender.ReplyInit()
}

func HandshakeComplete(sender *net.UDPAddr) {
	connections[sender.IP.String()].IsEstablished = true
	fmt.Printf("X	Connection to %v established\n\n", sender.IP.String())

	var broadcastPacket = new(Packets.PacketWriter)
	broadcastPacket.InitPacket()
	for _, value := range racers {
		broadcastPacket.WriteUInt32(OrbsCommandTypes.RacerRegister)
		broadcastPacket.WriteFloat32(value.Id)
	}

	connections[sender.IP.String()].CommandSender.SendBytes(broadcastPacket.GetMinimalData())
}

// case 4:	Save position, broadcast to other clients.
// 			Reply 5 if obj not registered, 6 if someone else registered
func ObjectUpdate(reader *Packets.PacketReader, connection *OrbsGo.OrbsConn) error {
	var id = reader.ReadFloat32()
	obj, keyExists := objs[id]

	if !keyExists || obj.Owner != connection.Address {
		reader.EmptyRead12Bytes()
		reader.EmptyRead12Bytes()
		reader.EmptyRead12Bytes()

		if !keyExists {
			connection.CommandSender.ObjectNotRegistered(id)
			return errors.New("OBJECT NOT REGISTERED")
		} else if obj.Owner != connection.Address {
			connection.CommandSender.ObjectAlreadyRegistered(id)
			return errors.New("OBJECT REGISTERED TO DIFFERENT OWNER")
		}
	}

	obj.LastPos.Set(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32())
	obj.LastRot.Set(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32())
	obj.LastSpeed.Set(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32())

	fmt.Printf("*	Update Obj: '%v' currently at %v\n\n", id, obj.LastPos)

	for _, value := range connections {
		value.CommandSender.ObjectUpdate(ObjectUpdateToBytes(obj))
	}

	return nil
}

func ObjectUpdateToBytes(obj *UnityGo.UnityGameObject) []byte {
	var broadcastPacket = new(Packets.PacketWriter)
	broadcastPacket.InitPacket()
	broadcastPacket.WriteUInt32(OrbsCommandTypes.ObjectUpdate)
	broadcastPacket.WriteFloat32(obj.Id)
	broadcastPacket.WriteVector3(obj.LastPos.X, obj.LastPos.Y, obj.LastPos.Z)
	broadcastPacket.WriteVector3(obj.LastRot.X, obj.LastRot.Y, obj.LastRot.Z)
	broadcastPacket.WriteVector3(obj.LastSpeed.X, obj.LastSpeed.Y, obj.LastSpeed.Z)

	return broadcastPacket.GetMinimalData()
}

// case 10:	reply with 11 to acknowledge, 6 if already registered
func RegisterObject(reader *Packets.PacketReader, connection *OrbsGo.OrbsConn) {
	var id = reader.ReadFloat32()
	_, keyExists := objs[id]

	if keyExists {
		connection.CommandSender.ObjectAlreadyRegistered(id)
	} else {
		objs[id] = new(UnityGo.UnityGameObject)
		objs[id].Init(id, connection.Address)

		connection.AddObj(id)

		connection.CommandSender.ObjectRegisteredSuccess(id)
	}
}

func RegisterRacer(reader *Packets.PacketReader, connection *OrbsGo.OrbsConn) {
	var id = reader.ReadFloat32()
	_, keyExists := racers[id]

	if keyExists {
		connection.CommandSender.RacerAlreadyRegistered(id)
	} else {
		racers[id] = new(OrbsGo.OrbsRacer)
		racers[id].Init(id, connection.Address)

		connection.AddRacer(id)

		connection.CommandSender.RacerRegisteredSuccess(id)

		for _, value := range connections {
			var broadcastPacket = new(Packets.PacketWriter)
			broadcastPacket.InitPacket()
			broadcastPacket.WriteUInt32(OrbsCommandTypes.RacerRegister)
			broadcastPacket.WriteFloat32(id)

			if value != connection {
				value.CommandSender.ObjectUpdate(broadcastPacket.GetMinimalData())
			}
		}
	}
}

func RacerUpdate(reader *Packets.PacketReader, connection *OrbsGo.OrbsConn) error {
	var id = reader.ReadFloat32()
	racer, keyExists := racers[id]

	if !keyExists || racer.Owner != connection.Address {
		reader.EmptyRead12Bytes()
		reader.EmptyRead12Bytes()
		reader.EmptyRead12Bytes()
		reader.EmptyRead4Bytes()
		reader.EmptyRead4Bytes()

		if !keyExists {
			connection.CommandSender.RacerNotRegistered(id)
			return errors.New("OBJECT NOT REGISTERED")
		} else if racer.Owner != connection.Address {
			connection.CommandSender.RacerAlreadyRegistered(id)
			return errors.New("OBJECT REGISTERED TO DIFFERENT OWNER")
		}
	}

	racer.LastPos.Set(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32())
	racer.LastRot.Set(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32())
	racer.LastSpeed.Set(reader.ReadFloat32(), reader.ReadFloat32(), reader.ReadFloat32())
	racer.VInput = reader.ReadFloat32()
	racer.HInput = reader.ReadFloat32()
	racer.PowerUpType = reader.ReadInt32()
	racer.PowerUpTier = reader.ReadInt32()

	fmt.Printf("*	Update Racer: '%v' currently at %v. V: %v, H:%v\n\n", id, racer.LastPos, racer.VInput, racer.HInput)

	for _, value := range connections {
		value.CommandSender.ObjectUpdate(RacerUpdateToBytes(racer))
	}

	return nil
}

func RacerUpdateToBytes(racer *OrbsGo.OrbsRacer) []byte {
	var broadcastPacket = new(Packets.PacketWriter)
	broadcastPacket.InitPacket()
	broadcastPacket.WriteUInt32(OrbsCommandTypes.RacerUpdate)
	broadcastPacket.WriteFloat32(racer.Id)
	broadcastPacket.WriteVector3(racer.LastPos.X, racer.LastPos.Y, racer.LastPos.Z)
	broadcastPacket.WriteVector3(racer.LastRot.X, racer.LastRot.Y, racer.LastRot.Z)
	broadcastPacket.WriteVector3(racer.LastSpeed.X, racer.LastSpeed.Y, racer.LastSpeed.Z)
	broadcastPacket.WriteFloat32(racer.VInput)
	broadcastPacket.WriteFloat32(racer.HInput)
	broadcastPacket.WriteInt(racer.PowerUpType)
	broadcastPacket.WriteInt(racer.PowerUpTier)

	return broadcastPacket.GetMinimalData()
}

func CloseConnection(IpAddress string) {
	if _, ok := connections[IpAddress]; !ok {
		return
	}

	fmt.Printf("!	%v has shut down connection\n\n", IpAddress)

	for _, value := range connections[IpAddress].OwnedObjects {
		delete(objs, value)
	}

	for _, value := range connections[IpAddress].OwnedRacers {
		delete(racers, value)
	}

	delete(connections, IpAddress)
}

func TimeoutConnection(ipKey string) {
	time.Sleep(INIT_CONNECTION_TIMEOUT * time.Millisecond)

	if _, exists := connections[ipKey]; exists && !connections[ipKey].IsEstablished {
		fmt.Printf("Initial connection from %v has timed out\n\n", ipKey)
		delete(connections, ipKey)
	}
}
