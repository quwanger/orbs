package main

import (
	"OrbsCommands"
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
var connections map[string]*UnityGo.UnityConn // Mapped using the IP string
var objs map[float32]*UnityGo.UnityGameObject
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

	connections = make(map[string]*UnityGo.UnityConn)
	objs = make(map[float32]*UnityGo.UnityGameObject)

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

	for command != OrbsCommands.EndPacket {

		fmt.Printf("Received command: '%v' from %v\n", command, sender)

		switch command {

		// First Connection
		case OrbsCommands.HandshakeStart:
			// Establish connection
			connection, keyExists := connections[sender.IP.String()]

			if !keyExists {
				fmt.Printf("X	STARTING CONNECTION TO: %v\n\n", sender.IP)
				connection = new(UnityGo.UnityConn)
				connection.Init(sender, outPacket, outConn)
				connections[sender.IP.String()] = connection
			}

			ReturnFire(connection)

			go TimeoutConnection(sender.IP.String())

		// Handshake complete
		case OrbsCommands.HandshakeComplete:
			connections[sender.IP.String()].IsEstablished = true
			fmt.Printf("X	Connection to %v established\n\n", sender.IP.String())

		// UnityGo.Unity object Update
		case OrbsCommands.ObjectUpdate:
			ObjectUpdate(reader, connections[sender.IP.String()], outPacket)

		// UnityGo.Unity Object Registration
		case OrbsCommands.ObjectRegister:
			RegisterObject(reader, connections[sender.IP.String()], outPacket)

		// Close connection
		case OrbsCommands.CloseConnection:
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
func ReturnFire(connection *UnityGo.UnityConn) {
	connection.CommandSender.ReplyInit()
}

// case 4:	Save position, broadcast to other clients.
// 			Reply 5 if obj not registered, 6 if someone else registered
func ObjectUpdate(reader *Packets.PacketReader, connection *UnityGo.UnityConn, outPacket *Packets.PacketWriter) error {
	var id = reader.ReadFloat32()
	obj, keyExists := objs[id]

	if !keyExists {
		connection.CommandSender.ObjectNotRegistered(id)

		reader.EmptyRead12Bytes()
		reader.EmptyRead12Bytes()
		reader.EmptyRead12Bytes()

		return errors.New("OBJECT NOT REGISTERED")
	} else if obj.Owner != connection.Address {
		connection.CommandSender.ObjectAlreadyRegistered(id)

		reader.EmptyRead12Bytes()
		reader.EmptyRead12Bytes()
		reader.EmptyRead12Bytes()

		return errors.New("OBJECT REGISTERED TO DIFFERENT OWNER")
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
	broadcastPacket.WriteUInt32(OrbsCommands.ObjectUpdate)
	broadcastPacket.WriteFloat32(obj.Id)
	broadcastPacket.WriteVector3(obj.LastPos.X, obj.LastPos.Y, obj.LastPos.Z)
	broadcastPacket.WriteVector3(obj.LastRot.X, obj.LastRot.Y, obj.LastRot.Z)
	broadcastPacket.WriteVector3(obj.LastSpeed.X, obj.LastSpeed.Y, obj.LastSpeed.Z)

	return broadcastPacket.GetMinimalData()
}

// case 10:	reply with 11 to acknowledge, 6 if already registered
func RegisterObject(reader *Packets.PacketReader, connection *UnityGo.UnityConn, outPacket *Packets.PacketWriter) {
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

func CloseConnection(IpAddress string) {
	if _, ok := connections[IpAddress]; !ok {
		return
	}

	fmt.Printf("!	%v has shut down connection\n\n", IpAddress)

	for _, value := range connections[IpAddress].OwnedObjects {
		delete(objs, value)
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
