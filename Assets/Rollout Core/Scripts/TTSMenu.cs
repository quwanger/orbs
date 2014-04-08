using UnityEngine;
using System.Collections.Generic;
using System.Collections;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using XInputDotNetPure;
#endif

public class TTSMenu : TTSBehaviour {
	
	// Lists of rigs, perks, and levels
	public List<GameObject> _rigs = new List<GameObject>();
	public RigType SelectedRig;
	
	public List<GameObject> _perks = new List<GameObject>();
	public PerkType SelectedPerk;
	
	public List<GameObject> _perksB = new List<GameObject>();
	public PowerupType SelectedPerkB;
	
	public List<GameObject> _levels = new List<GameObject>();
	public LevelMenuItem SelectedLevel;

	public List<TTSRacerConfig> players = new List<TTSRacerConfig>();
	public GameObject playerPackage;
	
	public string[] playersRigs;
	public string[] playersPerkA;
	public string[] playersPerkB;
	
	public bool createdSinglePlayer;
	
	public AudioClip backSound;
	public AudioClip hoverSound;
	public AudioClip selectSound;
	public AudioClip transSound;
	
	public GameObject dtp;
	
	public bool joystickDownX = false;
	public bool joystickDownY = false;
	
	// cameras
	// mainCamera		0
	// hubCamera3D		1
	// hubCameraGUI		2
	public Camera[] cameras;
	
	// player statistics gameObject folder
	public GameObject playerStatistics;
	
	public GameObject[] characterColor;
	private int activeColorIndex;
	public int[] playerID;
	public int[] playerControllerID;
	public int[] ID;
	
	
	public bool[] playerReady;
	
	// dynamic text fields for name and description of perk and rigs
	public GameObject perkName;
	public GameObject perkDescription;
	public GameObject rigName;
	
	// prevent menus from tweening at the same time
	public bool isTweening = false;
	
	// rigs
	private GameObject[] rigs;
	
	// varaibles to store information for the dynamic circle creation
	public Texture2D circleImage;
	
	public List<GameObject> acceleration_circles = new List<GameObject>();
	public List<GameObject> speed_circles = new List<GameObject>();
	public List<GameObject> handling_circles = new List<GameObject>();
	public List<GameObject> offense_circles = new List<GameObject>();
	public List<GameObject> defense_circles = new List<GameObject>();
	
	public GameObject[] playerText;
	
	// to control the orbs physics
	public TTSRacer racer;
	
	//Fade Variables
	private float alphaFadeValue = 0.0f;
    private float transitionTimeIn = 0.1f;
	//public Texture Overlay;
	
	public GameObject[] readyUp;
	public GameObject[] readyUpB;
	
	//Spawn zones
	public GameObject spawn_mp;
	public GameObject spawn_sp;
	
	// acceleration	0
	// speed		1
	// handling		2
	// offense		3
	// defense		4
	public int[] numCircles;
	
	// 0 MP Select	1		1x2
	// 1 MP MENU	1		2x1
	// 2 MP LOBBY	1		2x1
	// 3 READYUP	
	// 4 RIG		11		2x3
	// 5 PERKA		11		3x3
	// 6 PERKB		11		3x3
	// 7 LEVEL		11		3x2
	// 8 BLANK		
	public GameObject[] panels;
	
	// indices for each panel
	public int[] indices;
	
	public GameObject topHighlighter;
	public GameObject botHighlighter;
	
	// visible panel and the previously visible one
	public int activePanel;
	public int previousPanel;
	
	// how many players, and how many have selected orbs
	public int numPlayers = 0;
	public int chosenOrb = 1;
	
	// Is either a string saying multiplayer or singleplayer
	public TTSLevel.Gametype gameMode;

	// Server Menu
	public TTSServerMenu serverMenu;

	//XInput
	#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		PlayerIndex playerIndex;
		GamePadState state;
	#endif

	public 

