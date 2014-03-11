using UnityEngine;
using System.Collections.Generic;

public class TTSInitRace : MonoBehaviour
{

	public List<GameObject> Rigs = new List<GameObject>();
	public List<GameObject> Characters = new List<GameObject>();
	public List<GameObject> StartingPoints = new List<GameObject>();

	public GameObject racerGO;
	public GameObject cameraGO;
	public GameObject hudGO;

	private int startingPointIndex = 0;

	public enum RigTypes { Rhino, Scorpion, Default };
	public enum CharacterTypes { Character_Default, Character1, Character2 };

	//private string tempRigChoice = "Rig_Rhino";
	private string tempRigChoice;
	private string tempCharacterChoice = "Character_Default";
	public int numHumanPlayers = 1;
	public int tempNumAIPlayers = 0;

	GameObject rigToLoad;
	GameObject characterToLoad;

	TTSLevel level;

	public List<TTSRacer.RacerConfig> racerConfigs;

	// Use this for initialization
	void Start() {
		level = GetComponent<TTSLevel>();

		racerConfigs = new List<TTSRacer.RacerConfig>();
		racerConfigs.Add(testRacerConfig(true));
		//racerConfigs.Add(testRacerConfig(false));
		//racerConfigs.Add(testRacerConfig(false));

		numHumanPlayers = racerConfigs.FindAll(IsHuman).Count;
		
		if (level.currentGameType == TTSLevel.Gametype.Lobby) {
			LobbyInitialize();
		}
		else {
			InitializeRacers(racerConfigs);
		}
	}

	private bool IsHuman(TTSRacer.RacerConfig config) {
		if (config.LocalControlType == (int)TTSRacer.PlayerType.Player) return true;
		return false;
	}

	private void InitializeRacers(List<TTSRacer.RacerConfig> racerConfigs) {
		foreach (TTSRacer.RacerConfig config in racerConfigs) {

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

	private TTSRacer.RacerConfig testRacerConfig(bool Human) {
		TTSRacer.RacerConfig config = new TTSRacer.RacerConfig();
		config.Index = 0;
		config.RigType = 1;
		config.Perk1 = 0;
		config.Perk2 = 0;
		if (Human) {
			config.LocalControlType = TTSUtils.EnumToInt(TTSRacer.PlayerType.Player);
		}
		else
			config.LocalControlType = TTSUtils.EnumToInt(TTSRacer.PlayerType.AI);
		config.CharacterType = 0;

		return config;
	}

	private void LobbyInitialize() {
		InitToHuman(InstantiateRacer(testRacerConfig(true)));
	}

	public GameObject InstantiateRacer() {
		return InstantiateRacer(-1, -1);
	}

	public GameObject InstantiateRacer(TTSRacer.RacerConfig config) {
		// Make sure that the rig type isn't out of range
		config.RigType = (Rigs.Count > config.RigType) ? config.RigType : 0;
		config.CharacterType = (Characters.Count > config.CharacterType) ? config.CharacterType : 0;
		config.Index = (StartingPoints.Count > config.Index) ? config.Index : startingPointIndex;

		//gets the starting positions, sets them as taken if someone spawning on them already
		TTSStartingPoint startPoint = StartingPoints[config.Index].GetComponent<TTSStartingPoint>();
		startPoint.isTaken = true;
		startingPointIndex = (config.Index + 1) % StartingPoints.Count; // Always the next starting position. Loop around if array out of bounds.

		// Instantiate the gameobjects.
		GameObject rig = (GameObject)Instantiate(Rigs[config.RigType], startPoint.transform.position, startPoint.transform.rotation);
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

		racer.GetComponent<TTSRacer>().Initialized();

		if (level.DebugMode)
			racer.GetComponent<TTSRacer>().canMove = true;

		return racer;
	}

	public GameObject InstantiateRacer(int rigID, int startPointID) {

		//finds the rig to initialize
		if (rigID == -1) {
			foreach (GameObject rig in Rigs) {
				if (rig.GetComponent<TTSRig>().rigName == tempRigChoice) {
					rigToLoad = rig;
				}
			}
		}
		else {
			rigToLoad = Rigs[rigID];
		}

		//makes sure there is a rig to load if none selected
		if (rigToLoad == null) {
			rigID = Random.Range(0, Rigs.Count);
			rigToLoad = Rigs[rigID];
		}

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

		tempRacer.GetComponent<TTSRacer>().Initialized();

		return tempRacer;
	}

	private void InitToHuman(GameObject racer) {
		//set to player controlled and set the player type to Player
		TTSRacer racerControl = racer.GetComponent<TTSRacer>();

		racerControl.IsPlayerControlled = true;
		racerControl.player = TTSRacer.PlayerType.Player;

		//instantiate a camera for the player
		GameObject tempCamera = (GameObject)Instantiate(cameraGO);
		//handles splitting the screen for splitscreen
		if (numHumanPlayers > 1) {
			if (startingPointIndex % 2 == 0) {
				if (numHumanPlayers > 3) {
					if (startingPointIndex % 4 == 0)
						tempCamera.camera.rect = new Rect(0, 0, 0.5f, 0.5f);
					else
						tempCamera.camera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
				}
				else {
					tempCamera.camera.rect = new Rect(0, 0, 1.0f, 0.5f);
				}
			}
			else {
				if (numHumanPlayers > 3) {
					if (startingPointIndex % 3 == 0)
						tempCamera.camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
					else
						tempCamera.camera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
				}
				else {
					tempCamera.camera.rect = new Rect(0, 0.5f, 1.0f, 0.5f);
				}
			}
		}

		//tells the camera which racer to follow
		tempCamera.GetComponent<TTSFollowCamera>().racer = racerControl;
		tempCamera.GetComponent<TTSFollowCamera>().target = racerControl.displayMeshComponent;

		//instantiate a hud for the human racer and get it to follow the camera and racer
		GameObject tempHUD = (GameObject)Instantiate(hudGO);
		tempHUD.GetComponent<TTSFloatHud>().boundCamera = tempCamera.transform;
		tempHUD.GetComponent<TTSFloatHud>().racerToFollow = racer;

		//assinging the hud powerup for the racer
		Transform tempHudPowerup = tempHUD.transform.Find("CurrentPowerup");
		racer.GetComponent<TTSPowerup>().hudPowerup = tempHudPowerup.gameObject;

		racerControl.myCamera = tempCamera;
	}

	private void InitToAI(GameObject racer) {
		//this is for AI only
		//set the player type to AI
		racer.GetComponent<TTSRacer>().IsPlayerControlled = true;
		racer.GetComponent<TTSRacer>().player = TTSRacer.PlayerType.AI;
	}

	private void InitToMultiplayer(GameObject racer, TTSRacer.RacerConfig config) {
		racer.GetComponent<TTSRacer>().IsPlayerControlled = true;
		racer.GetComponent<TTSRacer>().player = TTSRacer.PlayerType.Multiplayer;

		TTSRacerNetHandler handler = new TTSRacerNetHandler(level.client, false, config.netID);
		handler.position = racer.transform.position;
		racer.GetComponent<TTSRacer>().SetNetHandler(handler);
	}

	// Update is called once per frame
	void Update() {

	}
}
