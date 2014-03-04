using UnityEngine;
using System.Collections.Generic;

public class TTSInitRace : MonoBehaviour {
	
	public List<GameObject> _rigs = new List<GameObject>();
	public List<GameObject> _characters = new List<GameObject>();
	public List<GameObject> _startingpoints = new List<GameObject>();
	
	public GameObject racerGO;
	public GameObject cameraGO;
	public GameObject hudGO;
	//public GameObject indicatorGO;
	
	public enum Rigs {Rhino, Scorpion, Default};
	public enum Characters {Character_Default, Character1, Character2};
	
	//private string tempRigChoice = "Rig_Rhino";
	private string tempRigChoice;
	private string tempCharacterChoice = "Character_Default";
	public int tempNumHumanPlayers = 1;
	public int numberOfRacers = 1;
	
	GameObject rigToLoad;
	GameObject characterToLoad;
	
	// Use this for initialization
	void Start () {
		
		for(int i = 0; i < numberOfRacers; i++){
			InstantiateRacers(i);
		}
	}
	
	public void InstantiateRacers(int i){
		//finds the rig to initialize
			foreach(GameObject rig in _rigs){
				if(rig.GetComponent<TTSRig>().rigName == tempRigChoice){
					rigToLoad = rig;
				}
			}
			//makes sure there is a rig to load if none selected
			if(rigToLoad == null){
				rigToLoad = _rigs[Random.Range(0, _rigs.Count)];
			}
			
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
			
			//do this only for human players
			if(i < tempNumHumanPlayers){
				//set to player controlled and set the player type to Player
				tempRacer.GetComponent<TTSRacer>().IsPlayerControlled = true;
				tempRacer.GetComponent<TTSRacer>().player = TTSRacer.PlayerType.Player;
			
				//instantiate a camera for the player
				GameObject tempCamera = (GameObject)Instantiate(cameraGO);
				//handles splitting the screen for splitscreen
				if(tempNumHumanPlayers > 1){
					if(i%2 == 0){
						if(tempNumHumanPlayers > 3){
							if(i%4 == 0)
								tempCamera.camera.rect = new Rect(0, 0, 0.5f, 0.5f);
							else
								tempCamera.camera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
						}else{
							tempCamera.camera.rect = new Rect(0, 0, 1.0f, 0.5f);
						}
					}else{
						if(tempNumHumanPlayers > 3){
							if(i%3 == 0)
								tempCamera.camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
							else
								tempCamera.camera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
						}else{
							tempCamera.camera.rect = new Rect(0, 0.5f, 1.0f, 0.5f);
						}
					}
				}
				
				//tells the camera which racer to follow
				tempCamera.GetComponent<TTSFollowCamera>().target = tempChar.transform;
				
				//instantiate a hud for the human racer and get it to follow the camera and racer
				GameObject tempHUD = (GameObject)Instantiate(hudGO);
				tempHUD.GetComponent<TTSFloatHud>().boundCamera = tempCamera.transform;
				tempHUD.GetComponent<TTSFloatHud>().racerToFollow = tempRacer;

				//assinging the hud powerup for the racer
				Transform tempHudPowerup = tempHUD.transform.Find("CurrentPowerup");
				tempRacer.GetComponent<TTSPowerup>().hudPowerup = tempHudPowerup.gameObject;
			
				Transform tempPP1 = tempHUD.transform.Find("PerkPool1Icon");
				tempPP1.GetComponent<TTSHudPerk>().InitializePerkPool1(tempRacer.GetComponent<TTSPerkManager>().equiptPerkPool1);
			
				Transform tempPP2 = tempHUD.transform.Find("PerkPool2Icon");
				tempPP2.GetComponent<TTSHudPerk>().InitializePerkPool2(tempRacer.GetComponent<TTSPerkManager>().equiptPerkPool2);
			
				tempRacer.GetComponent<TTSRacer>().myCamera = tempCamera;
			}else{
				//this is for AI only
				//set the player type to AI
				tempRacer.GetComponent<TTSRacer>().IsPlayerControlled = true;
				tempRacer.GetComponent<TTSRacer>().player = TTSRacer.PlayerType.AI;
			}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
