using UnityEngine;
using System.Collections.Generic;

public class TTSServerMenu : TTSBehaviour {
	public TTSClient client;
	public float lastUpdate;
	public float RequestInterval = 5.0f;

	public bool networkUpdated = true;
	public GameObject lobbyElementGO;

	public List<TTSLobby> lobbies = new List<TTSLobby>();

	// Use this for initialization
	void Start () {
		client = level.client;
		lastUpdate = -5.0f;
	}
	
	// Update is called once per frame
	void Update () {
		AddNewLobbies();
		SetPositions();

		if ((Time.time - lastUpdate) > RequestInterval) {
			Debug.Log("Request");
			client.RequestLobbyInfo(this);
			lastUpdate = Time.time;
		}

		networkUpdated = false;
	}

	public void SetPositions() {
		if (!networkUpdated)
			return;

		OnLobbyUpdate(serverLobbies);

		int inLobbyIndex = 0;
		int inProgressIndex = 0;

		foreach (TTSLobby lobby in lobbies) {
			lobby.SetPosition((lobby.InProgress) ? inProgressIndex : inLobbyIndex);

			if (lobby.InProgress)
				inProgressIndex++;
			else
				inLobbyIndex++;
		}
	}

	List<LobbyData> newLobbies = new List<LobbyData>();
	public void AddNewLobbies() {
		if (newLobbies.Count == 0)
			return;

		foreach (LobbyData lobby in newLobbies) {
			TTSLobby newLobby = ((GameObject)Instantiate(lobbyElementGO)).GetComponent<TTSLobby>();
			newLobby.gameObject.transform.parent = this.transform;

			newLobby.ID = lobby.ID;
			newLobby.Name = lobby.Name;
			newLobby.NumRacers = lobby.NumRacers;
			newLobby.MaxNumRacers = lobby.MaxNumRacers;
			newLobby.InProgress = lobby.InProgress;
			newLobby.BotsEnabled = lobby.BotsEnabled;
			newLobby.Level = (TTSLevel.LevelType)lobby.Level;

			newLobby.networkUpdated = true;

			lobbies.Add(newLobby);
		}

		newLobbies.Clear();
	}

	List<LobbyData> serverLobbies = new List<LobbyData>();
	public void ReceiveLobby(LobbyData lobby) {
		lock (serverLobbies) {
			serverLobbies.Add(lobby);
			networkUpdated = true;
		}
	}

	public void OnLobbyUpdate(List<LobbyData> serverLobbies) {
		lock (serverLobbies) {
			foreach (LobbyData lobby in serverLobbies) {
				TTSLobby existingLobby = lobbies.Find(x => x.ID == lobby.ID);

				if (existingLobby == null) {
					newLobbies.Add(lobby);
				}
				else {
					Debug.Log(lobby.Level);
					existingLobby.Name = lobby.Name;
					existingLobby.NumRacers = lobby.NumRacers;
					existingLobby.MaxNumRacers = lobby.MaxNumRacers;
					existingLobby.InProgress = lobby.InProgress;
					existingLobby.BotsEnabled = lobby.BotsEnabled;
					existingLobby.Level = (TTSLevel.LevelType)lobby.Level;

					existingLobby.networkUpdated = true;
				}
			}
			serverLobbies.Clear();
		}
	}
}

public struct LobbyData
{
	public int ID;// = -1;
	public string Name;// = "Server 1";
	public int NumRacers;// = 0;
	public int MaxNumRacers;// = 6;
	public bool InProgress;// = false;
	public bool BotsEnabled;// = true;
	public int Level;// = 1;
}