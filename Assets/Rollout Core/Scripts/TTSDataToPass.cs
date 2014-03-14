using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TTSDataToPass : MonoBehaviour {
	
	public List<GameObject> players;
	public string gametype;
	
	void Awake(){
		DontDestroyOnLoad(this);
	}
}
