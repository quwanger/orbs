using UnityEngine;
using System.Collections.Generic;


/*****************************************
 * Helper extension to reduce GetComponent and Find calls in code base.
 **/


public class TTSBehaviour : MonoBehaviour {
	
	//used for changing the stats of the racers based on their rig selection and perk selection
	[HideInInspector]
	public float accelerationIncrease = 2500.0f;
	[HideInInspector]
	public float speedIncrease = 10.0f;
	[HideInInspector]
	public float handlingIncrease = 3000.0f;
	[HideInInspector]
	public float offenseIncrease = 0.3f;
	[HideInInspector]
	public float defenseIncrease = 0.3f;

	public enum LevelMenuItem { level1, level2, level3, level4, level5, level6 };

	public enum RigType { Rhino, Scorpion, Spider, Dragon, Antique, NextGen, Testing };

	public enum CharacterTypes { character_yellow, character_purple, character_orange, character_blue, character_red, character_green };

	public enum PowerupType {None, EntropyCannon, DrezzStones, TimeBonus, SuperC, Shield, Helix, Shockwave, Leech, Lottery};
	
	public enum PerkType {PhotoFinish, HotStart, DiamondCoat, ManOWar, Evolution, Speed, Acceleration, Handling, None};
	//public enum PerksPool2 {None, EntropyCannon, DrezzStones, TimeBonus, SuperC, Shield, Helix, Shockwave, Leech, Lottery};

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
