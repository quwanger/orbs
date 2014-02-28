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

	OutPacket     *Packets.PacketWriter
	OutConnection *net.UDPConn

	IPAddress string

	OwnedObjects []float32
	// OwnedRacers []float32
	// OwnedPowerups []float32
}

func (this *OrbsConnection) Init(sender *net.UDPAddr, outConnection *net.UDPConn) {
	this.IsEstablished = false

	this.Address = sender
	this.IPAddress = sender.IP.String()
	this.ReturnAddress = &net.UDPAddr{IP: sender.IP, Port: 6969}

	this.OutPacket = new(Packets.PacketWriter)
	this.OutPacket.InitPacket()

	this.OutConnection = outConnection
	// this.CommandSender = new(UnityCommand)
	// this.CommandSender.Init(this.OutPacket, this.ReturnAddress, outConn)
}

func (this *OrbsConnection) WriteData(data []byte) {
	this.CheckPacketSize(len(data))
	this.OutPacket.WriteBytes(data)
}

func (this *OrbsConnection) SendPacket() {
	if this.OutPacket.Size() == 0 {
		return
	}

	n, _ := this.OutConnection.WriteToUDP(this.OutPacket.Data[0:this.OutPacket.Size()], this.ReturnAddress)
	this.OutPacket.Clear()

	fmt.Printf(">	Sent %v bytes to %v\n", n, this.IPAddress)
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
