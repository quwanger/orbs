using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TTSDataToPass : MonoBehaviour {
	
	public List<TTSRacerConfig> players;
	public TTSLevel.Gametype gametype;
	
	void Awake(){
		DontDestroyOnLoad(this);
	}
}
