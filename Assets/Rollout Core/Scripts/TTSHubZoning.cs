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
			collision.gameObject.GetComponent<AudioSource>().enabled = false;	
 			
			level.GetComponent<TTSMenu>().gameMode = TTSLevel.Gametype.TimeTrial;
			level.GetComponent<TTSMenu>().activePanel = 4;
			level.GetComponent<TTSMenu>().movePanel();	
		}
	}
}


/*

private float alphaFadeValue = 1.0f;
private float transitionTimeIn = 6.0f;
private float transitionTimeOut = 6.0f;
private bool fadeIn = true;
private bool fadeOut = false;
private AsyncOperation loader;
public Texture Overlay;
private bool transitioning = false;
	
 void OnGUI() {
 	if(fadeIn && alphaFadeValue > 0.0f) {
 		alphaFadeValue -= Mathf.Clamp01(Time.deltaTime / transitionTimeIn);
 	} else if(fadeIn) {
 		fadeIn = false;
 		alphaFadeValue = 0.0f;
 	}
 	
 	if(loader != null) {
 		if(!fadeIn && fadeOut && alphaFadeValue < 1.0f && loader.progress > 0.7f) {
 			alphaFadeValue += Mathf.Clamp01(Time.deltaTime / transitionTimeOut);
 		} else if(!fadeIn && fadeOut && loader.progress > 0.7f) {
 			Debug.Log ("Got Here");
 			loader.allowSceneActivation = true;
 		}
 	}
 	
 	GUI.color = new Color(0, 0, 0, alphaFadeValue);
 	GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height ), Overlay);
 }
 
 
 public void transitionToSceneWithLoad(string scene) {
 	Debug.Log ("called.");
 	if(!transitioning) {
 		transitioning = true;
 		Debug.Log ("Started async...");
 		loader = Application.LoadLevelAsync (scene);
 		fadeOut = true;
 	loader.allowSceneActivation = false;
 	}
	}
}*/