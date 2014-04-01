using UnityEngine;
using System.Collections.Generic;

public class TTSInitRace : MonoBehaviour
{
	public bool DebugMode = false;
	public List<GameObject> Rigs = new List<GameObject>();
	public List<GameObject> Characters = new List<GameObject>();
	public List<GameObject> StartingPoints = new List<GameObject>();

	public GameObject racerGO;
	public GameObject cameraGO;
	public GameObject hudGO;
	public GameObject minimapGO;
	public GameObject playericonSmall;
	public GameObject playericonBig;

	private int startingPointIndex = 0;

	//private string tempRigChoice = "Rig_Rhino";
	private string tempRigChoice;
	private string tempCharacterChoice = "Character_Default";
	public int numHumanPlayers = 1;
	public int numAIPlayers = 0;
	
	TTSLevel level;

	public List<TTSRacerConfig> racerConfigs;
	
	// ?
	//public enum Rigs {Rhino, Scorpion, Default};
	//public enum Characters {Character_Default, Character1, Character2};
	public TTSLevel.Gametype gameType;

	// Use this for initialization
	void Start() {
		
		// Menu passing in data
		GameObject dataToPass = GameObject.Find("DataToPass");
		if(dataToPass && dataToPass.GetComponent<TTSDataToPass>().gametype != TTSLevel.Gametype.Lobby){
			racerConfigs = GameObject.Find("DataToPass").GetComponent<TTSDataToPass>().players;
			gameType = GameObject.Find("DataToPass").GetComponent<TTSDataToPass>().gametype;

			numHumanPlayers = racerConfigs.FindAll(IsHuman).Count;

			foreach (TTSRacerConfig config in racerConfigs) {
				Debug.Log(config.ControllerID);
				Debug.Log((TTSBehaviour.RigType)config.RigType);
				Debug.Log((TTSBehaviour.PerkType)config.PerkA);
				Debug.Log((TTSBehaviour.PowerupType)config.PerkB);
				Debug.Log(config.Index);
			}

			Debug.Log(gameType);
		}

		level = GetComponent<TTSLevel>();

		if (DebugMode || racerConfigs == null) {
			Debug.Log("INIT RACE: GENERATING RACERS");

			racerConfigs = new List<TTSRacerConfig>();

			for (int i = 0; i < numHumanPlayers; i++) {
				racerConfigs.Add(testRacerConfig(true));
			}

			for (int i = 0; i < numAIPlayers; i++) {
				racerConfigs.Add(testRacerConfig(false));
			}
		}

		if (level.currentGameType == TTSLevel.Gametype.Lobby) {
			LobbyInitialize();
		}
		else {
			InitializeRacers(racerConfigs);
		}
	}

	private bool IsHuman(TTSRacerConfig config) {
		if (config.LocalControlType == (int)TTSRacer.PlayerType.Player) return true;
		return false;
	}

	private void InitializeRacers(List<TTSRacerConfig> racerConfigs) {
		foreach (TTSRacerConfig config in racerConfigs) {

			switch ((TTSRacer.PlayerType)config.LocalControlType) {

				case TTSRacer.PlayerType.Player:
					InitToHuman(InstantiateRacer(config));
					break;

				case TTSRacer.PlayerType.AI:
					InitToAI(InstantiateRacer(config));
					break;

				case TTSRacer.PlayerType.Multiplayer:
					break;

			}
		}
	}

	public TTSRacerConfig testRacerConfig(bool Human) {
		TTSRacerConfig config = new TTSRacerConfig();
		config.Index = 99; // So that the racers will use the starting point index.
		config.RigType = Random.Range(0, Rigs.Count);
		config.PerkA = (int)TTSBehaviour.PerkType.Acceleration;
		config.PerkB = (int)TTSBehaviour.PowerupType.Leech;
		if (Human) {
			config.LocalControlType = TTSUtils.EnumToInt(TTSRacer.PlayerType.Player);
		}
		else {
			config.LocalControlType = TTSUtils.EnumToInt(TTSRacer.PlayerType.AI);
		}

		config.CharacterType = 0;

		return config;
	}

	private void LobbyInitialize() {
		InitToHuman(InstantiateRacer(testRacerConfig(true)));
	}

	public GameObject InstantiateRacer() {
		return InstantiateRacer(-1, -1);
	}

