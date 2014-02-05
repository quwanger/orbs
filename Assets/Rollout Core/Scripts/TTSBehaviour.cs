using UnityEngine;
using System.Collections.Generic;


/*****************************************
 * Helper extension to reduce GetComponent and Find calls in code base.
 **/


public class TTSBehaviour : MonoBehaviour {
	
	//used for changing the stats of the racers based on their rig selection and perk selection
	public float accelerationIncrease = 500.0f;
	public float speedIncrease = 25.0f;
	public float handlingIncrease = 750.0f;
	public float offenseIncrease = 0.3f;
	public float defenseIncrease = 0.3f;

	public enum Powerup {None, EntropyCannon, DrezzStones, TimeBonus, SuperC, Shield, Helix, Shockwave, Leech};
	
	public enum PerksPool1 {PhotoFinish, HotStart, DiamondCoat, ManOWar, Evolution, Speed, Acceleration, Handling, None};
	//public enum PerksPool2 {Helix, SuperC, Drezz, Entropy, Leech, Nanite, EMP, None};

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

	public TTSWaypointManager waypointManager {
		get {
			return level.waypointManager;
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
			return GameObject.Find("hud(Clone)").GetComponent<TTSFloatHud>();
		}
	}
}
