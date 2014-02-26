package Orbs

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

// func (this *Vector3) Add(add *Vector3) *Vector3 {
// 	a := new(Vector3)
// 	a.X, a.Y, a.Z = this.X+add.X, this.Y+add.Y, this.Z+add.Z
// 	return a
// }
