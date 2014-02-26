package main

import (
	"Orbs"
	"OrbsCommandTypes"
	"Packets"
	"fmt"
	"net"
	"strconv"
)

var DebugMode bool
var inConn, outConn *net.UDPConn
var Lobbies []*Orbs.OrbsLobby
var IPToLobby map[string]*Orbs.OrbsLobby

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

	// Get the integer for the command type
	var command int = reader.ReadInt32()

	// As long as the command isn't 0, continue
	for command != OrbsCommandTypes.EndPacket {
		if DebugMode {
			fmt.Printf("Received command: '%v' from %v\n", command, sender)
		}

		switch command {

		case OrbsCommandTypes.HandshakeStart:
			fmt.Println(">	Handshake Start")

		case OrbsCommandTypes.EndPacket:
			fmt.Println(">	EOF")

		case OrbsCommandTypes.LobbyRegister:
			// Get Lobby ID
			var lobbyID int = reader.ReadInt32()
			fmt.Printf(">	Registering to %v\n", lobbyID)

			if lobbyID >= len(Lobbies) { // Error
				fmt.Printf("	X	Lobby ID %v does not exist\n", lobbyID)

			} else {
				// Create new connection and add it to the lobby
				var newConnection *Orbs.OrbsConnection = new(Orbs.OrbsConnection)
				newConnection.Init(sender, outConn)

				success := Lobbies[lobbyID].AddConnection(newConnection)

				if success {
					IPToLobby[newConnection.IPAddress] = Lobbies[lobbyID]
				}
			}

		// Send the packet to it's respective lobby
		default:
			if value, keyExists := IPToLobby[sender.IP.String()]; keyExists {
				value.ProcessPacket(sender, reader, command)
				return
			}
		}

		command = reader.ReadInt32()
	}
}

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
	println("v1.1                            2014")
	println("____________________________________")

	var err error

	inConn, err = net.ListenUDP("udp", &net.UDPAddr{net.IPv4zero, 6666, ""})
	chk(err)

	outConn, err = net.ListenUDP("udp", &net.UDPAddr{IP: net.IPv4zero, Port: 0})
	chk(err)

	Lobbies = make([]*Orbs.OrbsLobby, 6)
	IPToLobby = make(map[string]*Orbs.OrbsLobby)

	for i, _ := range Lobbies {
		Lobbies[i] = new(Orbs.OrbsLobby)
		Lobbies[i].Init(i, "Lobby "+strconv.Itoa(i))
	}
}

func chk(err error) {
	if err != nil {
		panic(err)
	}
}
