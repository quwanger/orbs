using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TTSMenu : TTSMenuEnums {
	
	public List<GameObject> _rigs = new List<GameObject>();
	public RigMenuItem SelectedRig;
	
	public List<GameObject> _perks = new List<GameObject>();
	public PerkMenuItem SelectedPerk;
	
	public List<GameObject> _perksB = new List<GameObject>();
	public PerkMenuItemB SelectedPerkB;
	
	public List<GameObject> _levels = new List<GameObject>();
	public LevelMenuItem SelectedLevel;
	
	//Cameras
	public Camera menuCamera;
	public Camera hubCamera;
	
	//Containers
	public GUITexture backPanel;
	
	//Fade Variables
	private float alphaFadeValue = 0.0f;
    private float transitionTimeIn = 0.1f;
    public bool zone = false;
	public bool unzone = false;
	public Texture Overlay;
	
	//Racer
	public TTSRacer racer;

	//Spawn zones
	public GameObject spawn_mp;
	public GameObject spawn_sp;
	
	public GameObject perkName;
	public GameObject perkDescription;
	
	//Selection indices
	public int selectedRigIndex = 11;
	public int selectedPerkIndex = 11;
	public int selectedPerkBIndex = 11;
	public int selectedLevelIndex = 11;
	public int selectedMultiplayerIndex = 11;
	public int changer = 3;
	private bool isTweening = false;
	private GameObject[] rigs;
	
	public Texture2D image;
	
	public List<GameObject> acceleration_circles = new List<GameObject>();
	public List<GameObject> speed_circles = new List<GameObject>();
	public List<GameObject> handling_circles = new List<GameObject>();
	public List<GameObject> offense_circles = new List<GameObject>();
	public List<GameObject> defense_circles = new List<GameObject>();
	
	private int numAccCircles;
	private int numSpeedCircles;
	private int numHandlCircles;
	private int numOffCircles;
	private int numDefCircles;

	void Awake(){

	}
	
	void Start() {
		//Turn off the main camera and enable menu cam
		menuCamera.enabled = false;
		hubCamera.enabled = true;

		//The menu background
		//Rect tempInset = new Rect(-1920, -360, 4960, 720);
		Rect tempInset = new Rect(-640, -360, 4960, 720);
		backPanel.pixelInset = tempInset;
		
		//Initiate indices
		GameObject[] perks = GameObject.FindGameObjectsWithTag("PerkMenuItem");
		GameObject[] perksB = GameObject.FindGameObjectsWithTag("PerkMenuItemB");
		GameObject[] levels = GameObject.FindGameObjectsWithTag("LevelMenuItem");
		
		rigs = GameObject.FindGameObjectsWithTag("RigMenuItem");
		
		perkDescription.guiText.font.material.color = new Color32(70, 70, 70, 255);
		
		foreach(GameObject r in rigs)
		{
			_rigs.Add(r);
		}
		
		foreach(GameObject p in perks)
		{
			_perks.Add(p);
		}
		
		foreach(GameObject p in perksB)
		{
			_perksB.Add(p);
		}
		
		foreach(GameObject l in levels)
		{
			_levels.Add(l);
		}
		
		createCircles("Acceleration", -430, -152);
		createCircles("Speed", -430, -122);
		createCircles("Handling", -430, -182);
		createCircles("Offense", -455, 98);
		createCircles("Defense", -455, 128);
		
		//Control highlights
		HighlightRig();
		HighlightPerkB();
		HighlightPerk();
		
	}

	// Update is called once per frame
	void Update () {		
		menuControls();
		
		if(Input.GetKeyDown(KeyCode.Return)) {
			if(changer>0&&!isTweening){
				changer -= 1;
				if(changer != 1){
					isTweening = true;
					iTween.MoveBy(backPanel.gameObject, iTween.Hash("x", -0.73, "time", 2.0f, "onComplete", "stoppedTweening", "onCompleteTarget", gameObject));
				}
			}	
		}
		
		if(Input.GetKeyDown(KeyCode.Backspace)) {
			if(changer<4&&!isTweening){	
				changer += 1;
				if(changer !=2){
					isTweening = true;
					iTween.MoveBy(backPanel.gameObject, iTween.Hash("x", 0.73, "time", 2.0f, "onComplete", "stoppedTweening", "onCompleteTarget", gameObject));
					//
				}
			}
		}

		if(zone) {
			racer.canMove = false;
			racer.transform.rotation = Quaternion.Euler(0, 0, 180);
			racer.calcOrientation = false;
			racer.ManualOrientation(new Vector3(spawn_sp.transform.position.x, spawn_sp.transform.position.y, spawn_sp.transform.position.z));
			racer.SlowToStopToPosition(spawn_mp);
			
			menuCamera.enabled = true;
			hubCamera.enabled = false;
			
			if(alphaFadeValue < 1.0f)
				alphaFadeValue += transitionTimeIn;
		}else if(unzone){
			if(alphaFadeValue > 0)
				alphaFadeValue -= transitionTimeIn;
		}
	}
	
	private void menuControls(){
		int index = 0;
		switch(changer){
			case 0:
				index = selectedLevelIndex;
			break;
			
			case 1:
				index = selectedPerkBIndex;
			break;
			
			case 2:
				index = selectedPerkIndex;
			break;
			
			case 3:
				index = selectedRigIndex;
			break;
			
			case 4:
				index = selectedMultiplayerIndex;
			break;
			
			default:
			//Play a sound?
			break;
		}
		
		if(Input.GetKeyDown(KeyCode.W))
			ChangeIndex("up", index);
		else if(Input.GetKeyDown(KeyCode.S))
			ChangeIndex("down", index);
		else if(Input.GetKeyDown(KeyCode.A))
			ChangeIndex("left", index);
		else if(Input.GetKeyDown(KeyCode.D))
			ChangeIndex("right", index);
	}
	
	public void stoppedTweening(){
		isTweening = false;	
	}

	private void ChangeIndex(string direction, int index){	
		// 3x2 menu system
		if(changer == 3)
		{
			if(direction == "left"){
				if(index%10 > 1)
					index -= 1;
			}
			
			else if(direction == "right"){
				if(index%10 < 3)
					index += 1;
			}
			
			else if(direction == "down"){
				if(index==11 || index==12 || index==13)
					index += 10;
			}
			
			else if(direction == "up"){
				if(index==21 || index==22 || index==23)
					index -= 10;
			}
		}
		
		// 3x3 menu system
		if(changer == 2 || changer == 1)
		{
			if(direction == "right"){
				if(index%10 == 1 || index%10 == 2)
					index ++;
			}
			
			if(direction == "left"){
				if(index%10 == 2 || index%10 == 3)
					index --;
			}
			
			if(direction == "up"){
				if(index>20 && index<34)
					index -= 10;
			}
			
			if(direction =="down"){
				if(index>10 && index<24)
					index += 10;
			}	
		}

		switch(changer){
			case 0:
				selectedLevelIndex = index;
				//HighlightLevel();
			break;
			
			case 1:
				selectedPerkBIndex = index;
				HighlightPerkB();
			break;
			
			case 2:
				selectedPerkIndex = index;
				HighlightPerk();
			break;
			
			case 3:
				selectedRigIndex = index;
				HighlightRig();
			break;
			
			case 4:
				selectedMultiplayerIndex = index;
				HighlightMultiplayer ();
			break;
			
			default:
			//Play a sound?
			break;
		}
		
		//HighlightRig();
	}
	
	private void HighlightPerk(){
		deactiveCircles();
		foreach(GameObject p in _perks){	
			if(p.GetComponent<TTSMenuItemPerk>().index!=selectedPerkIndex)
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
							toggleCircles("Acceleration", numAccCircles + 3);
						
						if(p.GetComponent<TTSMenuItemPerk>().name == "Speed")
							toggleCircles("Speed", numSpeedCircles + 3);
						
						if(p.GetComponent<TTSMenuItemPerk>().name == "Handling")
							toggleCircles("Handling", numHandlCircles + 3);
						
						if(p.GetComponent<TTSMenuItemPerk>().name == "MAN-O-WAR")
							toggleCircles("Offense", numOffCircles + 3);
						
						if(p.GetComponent<TTSMenuItemPerk>().name == "Diamond Coat")
							toggleCircles("Defense", numDefCircles + 3);
					}
				}
			}
		}
	}
	
	private void HighlightPerkB(){
		foreach(GameObject p in _perksB){	
			if(p.GetComponent<TTSMenuItemPerk>().index!= selectedPerkBIndex)
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
			if(r.GetComponent<TTSMenuItemRig>().index!=selectedRigIndex)
				r.SetActive(false);

			else{
				SelectedRig = r.GetComponent<TTSMenuItemRig>().rig;
				r.SetActive(true);
				
				numAccCircles = r.GetComponent<TTSMenuItemRig>().acceleration;
				numSpeedCircles = r.GetComponent<TTSMenuItemRig>().speed;
				numHandlCircles = r.GetComponent<TTSMenuItemRig>().handling;
				numOffCircles = r.GetComponent<TTSMenuItemRig>().offense;
				numDefCircles = r.GetComponent<TTSMenuItemRig>().defense;
				
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
	void toggleCircles(string circleName, int numCircles){
		for(int i = 1; i <= numCircles; i++){
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
		toggleCircles("Acceleration", numAccCircles);
		toggleCircles("Speed", numSpeedCircles);
		toggleCircles("Handling", numHandlCircles);
		toggleCircles("Offense", numOffCircles);
		toggleCircles("Defense", numDefCircles);
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
			
			obj.guiTexture.texture = image;
			
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
		if(zone || unzone){
        	GUI.color = new Color(0, 0, 0, alphaFadeValue);
       		//GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height ), Overlay);
		}
	}
}