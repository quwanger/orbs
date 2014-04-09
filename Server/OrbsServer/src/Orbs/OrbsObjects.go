//Used for storing the racer data

package Orbs

type OrbsRacer struct {
	ID            float32
	Index         int
	RigType       int
	PerkAType     int
	PerkBType     int
	CharacterType int
	Name          string
	ControlType   int
	Owner         *OrbsConnection

	isReady bool
}

func (this *OrbsRacer) Init(id float32, index int, rig int, perkA int, perkB int, character int, name string, controlType int, owner *OrbsConnection) {
	this.ID = id
	this.Index = index
	this.RigType = rig
	this.PerkAType = perkA
	this.PerkBType = perkB
	this.CharacterType = character
	this.Name = name
	this.ControlType = controlType
	this.Owner = owner
	this.isReady = false

	// println("Racer", name, "has been initialized: ", rig)
}

// type OrbsProp struct {
// 	ID        float32
// 	Owner     *OrbsConnection
// 	NeedOwner bool
// }

type OrbsPowerupPlatform struct {
	ID          float32
	Owner       *OrbsConnection
	PowerupType int
}
