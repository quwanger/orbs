using UnityEngine;
using System.Collections.Generic;



public class TTSLevel : MonoBehaviour {

	public List<GameObject> _racers;
	public List<Camera> _cameras;
	
	
	public GameObject countdown;
	public bool DebugMode = true;
	
	public static TTSLevel instance { get; private set;}
	
	
	#region MonoBehaviour Methods
	void Awake() {
		instance = this;
		
	}
	
	void Start() {
		if(!DebugMode) {
			StartCountdown();
		} else {
			foreach(GameObject racer in _racers) {
				racer.GetComponent<TTSRacer>().canMove = true;
			}
		}
	}
	#endregion

	#region Accessors
	public TTSLevel level {
		get {
			return instance;
		}
		set {
			ThrowSetException("level");
		}
	}
	
	public Camera[] cameras {
		get {
			//Return an immutable array.
			return _cameras.ToArray();
		}
		set {
			ThrowSetException("cameras");
		}
	}
	
	public GameObject[] racers {
		get {
			return _racers.ToArray();
		}
		set {
			ThrowSetException("racers");
		}
	}
	
	private void ThrowSetException(string source) {
		Debug.LogError("Tried to set a readonly property: " + source);
	}
	#endregion
	
	
	#region Registry Methods
	public void RegisterRacer(GameObject go) {
		//ONLY to be called by the TTSRacer class
		_racers.Add(go);
	}
	
	public void RegisterCamera(Camera cam) {
		_cameras.Add(cam);
	}
	#endregion
	
	#region Game Event Methods
	public void StartCountdown() {
		countdown.GetComponent<Animation>().Play();
	}
	
	public void StartRace() {
		GameObject.Find("Soundtrack").GetComponent<TTSSoundtrackManager>().StartSoundtrack();
		
		foreach(GameObject racer in racers) {
			racer.GetComponent<TTSRacer>().canMove = true;
		}
		
		GetComponent<TTSTimeManager>().StartTimer();
	}
	#endregion
	
}