	// Use this for initialization
	void Start () {		
		players.Clear();
		previousPanel = 8;
		
		playerText = GameObject.FindGameObjectsWithTag("playerText");
		
		Camera.main.enabled = true;
		cameras[1].enabled = false;	
		
		// populate arrays with their gameObjects
		GameObject[] perks = GameObject.FindGameObjectsWithTag("PerkMenuItem");
		GameObject[] perksB = GameObject.FindGameObjectsWithTag("PerkMenuItemB");
		GameObject[] levels = GameObject.FindGameObjectsWithTag("LevelMenuItem");
		rigs = GameObject.FindGameObjectsWithTag("RigMenuItem");
		
		botHighlighter.SetActive(false);
		
		foreach(GameObject r in rigs)
			_rigs.Add(r);
		
		foreach(GameObject p in perks)
			_perks.Add(p);

		foreach(GameObject p in perksB)
			_perksB.Add(p);

		foreach(GameObject l in levels)
			_levels.Add(l);
		
		// set color of perk description font
		perkDescription.guiText.font.material.color = new Color32(70, 70, 70, 255);

		// set the indices for each panel
		for(int i = 0; i < 3; i++)
			indices[i] = 1;
		
		for(int i = 4; i < 8; i++)
			indices[i] = 11;
		
		// create the circles for the stats
		createCircles("Acceleration", -430, -152);
		createCircles("Speed", -430, -122);
		createCircles("Handling", -430, -182);
		createCircles("Defense", -455, 98);
		createCircles("Offense", -455, 128);
		
		HighlightRig();
		HighlightPerk();
		HighlightPerkB();
		HighlightLevel();
	}
	
	bool isAPressed = false;
	bool isBPressed = false;
	bool isYPressed = false;

