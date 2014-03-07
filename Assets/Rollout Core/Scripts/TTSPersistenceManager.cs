using UnityEngine;
using System.Collections;

public class TTSPersistenceManager : MonoBehaviour {
	
	public string NextScene;
	public string chosenPerk;
	public string chosenCharacter;
	public string chosenLevel;
	public string numHumans;
	public GameObject controller;

	// Use this for initialization
	void Awake () 
	{
		if(Application.loadedLevel.ToString() == "city1-1")
		{
			/*chosenPerk = controller.GetComponent<TTSMenuController>().chosenPerk.ToString();
			PlayerPrefs.SetString("Perk", chosenPerk);
			chosenCharacter = controller.GetComponent<TTSMenuController>().chosenCharacter.ToString();
			PlayerPrefs.SetString("Character", chosenCharacter);
			chosenLevel = controller.GetComponent<TTSMenuController>().chosenLevel.ToString();
			PlayerPrefs.SetString("Level", chosenLevel);
			numHumans = controller.GetComponent<TTSMenuController>().numHumans.ToString();
			PlayerPrefs.SetString("Players", numHumans);*/
		}
		PlayerPrefs.SetString("Next Scene", NextScene);
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