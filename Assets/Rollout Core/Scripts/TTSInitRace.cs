using UnityEngine;
using System.Collections.Generic;

public class TTSInitRace : MonoBehaviour
{

	public List<GameObject> _rigs = new List<GameObject>();
	public List<GameObject> _characters = new List<GameObject>();
	public List<GameObject> _startingpoints = new List<GameObject>();

	public GameObject racerGO;
	public GameObject cameraGO;
	public GameObject hudGO;

	private int startingPointIndex = 0;

	public enum Rigs { Rhino, Scorpion, Default };
	public enum Characters { Character_Default, Character1, Character2 };

	//private string tempRigChoice = "Rig_Rhino";
	private string tempRigChoice;
	private string tempCharacterChoice = "Character_Default";
	public int tempNumHumanPlayers = 1;
	public int tempNumAIPlayers = 0;

	GameObject rigToLoad;
	GameObject characterToLoad;

	// Use this for initialization
	void Start() {
		for (int i = 0; i < tempNumHumanPlayers; i++) {
			InitRacerToHuman(InstantiateRacer());
		}
		for (int i = 0; i < tempNumAIPlayers; i++) {
			InitToAI(InstantiateRacer());
		}
	}

	public void InitMultiplayerRacer(TTSRacerNetHandler handler) {
		InitToMultiplayer(InstantiateRacer(handler.Rig, handler.Index), handler);
	}

	public GameObject InstantiateRacer() {
		return InstantiateRacer(-1, -1);
	}

	public GameObject InstantiateRacer(int rigID, int startPointID) {
		//finds the rig to initialize
		if (rigID == -1) {
			foreach (GameObject rig in _rigs) {
				if (rig.GetComponent<TTSRig>().rigName == tempRigChoice) {
					rigToLoad = rig;
				}
			}
		}
		else {
			rigToLoad = _rigs[rigID];
		}
		//makes sure there is a rig to load if none selected
		if (rigToLoad == null) {
			rigToLoad = _rigs[Random.Range(0, _rigs.Count)];
		}

		//checks for the character (in this case, default sphere)
		foreach (GameObject character in _characters) {
			if (character.name == tempCharacterChoice)
				characterToLoad = character;
		}
		if (characterToLoad == null) {
			characterToLoad = _characters[0];
		}

		//gets the starting positions, sets them as taken if someone spawning on them already
		GameObject sp = _startingpoints[(startPointID != -1) ? startPointID : startingPointIndex];
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
		tempRacer.GetComponent<TTSRacer>().CurrentRig = tempRig;

		return tempRacer;
	}

	private void InitRacerToHuman(GameObject racer) {
		//set to player controlled and set the player type to Player
		TTSRacer racerControl = racer.GetComponent<TTSRacer>();

		racerControl.IsPlayerControlled = true;
		racerControl.player = TTSRacer.PlayerType.Player;

		//instantiate a camera for the player
		GameObject tempCamera = (GameObject)Instantiate(cameraGO);
		//handles splitting the screen for splitscreen
		if (tempNumHumanPlayers > 1) {
			if (startingPointIndex % 2 == 0) {
				if (tempNumHumanPlayers > 3) {
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
				if (tempNumHumanPlayers > 3) {
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

	private void InitToMultiplayer(GameObject racer, TTSRacerNetHandler handler) {
		racer.GetComponent<TTSRacer>().IsPlayerControlled = true;
		racer.GetComponent<TTSRacer>().player = TTSRacer.PlayerType.Multiplayer;
		handler.position = racer.transform.position;
		racer.GetComponent<TTSRacer>().SetNetHandler(handler);
	}

	// Update is called once per frame
	void Update() {

	}
}