	// Update is called once per frame
	void Update () {
		Debug.Log(gameMode);

		if (gameMode != TTSLevel.Gametype.Lobby) {
			
			playerText[0].guiText.text = ("Player" + (chosenOrb));
			playerText[1].guiText.text = ("Player" + (chosenOrb));
			
			
			string tempJoystick = "joystick 1 button 0";
			string tempJoystickB = "joystick 1 button 3";

			if (gameMode == TTSLevel.Gametype.MultiplayerLocal) {
				tempJoystick = ("joystick " + playerControllerID[chosenOrb-1] + " button 0");
				tempJoystickB = ("joystick " + playerControllerID[chosenOrb-1] + " button 3");
			}
			
			if(activePanel == 4 || activePanel == 5 || activePanel == 6){
				// Y BUTTON RIG SELECT
				if((GetButtonDown(playerControllerID[chosenOrb-1], "Y") && !isYPressed) || Input.GetKeyDown(KeyCode.Y)){
					isYPressed = true;
					if (activePanel == 4) {
						characterColor[activeColorIndex].guiTexture.enabled = false;

						activeColorIndex = (activeColorIndex + 1) % characterColor.Length;
						characterColor[activeColorIndex].guiTexture.enabled = true;
					}
				} else if(GetButtonDown(playerControllerID[chosenOrb-1], "Y") == false){
					isYPressed = false;
				}
				
				// if(Input.GetKeyDown(tempJoystick)){
				if(GetButtonDown(playerControllerID[chosenOrb-1], "A") && !isAPressed){
					isAPressed = true;
					changePanels("right");
				} else if(GetButtonDown(playerControllerID[chosenOrb-1], "A") == false) {
					isAPressed = false;
				}
			}
			
			// else if()
			// {
			// 	// if(Input.GetKeyDown("joystick 1 button 0")){
			// 	if(GetButtonDown(playerControllerID[chosenOrb-1], "A") && !isAPressed){
			// 		isAPressed = true;
			// 		changePanels("right");
			// 	} else if(GetButtonDown(playerControllerID[chosenOrb-1], "A") == false) {
			// 		isAPressed = false;
			// 	}
			// }	
			else if(activePanel == 0 || activePanel == 1 || activePanel == 2 || activePanel == 7 || activePanel == 3){
				// if(Input.GetKeyDown("joystick 1 button 0")){
				if(GetButtonDown(1, "A") && !isAPressed){
					isAPressed = true;
					changePanels("right");
				} else if(GetButtonDown(1, "A") == false) {
					isAPressed = false;
				}
			}

			//if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick 1 button 7"))
			if(Input.GetKeyDown(KeyCode.Return))
				changePanels("right");
			
			if(Input.GetKeyDown(KeyCode.Backspace) || (GetButtonDown(playerControllerID[chosenOrb-1], "B") && !isBPressed)){//Input.GetKeyDown("joystick 1 button 1"))
				changePanels("left");
				isBPressed = true;
			} else if(GetButtonDown(playerControllerID[chosenOrb-1], "B") == false){
				isBPressed = false;
			}

			//if(panels[4].transform.position.x == 0.5 || panels[5].transform.position.x == 0.5 || 
			//   panels[6].transform.position.x == 0.5 || panels[7].transform.position.x == 0.5)
			//	playerStatistics.SetActive(true);
			//else
			//	playerStatistics.SetActive(false);
			
			menuControls();
			
			racer.canMove = false;
			racer.transform.rotation = Quaternion.Euler(0, 0, 180);
			racer.calcOrientation = false;
			racer.ManualOrientation(new Vector3(spawn_sp.transform.position.x, spawn_sp.transform.position.y, spawn_sp.transform.position.z));
			racer.SlowToStopToPosition(spawn_mp);
			racer.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
				
			Camera.main.enabled = false;
			cameras[1].enabled = true;
		}
		int tempPlayers = numPlayers+1;
		
		if((gameMode == TTSLevel.Gametype.MultiplayerOnline || gameMode == TTSLevel.Gametype.TimeTrial)  && createdSinglePlayer == false){
		//if((gameMode == TTSLevel.Gametype.MultiplayerOnline || gameMode == TTSLevel.Gametype.TimeTrial)  && activePanel == 4 && !isTweening){

			//Debug.Log(activePanel);
			// if((Input.GetKeyDown("joystick " + tempPlayers + " button 0") || Input.GetKeyDown(KeyCode.Return)) && playerReady[0] == false){
			//if(((GetButtonDown(playerControllerID[chosenOrb-1], "A") && !isAPressed) || Input.GetKeyDown(KeyCode.Return)) && playerReady[0] == false){
				//Debug.Log("hello");
				createdSinglePlayer = true;
				// if(activePanel == 6){
				playerReady[0] = true;
				playerID[numPlayers] = numPlayers + 1;
				playerControllerID[numPlayers] = 1;
				numPlayers++;
			
				TTSRacerConfig tempConfig = new TTSRacerConfig();
				tempConfig.ControllerID = numPlayers;
				tempConfig.Index = players.Count;
			
				players.Add(tempConfig);
				Debug.Log("players " + players.Count);
				// }
			// }
			//else if(GetButtonDown(playerControllerID[chosenOrb-1], "A") == false){
			//	isAPressed = false;
			//}
		}
		
		if(gameMode == TTSLevel.Gametype.MultiplayerLocal && activePanel == 3 && !isTweening){
			// if(Input.GetKeyDown("joystick 1 button 7") && playerReady[0] == false){
			if(GetButtonDown(1, "Start") && playerReady[0] == false){
				playerReady[0] = true;
				playerID[numPlayers] = numPlayers + 1;
				playerControllerID[numPlayers] = 1;
				readyUpB[numPlayers].SetActive(false);
				readyUp[numPlayers].SetActive(true);
				numPlayers++;

				TTSRacerConfig tempConfig = new TTSRacerConfig();
				tempConfig.ControllerID = numPlayers;
				tempConfig.Index = players.Count;

				players.Add(tempConfig);
			}
			
			if(GetButtonDown(2, "Start") && playerReady[1] == false){
				playerReady[1] = true;
				playerID[numPlayers] = numPlayers + 1;
				playerControllerID[numPlayers] = 2;
				readyUpB[numPlayers].SetActive(false);
				readyUp[numPlayers].SetActive(true);
				numPlayers++;

				TTSRacerConfig tempConfig = new TTSRacerConfig();
				tempConfig.ControllerID = numPlayers;
				tempConfig.Index = players.Count;

				players.Add(tempConfig);
			}
			
			if(GetButtonDown(3, "Start") && playerReady[2] == false){
				playerReady[2] = true;
				playerID[numPlayers] = numPlayers + 1;
				playerControllerID[numPlayers] = 3;
				readyUpB[numPlayers].SetActive(false);
				readyUp[numPlayers].SetActive(true);
				numPlayers++;

				TTSRacerConfig tempConfig = new TTSRacerConfig();
				tempConfig.ControllerID = numPlayers;
				tempConfig.Index = players.Count;

				players.Add(tempConfig);
			}
			
			if(GetButtonDown(4, "Start") && playerReady[3] == false){
				playerReady[3] = true;
				playerID[numPlayers] = numPlayers + 1;
				playerControllerID[numPlayers] = 4;
				readyUpB[numPlayers].SetActive(false);
				readyUp[numPlayers].SetActive(true);
				numPlayers++;

				TTSRacerConfig tempConfig = new TTSRacerConfig();
				tempConfig.ControllerID = numPlayers;
				tempConfig.Index = players.Count;

				players.Add(tempConfig);
			}
		}
	}
	
