using UnityEngine;
using System.Collections.Generic;



public class TTSLevel : MonoBehaviour {

	public List<GameObject> _racers = new List<GameObject>();
	public List<Camera> _maincameras = new List<Camera>();
	public List<Camera> _otherCameras = new List<Camera>();
	
	
	public bool raceHasStarted = false;
	public GameObject countdown;
	public bool DebugMode = true;
	
	private TTSScoreboard scoreboard;
	
	public Font[] fonts;
	
	public static TTSLevel instance { get; private set;}
	
	private float smooth;
	
	#region MonoBehaviour Methods
	void Awake() {
		instance = this;
		
	}
	
	void Start() {
		if(DebugMode) {
			StartRace();
		} else {
			/*foreach(GameObject racer in _racers) {
				racer.GetComponent<TTSRacer>().canMove = true;
			}*/
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
	
	public TTSTimeManager time {
		get {
			return this.GetComponent<TTSTimeManager>();
		}
	}
	
	public Camera[] cameras {
		get {
			//Return an immutable array.
			return _maincameras.ToArray();
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
	
	public void RegisterCamera(Camera cam, bool isAuxillary) {
		if(isAuxillary) {
			_otherCameras.Add(cam);
		} else {
			_maincameras.Add(cam);
		}
	}
	#endregion
	
	#region Game Event Methods
	public void StartCountdown() {
		if(!DebugMode) {
			if(countdown != null) {
				countdown.GetComponent<Animation>().Play();
			} else {
				Debug.LogWarning("Countdown not assigned.");
			}
		}
	}
	
	public void StartRace() {
		raceHasStarted = true;
		
		GameObject.Find("Soundtrack").GetComponent<TTSSoundtrackManager>().StartSoundtrack();
		
		foreach(GameObject racer in racers) {
			racer.GetComponent<TTSRacer>().canMove = true;
		}
		
		GetComponent<TTSTimeManager>().StartTimer();
	}
	
	public void SwitchToAuxCamera(string cameraname) {
		//Disable all cameras
		Camera[] cams = Camera.allCameras;
		foreach(Camera cam in cams) {
			cam.enabled = false;
		}
		
		foreach(Camera cam in _otherCameras) {
			if(cam.name == cameraname) {
				cam.enabled = true;
				break;
			}
		}
		
		//sanity
		if(Camera.main == null) {
			Debug.LogError("Auxillary Camera \"" + cameraname + "\" Not found.");
		}
		
	}
	
	public void SwitchToGameCameras() {
		Camera[] cams = Camera.allCameras;
		foreach(Camera cam in cams) {
			cam.enabled = false;
		}
		
		foreach(Camera cam in _maincameras) {
			cam.enabled = true;
		}
	}
	
	public void FinishLevel() {
		
		foreach(GameObject racer in racers) {
			racer.GetComponent<TTSRacer>().SlowToStop();
		}
		
		GetComponent<TTSTimeManager>().StopTimer();
		scoreboard = this.gameObject.AddComponent<TTSScoreboard>();
	}
	
	#endregion
}
