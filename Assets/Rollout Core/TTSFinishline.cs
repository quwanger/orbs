using UnityEngine;
using System.Collections;

public class TTSFinishline : TTSBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	
	}

	void OnTriggerEnter(Collider collision) {
		foreach(GameObject racer in racers) {
			if(collision.gameObject == racer) {
				racer.GetComponent<TTSRacer>().canMove=false;
				level.FinishLevel();
				break;
			}
				
		}
	}
}