	private void menuControls(){

		// if statement is to remove an error when starting a level
		int index = 7;
		if(activePanel != 8)
			index = indices[activePanel];

		string tempJoystick = "DPad_XAxis_1";
		string tempJoystickB = "DPad_YAxis_1";
		
		if(gameMode == TTSLevel.Gametype.MultiplayerLocal){	
			tempJoystick = ("DPad_XAxis_" + playerControllerID[chosenOrb-1]);
			tempJoystickB = ("DPad_YAxis_" + playerControllerID[chosenOrb-1]);
		}

		Vector2 controlDirection = GetControlDirection(playerControllerID[chosenOrb-1]);
		
		if(activePanel == 4 || activePanel == 5 || activePanel == 6){
			// CONTROLLER
			if(Mathf.Abs(controlDirection.x) > 0.5f && !joystickDownX){
				joystickDownX = true;
				if(controlDirection.x > 0){
					//right
					ChangeIndex("right", index);
				}else if (controlDirection.x < 0){
					//left
					ChangeIndex("left", index);
				}
			}else if(Mathf.Abs(controlDirection.x) == 0 && joystickDownX){
				joystickDownX = false;
			}
			
			if(Mathf.Abs(controlDirection.y) > 0.5f && !joystickDownY){
				joystickDownY = true;
				// if(Input.GetAxisRaw(tempJoystickB) == 1){
				if(controlDirection.y > 0){
					//up
					ChangeIndex("up", index);
				}else if (controlDirection.y < 0){
					//down
					ChangeIndex("down", index);
				}
			}else if(controlDirection.y == 0 && joystickDownY){
				joystickDownY = false;
			}
		}
		
		//player one should control panels 0, 1, 2, and 7
		else if(activePanel == 0 || activePanel == 1 || activePanel == 2 || activePanel == 7)
		{
			controlDirection = GetControlDirection(1);
			// CONTROLLER
			// if(Input.GetAxisRaw("DPad_XAxis_1") != 0 && !joystickDownX){
			if(Mathf.Abs(controlDirection.x) > 0.5f && !joystickDownX){
				joystickDownX = true;
				if(controlDirection.x > 0){
					//right
					ChangeIndex("right", index);
				}else if(controlDirection.x < 0){
					//left
					ChangeIndex("left", index);
				}
			}else if(controlDirection.x == 0 && joystickDownX){
				joystickDownX = false;
			}
			
			// if(Input.GetAxisRaw("DPad_YAxis_1") != 0 && !joystickDownY){
			if(Mathf.Abs(controlDirection.y) > 0.5f && !joystickDownY){
				joystickDownY = true;
				// if(Input.GetAxisRaw("DPad_YAxis_1") == 1){
				if(controlDirection.y > 0){
					//up
					ChangeIndex("up", index);
				// }else{
				}else if(controlDirection.y < 0){
					//down
					ChangeIndex("down", index);
				}
			}else if(controlDirection.y == 0 && joystickDownY){
				joystickDownY = false;
			}
		}
		
		// KEYBOARD
		if(Input.GetKeyDown(KeyCode.W))
			ChangeIndex("up", index);
		else if(Input.GetKeyDown(KeyCode.S))
			ChangeIndex("down", index);
		else if(Input.GetKeyDown(KeyCode.A))
			ChangeIndex("left", index);
		else if(Input.GetKeyDown(KeyCode.D))
			ChangeIndex("right", index);
	}
	
