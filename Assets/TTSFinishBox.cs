using UnityEngine;
using System.Collections.Generic;

public class TTSFinishBox : TTSBehaviour {
	
	public List<GameObject> FinishedRacerStats = new List<GameObject>();
	public List<GameObject> panels = new List<GameObject>();
	public GameObject racerStatGameObject;
	public GameObject racerFinishPanel;
	
	public List<string> positions = new List<string>();
	public List<RigType> rigs = new List<RigType>();
	public List<string> times = new List<string>();
	
	public int place = 1;
	
	private GameObject[] minimap;
	// Use this for initialization
	void Start () {
		minimap = GameObject.FindGameObjectsWithTag("minimap");
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider collision) {
		if(!level.DebugMode){
			if(collision.gameObject.GetComponent<TTSRacer>()){
				foreach(GameObject m in minimap){
					m.SetActive(false);
				}
				
				TTSRacer tempRacer = collision.gameObject.GetComponent<TTSRacer>();
				
				tempRacer.canMove=false;
				tempRacer.finished = true;
				if(tempRacer.player != TTSRacer.PlayerType.AI){
					if(level.currentGameType == TTSLevel.Gametype.MultiplayerLocal){
						//splitscreen fadeout stuff
						//tempRacer.myCamera.GetComponent<TTSCameraFade>().StartFade(new Color(0,0,0,1.0f), 1.0f);
					}
				}
				
				addToFinishedRacers(collision.gameObject);
				updateFinishScreens();
				place++;
			}
		}
		
		if(level.currentGameType == TTSLevel.Gametype.MultiplayerLocal){
			foreach(GameObject racer in racers){
				if(racer.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player){
					if(!racer.GetComponent<TTSRacer>().finished){
						return;
					}
				}
			}
		}else{
			foreach(GameObject racer in racers){
				if(!racer.GetComponent<TTSRacer>().finished){
					return;
				}
			}
		}
		
		level.raceHasFinished = true;
	}
	
	public void createNewFinishPanel(GameObject racer, bool visible){
		GameObject go = (GameObject)Instantiate(racerFinishPanel);
		go.GetComponent<TTSFinishline>().myFinishedRacer = racer.GetComponent<TTSFinishedRacer>();
		
		if(visible)
			go.GetComponent<TTSFinishline>().isVisible = true;
		else
			go.GetComponent<TTSFinishline>().isVisible = false;
		
		panels.Add (go);
		
		go.GetComponent<TTSFinishline>().PopulatePanel();
	}
	
	public void updateFinishScreens(){
		//called every time a racer crosses the finish line
		
		foreach(GameObject finishLine in panels){
			
			if (!finishLine.GetComponent<TTSFinishline>().isVisible){
				if (finishLine.activeInHierarchy){
					finishLine.SetActive(false);
				}
			}
			
			for(int i=0; i<positions.Count; i++){
				finishLine.GetComponent<TTSFinishline>().positions[i].text = positions[i];
				finishLine.GetComponent<TTSFinishline>().rigs[i].text = rigs[i].ToString();
				finishLine.GetComponent<TTSFinishline>().times[i].text = times[i];
			}
		}
	}
	
	public void addToFinishedRacers(GameObject racerToAdd){
		
		GameObject go = (GameObject)Instantiate(racerStatGameObject);
		TTSFinishedRacer finishedRacer = go.GetComponent<TTSFinishedRacer>();
		//racerToAdd is the racer that just passed the finish line
		//	get the time from its hud
		finishedRacer.time = level.GetComponent<TTSTimeManager>().GetCurrentTimeString();
		//	get the perks from TTSPerkManager
		finishedRacer.perkpool1 = racerToAdd.GetComponent<TTSPerkManager>().equiptPerkPool1;
		finishedRacer.perkpool2 = racerToAdd.GetComponent<TTSPerkManager>().equiptPerkPool2;
		//	get the player id from TTSRacer
		finishedRacer.id = racerToAdd.GetComponent<TTSRacer>().playerNum;
		//	get the rig from TTSRacer (currentRig)
		finishedRacer.rig = racerToAdd.GetComponent<TTSRacer>().CurrentRig.rigType;
		finishedRacer.place = place;
		FinishedRacerStats.Add(go);
		
		if(racerToAdd.GetComponent<TTSRacer>().player != TTSRacer.PlayerType.AI){
			createNewFinishPanel(go, true);
		}
		
		//these are the lists that will be used to populate the list appearing on the right side of the finish panel
		positions.Add (place.ToString());
		rigs.Add(racerToAdd.GetComponent<TTSRacer>().CurrentRig.rigType);
		times.Add(finishedRacer.time);
	}
}
