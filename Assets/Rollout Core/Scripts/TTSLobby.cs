using UnityEngine;
using System.Collections;

public class TTSLobby : MonoBehaviour
{
	public GUIText botsText;
	public GUIText levelText;
	public GUIText playersText;
	public GUIText roomText;
	
	public bool networkUpdated = true;

	public TTSLevel.LevelType Level = TTSLevel.LevelType.backroad;

	public int ID = -1;
	public string Name = "Server 1";
	public int NumRacers = 0;
	public int MaxNumRacers = 6;
	public bool InProgress = false;
	public bool BotsEnabled = true;

	public int LevelID {
		get {
			return (int)Level;
		}
		set {
			SetLevel(value);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (networkUpdated) {
			SetGUI();
			networkUpdated = false;
		}
	}

	public void SetGUI() {
		botsText.text = (BotsEnabled == true) ? "X" : "";
		levelText.text = GetLevelName();
		playersText.text = NumRacers + "/" + MaxNumRacers;
		roomText.text = Name;
	}

	public void SetPosition(int index) {
		int verticalOffset = ((InProgress)? 154 : -80) - index * 25;

		botsText.pixelOffset = new Vector2(botsText.pixelOffset.x, verticalOffset);
		levelText.pixelOffset = new Vector2(levelText.pixelOffset.x, verticalOffset);
		playersText.pixelOffset = new Vector2(playersText.pixelOffset.x, verticalOffset);
		roomText.pixelOffset = new Vector2(roomText.pixelOffset.x, verticalOffset);
	}
	
	public Rect GetPosition(){
		return new Rect(-539, botsText.pixelOffset.y-19, 1087, 20);
	}

	public void SetLevel(int levelID) {
		Level = (TTSLevel.LevelType)levelID;
	}

	public void Init(int id, string name, int numRacers, int maxRacers, bool inProgress, bool botsEnabled, int levelID) {
		ID = id;
		Name = name;
		NumRacers = numRacers;
		MaxNumRacers = maxRacers;
		InProgress = inProgress;
		BotsEnabled = botsEnabled;
		LevelID = levelID;

		Debug.Log("Lobby Received " + id + " " + Name);
	}

	public string GetLevelName() {
		switch (Level) {
			case TTSLevel.LevelType.backroad:
				return "Backroad Blitz";
			case TTSLevel.LevelType.cliff:
				return "Cliffside Chaos";
			case TTSLevel.LevelType.downtown:
				return "Downtown Domination";
			case TTSLevel.LevelType.future1:
				return "Digital Palace";
			case TTSLevel.LevelType.future2:
				return "Vindicae";
			case TTSLevel.LevelType.night:
				return "Night Fright";
		}
		return "???";
	}
}