	private void ChangeIndex(string direction, int index){	
		
		// 2x3 menu system
		if(activePanel == 7){
			if(direction == "left"){
				if(index == 12 || index == 22 || index == 32){
					index -= 1;
					audio.PlayOneShot(hoverSound);
				}
			}

			else if(direction == "right"){
				if(index == 11 || index == 21 || index == 31){
					index += 1;
					audio.PlayOneShot(hoverSound);
				}
			}
			
			else if(direction == "down"){
				if(index < 30){
					index += 10;
					audio.PlayOneShot(hoverSound);
				}
			}

			else if(direction == "up"){
				if(index > 20){
					index -= 10;
					audio.PlayOneShot(hoverSound);
				}
			}
		}
		
		// 3x2 menu system
		if(activePanel == 4){
			if(direction == "left"){
				if(index%10 > 1){
					index -= 1;
					audio.PlayOneShot(hoverSound);
				}
			}
			
			else if(direction == "right"){
				if(index%10 < 3){
					index += 1;
					audio.PlayOneShot(hoverSound);
				}
			}
			
			else if(direction == "down"){
				if(index==11 || index==12 || index==13){
					index += 10;
					audio.PlayOneShot(hoverSound);
				}
			}
			
			else if(direction == "up"){
				if(index==21 || index==22 || index==23){
					index -= 10;
					audio.PlayOneShot(hoverSound);
				}
			}
		}
		
		// 3x3 menu system
		if(activePanel == 5 || activePanel == 6){
			if(direction == "right"){
				if(index%10 == 1 || index%10 == 2){
					index ++;
					audio.PlayOneShot(hoverSound);
				}
			}
			
			if(direction == "left"){
				if(index%10 == 2 || index%10 == 3){
					index --;
					audio.PlayOneShot(hoverSound);
				}
			}
			
			if(direction == "up"){
				if(index>20 && index<34){
					index -= 10;
					audio.PlayOneShot(hoverSound);
				}
			}
			
			if(direction =="down"){
				if(index>10 && index<24){
					index += 10;
					audio.PlayOneShot(hoverSound);
				}
			}			
		}
		
		// 2x1
		if(activePanel == 1 || activePanel == 2){
			if(direction == "right"){
				if(index == 1){
					index = 2;
					audio.PlayOneShot(hoverSound);
				}
			}
				
			if(direction == "left"){
				if(index == 2){
					index = 1;
					audio.PlayOneShot(hoverSound);
				}
			}
		}
		
		// 1x2
		if(activePanel == 0){
			if(direction == "up"){
				if(index == 2){
					index = 1;
					audio.PlayOneShot(hoverSound);
				}
			}
				
			if(direction == "down"){
				if(index == 1){
					index = 2;
					audio.PlayOneShot(hoverSound);
				}
			}
		}

		// update the indice
		indices[activePanel] = index;
				
		// call appropriate panel functions
		switch(activePanel){
			case 0:
				HighlightMPSelect();
			break;
			
			case 1:
				HighlightMPMenu();
			break;
			
			case 2:
				HighlightMPLobby();
			break;
			
			case 3:
				HighlightReadyup();
			break;
			
			case 4:
				HighlightRig();
			break;
						
			case 5:
				HighlightPerk();
			break;
					
			case 6:
				HighlightPerkB();
			break;
					
			case 7:
				HighlightLevel();
			break;

			default:
			//Play a sound?
			break;
		}
	}

	public void SaveRacers() {
		dtp.GetComponent<TTSDataToPass>().players = this.players;
		dtp.GetComponent<TTSDataToPass>().players.AddRange(level.client.RegisteredRacerConfigs);
		dtp.GetComponent<TTSDataToPass>().gametype = gameMode;
	}
	
