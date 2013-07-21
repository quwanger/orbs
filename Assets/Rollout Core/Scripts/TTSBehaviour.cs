using UnityEngine;
using System.Collections.Generic;


/*****************************************
 * Helper extension to reduce GetComponent and Find calls in code base.
 **/


public class TTSBehaviour : MonoBehaviour {
	
	public TTSLevel level {
		 get { 
			if(TTSLevel.instance != null) {
				return TTSLevel.instance; 
			} else {
				Debug.LogError("FATAL: You need to have a TTSLevel object in the scene to use level.");
				return new TTSLevel();
			}
		}
	}
	
	public GameObject[] racers {
		get {
			return level.racers;
		}
	}
	
	public AudioSource sfx {
		get {
			return Camera.main.audio;
		}
	}
	
	public TTSCameraEffects vfx {
		get {
			return Camera.main.GetComponent<TTSCameraEffects>();
		}
	}
	
}
