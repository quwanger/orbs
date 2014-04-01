using UnityEngine;
using System.Collections;

public class TTSPlayerInfo : TTSBehaviour { 
	//public GameObject TTSLevel;
	public string rig;
	//public string perkA;
	//public string perkB;
	public int playerID;
	
	public PerkType perkA;
	public PowerupType perkB;
	
	void Awake(){
		DontDestroyOnLoad(this);
	}
	
	// Use this for initialization
	void Start () {
		//rig = level.GetComponent<TTSMenu>().SelectedRig.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
