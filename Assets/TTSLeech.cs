using UnityEngine;
using System.Collections;

public class TTSLeech : TTSBehaviour {
	
	private GameObject[] sortedRacers;
	
	// Use this for initialization
	void Start () {
		sortRacers();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void sortRacers(){
		for(int i=0; i<racers.Length; i++){
			int highestRacer = -1;
			
			for(int j=i; j<racers.Length; j++){
				if(highestRacer < 0){
					highestRacer = j;
				}
				else{
					float distanceFrom = (racers[j].transform.position - transform.position).magnitude;
					float distanceFromTemp = (racers[highestRacer].transform.position - transform.position).magnitude;
					
					if(distanceFrom < distanceFromTemp)
						highestRacer = j;
				}
			}
			
			// Switch objects in array
			GameObject tempRacer = racers[highestRacer];
			racers[highestRacer] = racers[i];
			racers[i] = tempRacer;
			
			highestRacer = -1;
		}
	}
}
