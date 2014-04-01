using UnityEngine;
using System.Collections.Generic;

public class TTSLobbyMenu : TTSBehaviour
{
	public TTSClient client;
	public bool lobbyJoined = false;
	public bool networkUpdated = true;
	private int lobbyID = -1;

	public string levelTitle = "Level";
	public string[] playerNames = new string[6];

	public GUIText levelText;
	public GUIText playerCountText;
	public GUIText[] playerTexts = new GUIText[6];

	public string nullPlayerText = "Waiting for player";

	// Use this for initialization
	void Start() {
		client = level.client;
		// Create a player
		//TTSRacerConfig tempConfig = level.initRace.testRacerConfig(true);
		//tempConfig.Name = "BOBBY BOBB";
		//level.menu.players.Add(tempConfig);

		OnPlayerUpdate();
		OnLevelUpdate();
	}

	// Update is called once per frame
	void Update() {
		if (!networkUpdated)
			return;
		
		if (lobbyJoined) { OnLobbyJoin(); }

		OnPlayerUpdate();
		OnLevelUpdate();
	}

	public void OnLobbyJoin() {

		// Register it
		lock (level.menu.players) {

			foreach (TTSRacerConfig player in level.menu.players) {
				if (player.LocalControlType == (int)TTSRacer.PlayerType.Player) {
					// Create a racer net handler using the config.
					TTSRacerNetHandler handler = new TTSRacerNetHandler(client, player, lobbyID);
				}
			}
		}

		lobbyJoined = false;
	}

	public void OnLevelUpdate() {
		levelText.text = levelTitle;
	}

	public void OnPlayerUpdate() {
		int guiIndex = 0;
		foreach (string value in playerNames) {
			if (value != "") {
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

	#region From Network Thread
	public void JoinLobby(TTSLobby lobby) {
		lobbyID = lobby.ID;
		lobbyJoined = true;
		networkUpdated = true;
	}
	#endregion
}