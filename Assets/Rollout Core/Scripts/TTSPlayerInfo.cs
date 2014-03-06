using UnityEngine;
using System.Collections;

public class TTSPlayerInfo : TTSBehaviour { 
	//public GameObject TTSLevel;
	public string rig;
	public string perkA;
	public string perkB;
	// Use this for initialization
	void Start () {
		rig = level.GetComponent<TTSMenu>().SelectedRig.ToString();
		perkA = level.GetComponent<TTSMenu>().SelectedPerk.ToString();
		perkB = level.GetComponent<TTSMenu>().SelectedPerkB.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
