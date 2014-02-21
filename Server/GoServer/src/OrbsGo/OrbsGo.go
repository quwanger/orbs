package OrbsGo

import (
	"OrbsCommandTypes"
	"Packets"
	"UnityGo"
	"fmt"
	"net"
)

// Orbs Command

type OrbsCommand struct {
	UnityGo.UnityCommand
}

func (this *OrbsCommand) RacerRegister() {
	fmt.Println("")
}

func (this *OrbsCommand) ReturnNumRacers() {
	this.CheckWritable(this.VAR_SIZE * 2)
	this.WriteCommand(OrbsCommandTypes.RacerIsNotRegistered) // fix
	this.OutPacket.WriteUInt32(OrbsCommandTypes.ReturnNumRacers)
}

func (this *OrbsCommand) RacerUpdate(data []byte) {
	this.SendBytes(data)
}

func (this *OrbsCommand) RacerNotRegistered(id float32) {
	this.CheckWritable(this.VAR_SIZE * 2)
	this.WriteCommand(OrbsCommandTypes.RacerIsNotRegistered)
	this.OutPacket.WriteFloat32(id)
}

func (this *OrbsCommand) RacerAlreadyRegistered(id float32) {
	this.CheckWritable(this.VAR_SIZE * 2)
	this.WriteCommand(OrbsCommandTypes.RacerAlreadyRegistered)
	this.OutPacket.WriteFloat32(id)
}

func (this *OrbsCommand) RacerRegisteredSuccess(id float32) {
	this.CheckWritable(this.VAR_SIZE * 2)
	fmt.Printf("X	Added new racer %v\n\n", id)
	this.WriteCommand(OrbsCommandTypes.RacerRegisterOK)
	this.OutPacket.WriteFloat32(id)
}

// Orbs Connection

type OrbsConn struct {
	UnityGo.UnityConn
	CommandSender *OrbsCommand
	OwnedRacers   []float32
}

func (this *OrbsConn) Init(sender *net.UDPAddr, outPacket *Packets.PacketWriter, outConn *net.UDPConn) {
	this.Address = sender
	this.ReturnAddress = &net.UDPAddr{IP: sender.IP, Port: 6969}
	this.IsEstablished = false
	this.OutPacket = outPacket
	this.CommandSender = new(OrbsCommand)
	this.CommandSender.Init(this.OutPacket, this.ReturnAddress, outConn)
}

func (this *OrbsConn) AddRacer(id float32) {
	this.OwnedRacers = append(this.OwnedRacers, id)
}

// Orbs RacerObject

type OrbsRacer struct {
	UnityGo.UnityGameObject

	VInput      float32
	HInput      float32
	PowerUpType int
	PowerUpTier int
}