	private void changePanels(string direction){
		
		// TIMETRIAL
		if(direction == "right"){
			if(gameMode == TTSLevel.Gametype.TimeTrial)
			{
				// if you're on the rig panel and it's not tweening:
				//		set isTweening to true
				//		set PreviousPanel
				//		increment activePanel
				//		move the panels
				if(activePanel == 4 && !isTweening){
					isTweening = true;
					previousPanel = activePanel;
					activePanel++;
					movePanel();
				}
				
				else if(activePanel == 5 && !isTweening){
					previousPanel = activePanel;
					activePanel++;
				}
				
				else if(activePanel == 6){
					isTweening = true;
					previousPanel = activePanel;
					activePanel++;
					
					foreach (TTSRacerConfig player in players) {
						if (player.ControllerID == chosenOrb) {
							player.PerkA = (int)SelectedPerk;
							player.PerkB = (int)SelectedPerkB;
							player.RigType = (int)SelectedRig;
							player.CharacterType = (int)characterColor[activeColorIndex].GetComponent<TTSCharacter>().characterType;
							player.racerID = playerID[chosenOrb - 1];
							player.racerControllerID = playerControllerID[chosenOrb - 1];
							
							Debug.Log("Player " + player.racerID + " is controller by controller " + player.racerControllerID);
						}
					}
					
					dtp.GetComponent<TTSDataToPass>().players = this.players;
					dtp.GetComponent<TTSDataToPass>().gametype = gameMode;
					
					movePanel();
				}
				
				else if(activePanel == 7 && !isTweening){
					previousPanel = activePanel;
					levelSelection();
				}
			}
		 
			// ONLINE
			else if (gameMode == TTSLevel.Gametype.MultiplayerOnline) {
				// mp 
				if(activePanel == 0 && !isTweening){
					activePanel += 4;
					previousPanel = (activePanel - 4);
					isTweening = true;
					movePanel();
				}
				
				else if((activePanel == 4 || activePanel == 5 || activePanel == 6) && !isTweening){
					activePanel++;
					if(activePanel == 5){
						if(!isTweening){
							previousPanel = (activePanel-1);
							isTweening = true;
							movePanel();
						}
					}

					else if (activePanel == 7 && !isTweening) {
						foreach (TTSRacerConfig player in players) {
							if (player.ControllerID == chosenOrb) {
								player.PerkA = (int)SelectedPerk;
								player.PerkB = (int)SelectedPerkB;
								player.RigType = (int)SelectedRig;
								player.CharacterType = (int)characterColor[activeColorIndex].GetComponent<TTSCharacter>().characterType;
								player.racerID = playerID[chosenOrb - 1];
								player.racerControllerID = playerControllerID[chosenOrb - 1];
							}
						}
						// go to mp menu
						activePanel = 1;
						previousPanel = 6;
						isTweening = true;
						movePanel();
					}
				}
				
				// mp lobby
				else if (activePanel == 1 && !isTweening) {
					serverMenu.JoinLobby();
					activePanel++;
					previousPanel = (activePanel-1);
					isTweening = true;
					movePanel();
				}
			}
			
			// SPLITSCREEN
			else if (gameMode == TTSLevel.Gametype.MultiplayerLocal) {
				if(activePanel == 0 && !isTweening){
					isTweening = true;
					previousPanel = activePanel;
					
					if(indices[0] == 1){
						gameMode = TTSLevel.Gametype.MultiplayerOnline;
						activePanel++;
					}
					
					else if(indices[0] == 2){
						gameMode = TTSLevel.Gametype.MultiplayerLocal;
						activePanel+= 3;

					}
					movePanel();
				}
				
				else if(activePanel == 3 && !isTweening){
					isTweening = true;
					previousPanel = activePanel;
					activePanel++;
					movePanel();
				}

				else if(activePanel == 4 && !isTweening){
					isTweening = true;
					previousPanel = activePanel;
					activePanel++;
					movePanel();
				}
				
				else if(activePanel == 5 && !isTweening){
					previousPanel = activePanel;
					activePanel++;
				}
				
				else if(activePanel == 6){	
					foreach (TTSRacerConfig player in players) {
						if (player.ControllerID == chosenOrb) {
							player.PerkA = (int)SelectedPerk;
							player.PerkB = (int)SelectedPerkB;
							player.RigType = (int)SelectedRig;
							player.CharacterType = (int)characterColor[activeColorIndex].GetComponent<TTSCharacter>().characterType;
							player.racerID = playerID[chosenOrb - 1];
							player.racerControllerID = playerControllerID[chosenOrb - 1];
							
							Debug.Log("Player " + player.racerID + " is controller by controller " + player.racerControllerID);
						}
					}
					
					// go to levelSelect
					if (chosenOrb == numPlayers) {
						Debug.Log("single player");
						previousPanel = activePanel;
						activePanel++;
						dtp.GetComponent<TTSDataToPass>().players = this.players;
						dtp.GetComponent<TTSDataToPass>().gametype = gameMode;
						movePanel();
					}
					
					// go to rigMenu
					else if(chosenOrb != numPlayers && !isTweening){
						activePanel = 4;
						previousPanel = 6;
						chosenOrb++;
						movePanel();
					}	
					
					isTweening = true;
				}
				
				else if(activePanel == 7 && !isTweening){
					previousPanel = activePanel;
					levelSelection();
				}
			}

		}

		if(direction == "left"){
			if(gameMode == TTSLevel.Gametype.TimeTrial){
				if(activePanel == 4 && !isTweening){
					// go back to hub world
				}
				
				else if(activePanel == 5 && !isTweening){
					// go back to rig select
				}
				
				else if(activePanel == 6 && !isTweening){
					// go back to perk 1
				}
				
				else if(activePanel == 7 && !isTweening){
					// go back to perk 2
				}
			}
		}
	}
	
