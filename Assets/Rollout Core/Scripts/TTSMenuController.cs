using UnityEngine;
using System.Collections.Generic;




public class TTSMenuController : TTSMenuEnums {
	
	public List<GameObject> _characters = new List<GameObject>();
	public CharacterMenuItem SelectedCharacter;
	
	public List<GameObject> _perks = new List<GameObject>();
	public PerkMenuItem SelectedPerk;
	
	public List<GameObject> _rigs = new List<GameObject>();
	
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
	
	//used to change the menuController
	public int changer = 2;
	public int selectedCharacterIndex = 11;
	public int selectedPerkIndex = 11;
		
	public PerkMenuItem chosenPerk;
	public CharacterMenuItem chosenCharacter;
	
	private Texture2D texture;
	
	private string text;
	private string text2;
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
		
		foreach(GameObject c in characters)
		{
			_characters.Add(c);
		}
		
		foreach(GameObject p in perks)
		{
			_perks.Add(p);
		}
		
		//blue outline around square on launch 
		HighlightCharacter();
		//HighlightPerk();
	}
	
	// Update is called once per frame
	void Update () {			
		menuControls();

		if(Input.GetKeyDown(KeyCode.L)) 
		{
			if(changer>0)
			{
				//Debug.Log (changer);
				changer -= 1;
				//Debug.Log (changer);
				Controller();
			}
		}
		
		else if(Input.GetKeyDown(KeyCode.K)) 
		{
			if(changer<2)
			{
				
				changer += 1;
				
				Controller();
			}
		}
		
		else if(Input.GetKeyDown(KeyCode.Y))
		{
			//float progress;

			AsyncOperation async = Application.LoadLevelAsync("city1-1");

	      	//progress = async.isDone;
			//Debug.Log(progress);
		} 
		
		else if(Input.GetKeyDown(KeyCode.B))
		{
			if(changer == 1)
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
		//22%10 = 2 - 1 = 1 = 1 22 = 22-1 = 21
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
		//22%10 = 2 - 1 = 1 = 1 22 = 22-1 = 21
		HighlightPerk();
	}
	
	private void HighlightCharacter(){
		foreach(GameObject c in _characters)
		{
			if(selectedCharacterIndex == c.GetComponent<TTSMenuItemCharacter>().index)
			{
				c.GetComponent<TTSMenuItemCharacter>().isSelected = true;
				SelectedCharacter = c.GetComponent<TTSMenuItemCharacter>().character;
				c.GetComponent<TTSMenuItemCharacter>().border.renderer.material.color = Color.yellow;
			}else{
				c.GetComponent<TTSMenuItemCharacter>().border.renderer.material.color = Color.white;
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
				p.GetComponent<TTSMenuItemPerk>().border.renderer.material.color = Color.yellow;
				//message();
			}else{
				p.GetComponent<TTSMenuItemPerk>().border.renderer.material.color = Color.white;
			}
		}
	}
	
	void OnGUI(){
		if(changer==1)
		{
			//Debug.Log (changer);
			GUI.color = new Color(0f,0f,0f);
			GUI.skin.box.normal.background = texture;
			//GUI.Box(new Rect(Screen.width/4,Screen.height/0.8f,Screen.width/2,Screen.height/2), "");
			GUI.color = new Color(1F,1F,1F,0.5f);
			GUI.skin.label.alignment = TextAnchor.UpperCenter;
			GUI.skin.label.fontSize = 24;
			GUI.skin.label.fontStyle = FontStyle.Bold;

			text = SelectedPerk.ToString();
			//message.ToUpper ();
			
			GUI.Label (new Rect(Screen.width/4,Screen.height/1.2f,Screen.width/2,Screen.height), text);
		}
		
		else if(changer==2)
		{
			//Debug.Log (changer);
			GUI.color = new Color(0f,0f,0f);
			GUI.skin.box.normal.background = texture;
			//GUI.Box(new Rect(Screen.width/4,Screen.height/0.8f,Screen.width/2,Screen.height/2), "");
			GUI.color = new Color(1F,1F,1F,0.5f);
			GUI.skin.label.alignment = TextAnchor.UpperCenter;
			GUI.skin.label.fontSize = 24;
			GUI.skin.label.fontStyle = FontStyle.Bold;

			text2 = SelectedCharacter.ToString();
			//message.ToUpper ();
			
			GUI.Label (new Rect(Screen.width/4,Screen.height/1.2f,Screen.width/2,Screen.height), text2);
		}
	}
	
	// 0 = left position
	// 1 = middle position
	// 2 = right position
	void Controller () {
		switch(changer) {
			case 0:
			iTween.MoveTo(BlueP,LeftRearNode.transform.position,2);
			iTween.MoveTo(GreenP,LeftMidNode.transform.position,2);
			iTween.MoveTo(GreyP,FrontNode.transform.position,2);
			break;
			
			case 1:
			iTween.MoveTo(BlueP,LeftMidNode.transform.position,2);
			iTween.MoveTo(GreenP,FrontNode.transform.position,2);
			iTween.MoveTo(GreyP,RightMidNode.transform.position,2);
			HighlightPerk();
		
			break;
			
			case 2:
			iTween.MoveTo(BlueP,FrontNode.transform.position,2);
			iTween.MoveTo(GreenP,RightMidNode.transform.position,2);
			iTween.MoveTo(GreyP,RightRearNode.transform.position,2);
			break;
			
			default:
			//Play a sound?
			break;
		}	
	}
}