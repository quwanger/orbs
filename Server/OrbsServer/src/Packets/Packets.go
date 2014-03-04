package Packets

import (
	"bytes"
	"encoding/binary"
	"errors"
	"fmt"
	"math"
)

const (
	SIZEOF_INT32   = 4
	SIZEOF_UINT32  = 4
	SIZEOF_UINT64  = 8
	SIZEOF_FLOAT32 = 4
)

type PacketReader struct {
	Data      []byte
	readIndex int

	reader bytes.Reader
}

func (this *PacketReader) GetReadIndex() int {
	return this.readIndex
}

func (this *PacketReader) GetStatus() {
	fmt.Printf("%v/%v bytes read", this.readIndex, len(this.Data))
}

func (this *PacketReader) InitData() {
	fmt.Println(this.Data[0:4])
}

func (this *PacketReader) EmptyRead4Bytes() {
	this.readIndex += 4
}

func (this *PacketReader) EmptyRead8Bytes() {
	this.readIndex += 8
}

func (this *PacketReader) EmptyRead12Bytes() {
	this.readIndex += 12
}

func (this *PacketReader) EmptyReadBytes(size int) {
	this.readIndex += size
}

func (this *PacketReader) Rewind4Bytes() {
	this.readIndex -= 4
}

func (this *PacketReader) ReadString(size int) string {
	this.readIndex += size
	return string(this.Data[this.readIndex-size : this.readIndex])
}

func (this *PacketReader) Read16String() string {
	return this.ReadString(16)
}

func (this *PacketReader) Read32String() string {
	return this.ReadString(32)
}

func (this *PacketReader) ReadUInt64() uint64 {
	a, n := binary.Uvarint(this.Data[this.readIndex : this.readIndex+SIZEOF_UINT64])

	if n == 0 {
		this.handleError(errors.New("n == 0: buf too small"))
		return 0
	} else if n < 0 {
		this.handleError(errors.New("n  < 0: value larger than 64 bits (overflow) and -n is the number of bytes read"))
		return 0
	}

	this.readIndex += SIZEOF_UINT64

	return a
}

func (this *PacketReader) ReadInt32() int {
	return int(this.ReadUInt32())
}

func (this *PacketReader) ReadUInt32() uint32 {
	this.readIndex += SIZEOF_UINT32
	return binary.LittleEndian.Uint32(this.Data[this.readIndex-SIZEOF_UINT32 : this.readIndex])
}

func (this *PacketReader) ReadFloat32() float32 {
	var value float32
	var hash = this.Data[this.readIndex : this.readIndex+SIZEOF_FLOAT32]
	this.readIndex += SIZEOF_FLOAT32

	err := binary.Read(bytes.NewReader(hash), binary.LittleEndian, &value)

	if err != nil {
		this.handleError(err)
		return 0
	}

	return value
}

func (this *PacketReader) ReadBytes(amount int) []byte {
	this.readIndex += amount
	return this.Data[this.readIndex-amount : this.readIndex]
}

func (this *PacketReader) handleError(err error) {
	println(err)
}

type PacketWriter struct {
	Data      []byte
	readIndex int
}

func (this *PacketWriter) Size() int {
	return this.readIndex
}

func (this *PacketWriter) InitPacket() {
	this.Data = make([]byte, 1024)
	this.readIndex = 0
}

func (this *PacketWriter) Clear() {
	this.Data = make([]byte, 1024)
	this.readIndex = 0
}

func (this *PacketWriter) WriteInt(data int) {
	this.WriteUInt32(uint32(data))
}

func (this *PacketWriter) WriteUInt32(data uint32) {
	binary.LittleEndian.PutUint32(this.Data[this.readIndex:this.readIndex+SIZEOF_UINT32], data)
	this.readIndex += SIZEOF_UINT32
}

func (this *PacketWriter) WriteFloat32(data float32) {
	b := math.Float32bits(data)
	binary.LittleEndian.PutUint32(this.Data[this.readIndex:this.readIndex+SIZEOF_FLOAT32], b)
	this.readIndex += SIZEOF_FLOAT32
}

func (this *PacketWriter) WriteVector3(x float32, y float32, z float32) {
	this.WriteFloat32(x)
	this.WriteFloat32(y)
	this.WriteFloat32(z)
}

func (this *PacketWriter) WriteString(data string) {
	this.WriteBytes([]byte(data))
}

func (this *PacketWriter) WriteBytes(data []byte) {
	for _, value := range data {
		this.Data[this.readIndex] = value
		this.readIndex++
	}
}

func (this *PacketWriter) GetMinimalData() []byte {
	return this.Data[0:this.Size()]
}
