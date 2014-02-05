using UnityEngine;
using System.Collections;

public class TTSHudPlace : TTSBehaviour {
	
	// Use this for initialization
	void Start () {
	}
	
	void Update () {
		foreach(GameObject r in racers){
			r.GetComponent<TTSRacer>().distanceToFinish = r.GetComponent<TTSRacer>().currentWaypoint.distanceFromEnd;
			r.GetComponent<TTSRacer>().distanceToFinish += Vector3.Distance(r.transform.position, r.GetComponent<TTSRacer>().nextWaypoint.transform.position);
		}
		
		
	}
}