	public GameObject InstantiateRacer(TTSRacerConfig config) {
		// Make sure that the rig type isn't out of range
		config.RigType = (Rigs.Count > config.RigType) ? config.RigType : 0;
		config.CharacterType = (Characters.Count > config.CharacterType) ? config.CharacterType : 0;
		config.Index = (StartingPoints.Count > config.Index) ? config.Index : startingPointIndex;

		//gets the starting positions, sets them as taken if someone spawning on them already
		TTSStartingPoint startPoint = StartingPoints[config.Index].GetComponent<TTSStartingPoint>();
		startPoint.isTaken = true;
		startingPointIndex = (config.Index + 1) % StartingPoints.Count; // Always the next starting position. Loop around if array out of bounds.

		// Instantiate the gameobjects.
		GameObject rig = (GameObject)Instantiate(getRig((TTSBehaviour.RigType)config.RigType), startPoint.transform.position, startPoint.transform.rotation);
		GameObject character = (GameObject)Instantiate(Characters[config.CharacterType], startPoint.transform.position, startPoint.transform.rotation);
		GameObject racer = (GameObject)Instantiate(racerGO, startPoint.transform.position, startPoint.transform.rotation);

		//parents the racer properly
		character.transform.parent = racer.transform;
		rig.transform.parent = character.transform;

		//makes the sphere mesh follow the Racer2.0
		racer.GetComponent<TTSRacer>().displayMeshComponent = character.transform;

		//sets the currentrig variable of the racer to the rig selecte above
		racer.GetComponent<TTSRacer>().CurrentRig = rig.GetComponent<TTSRig>();
		racer.GetComponent<TTSRacer>().rigID = config.RigType;

		//sets an id for each racer
		racer.GetComponent<TTSRacer>().playerNum = humanPlayerCounter;

		//instantiates minimap icon
		GameObject iconSmall = (GameObject)Instantiate(playericonSmall);
		GameObject iconBig = (GameObject)Instantiate(playericonBig);

		//add perks
		racer.GetComponent<TTSPerkManager>().equiptPerkPool1 = (TTSBehaviour.PerkType)config.PerkA;
		racer.GetComponent<TTSPerkManager>().equiptPerkPool2 = (TTSBehaviour.PowerupType)config.PerkB;

		//assigns the icon to correct racer
		racer.GetComponent<TTSRacer>().minimapIconSmall = iconSmall;
		racer.GetComponent<TTSRacer>().minimapIconBig = iconBig;

		racer.GetComponent<TTSRacer>().Initialized();

		if (level.DebugMode)
			racer.GetComponent<TTSRacer>().canMove = true;

		return racer;
	}
	
	GameObject rigToLoad;
	GameObject characterToLoad;
	public GameObject InstantiateRacer(int rigID, int startPointID) {

		//finds the rig to initialize
		/*foreach(GameObject rig in _rigs){
			if(rig.GetComponent<TTSRig>().rigName == tempRigChoice){
				rigToLoad = rig;
			}
		}
		else {
			rigToLoad = Rigs[rigID];
		}

		//makes sure there is a rig to load if none selected

		if(rigToLoad == null){
			rigToLoad = _rigs[Random.Range(0, _rigs.Count)];
		}*/

		//checks for the character (in this case, default sphere)
		foreach (GameObject character in Characters) {
			if (character.name == tempCharacterChoice)
				characterToLoad = character;
		}
		if (characterToLoad == null) {
			characterToLoad = Characters[0];
		}

		//gets the starting positions, sets them as taken if someone spawning on them already
		GameObject sp = StartingPoints[(startPointID != -1) ? startPointID : startingPointIndex];
		sp.GetComponent<TTSStartingPoint>().isTaken = true;
		startingPointIndex++;

		//instantiates the rig
		GameObject tempRig = (GameObject)Instantiate(rigToLoad, sp.transform.position, sp.transform.rotation);
		rigToLoad = null;

		//instantiates the character mesh (sphere)
		GameObject tempChar = (GameObject)Instantiate(characterToLoad, sp.transform.position, sp.transform.rotation);

		//instantiates Racer2.0
		GameObject tempRacer = (GameObject)Instantiate(racerGO, sp.transform.position, sp.transform.rotation);

		//parents the racer properly
		tempChar.transform.parent = tempRacer.transform;
		tempRig.transform.parent = tempChar.transform;

		//makes the sphere mesh follow the Racer2.0
		tempRacer.GetComponent<TTSRacer>().displayMeshComponent = tempChar.transform;
		//sets the currentrig variable of the racer to the rig selecte above
		tempRacer.GetComponent<TTSRacer>().CurrentRig = tempRig.GetComponent<TTSRig>();
		tempRacer.GetComponent<TTSRacer>().rigID = rigID;
		
		// MENU LOAD
		//tempRacer.GetComponent<TTSRacer>().CurrentRig = tempRig;

		tempRacer.GetComponent<TTSRacer>().Initialized();

		return tempRacer;
	}

