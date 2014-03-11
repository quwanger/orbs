using UnityEngine;
using System.Collections.Generic;

public class TTSInitRace : MonoBehaviour {
	
	//Storing GO info
	public List<GameObject> _rigs = new List<GameObject>();
	public List<GameObject> _characters = new List<GameObject>();
	public List<GameObject> _startingpoints = new List<GameObject>();
	
	//For instantiation
	public GameObject racerGO;
	public GameObject cameraGO;
	public GameObject hudGO;
	public GameObject minimapGO;
	public GameObject playericonSmall;
	public GameObject playericonBig;

	//For minimap ID and camera ID
	private int counter = 1;
	
	public enum Rigs {Rhino, Scorpion, Default};
	public enum Characters {Character_Default, Character1, Character2};
	
	public List<GameObject> _playerBundles;
	public string gameType;
	
	private string tempRigChoice = null;
	private string tempCharacterChoice = "Character_Default";
	public int tempNumHumanPlayers = 1;
	public int numHumanPlayers;
	public int numberOfRacers = 1;
	
	GameObject rigToLoad;
	GameObject characterToLoad;
	
	// Use this for initialization
	void Start () {
		
		if(GameObject.Find("DataToPass")){
			_playerBundles = GameObject.Find("DataToPass").GetComponent<TTSDataToPass>().players;
			gameType = GameObject.Find("DataToPass").GetComponent<TTSDataToPass>().gametype;
		}
		
		numHumanPlayers = _playerBundles.Count;
		
		//Loop thru racers and create all the cameras, huds...etc.
		for(int i = 0; i < numberOfRacers; i++){
			if(i < numHumanPlayers){
				switch(_playerBundles[i].GetComponent<TTSPlayerInfo>().rig){
				case("Rhino"):
					rigToLoad = _rigs[0];
					break;
				case("Spider"):
					rigToLoad = _rigs[1];
					break;
				case("Dragon"):
					rigToLoad = _rigs[2];
					break;
				case("NextGen"):
					rigToLoad = _rigs[3];
					break;
				case("Antique"):
					rigToLoad = _rigs[4];
					break;
				case("Scorpion"):
					rigToLoad = _rigs[5];
					break;
				default:
					rigToLoad = _rigs[Random.Range(0, _rigs.Count)];
					break;
				}
			}else{
				//load random rig for AI racers
				rigToLoad = _rigs[Random.Range(0, _rigs.Count)];
			}
			InstantiateRacers(i);
		}
	}
	
