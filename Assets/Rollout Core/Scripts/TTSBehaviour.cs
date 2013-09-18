using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Helper extension to reduce GetComponent and Find calls in code base.
/// </summary>
public class TTSBehaviour : MonoBehaviour {

	public enum Powerup {None, EntropyCannon, DrezzStones, TimeBonus, SuperC};
	
	public enum Perks {None, PhotoFinish, HotStart};
	
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

	/// <summary>
	/// List of all racers in the scene
	/// </summary>
	public GameObject[] racers {
		get {
			return level.racers;
		}
	}
	
	/// <summary>
	/// Audio directly attached to main camera
	/// </summary>
	public AudioSource sfx {
		get {
			return Camera.main.audio;
		}
	}
	
	/// <summary>
	/// Visual effects such as boost/damage effects
	/// </summary>
	public TTSCameraEffects vfx {
		get {
			return Camera.main.GetComponent<TTSCameraEffects>();
		}
	}
	
	/// <summary>
	/// Time since game started
	/// </summary>
	public TTSTimeManager time {
		get {
			return level.GetComponent<TTSTimeManager>();
		}
	}
	
	/// <summary>
	/// Get random value of an enum type
	/// </summary>
	/// <typeparam name="T">Whatever enum you want a random val of</typeparam>
	/// <returns>Random enum value of type T</returns>
	public static T GetRandomEnum<T>()
	{
		System.Array A = System.Enum.GetValues(typeof(T));
		T V = (T)A.GetValue(UnityEngine.Random.Range(0,A.Length));
		return V;
	}
	
	/// <summary>
	/// Get the hud
	/// </summary>
	public TTSFloatHud hud {
		get {
			//GGHHEETTOO
			return GameObject.Find("hud").GetComponent<TTSFloatHud>();
		}
	}
	
}
