package main

import (
	"OrbsCommandTypes"
	"Packets"
	"fmt"
	"net"
)

var inConn, outConn *net.UDPConn

var outPacket *Packets.PacketWriter

var serverAddress *net.UDPAddr

var DebugMode = true

func main() {
	println("")
	println("▄▄▄█████▓▓█████   ██████ ▄▄▄█████▓")
	println("▓  ██▒ ▓▒▓█   ▀ ▒██    ▒ ▓  ██▒ ▓▒")
	println("▒ ▓██░ ▒░▒███   ░ ▓██▄   ▒ ▓██░ ▒░")
	println("░ ▓██▓ ░ ▒▓█  ▄   ▒   ██▒░ ▓██▓ ░ ")
	println("  ▒██▒ ░ ░▒████▒▒██████▒▒  ▒██▒ ░ ")
	println("  ▒ ░░   ░░ ▒░ ░▒ ▒▓▒ ▒ ░  ▒ ░░   ")
	println("    ░     ░ ░  ░░ ░▒  ░ ░    ░    ")
	println("  ░         ░   ░  ░  ░    ░      ")
	println("            ░  ░      ░           ")
	println(" Orbs Tester v1.0             2014")
	println("__________________________________")

	serverAddress = &net.UDPAddr{net.IPv4(127, 0, 0, 1), 6666, ""}
	outPacket = new(Packets.PacketWriter)
	outPacket.InitPacket()

	var err error

	inConn, err = net.ListenUDP("udp", &net.UDPAddr{net.IPv4zero, 6969, ""})
	chk(err)

	outConn, err = net.ListenUDP("udp", &net.UDPAddr{IP: net.IPv4zero, Port: 0})
	chk(err)

	// outPacket.WriteInt(OrbsCommandTypes.LobbyRegister)
	// outPacket.WriteInt(1)

	// outPacket.WriteInt(OrbsCommandTypes.RacerRegister)
	// outPacket.WriteFloat32(123.123)

	outPacket.WriteInt(OrbsCommandTypes.RacerUpdate)
	outPacket.WriteVector3(1, 1, 1)
	outPacket.WriteVector3(1, 1, 1)
	outPacket.WriteVector3(1, 1, 1)
	outPacket.WriteFloat32(123.123)
	outPacket.WriteFloat32(123.123)

	SendPacket()
}

func SendPacket() {
	if outPacket.Size() == 0 {
		return
	}

	n, err := outConn.WriteToUDP(outPacket.Data[0:outPacket.Size()], serverAddress)

	if DebugMode {
		fmt.Printf("<-- Sending %v bytes to %v\n\n", n, serverAddress)
	}

	if err != nil {
		panic(err)
	}
}

func chk(err error) {
	if err != nil {
		panic(err)
	}
}
