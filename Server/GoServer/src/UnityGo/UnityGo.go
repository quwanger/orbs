package UnityGo

import (
	"OrbsCommands"
	"Packets"
	"fmt"
	"net"
)

// VECTOR3

type Vector3 struct {
	X float32
	Y float32
	Z float32
}

func (this *Vector3) Set(x float32, y float32, z float32) {
	this.X = x
	this.Y = y
	this.Z = z
}

func (this *Vector3) Add(add *Vector3) *Vector3 {
	a := new(Vector3)
	a.X, a.Y, a.Z = this.X+add.X, this.Y+add.Y, this.Z+add.Z
	return a
}

// UNITY COMMAND

type UnityCommand struct {
	Address   *net.UDPAddr
	OutPacket *Packets.PacketWriter

	OutConn *net.UDPConn

	VAR_SIZE int
}

func (this *UnityCommand) Init(writer *Packets.PacketWriter, address *net.UDPAddr, outConn *net.UDPConn) {
	this.OutPacket = writer
	this.Address = address
	this.OutConn = outConn
	this.VAR_SIZE = 4
}

func (this *UnityCommand) checkWritable(dataLength int) {
	if this.OutPacket.Size()+dataLength > len(this.OutPacket.Data) {
		fmt.Printf(">	POTENTIAL DATA OVERFLOW AT %v BYTES. SENDING/CLEARING PACKET\n", this.OutPacket.Size())
		this.SendPacket()
		this.OutPacket.Clear()
	}
}

func (this *UnityCommand) ReplyInit() {
	this.checkWritable(this.VAR_SIZE)
	this.OutPacket.WriteUInt32(OrbsCommands.HandshakeAcknowledge)
}

func (this *UnityCommand) ObjectUpdate(data []byte) {
	this.checkWritable(len(data))
	this.OutPacket.WriteBytes(data)
}

func (this *UnityCommand) ObjectNotRegistered(id float32) {
	this.checkWritable(this.VAR_SIZE * 2)
	this.OutPacket.WriteUInt32(OrbsCommands.ObjectIsNotRegistered)
	this.OutPacket.WriteFloat32(id)
}

func (this *UnityCommand) ObjectAlreadyRegistered(id float32) {
	this.checkWritable(this.VAR_SIZE * 2)
	fmt.Printf("*	OBJECT #%v REGISTERED TO DIFFERENT OWNER\n\n", id)
	this.OutPacket.WriteUInt32(OrbsCommands.ObjectAlreadyRegistered)
	this.OutPacket.WriteFloat32(id)
}

func (this *UnityCommand) ObjectRegisteredSuccess(id float32) {
	this.checkWritable(this.VAR_SIZE * 2)
	fmt.Printf("X	Added new obj %v\n\n", id)
	this.OutPacket.WriteUInt32(OrbsCommands.ObjectRegisterOK)
	this.OutPacket.WriteFloat32(id)
}

func (this *UnityCommand) ClearData() {
	this.OutPacket.Clear()
}

func (this *UnityCommand) SendPacket() {
	if this.OutPacket.Size() == 0 {
		return
	}

	n, err := this.OutConn.WriteToUDP(this.OutPacket.Data[0:this.OutPacket.Size()], this.Address)
	fmt.Printf("<-- Sending %v bytes to %v\n\n", n, this.Address)

	if err != nil {
		panic(err)
	}
}

// UNITY CONNECTION

type UnityConn struct {
	Address       *net.UDPAddr
	ReturnAddress *net.UDPAddr
	IsEstablished bool
	OwnedObjects  []float32
	OutPacket     *Packets.PacketWriter
	CommandSender *UnityCommand
}

func (this *UnityConn) Init(sender *net.UDPAddr, outPacket *Packets.PacketWriter, outConn *net.UDPConn) {
	this.Address = sender
	this.ReturnAddress = &net.UDPAddr{IP: sender.IP, Port: 6969}
	this.IsEstablished = false
	this.OutPacket = outPacket
	this.CommandSender = new(UnityCommand)
	this.CommandSender.Init(this.OutPacket, this.ReturnAddress, outConn)
}

func (this *UnityConn) AddObj(id float32) {
	this.OwnedObjects = append(this.OwnedObjects, id)
}

// UNITY GAMEOBJECT

type UnityGameObject struct {
	Id        float32
	LastPos   *Vector3
	LastRot   *Vector3
	LastSpeed *Vector3
	Owner     *net.UDPAddr
}

func (this *UnityGameObject) Init(id float32, owner *net.UDPAddr) {
	this.Id = id
	this.LastPos = new(Vector3)
	this.LastRot = new(Vector3)
	this.LastSpeed = new(Vector3)
	this.Owner = owner
}
