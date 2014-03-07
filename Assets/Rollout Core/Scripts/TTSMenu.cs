using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TTSMenu : TTSMenuEnums {
	
	// Lists of rigs, perks, and levels
	public List<GameObject> _rigs = new List<GameObject>();
	public RigMenuItem SelectedRig;
	
	public List<GameObject> _perks = new List<GameObject>();
	public PerkMenuItem SelectedPerk;
	
	public List<GameObject> _perksB = new List<GameObject>();
	public PerkMenuItemB SelectedPerkB;
	
	public List<GameObject> _levels = new List<GameObject>();
	public LevelMenuItem SelectedLevel;
	
	// cameras
	// mainCamera		0
	// hubCamera3D		1
	// hubCameraGUI		2
	public Camera[] cameras;
	
	// player statistics gameObject folder
	public GameObject playerStatistics;
	
	// dynamic text fields for name and description of perk
	public GameObject perkName;
	public GameObject perkDescription;
	
	// prevent menus from tweening at the same time
	private bool isTweening = false;
	
	// rigs
	private GameObject[] rigs;
	
	// varaibles to store information for the dynamic circle creation
	public Texture2D circleImage;
	
	public List<GameObject> acceleration_circles = new List<GameObject>();
	public List<GameObject> speed_circles = new List<GameObject>();
	public List<GameObject> handling_circles = new List<GameObject>();
	public List<GameObject> offense_circles = new List<GameObject>();
	public List<GameObject> defense_circles = new List<GameObject>();
	
	// to control the orbs physics
	public TTSRacer racer;
	
	//Fade Variables
	private float alphaFadeValue = 0.0f;
    private float transitionTimeIn = 0.1f;
	//public Texture Overlay;
	
	//Spawn zones
	public GameObject spawn_mp;
	public GameObject spawn_sp;
	
	// acceleration	0
	// speed		1
	// handling		2
	// offense		3
	// defense		4
	public int[] numCircles;
	
	// 0 MP Select	1		2x1
	// 1 MP MENU	1		2x1
	// 2 MP LOBBY	1		2x1
	// 3 READYUP	
	// 4 RIG		11		2x3
	// 5 PERKA		11		3x3
	// 6 PERKB		11		3x3
	// 7 LEVEL		11		3x2
	public GameObject[] panels;
	
	// indices for each panel
	public int[] indices;
	
	// visible panel and the previously visible one
	public int activePanel;
	public int previousPanel;
	
	// how many players, and how many have selected orbs
	public int numPlayers = 3;
	public int chosenOrb = 1;
	
	// Is either a string saying multiplayer or singleplayer
	public string gameMode;

	// Use this for initialization
	void Start () {
		previousPanel = 7;
		
		cameras[0].enabled = true;
		cameras[1].enabled = false;
		cameras[2].enabled = false;
		
		// populate arrays with their gameObjects
		GameObject[] perks = GameObject.FindGameObjectsWithTag("PerkMenuItem");
		GameObject[] perksB = GameObject.FindGameObjectsWithTag("PerkMenuItemB");
		GameObject[] levels = GameObject.FindGameObjectsWithTag("LevelMenuItem");
		rigs = GameObject.FindGameObjectsWithTag("RigMenuItem");
		
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
		createCircles("Offense", -455, 98);
		createCircles("Defense", -455, 128);
		
		HighlightRig();
		HighlightPerk();
		HighlightPerkB();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(gameMode == "singleplayer" || gameMode == "splitscreen" || gameMode == "online"){
			if(Input.GetKeyDown(KeyCode.Return))
				changePanels("right");
			
			if(Input.GetKeyDown(KeyCode.Backspace))
				changePanels("left");			

			if(panels[4].transform.position.x == 0.5 || panels[5].transform.position.x == 0.5 || 
			   panels[6].transform.position.x == 0.5 || panels[7].transform.position.x == 0.5)
				playerStatistics.SetActive(true);
			else
				playerStatistics.SetActive(false);
			
			menuControls();
			
			racer.canMove = false;
			racer.transform.rotation = Quaternion.Euler(0, 0, 180);
			racer.calcOrientation = false;
			racer.ManualOrientation(new Vector3(spawn_sp.transform.position.x, spawn_sp.transform.position.y, spawn_sp.transform.position.z));
			racer.SlowToStopToPosition(spawn_mp);
				
			cameras[0].enabled = false;
			cameras[1].enabled = true;
			cameras[2].enabled = true;
		}
	}
	
	private void menuControls(){
		int index = indices[activePanel];
		
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
				}
			}

			else if(direction == "right"){
				if(index == 11 || index == 21 || index == 31){
					index += 1;
				}
			}
			
			else if(direction == "down"){
				if(index < 30){
					index += 10;
				}
			}

			else if(direction == "up"){
				if(index > 20){
					index -= 10;
				}
			}
		}
		
		// 3x2 menu system
		if(activePanel == 4){
			if(direction == "left"){
				if(index%10 > 1){
					index -= 1;
				}
			}
			
			else if(direction == "right"){
				if(index%10 < 3){
					index += 1;
				}
			}
			
			else if(direction == "down"){
				if(index==11 || index==12 || index==13){
					index += 10;
				}
			}
			
			else if(direction == "up"){
				if(index==21 || index==22 || index==23){
					index -= 10;
				}
			}
		}
		
		// 3x3 menu system
		if(activePanel == 5 || activePanel == 6){
			if(direction == "right"){
				if(index%10 == 1 || index%10 == 2){
					index ++;
				}
			}
			
			if(direction == "left"){
				if(index%10 == 2 || index%10 == 3){
					index --;
				}
			}
			
			if(direction == "up"){
				if(index>20 && index<34){
					index -= 10;
				}
			}
			
			if(direction =="down"){
				if(index>10 && index<24){
					index += 10;
				}
			}			
		}
		
		// 2x1
		if(activePanel == 0 || activePanel == 1 || activePanel == 2){
			if(direction == "right"){
				if(index == 1){
					index = 2;
				}
			}
				
			if(direction == "left"){
				if(index == 2){
					index = 1;
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
	
	private void changePanels(string direction){
		
		if(direction == "right"){
			if(gameMode == "singleplayer"){
				if(activePanel < 7){
					activePanel++;
					if(activePanel == 5 || activePanel == 7 && !isTweening){
						previousPanel = (activePanel-1);
						isTweening = true;
						movePanel();
					}
				}
			}	
			
			else if(gameMode == "splitscreen"){
				if(activePanel == 0){
					activePanel += 3;
					previousPanel = (activePanel - 3);
					isTweening = true;
					movePanel();
				}
				
				else if(activePanel < 7){
					activePanel++;
					if(activePanel == 4 || activePanel == 5 && !isTweening){
						previousPanel = (activePanel-1);
						isTweening = true;
						movePanel();
					}
					
					
					else if(activePanel == 7 && !isTweening){
						// go to levelSelect
						if(chosenOrb == numPlayers){
							previousPanel = (activePanel-1);;
							isTweening = true;
							movePanel();
						}
						
						// go to rigMenu
						else if(chosenOrb != numPlayers){
							activePanel = 4;
							previousPanel = 6;
							isTweening = true;
							chosenOrb++;
							movePanel();
						}
					}
				}
			}
		}
		
		if(direction == "left"){
			if(gameMode == "singleplayer"){
				if(activePanel > 4){
					activePanel--;
					if(activePanel == 4 || activePanel == 6 && !isTweening){
						previousPanel = (activePanel+1);
						isTweening = true;
						movePanel();
					}
				}
			}
			
			else if(gameMode == "splitscreen"){
				if(activePanel == 3){
					activePanel -= 3;
					previousPanel = (activePanel + 3);
					isTweening = true;
					movePanel();
				}
			}
		}
	}
	
	public void movePanel(){
		// move in next panel
		iTween.MoveTo(panels[activePanel], iTween.Hash("x", 0.5, "time", 2.0f, "onComplete", "stoppedTweening", "onCompleteTarget", gameObject));
				
		// move out last panel
		iTween.MoveTo(panels[previousPanel], iTween.Hash("x", -5, "time", 2.0f, "onComplete", "stoppedTweening", "onCompleteTarget", gameObject));
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
		if(indices[0] == 1)
			gameMode = "splitscreen";
		
		else if(indices[0] == 2)
			gameMode = "online";
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
			if(r.GetComponent<TTSMenuItemRig>().index!=indices[4])
				r.SetActive(false);

			else{
				SelectedRig = r.GetComponent<TTSMenuItemRig>().rig;
				r.SetActive(true);
				
				numCircles[0]  = r.GetComponent<TTSMenuItemRig>().acceleration;
				numCircles[1]  = r.GetComponent<TTSMenuItemRig>().speed;
				numCircles[2]  = r.GetComponent<TTSMenuItemRig>().handling;
				numCircles[3]  = r.GetComponent<TTSMenuItemRig>().offense;
				numCircles[4]  = r.GetComponent<TTSMenuItemRig>().defense;
				
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
			obj.layer = 11;
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
		if(gameMode == "singleplayer" || gameMode == "splitscreen" || gameMode == "online"){
        	GUI.color = new Color(0, 0, 0, alphaFadeValue);
       		//GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height ), Overlay);
		}
	}
}
		