using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TTSMenuController : TTSMenuEnums {
	
	public List<GameObject> _characters = new List<GameObject>();
	public CharacterMenuItem SelectedCharacter;
	
	public List<GameObject> _perks = new List<GameObject>();
	public PerkMenuItem SelectedPerk;
	
	public List<GameObject> _rigs = new List<GameObject>();
	
	public List<GameObject> _levels = new List<GameObject>();
	public LevelMenuItem SelectedLevel;

	private bool isTweenCompleted = true;
	
	//each of the panels (blue, green, grey)
	public GameObject BlueP;
	public GameObject GreenP;
	public GameObject GreyP;
	
	//nodes are attached to verticies on the menuController model
	public GameObject FrontNode;
	public GameObject LeftMidNode;
	public GameObject LeftRearNode;
	public GameObject RightMidNode;
	public GameObject RightRearNode;
	
	public AudioClip menuHighlight;
	public AudioClip menuSelect;
	
	public GameObject CharacterHighlighter;
	public GameObject PerkHighlighter;
	public GameObject LevelHighlighter;

	//used to change the menuController
	public int changer = 2;
	
	public int selectedCharacterIndex = 11;
	public int selectedPerkIndex = 11;
	public int selectedLevelIndex = 11;

	public PerkMenuItem chosenPerk;
	public CharacterMenuItem chosenCharacter;
	public LevelMenuItem chosenLevel;
	public int numHumans;
	
	public GUIStyle TextStyle;
	private string text;
	private Texture2D texture;
	
	public GameObject[] characters;
	public GameObject[] decals;
	
	void Awake(){	
		texture = new Texture2D(1,1);
		texture.SetPixel(0,0,new Color(1f,1f,1f,1f));
		texture.Apply();
	}
	
	// Use this for initialization
	void Start(){
		numHumans = 1;
		GameObject[] perks = GameObject.FindGameObjectsWithTag("PerkMenuItem");
		GameObject[] levels = GameObject.FindGameObjectsWithTag("LevelMenuItem");
		characters = GameObject.FindGameObjectsWithTag("CharacterMenuItem");
		
		foreach(GameObject c in characters)
		{
			_characters.Add(c);
			c.GetComponent<TTSMenuItemCharacter>().decal.renderer.enabled = false;
		}
		
		foreach(GameObject p in perks)
		{
			_perks.Add(p);
		}
		
		foreach(GameObject l in levels)
		{
			_levels.Add(l);
		}
		
		HighlightCharacter();
	}
	
	void LaunchTween(){
		isTweenCompleted = true;
    }
	
	void LaunchDelay(){
		if(changer==2){
			foreach(GameObject c in characters){
				if(c.GetComponent<TTSMenuItemCharacter>().isSelected==true){
					c.GetComponent<TTSMenuItemCharacter>().decal.renderer.enabled = true;
				}
			}
		}	
	}
	
    void Example(){
        Invoke("LaunchTween", 1);
    }
	
	void Delay(){
		Invoke("LaunchDelay", 1);
	}
	// Update is called once per frame
	void Update () {
		if(isTweenCompleted){
			menuControls();
		}
	
		if(Input.GetKeyDown(KeyCode.Return)) {
			if(changer>0){
				
				foreach(GameObject c in characters)
				{
					c.GetComponent<TTSMenuItemCharacter>().decal.renderer.enabled = false;
				}
				
				changer -= 1;
				audio.PlayOneShot(menuSelect);
				Controller();
			}	
		}
		
		if(Input.GetKeyDown(KeyCode.Backspace)){
			if(changer<2){	
				changer += 1;
				
				if(changer==2)
					Delay();
				
				Controller();	
			}
		}
		
		if(Input.GetKeyDown(KeyCode.Y)){
			Application.LoadLevelAsync("loadingScene");
		}
		
		if(Input.GetKeyDown(KeyCode.B)){				
			if(changer == 0)
				chosenLevel = SelectedLevel;
			
			else if(changer == 1)
				chosenPerk = SelectedPerk;
			
			else if(changer == 2)
				chosenCharacter = SelectedCharacter;
		}	
	}

	private void menuControls(){
		int index = 0;
		switch(changer){
			case 0:
				index = selectedLevelIndex;
			break;
			
			case 1:
				index = selectedPerkIndex;
			break;
			
			case 2:
				index = selectedCharacterIndex;
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
	
	private void ChangeIndex(string direction, int index){		
		if(direction == "left"){
			if((index-4)>10)
				index -= 10;
		}
		
		else if(direction == "right"){
			if((index+9)<30)
				index += 10;
		}
		
		else if(direction == "down"){
			if((index%10+1) <= 3)
				index += 1;
		}
		
		else if(direction == "up"){
			if((index%10-1) >= 1)
				index -= 1;
		}
	
		switch(changer){
			case 0:
				selectedLevelIndex = index;
				HighlightLevel();
			break;
			
			case 1:
				selectedPerkIndex = index;
				HighlightPerk();
			break;
			
			case 2:
				selectedCharacterIndex = index;
				HighlightCharacter();
			break;
			
			default:
			//Play a sound?
			break;
		}
	}
	
	private void HighlightCharacter(){
		foreach(GameObject c in _characters){
			c.GetComponent<TTSMenuItemCharacter>().decal.renderer.enabled = false;
			if(selectedCharacterIndex == c.GetComponent<TTSMenuItemCharacter>().index){
				c.GetComponent<TTSMenuItemCharacter>().isSelected = true;
				
				c.GetComponent<TTSMenuItemCharacter>().decal.renderer.enabled = true;
				
				SelectedCharacter = c.GetComponent<TTSMenuItemCharacter>().character;
			
				Vector3 positionVector = new Vector3((c.transform.position.x + 1.0f),
										  c.transform.position.y,
										  CharacterHighlighter.transform.position.z);
				audio.PlayOneShot(menuHighlight);
				iTween.MoveTo(CharacterHighlighter,positionVector,0);
			}
			
			if(selectedCharacterIndex != c.GetComponent<TTSMenuItemCharacter>().index){
				c.GetComponent<TTSMenuItemCharacter>().isSelected = false;
			}
		}
	}
		
	private void HighlightPerk(){
		foreach(GameObject p in _perks){
			if(selectedPerkIndex == p.GetComponent<TTSMenuItemPerk>().index){
				p.GetComponent<TTSMenuItemPerk>().isSelected = true;	
				SelectedPerk = p.GetComponent<TTSMenuItemPerk>().perk;
			
				Vector3 positionVector = new Vector3((p.transform.position.x + 1.0f),
										  p.transform.position.y,
										  PerkHighlighter.transform.position.z);
				audio.PlayOneShot(menuHighlight);
				iTween.MoveTo(PerkHighlighter,positionVector,0);
			}
		}
	}
	
	private void HighlightLevel(){
		foreach(GameObject l in _levels){
			if(selectedLevelIndex == l.GetComponent<TTSMenuItemLevel>().index){
				l.GetComponent<TTSMenuItemLevel>().isSelected = true;
				SelectedLevel = l.GetComponent<TTSMenuItemLevel>().level;
			
				Vector3 positionVector = new Vector3((l.transform.position.x + 1.0f),
										  l.transform.position.y,
										  LevelHighlighter.transform.position.z);
				audio.PlayOneShot(menuHighlight);
				iTween.MoveTo(LevelHighlighter,positionVector,0);
			}
		}
	}
	
	void OnGUI(){
		TextStyle.fontSize = 36;
		TextStyle.normal.textColor = Color.white;
		TextStyle.fontStyle = FontStyle.Bold;
		
		switch(changer){
			case 0:
				text = SelectedLevel.ToString();
			break;
			
			case 1:
				text = SelectedPerk.ToString();
			break;
			
			case 2:
				text = SelectedCharacter.ToString();
			break;
			
			default:
			//Play a sound?
			break;
		}
		GUI.Label (new Rect(180,20,200,200), text,TextStyle );
	}
	
	// 0 = left position
	// 1 = middle position
	// 2 = right position
	void Controller(){
		isTweenCompleted = false;
		
		switch(changer){
			case 0:
			// start invoke
			iTween.MoveTo(BlueP, iTween.Hash("position",LeftRearNode.transform.position, "x", 1.0f, "onComplete", "TweenComplete", "onCompleteTarget", gameObject));
			iTween.MoveTo(GreenP, iTween.Hash("position",LeftMidNode.transform.position, "x", 1.0f, "onComplete", "TweenComplete", "onCompleteTarget", gameObject));
			iTween.MoveTo(GreyP, iTween.Hash("position",FrontNode.transform.position, "x", 1.0f, "onComplete", "TweenComplete", "onCompleteTarget", gameObject));
			Example();
			break;
			
			case 1:
			// start invoke
			iTween.MoveTo(BlueP, iTween.Hash("position",LeftMidNode.transform.position, "x", 1.0f, "onComplete", "TweenComplete", "onCompleteTarget", gameObject));
			iTween.MoveTo(GreenP, iTween.Hash("position",FrontNode.transform.position, "x", 1.0f, "onComplete", "TweenComplete", "onCompleteTarget", gameObject));
			iTween.MoveTo(GreyP, iTween.Hash("position",RightMidNode.transform.position, "x", 1.0f, "onComplete", "TweenComplete", "onCompleteTarget", gameObject));
			Example();
			break;
			
			case 2:
			// start invoke
			iTween.MoveTo(BlueP, iTween.Hash("position",FrontNode.transform.position, "x", 1.0f, "onComplete", "TweenComplete", "onCompleteTarget", gameObject));
			iTween.MoveTo(GreenP, iTween.Hash("position",RightMidNode.transform.position, "x", 1.0f, "onComplete", "TweenComplete", "onCompleteTarget", gameObject));
			iTween.MoveTo(GreyP, iTween.Hash("position",RightRearNode.transform.position, "x", 1.0f, "onComplete", "TweenComplete", "onCompleteTarget", gameObject));
			Example();		
			break;
			
			default:
			//Play a sound?
			break;
		}	
	}
}