using UnityEngine;
using System.Collections;



public class TTSLevel : MonoBehaviour {

	public GameObject[] racers;
	// Use this for initialization
	void Start () {
		GameObject.Find("Countdown").GetComponent<Animation>().Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void StartRace() {
		GameObject.Find("Soundtrack").GetComponent<TTSSoundtrackManager>().StartSoundtrack();
		
		foreach(GameObject racer in racers) {
			racer.GetComponent<TTSRacer>().canMove = true;
		}
	}
}
