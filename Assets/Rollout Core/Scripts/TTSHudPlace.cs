using UnityEngine;
using System.Collections;

public class TTSHudPlace : TTSBehaviour {
	
	// Use this for initialization
	void Start () {
	}
	
	void Update () {
		
		if(this.GetComponent<TTSLevel>().raceHasStarted){
		
			foreach(GameObject r in racers){
				r.GetComponent<TTSRacer>().distanceToFinish = r.GetComponent<TTSRacer>().currentWaypoint.distanceFromEnd;
				r.GetComponent<TTSRacer>().distanceToFinish += Vector3.Distance(r.transform.position, r.GetComponent<TTSRacer>().nextWaypoint.transform.position);
			}
			
			float farthestAway = 0;
			
			int currentPlace = 1;
			
			for(int i=0; i < racers.Length; i++){
				for(int j=0; j < racers.Length; j++){
					if(racers[j].GetComponent<TTSRacer>().distanceToFinish < racers[i].GetComponent<TTSRacer>().distanceToFinish){
						currentPlace++;
					}
				}
				racers[i].GetComponent<TTSRacer>().place = currentPlace;
				currentPlace = 1;
			}
			
		}
		
	}
}
