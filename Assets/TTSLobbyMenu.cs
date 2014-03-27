using UnityEngine;
using System.Collections.Generic;

public class TTSLobbyMenu : TTSBehaviour {
	public TTSClient client;
	public bool networkUpdated = true;
	private int lobbyID = -1;

	public string levelTitle = "Level";
	public string[] playerNames = new string[6];

	public GUIText levelText;
	public GUIText playerCountText;
	public GUIText[] playerTexts = new GUIText[6];

	public string nullPlayerText = "Waiting for player";

	// Use this for initialization
	void Start () {
		client = level.client;

		//playerNames[0] = "sunmock";
		OnPlayerUpdate();
		OnLevelUpdate();
	}
	
	// Update is called once per frame
	void Update() {
		if (!networkUpdated)
			return;

		OnPlayerUpdate();
		OnLevelUpdate();
	}

	public void OnJoin(TTSLobby lobby) {
		lobbyID = lobby.ID;
	}

	public void OnLevelUpdate() {
		levelText.text = levelTitle;
	}

	public void OnPlayerUpdate() {
		int guiIndex = 0;
		foreach (string value in playerNames) {
			if(value != ""){
				playerTexts[guiIndex].text = value;
				guiIndex++;
			}
		}

		playerCountText.text = guiIndex + "/" + playerTexts.Length;

		for (int i = guiIndex; i < playerTexts.Length; i++) {
			playerTexts[i].text = nullPlayerText;
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