using UnityEngine;
using System.Collections.Generic;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using XInputDotNetPure;
#endif

public class TTSServerMenu : TTSBehaviour {
	public TTSClient client;
	public float lastUpdate;
	public float RequestInterval = 1.0f;
	public bool networkUpdated = true;

	public GameObject lobbyElementGO;
	public GameObject highlighter;

	public List<TTSLobby> lobbies = new List<TTSLobby>();
	
	public TTSLobby highlightedLobby;
	public AudioClip hoverSound;
	public List<TTSLobby> inProgressLobbies {
		get {
			return lobbies.FindAll(x => x.InProgress == true);
		}
	}
	
	public List<TTSLobby> inLobbyLobbies {
		get {
			return lobbies.FindAll(x => x.InProgress == false);
		}
	}
	public TTSLobbyMenu lobbyMenu;
	
	#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		PlayerIndex playerIndex;
		GamePadState state;
	#endif

	// Use this for initialization
	void Start () {
		client = level.client;
		client.RequestLobbyInfo(this);
		lastUpdate = -5.0f;
	}
	
	public bool joystickDownY = false;
	// Update is called once per frame
	void Update () {
		AddNewLobbies();
		SetPositions();
		
		if(level.menu.activePanel == 1){
			Vector2 controlDirection = GetControlDirection(1);
			if(Input.GetKeyDown(KeyCode.W) || (controlDirection.y > 0.5f && !joystickDownY)){
				UpButton();
				audio.PlayOneShot(hoverSound);
				joystickDownY = true;
				
			}
			
			else if(Input.GetKeyDown(KeyCode.S) || (controlDirection.y < -0.5f && !joystickDownY)){
				DownButton();
				audio.PlayOneShot(hoverSound);
				joystickDownY = true;
			}
			
			else if(Mathf.Abs(controlDirection.y) < 0.5f && joystickDownY){
				joystickDownY = false;
			}
		}

		if ((Time.time - lastUpdate) > RequestInterval) {
			client.RequestLobbyInfo(this);
			lastUpdate = Time.time;
		}

		networkUpdated = false;
	}

	public void SetPositions() {
		if (!networkUpdated)
			return;
		
		OnLobbyUpdate();

		int inLobbyIndex = 0;
		int inProgressIndex = 0;

		foreach (TTSLobby lobby in lobbies) {
			lobby.SetPosition((lobby.InProgress) ? inProgressIndex : inLobbyIndex);

			if (lobby.InProgress)
				inProgressIndex++;
			else
				inLobbyIndex++;
		}

		if(highlightedLobby == null){
			foreach(TTSLobby lobby in lobbies){
				if(lobby.ID == 0){
					highlighter.guiTexture.pixelInset = lobby.GetPosition();
				}
			}
		}	
		
		else{
			highlighter.guiTexture.pixelInset = highlightedLobby.GetPosition();
		}
	}

	List<LobbyData> newLobbies = new List<LobbyData>();
	public void AddNewLobbies() {
		if (newLobbies.Count == 0)
			return;
		
		networkUpdated = true;
		
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

			if (highlightedLobby == null) {
				SetHighlighter(newLobby);
			}
		}

		newLobbies.Clear();
	}

	List<LobbyData> serverLobbies = new List<LobbyData>();
	public void ReceiveLobby(LobbyData lobby) {
		lock (serverLobbies) {
			int index = serverLobbies.FindIndex(x => x.ID == lobby.ID);
			if (index != -1) {
				serverLobbies[index] = lobby;
			}
			else {
				serverLobbies.Add(lobby);
			}
			networkUpdated = true;
		}
	}

	public void OnLobbyUpdate() {
		lock (serverLobbies) {
			foreach (LobbyData lobby in serverLobbies) {
				TTSLobby existingLobby = lobbies.Find(x => x.ID == lobby.ID);

				if (existingLobby == null) {
					newLobbies.Add(lobby);
				}
				else {
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
	
	public void UpButton(){
		if (lobbies.Count == 0) return;

		if (highlightedLobby == null) {
			highlightedLobby = lobbies[0];
			highlighter.guiTexture.pixelInset = lobbies[0].GetPosition();
		}
		
		bool InProgress = highlightedLobby.InProgress;
		
		List<TTSLobby> currentList = (InProgress)?inProgressLobbies : inLobbyLobbies;
		
		int toIndex = currentList.FindIndex(x => x == highlightedLobby) - 1;

		if(toIndex <= -1){
			if(((InProgress)?inLobbyLobbies : inProgressLobbies).Count != 0){
				currentList = (InProgress)?inLobbyLobbies : inProgressLobbies;
			}
			
			toIndex = currentList.Count - 1;
		}
		
		SetHighlighter(currentList[toIndex]);
	}

	public void DownButton() {
		if (lobbies.Count == 0) return;
		
		if(highlightedLobby == null){
			highlightedLobby = lobbies[0];
			highlighter.guiTexture.pixelInset = lobbies[0].GetPosition();
		}
		
		bool InProgress = highlightedLobby.InProgress;
		
		List<TTSLobby> currentList = (InProgress)?inProgressLobbies : inLobbyLobbies;
		
		int toIndex = currentList.FindIndex(x => x == highlightedLobby) + 1;

		if(toIndex >= currentList.Count){
			if(((InProgress)?inLobbyLobbies : inProgressLobbies).Count != 0){
				currentList = (InProgress)?inLobbyLobbies : inProgressLobbies;
			}
			
			toIndex = 0;
		}
		
		SetHighlighter(currentList[toIndex]);
	}

	public Texture availableHighlighter;
	public Texture unavailableHighlighter;
	private void SetHighlighter(TTSLobby destination) {
		highlightedLobby = destination;
		highlighter.guiTexture.pixelInset = destination.GetPosition();

		if (availableHighlighter == null)
			return;

		if (highlightedLobby.InProgress) {
			highlighter.guiTexture.texture = unavailableHighlighter;
		}
		else {
			highlighter.guiTexture.texture = availableHighlighter;
		}
	}

	public void JoinLobby() {
		if (!highlightedLobby.InProgress) {
			// Send request to server
			client.ConnectToLobby(highlightedLobby.ID, this, lobbyMenu);
			// Wait to join server screen
		}
	}

	#region network thread
	public void OnLobbyJoin(int lobbyID) {
		TTSLobby joinedLobby = lobbies.Find(x => x.ID == lobbyID);
		lobbyMenu.JoinLobby(joinedLobby);
	}
	#endregion

	Vector2 GetControlDirection(int player){
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
		// float VInput = state.ThumbSticks.Left.Y;
		// float HInput = state.ThumbSticks.Left.X;

		// Debug.Log(state.DPad.Up + " " + state.DPad.Down + " " + state.DPad.Left + " " + state.DPad.Right);

		float VInput = (state.DPad.Up == ButtonState.Pressed) ? 1 : ((state.DPad.Down == ButtonState.Pressed)? -1 : 0);
		float HInput = (state.DPad.Right == ButtonState.Pressed) ? 1 : ((state.DPad.Left == ButtonState.Pressed)? -1 : 0);

		//Debug.Log(VInput + " " + HInput + " " + joystickDownY + joystickDownX);

		return new Vector2(HInput, VInput);
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