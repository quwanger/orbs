using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TTSHubZoning : TTSBehaviour {
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider collision){
		if(collision.gameObject.name == "Racer 2.0")
		{
			//AsyncOperation async = Application.LoadLevelAsync("loadingScene");
			//Application.LoadLevel("selectionScreen");
			//GameObject.Find("TTSLEVEL").GetComponent
			level.GetComponent<TTSMenu>().zone=true;
		}
	}
}
