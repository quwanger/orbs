using UnityEngine;
using System.Collections;

public class TTSHubZoning : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider collision){
		if(collision.gameObject == GameObject.Find("Racer 2.0"))
		{
			//AsyncOperation async = Application.LoadLevelAsync("loadingScene");
			Application.LoadLevel("selectionScreen");
			
		}
	}
}
