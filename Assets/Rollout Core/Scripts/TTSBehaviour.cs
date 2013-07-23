using UnityEngine;
using System.Collections.Generic;


/*****************************************
 * Helper extension to reduce GetComponent and Find calls in code base.
 **/


public class TTSBehaviour : MonoBehaviour {
	
	public enum Powerup {None, EntropyCannon, DrezzStones, TimeBonus};
	
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
	
	public TTSTimeManager time {
		get {
			return level.GetComponent<TTSTimeManager>();
		}
	}
	
	public static T GetRandomEnum<T>()
	{
		System.Array A = System.Enum.GetValues(typeof(T));
		T V = (T)A.GetValue(UnityEngine.Random.Range(0,A.Length));
		return V;
	}
	
	public TTSFloatHud hud {
		get {
			//GGHHEETTOO
			return GameObject.Find("hud").GetComponent<TTSFloatHud>();
		}
	}
	
}
