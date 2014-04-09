package Orbs

import (
	"Packets"
	"fmt"
	"net"
)

type OrbsConnection struct {
	Address       *net.UDPAddr
	ReturnAddress *net.UDPAddr

	IsEstablished bool
	IsRaceReady   bool

	OutPacket     *Packets.PacketWriter
	OutConnection *net.UDPConn

	IPAddress string

	OwnedObjects []float32
	// OwnedRacers []float32
	// OwnedPowerups []float32

	DebugMode bool
}

func (this *OrbsConnection) Init(sender *net.UDPAddr, outConnection *net.UDPConn) {
	this.DebugMode = false
	this.IsEstablished = false

	this.Address = sender
	this.IPAddress = sender.IP.String()
	this.ReturnAddress = &net.UDPAddr{IP: sender.IP, Port: 6969}

	this.OutPacket = new(Packets.PacketWriter)
	this.OutPacket.InitPacket()

	this.OutConnection = outConnection
	this.IsRaceReady = false
	// this.CommandSender = new(UnityCommand)
	// this.CommandSender.Init(this.OutPacket, this.ReturnAddress, outConn)
}

func (this *OrbsConnection) AddObject(id float32) {
	this.OwnedObjects = append(this.OwnedObjects, id)
}

func (this *OrbsConnection) WriteData(data []byte) {
	this.CheckPacketSize(len(data))
	this.OutPacket.WriteBytes(data)
}

func (this *OrbsConnection) SendPacket() {
	if this.OutPacket.Size() == 0 {
		return
	}

	n, _ := this.OutConnection.WriteToUDP(this.OutPacket.GetMinimalData(), this.ReturnAddress)
	this.OutPacket.Clear()

	if this.DebugMode {
		fmt.Printf(">	Sent %v bytes to %v\n", n, this.IPAddress)
	}
}

// Avoid use
func (this *OrbsConnection) SendData(data []byte) {
	if this.OutPacket.Size() == 0 {
		return
	}

	n, _ := this.OutConnection.WriteToUDP(data, this.ReturnAddress)

	if this.DebugMode {
		fmt.Printf(">*	Sent %v bytes to %v\n", n, this.IPAddress)
	}
}

func (this *OrbsConnection) CheckPacketSize(dataLength int) {
	if this.OutPacket.Size()+dataLength > len(this.OutPacket.Data) {
		fmt.Printf("W	POTENTIAL DATA OVERFLOW AT %v BYTES. SENDING/CLEARING PACKET\n", this.OutPacket.Size())

		this.SendPacket()
		this.OutPacket.Clear()
	}
}

func (this *OrbsConnection) GenericCommand(command int) {
	this.CheckPacketSize(Packets.SIZEOF_INT32)
	this.OutPacket.WriteInt(command)
}
