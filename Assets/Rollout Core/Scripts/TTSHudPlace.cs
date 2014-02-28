using UnityEngine;
using System.Collections;

public class TTSHudPlace : TTSBehaviour {
	
	// Use this for initialization
	void Start () {
	}
	
	void Update () {
		
		if(this.GetComponent<TTSLevel>().raceHasStarted){

			TTSRacer racer;
			foreach(GameObject r in racers){
				racer = r.GetComponent<TTSRacer>();
				if (racer.nextWaypoint != null) {
					racer.distanceToFinish = racer.currentWaypoint.distanceFromEnd;
					racer.distanceToFinish += Vector3.Distance(r.transform.position, racer.nextWaypoint.transform.position);
				}
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
