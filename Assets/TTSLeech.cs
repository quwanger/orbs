using UnityEngine;
using System.Collections;

public class TTSLeech : TTSBehaviour {
	
	private GameObject[] sortedRacers;
	private bool isInitiated = false;
	private Vector3 newPosition;
	private float randX;
	private float randY;
	
	private Vector3 currentTarget;
	
	public TTSRacer currentRacer;
	
	// Use this for initialization
	void Start () {
		sortRacers();
		
		randX = Random.Range(-10.0f, 10.0f);
		randY = Random.Range(1.0f, 10.0f);
		
		newPosition = new Vector3(transform.position.x + randX, transform.position.y + randY, transform.position.z);
		
		
		Invoke("ActivateHoming", 0.75f);
	}
	
	// Update is called once per frame
	void Update () {
		if(!isInitiated)
			initialMovement();
		else
			doHoming();
	}
	
	private void initialMovement(){
		//newPosition = new Vector3(transform.parent.transform.position.x + randX, transform.parent.transform.position.y + randY, transform.parent.transform.position.z);
		transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);
	}
	
	private void doHoming(){
		this.rigidbody.velocity *= 20.0f;
	}
	
	private void ActivateHoming() {
		isInitiated = true;
		Debug.Log("Leech Initiated!");
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
