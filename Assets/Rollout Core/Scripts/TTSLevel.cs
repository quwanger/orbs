using UnityEngine;
using System.Collections.Generic;



public class TTSLevel : MonoBehaviour {

	public List<GameObject> _racers = new List<GameObject>();
	public List<Camera> _maincameras = new List<Camera>();
	public List<Camera> _otherCameras = new List<Camera>();
	
	public enum Gametype {Lobby, SinglePlayer, MultiplayerLocal, MultiplayerOnline, TimeTrial};
	public Gametype currentGameType = Gametype.MultiplayerLocal;
	
	public enum LevelType { cliff, night, backroad, downtown, future1, future2 };
	public LevelType currentLevel = LevelType.backroad;
	
	public bool raceHasStarted = false;
	public bool raceHasFinished = false;
	public GameObject countdown;
	public bool DebugMode = true;
	public bool PerksEnabled = true;
	
	private TTSScoreboard scoreboard;
	
	public Font[] fonts;

	public TTSClient client;
	public TTSInitRace initRace;
	
	public static TTSLevel instance { get; private set;}
	
	#region MonoBehaviour Methods
	void Awake() {
		instance = this;
		client = GetComponent<TTSClient>();
		initRace = GetComponent<TTSInitRace>();
	}
	
	void Start() {	
		if(DebugMode) {
			StartRace();
			foreach(GameObject racer in _racers){
				if(racer.GetComponent<TTSRacer>().debugStart){
					racer.transform.position = racer.GetComponent<TTSRacer>().debugStart.transform.position;
				}
			}
		}else{
			StartCountdown();
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
	
	public TTSWaypointManager waypointManager {
		get {
			return this.GetComponent<TTSWaypointManager>();
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
			racer.GetComponent<Rigidbody>().useGravity = true;
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
		
		/*foreach(GameObject racer in racers) {
			racer.GetComponent<TTSRacer>().SlowToStop();
		}*/
		
		raceHasFinished = true;
		
		GetComponent<TTSTimeManager>().StopTimer();
		scoreboard = this.gameObject.AddComponent<TTSScoreboard>();
	}
	
	#endregion
}
