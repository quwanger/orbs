using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TTSDataToPass : MonoBehaviour {
	
	public List<GameObject> players;
	public TTSLevel.Gametype gametype;
	
	void Awake(){
		DontDestroyOnLoad(this);
	}
}
