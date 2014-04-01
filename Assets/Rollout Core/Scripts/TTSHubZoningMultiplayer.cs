using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TTSHubZoningMultiplayer : TTSBehaviour {
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider collision){
		if(collision.gameObject.name == "Racer 2.0")
		{
			level.menu.gameMode = TTSLevel.Gametype.MultiplayerOnline;
			level.menu.activePanel = 0;
			level.menu.movePanel();
			//level.GetComponent<TTSMenu>().panels[4].SetActive(true);
			//iTween.MoveTo(level.GetComponent<TTSMenu>().panels[4]], iTween.Hash("x", 0.5, "time", 2.0f, "onComplete", "stoppedTweening", "onCompleteTarget", gameObject));
		}
	}
}