using UnityEngine;
using System.Collections.Generic;
using XInputDotNetPure;

public class TTSLevel : MonoBehaviour {

	public List<GameObject> _racers = new List<GameObject>();
	public List<Camera> _maincameras = new List<Camera>();
	public List<Camera> _otherCameras = new List<Camera>();
	
	public enum Gametype {Lobby, MultiplayerLocal, MultiplayerOnline, TimeTrial, Arcade, Debug};
	public Gametype currentGameType = Gametype.MultiplayerLocal;
	
	public enum LevelType { cliff, night, backroad, downtown, future1, future2 };
	public LevelType currentLevel;

	public bool useKeyboard = true;
	
	public bool raceHasStarted = false;
	public bool raceHasFinished = false;
	public bool humanPlayersFinished = false;
	public GameObject countdown;
	public bool countdownStarted = false;
	public bool DebugMode = true;
	public bool PerksEnabled = true;
	
	private TTSScoreboard scoreboard;

	private string levelSelected = null;
	
	public Font[] fonts;

	public TTSClient client;
	public TTSInitRace initRace;

	public string fakeBestTime;
	public string fakeWorldRecord;
	
	public static TTSLevel instance { get; private set;}

	PlayerIndex playerIndex;
	GamePadState state;
	
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
			if (currentGameType != Gametype.MultiplayerOnline) {
				StartCountdown();
			}else{
				GameObject.Find ("PauseMenu").active = false;
			}
		}
	}

	void DestroyDataToPass(){
		GameObject dataToPass = GameObject.Find("DataToPass");
		Destroy(dataToPass);
	}

	void Update() {
		if(DebugMode || !DebugMode) {
			if(Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.Return)){
				if(Input.GetKeyDown(KeyCode.T)){
					DestroyDataToPass();
					Application.LoadLevel("city1-1");
				}
				if(Input.GetKeyDown(KeyCode.Y)){
					DestroyDataToPass();
					Application.LoadLevel("city1-2");
				}
				if(Input.GetKeyDown(KeyCode.U)){
					DestroyDataToPass();
					Application.LoadLevel("rural1-1");
				}
				if(Input.GetKeyDown(KeyCode.I)){
					DestroyDataToPass();
					Application.LoadLevel("cliffsidechoas");
				}
				if(Input.GetKeyDown(KeyCode.O)){
					DestroyDataToPass();
					Application.LoadLevel("future1-1");
				}
				if(Input.GetKeyDown(KeyCode.P)){
					DestroyDataToPass();
					Application.LoadLevel("future1-2");
				}
				if(Input.GetKeyDown(KeyCode.LeftBracket)){
					DestroyDataToPass();
					Application.LoadLevel("hub-world");
				}
				if(Input.GetKeyDown(KeyCode.R)){
					Application.LoadLevel(Application.loadedLevel);
				}
			}
		}
		
		if(humanPlayersFinished){
			//bool isAPressed = (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) ? true : false;
			bool isBPressed = (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed) ? true : false;

			/*Debug.Log("ALL RACERS FINISHED");
			if(Input.GetKeyDown(KeyCode.A) || isAPressed){
				Application.LoadLevel(Application.loadedLevel);
			}*/
			
			if(Input.GetKeyDown(KeyCode.B) || isBPressed){
				GameObject dataToPass = GameObject.Find("DataToPass");
				Destroy(dataToPass);
				Application.LoadLevel("hub-world");
			}
		}

		if (currentGameType == Gametype.MultiplayerOnline && client.startRaceCountdown && countdownStarted == false) {
			StartCountdown();
		}
	}
	#endregion

	#region Accessors/Helpers
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

	private TTSMenu _menu;
	public TTSMenu menu {
		get {
			return (_menu != null) ? _menu : (_menu = GetComponent<TTSMenu>());
		}
	}
	
	private void ThrowSetException(string source) {
		Debug.LogError("Tried to set a readonly property: " + source);
	}
	
	// Helpers
	public string LevelTypeToFileName(LevelType type) {
		switch (type) {
			case LevelType.backroad:
				return "city1-1";
			case LevelType.downtown:
				return "city1-2";
			case LevelType.cliff:
				return "cliffsidechoas";
			case LevelType.future1:
				return "future1-1";
			case LevelType.future2:
				return "future1-2";
			case LevelType.night:
				return "rural1-1";
		}
		return "hub-world";
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
				countdownStarted = true;
			} else {
				Debug.LogWarning("Countdown not assigned.");
			}
		}
	}
	
	public void StartRace() {
		raceHasStarted = true;
		
		GameObject.Find("Soundtrack").GetComponent<TTSSoundtrackManager>().StartSoundtrack();
		
		foreach(GameObject racer in racers) {
			racer.rigidbody.constraints = RigidbodyConstraints.None;
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

	bool GetButtonDown(int player, string button){
		PlayerIndex playerIndex = PlayerIndex.One;

		switch(player){
			case 1:
				playerIndex = PlayerIndex.One;
				break;

			case 2:
				playerIndex = PlayerIndex.Two;
				break;
			
			case 3:
				playerIndex = PlayerIndex.Three;
				break;
			
			case 4:
				playerIndex = PlayerIndex.Four;
				break;
		}

		state = GamePad.GetState(playerIndex);

		switch(button){
			case "A":
			case "a":
				return (state.Buttons.A == ButtonState.Pressed) ? true : false;

			case "Y":
			case "y":
				return (state.Buttons.Y == ButtonState.Pressed) ? true : false;

			case "Start":
			case "start":
				return (state.Buttons.Start == ButtonState.Pressed) ? true : false;
		}

		return false;
	}
}