	private void levelSelection(){
		if(SelectedLevel.ToString() == "level1"){
				dtp.GetComponent<TTSDataToPass>().levelToLoad = "city1-1";
				Application.LoadLevel("city1-1");
			}
		
			else if(SelectedLevel.ToString() == "level2"){
				dtp.GetComponent<TTSDataToPass>().levelToLoad = "city1-2";
				Application.LoadLevel("city1-2");
			}
							
			else if(SelectedLevel.ToString() == "level3"){
				dtp.GetComponent<TTSDataToPass>().levelToLoad = "rural1-1";
				Application.LoadLevel("rural1-1");
			}
				
			else if(SelectedLevel.ToString() == "level4"){
				dtp.GetComponent<TTSDataToPass>().levelToLoad = "cliffsidechoas";
				Application.LoadLevel("cliffsidechoas");
			}

			else if(SelectedLevel.ToString() == "level5"){
				dtp.GetComponent<TTSDataToPass>().levelToLoad = "future1-1";
				Application.LoadLevel("future1-1");
			}
			
			Application.LoadLevel("LoadingScene");
	}

	public void movePanel() {
		if(activePanel == 4 || activePanel == 5 || activePanel == 6)
			playerStatistics.transform.parent = panels[activePanel].transform;
		
		audio.PlayOneShot(transSound);

		// move in next panel
		iTween.MoveTo(panels[activePanel], iTween.Hash("x", 0.5, "time", 1.0f, "onComplete", "stoppedTweening", "onCompleteTarget", gameObject));
				
		// move out last panel
		iTween.MoveTo(panels[previousPanel], iTween.Hash("x", 5, "time", 1.0f, "onComplete", "stoppedTweening", "onCompleteTarget", gameObject));
	}
	
	public void stoppedTweening(){
		isTweening = false;	
	}
	
	private void HighlightReadyup(){
	}
	
	private void HighlightMPMenu(){
	}
	
	private void HighlightMPLobby(){
	}
	
	private void HighlightMPSelect(){
		if(indices[0] == 1){
			//gameMode = TTSLevel.Gametype.MultiplayerOnline;
			topHighlighter.SetActive(true);
			botHighlighter.SetActive(false);
		}
		
		else if(indices[0] == 2){
			//gameMode = TTSLevel.Gametype.MultiplayerLocal;
			topHighlighter.SetActive(false);
			botHighlighter.SetActive(true);
		}
	}
	
	private void HighlightLevel(){
		foreach(GameObject l in _levels){
			if(l.GetComponent<TTSMenuItemLevel>().index!=indices[7])
				l.SetActive(false);
			
			else{
				SelectedLevel = l.GetComponent<TTSMenuItemLevel>().level;
				l.SetActive(true);
			}
		}
	}
	
	private void HighlightPerk(){
		deactiveCircles();
		foreach(GameObject p in _perks){	
			if(p.GetComponent<TTSMenuItemPerk>().index!=indices[5])
				p.SetActive(false);
			
			else{
				SelectedPerk = p.GetComponent<TTSMenuItemPerk>().perk;
				p.SetActive(true);
				perkName.guiText.text = p.GetComponent<TTSMenuItemPerk>().name;
				perkDescription.guiText.text = p.GetComponent<TTSMenuItemPerk>().description;
					
				foreach(GameObject r in _rigs){
					if(r.GetComponent<TTSMenuItemRig>().rig == SelectedRig){
						toggleAllCircles();
				
						if(p.GetComponent<TTSMenuItemPerk>().name == "Acceleration")
							toggleCircles("Acceleration", numCircles[0] + 3);
						
						if(p.GetComponent<TTSMenuItemPerk>().name == "Speed")
							toggleCircles("Speed", numCircles[1]  + 3);
						
						if(p.GetComponent<TTSMenuItemPerk>().name == "Handling")
							toggleCircles("Handling", numCircles[2]  + 3);
						
						if(p.GetComponent<TTSMenuItemPerk>().name == "MAN-O-WAR")
							toggleCircles("Offense", numCircles[3]  + 3);
						
						if(p.GetComponent<TTSMenuItemPerk>().name == "Diamond Coat")
							toggleCircles("Defense", numCircles[4]  + 3);

						if(p.GetComponent<TTSMenuItemPerk>().name == "Evolution") {
							toggleCircles("Acceleration", numCircles[0] + 1);
							toggleCircles("Speed", numCircles[1] + 1);
							toggleCircles("Handling", numCircles[2] + 1);
						}
					}
				}
			}
		}
	}
	
