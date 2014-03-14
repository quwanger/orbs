//Used for storing the racer data

package Orbs

type OrbsRacer struct {
	ID          float32
	Index       int
	RigType     int
	Perk1Type   int
	Perk2Type   int
	Name        string
	ControlType int
	Owner       *OrbsConnection

	isReady bool
}

func (this *OrbsRacer) Init(id float32, index int, rig int, perk1 int, perk2 int, name string, controlType int, owner *OrbsConnection) {
	this.ID = id
	this.Index = index
	this.RigType = rig
	this.Perk1Type = perk1
	this.Perk2Type = perk2
	this.Name = name
	this.ControlType = controlType
	this.Owner = owner
	this.isReady = false

	// println("Racer", name, "has been initialized: ", rig)
}
