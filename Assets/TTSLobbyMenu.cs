using UnityEngine;
using System.Collections.Generic;

public class TTSLobbyMenu : TTSBehaviour {
	public string levelTitle = "Level";
	public string[] playerNames = new string[6];

	public GUIText level;
	public GUIText playerCount;
	public GUIText[] players = new GUIText[6];

	public string nullPlayerText = "Waiting for player";

	// Use this for initialization
	void Start () {
		//playerNames[0] = "sunmock";
		OnPlayerUpdate();
		OnLevelUpdate();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void OnLevelUpdate() {
		level.text = levelTitle;
	}

	public void OnPlayerUpdate() {
		int guiIndex = 0;
		foreach (string value in playerNames) {
			if(value != ""){
				players[guiIndex].text = value;
				guiIndex++;
			}
		}

		playerCount.text = guiIndex + "/" + players.Length;

		for (int i = guiIndex; i < players.Length; i++) {
			players[i].text = nullPlayerText;
		}
	}

	private int getNumPlayers() {
		int i = 0;
		foreach (string value in playerNames) {
			if (value != "") i++;
		}
		return i;
	}
}