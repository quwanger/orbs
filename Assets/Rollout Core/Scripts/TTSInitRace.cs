using UnityEngine;
using System.Collections.Generic;

public class TTSInitRace : MonoBehaviour {
	
	public List<GameObject> _rigs = new List<GameObject>();
	public List<GameObject> _characters = new List<GameObject>();
	public List<GameObject> _startingpoints = new List<GameObject>();
	
	public GameObject racerGO;
	public GameObject cameraGO;
	public GameObject hudGO;
	
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
			foreach(GameObject rig in _rigs){
				//Debug.Log(rig.GetComponent<TTSRig>().rigName);
				if(rig.GetComponent<TTSRig>().rigName == tempRigChoice){
					rigToLoad = rig;
				}
			}
			if(rigToLoad == null){
				//Debug.Log (_rigs.Count);
				rigToLoad = _rigs[Random.Range(0, _rigs.Count)];
			}
			
			foreach(GameObject character in _characters){
				if(character.name == tempCharacterChoice)
					characterToLoad = character;
			}
			if(characterToLoad == null){
				characterToLoad = _characters[0];
			}
			
			GameObject sp = _startingpoints[i];
			sp.GetComponent<TTSStartingPoint>().isTaken = true;
			
			GameObject tempRig = (GameObject)Instantiate(rigToLoad, sp.transform.position, sp.transform.rotation );
			rigToLoad = null;
			
			GameObject tempChar = (GameObject)Instantiate(characterToLoad, sp.transform.position, sp.transform.rotation);
	
			GameObject tempRacer = (GameObject)Instantiate(racerGO, sp.transform.position, sp.transform.rotation);
			
			tempChar.transform.parent = tempRacer.transform;
			
			tempRig.transform.parent = tempChar.transform;
			
			tempRacer.GetComponent<TTSRacer>().displayMeshComponent = tempChar.transform;
			tempRacer.GetComponent<TTSRacer>().CurrentRig = tempRig;
			if(i < tempNumHumanPlayers){
				tempRacer.GetComponent<TTSRacer>().IsPlayerControlled = true;
				tempRacer.GetComponent<TTSRacer>().player = TTSRacer.PlayerType.Player;
			
				GameObject tempCamera = (GameObject)Instantiate(cameraGO);
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
				tempCamera.GetComponent<TTSFollowCamera>().target = tempChar.transform;
				
				GameObject tempHUD = (GameObject)Instantiate(hudGO);
				tempHUD.GetComponent<TTSFloatHud>().boundCamera = tempCamera.transform;
				tempHUD.GetComponent<TTSFloatHud>().racerToFollow = tempRacer;

				//assinging the hud powerup for the racer

				Transform tempHudPowerup = tempHUD.transform.Find("CurrentPowerup");

				tempRacer.GetComponent<TTSPowerup>().hudPowerup = tempHudPowerup.gameObject;
			}else{
				tempRacer.GetComponent<TTSRacer>().IsPlayerControlled = true;
				tempRacer.GetComponent<TTSRacer>().player = TTSRacer.PlayerType.AI;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
