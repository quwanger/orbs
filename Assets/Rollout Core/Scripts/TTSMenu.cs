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
	
	//Selection indices
	public int selectedRigIndex = 11;
	public int selectedPerkIndex = 1;
	public int selectedPerkBIndex = 1;
	public int selectedLevelIndex = 11;
	public int changer = 3;
	public bool isTweening = false;
	public GameObject[] rigs;
	
	//text variables
	public GUIText RigModelName;
	private string RigModelText;
	
	void Awake(){

	}
	
	void Start() {
		//Turn off the main camera and enable menu cam
		menuCamera.enabled = false;
		hubCamera.enabled = true;

		//The menu background
		Rect tempInset = new Rect(-640, -360, 3840, 720);
		backPanel.pixelInset = tempInset;
		
		//Initiate indices
		GameObject[] perks = GameObject.FindGameObjectsWithTag("PerkMenuItem");
		GameObject[] perksB = GameObject.FindGameObjectsWithTag("PerkMenuItemB");
		GameObject[] levels = GameObject.FindGameObjectsWithTag("LevelMenuItem");
		rigs = GameObject.FindGameObjectsWithTag("RigMenuItem");
		
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
					iTween.MoveBy(backPanel.gameObject, iTween.Hash("x", -0.89, "time", 2.0f, "onComplete", "stoppedTweening", "onCompleteTarget", gameObject));
				}
			}	
		}
		
		if(Input.GetKeyDown(KeyCode.Backspace)) {
			if(changer<3&&!isTweening){	
				changer += 1;
				if(changer !=2){
					isTweening = true;
					iTween.MoveBy(backPanel.gameObject, iTween.Hash("x", 0.89, "time", 2.0f, "onComplete", "stoppedTweening", "onCompleteTarget", gameObject));
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
		// rig menu system
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
		
		// perk menu system A
		if(changer == 2)
		{
			if(direction == "right"){
				if(index<8)
					index ++;
			}
			
			if(direction == "left"){
				if(index>1)
					index --;
			}
		}
		
		// perk menu system B
		if(changer == 1)
		{
			if(direction == "right"){
				if(index<7)
					index ++;
			}
			
			if(direction == "left"){
				if(index>1)
					index --;
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
			
			default:
			//Play a sound?
			break;
		}
		
		//HighlightRig();
	}
	
	
	private void HighlightPerk(){
		foreach(GameObject p in _perks){	
			if(p.GetComponent<TTSMenuItemPerk>().index!=selectedPerkIndex)
				p.SetActive(false);
			
			else{
				SelectedPerk = p.GetComponent<TTSMenuItemPerk>().perk;
				p.SetActive(true);
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
			}
		}
	}
	
	private void HighlightRig(){
		foreach(GameObject r in _rigs){	
			if(r.GetComponent<TTSMenuItemRig>().index!=selectedRigIndex)
				r.SetActive(false);

			else{
				SelectedRig = r.GetComponent<TTSMenuItemRig>().rig;
				r.SetActive(true);
			}
		}
	}
	
	void OnGUI(){
		if(zone || unzone){
        	GUI.color = new Color(0, 0, 0, alphaFadeValue);
       		//GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height ), Overlay);
		}
		
		switch(changer){
			case 0:
				RigModelText = SelectedLevel.ToString();
			break;
			
			case 1:
				RigModelText = SelectedPerkB.ToString();
			break;
			
			case 2:
				RigModelText = SelectedPerk.ToString();
			break;
			
			case 3:
				RigModelText = SelectedRig.ToString();
				RigModelName.text = RigModelText;
			break;
			
			default:
			//Play a sound?
			break;
		}
	}
}