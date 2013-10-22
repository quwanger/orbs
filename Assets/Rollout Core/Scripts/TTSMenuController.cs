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

	private bool frontUnlocked = true;
	private bool midUnlocked = false;
	private bool backUnlocked = false;
	
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
	
	public GameObject Highlighter;
	
	//used to change the menuController
	public int changer = 2;
	
	public int selectedCharacterIndex = 11;
	public int selectedPerkIndex = 11;
	public int selectedLevelIndex = 11;
	//public int selectedIndex = 11;	
	
	public PerkMenuItem chosenPerk;
	public CharacterMenuItem chosenCharacter;
	public LevelMenuItem chosenLevel;
	
	public GUIStyle TextStyle;
	
	private Texture2D texture;
	
	private string text;
	private string text2;
	private string text3;
	
	public GameObject[] decals;
	
	void Awake() {	
		//Debug.Log(SelectedPerk);
		texture = new Texture2D(1,1);
		texture.SetPixel(0,0,new Color(1f,1f,1f,1f));
		texture.Apply();
	}
	
	// Use this for initialization
	void Start () {
		GameObject[] characters = GameObject.FindGameObjectsWithTag("CharacterMenuItem");
		GameObject[] perks = GameObject.FindGameObjectsWithTag("PerkMenuItem");
		GameObject[] levels = GameObject.FindGameObjectsWithTag("LevelMenuItem");
			
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
		
		//blue outline around square on launch 
		HighlightCharacter();
	}

	void TweenComplete(){
		//Debug.Log("The Tween completed!");	
		//TweenComplete();
		
	}
	
	 void LaunchTween() {
        isTweenCompleted = true;
    }
	
    void Example() {
        Invoke("LaunchTween", 1);
    }

	// Update is called once per frame
	void Update () {
		
		if(isTweenCompleted){
			menuControls();
		}
		
		if(Input.GetKeyDown(KeyCode.Return)) 
		{
			if(changer>0)
			{
				changer -= 1;
				audio.PlayOneShot(menuSelect);
				Controller();
			}
		}
		
		else if(Input.GetKeyDown(KeyCode.Backspace)) 
		{
			if(changer<2)
			{
				changer += 1;
				Controller();
			}
		}
		
		else if(Input.GetKeyDown(KeyCode.Y))
		{
			AsyncOperation async = Application.LoadLevelAsync("loadingScene");
		} 
		
		else if(Input.GetKeyDown(KeyCode.B))
		{
						
			if(changer == 0)
			{
				chosenLevel = SelectedLevel;
			}
			
			else if(changer == 1)
			{
				chosenPerk = SelectedPerk;
			}
			
			else if(changer == 2)
			{
				chosenCharacter = SelectedCharacter;
			}
		}	
	}

	private void menuControls(){
		if(changer == 2)
		{
			if(Input.GetKeyDown(KeyCode.W))
				ChangeCharacterIndex("up");
			else if(Input.GetKeyDown(KeyCode.S))
				ChangeCharacterIndex("down");
			else if(Input.GetKeyDown(KeyCode.A))
				ChangeCharacterIndex("left");
			else if(Input.GetKeyDown(KeyCode.D))
				ChangeCharacterIndex("right");
		}
		
		else if(changer == 1)
		{
			if(Input.GetKeyDown(KeyCode.W))
				ChangePerkIndex("up");
			else if(Input.GetKeyDown(KeyCode.S))
				ChangePerkIndex("down");
			else if(Input.GetKeyDown(KeyCode.A))
				ChangePerkIndex("left");
			else if(Input.GetKeyDown(KeyCode.D))
				ChangePerkIndex("right");
		}
		
		else if(changer == 0)
		{
			if(Input.GetKeyDown(KeyCode.W))
				ChangeLevelIndex("up");
			else if(Input.GetKeyDown(KeyCode.S))
				ChangeLevelIndex("down");
			else if(Input.GetKeyDown(KeyCode.A))
				ChangeLevelIndex("left");
			else if(Input.GetKeyDown(KeyCode.D))
				ChangeLevelIndex("right");
		}
	}
	
	private void ChangeLevelIndex(string direction){
		if(direction == "left"){
			if((selectedLevelIndex-4)>10)
				selectedLevelIndex -= 10;
		}
		
		else if(direction == "right"){
			if((selectedLevelIndex+9)<30)
				selectedLevelIndex += 10;
		}
		
		else if(direction == "down"){
			if((selectedLevelIndex%10+1) <= 3)
				selectedLevelIndex += 1;
		}
		
		else if(direction == "up"){
			if((selectedLevelIndex%10-1) >= 1)
				selectedLevelIndex -= 1;
		}
		
		HighlightLevel();
	}
	
	private void ChangeCharacterIndex(string direction){
		if(direction == "left"){
			if((selectedCharacterIndex-4)>10)
				selectedCharacterIndex -= 10;
		}
		
		else if(direction == "right"){
			if((selectedCharacterIndex+9)<30)
				selectedCharacterIndex += 10;
		}
		
		else if(direction == "down"){
			if((selectedCharacterIndex%10+1) <= 3)
				selectedCharacterIndex += 1;
		}
		
		else if(direction == "up"){
			if((selectedCharacterIndex%10-1) >= 1)
				selectedCharacterIndex -= 1;
		}
		
		HighlightCharacter();
	}
	
	private void ChangePerkIndex(string direction){
		if(direction == "left"){
			if((selectedPerkIndex-4)>10)
				selectedPerkIndex -= 10;
		}
		
		else if(direction == "right"){
			if((selectedPerkIndex+9)<30)
				selectedPerkIndex += 10;
		}
		
		else if(direction == "down"){
			if((selectedPerkIndex%10+1) <= 3)
				selectedPerkIndex += 1;
		}
		
		else if(direction == "up"){
			if((selectedPerkIndex%10-1) >= 1)
				selectedPerkIndex -= 1;
		}
		
		HighlightPerk();
	}
	
	private void HighlightCharacter(){
		foreach(GameObject c in _characters)
		{
			c.GetComponent<TTSMenuItemCharacter>().decal.renderer.enabled = false;
			if(selectedCharacterIndex == c.GetComponent<TTSMenuItemCharacter>().index)
			{
				c.GetComponent<TTSMenuItemCharacter>().isSelected = true;
				
				c.GetComponent<TTSMenuItemCharacter>().decal.renderer.enabled = true;
				
				SelectedCharacter = c.GetComponent<TTSMenuItemCharacter>().character;
			
				Vector3 positionVector = new Vector3((c.transform.position.x + 1.0f),
										  c.transform.position.y,
										  CharacterHighlighter.transform.position.z);
				audio.PlayOneShot(menuHighlight);
				iTween.MoveTo(CharacterHighlighter,positionVector,0);
			}
		}
	}
		
	private void HighlightPerk(){
		foreach(GameObject p in _perks)
		{
			if(selectedPerkIndex == p.GetComponent<TTSMenuItemPerk>().index)
			{
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
		foreach(GameObject l in _levels)
		{
			if(selectedLevelIndex == l.GetComponent<TTSMenuItemLevel>().index)
			{
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
		if(changer==0)
		{
			TextStyle.fontSize = 36;
			TextStyle.normal.textColor = Color.white;
			TextStyle.fontStyle = FontStyle.Bold;
			text3 = SelectedLevel.ToString();

			GUI.Label (new Rect(180,20,200,200), text3,TextStyle );
		}
		
		else if(changer==1)
		{
			TextStyle.fontSize = 36;
			TextStyle.normal.textColor = Color.white;
			TextStyle.fontStyle = FontStyle.Bold;
			text = SelectedPerk.ToString();

			GUI.Label (new Rect(180,20,200,200), text,TextStyle );
		}
		
		else if(changer==2)
		{
			TextStyle.fontSize = 36;
			TextStyle.normal.textColor = Color.white;
			TextStyle.fontStyle = FontStyle.Bold;
			text2 = SelectedCharacter.ToString();

			GUI.Label (new Rect(180,20,200,200), text2,TextStyle );
		}
	}
	
	// 0 = left position
	// 1 = middle position
	// 2 = right position
	void Controller () {
		
		isTweenCompleted = false;
		
		switch(changer) {
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