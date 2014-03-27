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
		levelText.text = GetLevelString(Level);
		playersText.text = NumRacers + "/" + MaxNumRacers;
		roomText.text = Name;
	}

	public void SetPosition(int index) {
		int verticalOffset = ((InProgress)? -55 : 154) - index * 20;

		botsText.pixelOffset = new Vector2(botsText.pixelOffset.x, verticalOffset);
		levelText.pixelOffset = new Vector2(levelText.pixelOffset.x, verticalOffset);
		playersText.pixelOffset = new Vector2(playersText.pixelOffset.x, verticalOffset);
		roomText.pixelOffset = new Vector2(roomText.pixelOffset.x, verticalOffset);
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

	public string GetLevelString(TTSLevel.LevelType level) {
		switch(level){
			case TTSLevel.LevelType.backroad:
				return "Backroad Blitz";
			case TTSLevel.LevelType.cliff:
				return "Cliff";
			case TTSLevel.LevelType.downtown:
				return "Downtown";
			case TTSLevel.LevelType.future1:
				return "future 1";
			case TTSLevel.LevelType.future2:
				return "future 2";
			case TTSLevel.LevelType.night:
				return "Night time asdf";
		}
		return "???";
	}
}
