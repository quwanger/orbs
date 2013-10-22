using UnityEngine;
using System.Collections;

public class TTSPersistenceManager : MonoBehaviour {
	
	public string NextScene;
	public string chosenPerk;
	public string chosenCharacter;
	public string chosenLevel;
	public GameObject controller;

	// Use this for initialization
	void Awake () 
	{
		NextScene = "city1-1";
		chosenPerk = controller.GetComponent<TTSMenuController>().chosenPerk.ToString();
		chosenCharacter = controller.GetComponent<TTSMenuController>().chosenCharacter.ToString();
		chosenLevel = controller.GetComponent<TTSMenuController>().chosenLevel.ToString();
		
		DontDestroyOnLoad(this);
	}
	
	public void loadLevel()
	{
		Application.LoadLevel(NextScene);
	}
}
/*
using UnityEngine;
using System.Collections;

public class TTSPersistenceManager : MonoBehaviour {
	
	public string NextScene;
	public string FirstScene;
	
	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(this);
		Application.LoadLevel(FirstScene);
	}
	
}*/