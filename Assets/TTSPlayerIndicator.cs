using UnityEngine;
using System.Collections.Generic;

public class TTSPlayerIndicator : TTSBehaviour {
	
	public GameObject indicator;
	
	public List<GameObject> _indicators = new List<GameObject>();
	public List<GameObject> _racersToFollow = new List<GameObject>();
	
	// Use this for initialization
	void Start () {
		//creates a list of all OTHER racers (not the current racer)
		for(int i=0; i < racers.Length; i++){
			if(racers[i].GetComponent<TTSRacer>().myCamera == this.gameObject){
				Debug.Log(racers[i] + " and " + this.camera + " are a pair.");
			}else{
				_racersToFollow.Add(racers[i]);
				GameObject tempIndicator = (GameObject)Instantiate(indicator);
				tempIndicator.GetComponent<TTSIndicator>().racerToIndicate = racers[i];
				tempIndicator.GetComponent<TTSIndicator>().myCamera = this.camera;
				//tempIndicator.transform.parent = this.gameObject.transform;
				_indicators.Add(tempIndicator);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