	public void InstantiateRacers(int i){
		//finds the rig to initialize
		/*foreach(GameObject rig in _rigs){
			if(rig.GetComponent<TTSRig>().rigName == tempRigChoice){
				rigToLoad = rig;
			}
		}

		//makes sure there is a rig to load if none selected
		if(rigToLoad == null){
			rigToLoad = _rigs[Random.Range(0, _rigs.Count)];
		}*/
		
		//checks for the character (in this case, default sphere)
		foreach(GameObject character in _characters){
			if(character.name == tempCharacterChoice)
				characterToLoad = character;
		}
		if(characterToLoad == null){
			characterToLoad = _characters[0];
		}
		
		//gets the starting positions, sets them as taken if someone spawning on them already
		GameObject sp = _startingpoints[i];
		sp.GetComponent<TTSStartingPoint>().isTaken = true;
		
		//instantiates the rig
		GameObject tempRig = (GameObject)Instantiate(rigToLoad, sp.transform.position, sp.transform.rotation );
		rigToLoad = null;
		
		//instantiates the character mesh (sphere)
		GameObject tempChar = (GameObject)Instantiate(characterToLoad, sp.transform.position, sp.transform.rotation);
		
		//instantiates Racer2.0
		GameObject tempRacer = (GameObject)Instantiate(racerGO, sp.transform.position, sp.transform.rotation);

		//instantiates minimap icon
		GameObject tempIconSmall = (GameObject)Instantiate(playericonSmall);
		GameObject tempIconBig = (GameObject)Instantiate(playericonBig);
		
		//parents the racer properly
		tempChar.transform.parent = tempRacer.transform;
		tempRig.transform.parent = tempChar.transform;
	
		//place the indicator
		//GameObject tempIndicator = (GameObject)Instantiate(indicatorGO, new Vector3(tempRacer.transform.position.x, tempRacer.transform.position.y + 0.5f, tempRacer.transform.position.z), tempChar.transform.rotation);
		//tempIndicator.transform.parent = tempChar.transform;
		
		//makes the sphere mesh follow the Racer2.0
		tempRacer.GetComponent<TTSRacer>().displayMeshComponent = tempChar.transform;
		
		//sets the currentrig variable of the racer to the rig selecte above
		tempRacer.GetComponent<TTSRacer>().CurrentRig = tempRig;
	
		//sets an id for each racer
		tempRacer.GetComponent<TTSRacer>().playerNum = counter;
		
		//assigns the icon to correct racer
		tempRacer.GetComponent<TTSRacer>().minimapIconSmall = tempIconSmall;
		tempRacer.GetComponent<TTSRacer>().minimapIconBig = tempIconBig;

		//this is where the stuff for the human players
		if(i < numHumanPlayers){
			//set to player controlled and set the player type to Player
			tempRacer.GetComponent<TTSRacer>().IsPlayerControlled = true;
			tempRacer.GetComponent<TTSRacer>().player = TTSRacer.PlayerType.Player;
			
			//add perks
			tempRacer.GetComponent<TTSPerkManager>().equiptPerkPool1 = _playerBundles[i].GetComponent<TTSPlayerInfo>().perkA;
			tempRacer.GetComponent<TTSPerkManager>().equiptPerkPool2 = _playerBundles[i].GetComponent<TTSPlayerInfo>().perkB;

			//Instantiates a minimap for each human player and sets it to follow a racer
			GameObject tempMinimap = (GameObject)Instantiate(minimapGO);
			TTSMinimap miniWin = tempMinimap.GetComponent<TTSMinimap>();
			miniWin.player = tempRig.transform;
		
			//sets the minimap id
			miniWin.minimapID = i;

			GameObject tempCamera = (GameObject)Instantiate(cameraGO);
		
			//tells the camera which racer to follow
			tempCamera.GetComponent<TTSFollowCamera>().target = tempChar.transform;
			
			//instantiate a hud for the human racer and get it to follow the camera and racer
			GameObject tempHUD = (GameObject)Instantiate(hudGO);
			tempHUD.GetComponent<TTSFloatHud>().boundCamera = tempCamera.transform;
			tempHUD.GetComponent<TTSFloatHud>().racerToFollow = tempRacer;
		
			//Forces it to turn off layer 9 for the camera, but leave everything else as is
			tempCamera.camera.cullingMask &= ~ (1 << 9);
		
			//Splitscreen handling, case 2 for 2 player, 3 for 3 player...etc.
			//Places the cam, minimap, fadeout and icon for each case by changing the layers
			//and changing what each camera sees
			switch(numHumanPlayers){
				case 2:
					if(i==0) {
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
						foreach (Transform child in tempIconBig.transform) { child.gameObject.layer = 12; }
						tempIconBig.layer = 12;
				
						foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 12; }
						tempHUD.layer = 12;
				
						foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 12; }
						tempMinimap.layer = 12;
				
						//Set the main camera to not show player 2's stuff on player 1
						tempCamera.camera.cullingMask &= ~(1 << 13);
					} else {
						tempCamera.camera.rect = new Rect(0, 0, 1.0f, 0.5f);
						tempMinimap.camera.rect = new Rect(0.02f, 0.02f, 0.125f, 0.25f);
						tempMinimap.camera.cullingMask |= (1 << 13);
						tempCamera.GetComponent<TTSFollowCamera>().fadeX = -10.0f;
						tempCamera.GetComponent<TTSFollowCamera>().fadeY = Screen.height / 2;
						tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = Screen.width + 10.0f;
						tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = (Screen.height / 2) + 10.0f;
						
						foreach (Transform child in tempIconBig.transform) { child.gameObject.layer = 13; }
						tempIconBig.layer = 13;
				
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
					if(i==0) {
						tempCamera.camera.rect = new Rect(0, 0.5f, 1.0f, 0.5f);
						tempMinimap.camera.rect = new Rect(0.02f, 0.52f, 0.25f, 0.50f);
						tempMinimap.camera.cullingMask |= (1 << 12);
						tempCamera.GetComponent<TTSFollowCamera>().fadeX = -10.0f;
						tempCamera.GetComponent<TTSFollowCamera>().fadeY = -10.0f;
						tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = Screen.width;
						tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = Screen.height / 2;
				
						foreach (Transform child in tempIconBig.transform) { child.gameObject.layer = 12; }
						tempIconBig.layer = 12;
				
						foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 12; }
						tempHUD.layer = 12;
				
						foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 12; }
						tempMinimap.layer = 12;
				
						tempCamera.camera.cullingMask &= ~(1 << 13);
						tempCamera.camera.cullingMask &= ~(1 << 14);
					} else if(i==1) {
						tempCamera.camera.rect = new Rect(0, 0, 0.5f, 0.5f);
						tempMinimap.camera.rect = new Rect(0.02f, 0.02f, 0.125f, 0.25f);
						tempMinimap.camera.cullingMask |= (1 << 13);
						tempCamera.GetComponent<TTSFollowCamera>().fadeX = -10.0f;
						tempCamera.GetComponent<TTSFollowCamera>().fadeY = Screen.height / 2;
						tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = (Screen.width / 2);
						tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = (Screen.height / 2) + 10.0f;
				
						foreach (Transform child in tempIconBig.transform) { child.gameObject.layer = 13; }
						tempIconBig.layer = 13;
				
						foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 13; }
						tempHUD.layer = 13;
				
						foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 13; }
						tempMinimap.layer = 13;
				
						tempCamera.camera.cullingMask &= ~(1 << 12);
						tempCamera.camera.cullingMask &= ~(1 << 14);
					} else { 
						tempCamera.camera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
						tempMinimap.camera.rect = new Rect(0.52f, 0.02f, 0.125f, 0.25f);
						tempMinimap.camera.cullingMask |= (1 << 14);
						tempCamera.GetComponent<TTSFollowCamera>().fadeX = Screen.width / 2;
						tempCamera.GetComponent<TTSFollowCamera>().fadeY = Screen.height / 2;
						tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = (Screen.width / 2) + 10.0f;
						tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = (Screen.height / 2) + 10.0f;
				
						foreach (Transform child in tempIconBig.transform) { child.gameObject.layer = 14; }
						tempIconBig.layer = 14;
				
						foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 14; }
						tempHUD.layer = 14;
				
						foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 14; }
						tempMinimap.layer = 14;
				
						tempCamera.camera.cullingMask &= ~(1 << 12);
						tempCamera.camera.cullingMask &= ~(1 << 13);
					}
					break;

				case 4:
					if(i==0) {
						tempCamera.camera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
						tempMinimap.camera.rect = new Rect(0.02f, 0.52f, 0.125f, 0.25f);
						tempMinimap.camera.cullingMask |= (1 << 12);
						tempCamera.GetComponent<TTSFollowCamera>().fadeX = -10.0f;
						tempCamera.GetComponent<TTSFollowCamera>().fadeY = -10.0f;
						tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = (Screen.width / 2);
						tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = (Screen.height / 2);
				
						foreach (Transform child in tempIconBig.transform) { child.gameObject.layer = 12; }
						tempIconBig.layer = 12;
				
						foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 12; }
						tempHUD.layer = 12;
				
						foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 12; }
						tempMinimap.layer = 12;
				
						tempCamera.camera.cullingMask &= ~(1 << 13);
						tempCamera.camera.cullingMask &= ~(1 << 14);
						tempCamera.camera.cullingMask &= ~(1 << 15);
					} else if(i==1) {
						tempCamera.camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
						tempMinimap.camera.rect = new Rect(0.52f, 0.52f, 0.125f, 0.25f);
						tempMinimap.camera.cullingMask |= (1 << 13);
						tempCamera.GetComponent<TTSFollowCamera>().fadeX = Screen.width / 2;
						tempCamera.GetComponent<TTSFollowCamera>().fadeY = -10.0f;
						tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = (Screen.width / 2);
						tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = (Screen.height / 2);
				
						foreach (Transform child in tempIconBig.transform) { child.gameObject.layer = 13; }
						tempIconBig.layer = 13;
				
						foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 13; }
						tempHUD.layer = 13;
				
						foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 13; }
						tempMinimap.layer = 13;
				
						tempCamera.camera.cullingMask &= ~(1 << 12);
						tempCamera.camera.cullingMask &= ~(1 << 14);
						tempCamera.camera.cullingMask &= ~(1 << 15);
					} else if(i==2) {
						tempCamera.camera.rect = new Rect(0, 0, 0.5f, 0.5f);
						tempMinimap.camera.rect = new Rect(0.02f, 0.02f, 0.125f, 0.25f);
						tempMinimap.camera.cullingMask |= (1 << 14);
						tempCamera.GetComponent<TTSFollowCamera>().fadeX = -10.0f;
						tempCamera.GetComponent<TTSFollowCamera>().fadeY = Screen.height / 2;
						tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = (Screen.width / 2);
						tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = (Screen.height / 2) + 10.0f;
				
						foreach (Transform child in tempIconBig.transform) { child.gameObject.layer = 14; }
						tempIconBig.layer = 14;
				
						foreach (Transform child in tempHUD.transform) { child.gameObject.layer = 14; }
						tempHUD.layer = 14;
				
						foreach (Transform child in tempMinimap.transform) { child.gameObject.layer = 14; }
						tempMinimap.layer = 14;
				
						tempCamera.camera.cullingMask &= ~(1 << 13);
						tempCamera.camera.cullingMask &= ~(1 << 12);
						tempCamera.camera.cullingMask &= ~(1 << 15);
					} else {
						tempCamera.camera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
						tempMinimap.camera.rect = new Rect(0.52f, 0.02f, 0.125f, 0.25f);
						tempMinimap.camera.cullingMask |= (1 << 15);
						tempCamera.GetComponent<TTSFollowCamera>().fadeX = Screen.width / 2;
						tempCamera.GetComponent<TTSFollowCamera>().fadeY = Screen.height / 2;
						tempCamera.GetComponent<TTSFollowCamera>().fadeWidth = (Screen.width / 2) + 10.0f;
						tempCamera.GetComponent<TTSFollowCamera>().fadeHeight = (Screen.height / 2) + 10.0f;
				
						foreach (Transform child in tempIconBig.transform) { child.gameObject.layer = 15; }
						tempIconBig.layer = 15;
				
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
					foreach (Transform child in tempIconBig.transform) { child.gameObject.layer = 11; }
					tempIconBig.layer = 11;
			
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
			tempRacer.GetComponent<TTSPowerup>().hudPowerup = tempHudPowerup.gameObject;
		
			Transform tempPP1 = tempHUD.transform.Find("PerkPool1Icon");
			tempPP1.GetComponent<TTSHudPerk>().InitializePerkPool1(tempRacer.GetComponent<TTSPerkManager>().equiptPerkPool1);
		
			Transform tempPP2 = tempHUD.transform.Find("PerkPool2Icon");
			tempPP2.GetComponent<TTSHudPerk>().InitializePerkPool2(tempRacer.GetComponent<TTSPerkManager>().equiptPerkPool2);
		
			tempRacer.GetComponent<TTSRacer>().myCamera = tempCamera;
		
			tempCamera.GetComponent<TTSFollowCamera>().cameraNumber = counter;
		
		}else{
			//this is for AI only
			//set the player type to AI
			tempRacer.GetComponent<TTSRacer>().IsPlayerControlled = true;
			tempRacer.GetComponent<TTSRacer>().player = TTSRacer.PlayerType.AI;
		}
		
		counter++;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