	private void HighlightPerkB(){
		foreach(GameObject p in _perksB){	
			if(p.GetComponent<TTSMenuItemPerk>().index!= indices[6])
				p.SetActive(false);
			
			else{
				SelectedPerkB = p.GetComponent<TTSMenuItemPerk>().perkB;
				p.SetActive(true);
				perkName.guiText.text = p.GetComponent<TTSMenuItemPerk>().name;
				perkDescription.guiText.text = p.GetComponent<TTSMenuItemPerk>().description;	
			}
		}
	}
	
	private void HighlightMultiplayer(){
	}
	
	private void HighlightRig(){
		deactiveCircles();
		foreach(GameObject r in _rigs){	
			if(r.GetComponent<TTSMenuItemRig>().index!=indices[4]){
				r.GetComponent<TTSMenuItemRig>().rigImage.enabled = false;
				r.SetActive(false);
			}

			else{
				SelectedRig = r.GetComponent<TTSMenuItemRig>().rig;
				r.GetComponent<TTSMenuItemRig>().rigImage.enabled = true;
				r.SetActive(true);
				
				numCircles[0]  = r.GetComponent<TTSMenuItemRig>().acceleration;
				numCircles[1]  = r.GetComponent<TTSMenuItemRig>().speed;
				numCircles[2]  = r.GetComponent<TTSMenuItemRig>().handling;
				numCircles[3]  = r.GetComponent<TTSMenuItemRig>().offense;
				numCircles[4]  = r.GetComponent<TTSMenuItemRig>().defense;
				
				rigName.guiText.text = SelectedRig.ToString();	
				
				toggleAllCircles();
			}	
		}
	}
	
	// Circles x and y are 
	// X -> -430		Y -> -152 (acc), -122 (speed), -182 (handling)
	// X -> -455		Y -> 98 (def), 128 (off)
	
	void deactiveCircles(){
		foreach(GameObject ac in acceleration_circles)
			ac.SetActive(false);	
		
		foreach(GameObject sc in speed_circles)
			sc.SetActive(false);	
		
		foreach(GameObject hc in handling_circles)
			hc.SetActive(false);
		
		foreach(GameObject oc in offense_circles)
			oc.SetActive(false);
		
		foreach(GameObject dc in defense_circles)
			dc.SetActive(false);				
	}
	
	// turns on all the appropriate circles
	void toggleCircles(string circleName, int tempNumCircles){
		for(int i = 1; i <= tempNumCircles; i++){
			if(circleName == "Acceleration")
				acceleration_circles[i-1].SetActive(true);
			if(circleName == "Speed")
				speed_circles[i-1].SetActive(true);
			if(circleName == "Handling")
				handling_circles[i-1].SetActive(true);
			if(circleName == "Offense")
				offense_circles[i-1].SetActive(true);
			if(circleName == "Defense")
				defense_circles[i-1].SetActive(true);					
		}	
	}
	
	// passes all the circles into toggleCircles
	void toggleAllCircles(){
		toggleCircles("Acceleration", numCircles[0]);
		toggleCircles("Speed", numCircles[1]);
		toggleCircles("Handling", numCircles[2]);
		toggleCircles("Offense", numCircles[3]);
		toggleCircles("Defense", numCircles[4]);
	}
	
	// creates all the circles and puts them into arrays
	void createCircles(string parent, int x, int y){
		for(int i = 1; i <= 13; i++){
			GameObject obj = new GameObject("circle_" + parent + "_" + i.ToString());
			obj.AddComponent("GUITexture");
			obj.transform.parent = GameObject.Find(parent).transform;
			obj.layer = 24;
			obj.transform.localPosition = new Vector3(0,0,5);
			obj.transform.localScale = Vector3.zero;
			obj.guiTexture.pixelInset = new Rect((x+(i*15)), y, 10, 10);
			
			obj.guiTexture.texture = circleImage;
			
			if(parent == "Acceleration")
				acceleration_circles.Add(obj);
			
			if(parent == "Speed")
				speed_circles.Add(obj);
			
			if(parent == "Handling")
				handling_circles.Add(obj);
			
			if(parent == "Offense")
				offense_circles.Add(obj);
			
			if(parent == "Defense")
				defense_circles.Add(obj);
			
			obj.SetActive(false);
		}
	}
	
	void OnGUI(){
		if(gameMode != TTSLevel.Gametype.Lobby){
        	GUI.color = new Color(0, 0, 0, alphaFadeValue);
       		//GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height ), Overlay);
		}
	}

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