	//For minimap ID and camera ID
	private int humanPlayerCounter = 1;
	private void InitToHuman(GameObject racer) {
		//set to player controlled and set the player type to Player
		TTSRacer racerControl = racer.GetComponent<TTSRacer>();

		racerControl.IsPlayerControlled = true;
		racerControl.player = TTSRacer.PlayerType.Player;

		//Instantiates a minimap for each human player and sets it to follow a racer
		GameObject tempMinimap = (GameObject)Instantiate(minimapGO);
		TTSMinimap miniWin = tempMinimap.GetComponent<TTSMinimap>();
		miniWin.player = racerControl.CurrentRig.transform;

		//sets the minimap id
		miniWin.minimapID = humanPlayerCounter-1;

		//instantiate a camera for the player
		GameObject tempCamera = (GameObject)Instantiate(cameraGO);

		//tells the camera which racer to follow
		tempCamera.GetComponent<TTSFollowCamera>().target = racerControl.displayMeshComponent;

		//instantiate a hud for the human racer and get it to follow the camera and racer
		GameObject tempHUD = (GameObject)Instantiate(hudGO);
		tempHUD.GetComponent<TTSFloatHud>().boundCamera = tempCamera.transform;
		tempHUD.GetComponent<TTSFloatHud>().racerToFollow = racer;

		//Forces it to turn off layer 9 for the camera, but leave everything else as is
		tempCamera.camera.cullingMask &= ~(1 << 9);

		//Splitscreen handling, case 2 for 2 player, 3 for 3 player...etc.
		//Places the cam, minimap, fadeout and icon for each case by changing the layers
		//and changing what each camera sees
		switch (numHumanPlayers) {
			case 2:
				if (humanPlayerCounter == 1) {
					//Set the position for the camera, minimap and fadeout
					tempCamera.camera.rect = new Rect(0, 0.5f, 1.0f, 0.5f);
					tempMinimap.camera.rect = new Rect(0.02f, 0.52f, 0.125f, 0.25f);
					tempMinimap.camera.cullingMask |= (1 << 12);
					tempCamera.GetComponent<TTSFollowCamera>().fadeX = -10.0f;
					tempCamera.GetComponent<TTSFollowCamera>().fadeY = -10.0f;
					tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = Screen.width;
					tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = Screen.height / 2;

					//Foreach goes through each child GO to change the layer to display.
					//Used to hide huds/minimap/icon depending on which player you are
					foreach (Transform child in racerControl.minimapIconBig.transform) { child.gameObject.layer = 12; }
					racerControl.minimapIconBig.layer = 12;

					foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 12; }
					tempHUD.layer = 12;

					foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 12; }
					tempMinimap.layer = 12;

					//Set the main camera to not show player 2's stuff on player 1
					tempCamera.camera.cullingMask &= ~(1 << 13);
				}
				else {
					tempCamera.camera.rect = new Rect(0, 0, 1.0f, 0.5f);
					tempMinimap.camera.rect = new Rect(0.02f, 0.02f, 0.125f, 0.25f);
					tempMinimap.camera.cullingMask |= (1 << 13);
					tempCamera.GetComponent<TTSFollowCamera>().fadeX = -10.0f;
					tempCamera.GetComponent<TTSFollowCamera>().fadeY = Screen.height / 2;
					tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = Screen.width + 10.0f;
					tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = (Screen.height / 2) + 10.0f;

					foreach (Transform child in racerControl.minimapIconBig.transform) { child.gameObject.layer = 13; }
					racerControl.minimapIconBig.layer = 13;

					foreach (Transform child in tempHUD.transform) {
						child.gameObject.layer = 13;
					}
					tempHUD.layer = 13;

					foreach (Transform child in tempMinimap.transform) {
						child.gameObject.layer = 13;
					}
					tempMinimap.layer = 13;

					tempCamera.camera.cullingMask &= ~(1 << 12);
				}
				break;

			case 3:
				if (humanPlayerCounter == 1) {
					tempCamera.camera.rect = new Rect(0, 0.5f, 1.0f, 0.5f);
					tempMinimap.camera.rect = new Rect(0.02f, 0.52f, 0.25f, 0.50f);
					tempMinimap.camera.cullingMask |= (1 << 12);
					tempCamera.GetComponent<TTSFollowCamera>().fadeX = -10.0f;
					tempCamera.GetComponent<TTSFollowCamera>().fadeY = -10.0f;
					tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = Screen.width;
					tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = Screen.height / 2;

					foreach (Transform child in racerControl.minimapIconBig.transform) { child.gameObject.layer = 12; }
					racerControl.minimapIconBig.layer = 12;

					foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 12; }
					tempHUD.layer = 12;

					foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 12; }
					tempMinimap.layer = 12;

					tempCamera.camera.cullingMask &= ~(1 << 13);
					tempCamera.camera.cullingMask &= ~(1 << 14);
				}
				else if (humanPlayerCounter == 2) {
					tempCamera.camera.rect = new Rect(0, 0, 0.5f, 0.5f);
					tempMinimap.camera.rect = new Rect(0.02f, 0.02f, 0.125f, 0.25f);
					tempMinimap.camera.cullingMask |= (1 << 13);
					tempCamera.GetComponent<TTSFollowCamera>().fadeX = -10.0f;
					tempCamera.GetComponent<TTSFollowCamera>().fadeY = Screen.height / 2;
					tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = (Screen.width / 2);
					tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = (Screen.height / 2) + 10.0f;

					foreach (Transform child in racerControl.minimapIconBig.transform) { child.gameObject.layer = 13; }
					racerControl.minimapIconBig.layer = 13;

					foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 13; }
					tempHUD.layer = 13;

					foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 13; }
					tempMinimap.layer = 13;

					tempCamera.camera.cullingMask &= ~(1 << 12);
					tempCamera.camera.cullingMask &= ~(1 << 14);
				}
				else {
					tempCamera.camera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
					tempMinimap.camera.rect = new Rect(0.52f, 0.02f, 0.125f, 0.25f);
					tempMinimap.camera.cullingMask |= (1 << 14);
					tempCamera.GetComponent<TTSFollowCamera>().fadeX = Screen.width / 2;
					tempCamera.GetComponent<TTSFollowCamera>().fadeY = Screen.height / 2;
					tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = (Screen.width / 2) + 10.0f;
					tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = (Screen.height / 2) + 10.0f;

					foreach (Transform child in racerControl.minimapIconBig.transform) { child.gameObject.layer = 14; }
					racerControl.minimapIconBig.layer = 14;

					foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 14; }
					tempHUD.layer = 14;

					foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 14; }
					tempMinimap.layer = 14;

					tempCamera.camera.cullingMask &= ~(1 << 12);
					tempCamera.camera.cullingMask &= ~(1 << 13);
				}
				break;

			case 4:
				if (humanPlayerCounter == 1) {
					tempCamera.camera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
					tempMinimap.camera.rect = new Rect(0.02f, 0.52f, 0.125f, 0.25f);
					tempMinimap.camera.cullingMask |= (1 << 12);
					tempCamera.GetComponent<TTSFollowCamera>().fadeX = -10.0f;
					tempCamera.GetComponent<TTSFollowCamera>().fadeY = -10.0f;
					tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = (Screen.width / 2);
					tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = (Screen.height / 2);

					foreach (Transform child in racerControl.minimapIconBig.transform) { child.gameObject.layer = 12; }
					racerControl.minimapIconBig.layer = 12;

					foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 12; }
					tempHUD.layer = 12;

					foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 12; }
					tempMinimap.layer = 12;

					tempCamera.camera.cullingMask &= ~(1 << 13);
					tempCamera.camera.cullingMask &= ~(1 << 14);
					tempCamera.camera.cullingMask &= ~(1 << 15);
				}
				else if (humanPlayerCounter == 2) {
					tempCamera.camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
					tempMinimap.camera.rect = new Rect(0.52f, 0.52f, 0.125f, 0.25f);
					tempMinimap.camera.cullingMask |= (1 << 13);
					tempCamera.GetComponent<TTSFollowCamera>().fadeX = Screen.width / 2;
					tempCamera.GetComponent<TTSFollowCamera>().fadeY = -10.0f;
					tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = (Screen.width / 2);
					tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = (Screen.height / 2);

					foreach (Transform child in racerControl.minimapIconBig.transform) { child.gameObject.layer = 13; }
					racerControl.minimapIconBig.layer = 13;

					foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 13; }
					tempHUD.layer = 13;

					foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 13; }
					tempMinimap.layer = 13;

					tempCamera.camera.cullingMask &= ~(1 << 12);
					tempCamera.camera.cullingMask &= ~(1 << 14);
					tempCamera.camera.cullingMask &= ~(1 << 15);
				}
				else if (humanPlayerCounter == 3) {
					tempCamera.camera.rect = new Rect(0, 0, 0.5f, 0.5f);
					tempMinimap.camera.rect = new Rect(0.02f, 0.02f, 0.125f, 0.25f);
					tempMinimap.camera.cullingMask |= (1 << 14);
					tempCamera.GetComponent<TTSFollowCamera>().fadeX = -10.0f;
					tempCamera.GetComponent<TTSFollowCamera>().fadeY = Screen.height / 2;
					tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = (Screen.width / 2);
					tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = (Screen.height / 2) + 10.0f;

					foreach (Transform child in racerControl.minimapIconBig.transform) { child.gameObject.layer = 14; }
					racerControl.minimapIconBig.layer = 14;

					foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 14; }
					tempHUD.layer = 14;

					foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 14; }
					tempMinimap.layer = 14;

					tempCamera.camera.cullingMask &= ~(1 << 13);
					tempCamera.camera.cullingMask &= ~(1 << 12);
					tempCamera.camera.cullingMask &= ~(1 << 15);
				}
				else {
					tempCamera.camera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
					tempMinimap.camera.rect = new Rect(0.52f, 0.02f, 0.125f, 0.25f);
					tempMinimap.camera.cullingMask |= (1 << 15);
					tempCamera.GetComponent<TTSFollowCamera>().fadeX = Screen.width / 2;
					tempCamera.GetComponent<TTSFollowCamera>().fadeY = Screen.height / 2;
					tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = (Screen.width / 2) + 10.0f;
					tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = (Screen.height / 2) + 10.0f;

					foreach (Transform child in racerControl.minimapIconBig.transform) { child.gameObject.layer = 15; }
					racerControl.minimapIconBig.layer = 15;

					foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 15; }
					tempHUD.layer = 15;

					foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 15; }
					tempMinimap.layer = 15;

					tempCamera.camera.cullingMask &= ~(1 << 13);
					tempCamera.camera.cullingMask &= ~(1 << 14);
					tempCamera.camera.cullingMask &= ~(1 << 12);
				}
				break;

			default:
				foreach (Transform child in racerControl.minimapIconBig.transform) { child.gameObject.layer = 11; }
				racerControl.minimapIconBig.layer = 11;

				foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 12; }
				tempHUD.layer = 12;

				tempCamera.GetComponent<TTSFollowCamera>().fadeX = -10.0f;
				tempCamera.GetComponent<TTSFollowCamera>().fadeY = -10.0f;
				tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = Screen.width + 10.0f;
				tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = Screen.height + 10.0f;
				break;
		}

		//assinging the hud powerup for the racer
		Transform tempHudPowerup = tempHUD.transform.Find("CurrentPowerup");
		racer.GetComponent<TTSPowerup>().hudPowerup = tempHudPowerup.gameObject;

		Transform tempPP1 = tempHUD.transform.Find("PerkPool1Icon");
		tempPP1.GetComponent<TTSHudPerk>().InitializePerkPool1(racer.GetComponent<TTSPerkManager>().equiptPerkPool1);

		Transform tempPP2 = tempHUD.transform.Find("PerkPool2Icon");
		tempPP2.GetComponent<TTSHudPerk>().InitializePerkPool2(racer.GetComponent<TTSPerkManager>().equiptPerkPool2);

		racerControl.myCamera = tempCamera;

		tempCamera.GetComponent<TTSFollowCamera>().cameraNumber = humanPlayerCounter;

		humanPlayerCounter++;
	}

	private void InitToAI(GameObject racer) {
		//this is for AI only
		//set the player type to AI
		racer.GetComponent<TTSRacer>().IsPlayerControlled = true;
		racer.GetComponent<TTSRacer>().player = TTSRacer.PlayerType.AI;
	}

	private void InitToMultiplayer(GameObject racer, TTSRacerConfig config) {
		racer.GetComponent<TTSRacer>().IsPlayerControlled = true;
		racer.GetComponent<TTSRacer>().player = TTSRacer.PlayerType.Multiplayer;

		TTSRacerNetHandler handler = new TTSRacerNetHandler(level.client, false, config.netID);
		handler.position = racer.transform.position;
		racer.GetComponent<TTSRacer>().SetNetHandler(handler);
	}

	public void InitMultiplayerRacer(TTSRacerConfig config) {
		InitToMultiplayer(InstantiateRacer(config), config);
	}

	private GameObject getRig(TTSBehaviour.RigType type) {
		foreach (GameObject tempRig in Rigs) {
			if (tempRig.GetComponent<TTSRig>().rigType == type)
				return tempRig;
		}
		return Rigs[0];
	}

	// Update is called once per frame
	void Update() {

	}
}
