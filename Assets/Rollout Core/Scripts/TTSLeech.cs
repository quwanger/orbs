using UnityEngine;
using System.Collections;

public class TTSLeech : TTSBehaviour {
	
	private GameObject[] sortedRacers;
	private bool isInitiated = false;
	private Vector3 newPosition;
	private float randX;
	private float randY;
	
	public GameObject explosion;
	
	public float homingRadius = 25.0F;
	public float stickRadius = 1.5f;
	public float explosionRadius = 5.0f;
	public float explosionPower = 1000.0F;
	
	private Vector3 currentTarget;
	public TTSRacer currentRacer;
	public TTSWaypoint currentWaypoint;
	public TTSWaypoint previousWaypoint;
	public TTSWaypoint nextWaypoint;
	public Vector3 destinationPosition;
	private float leechVelocity = 80.0f;
	
	private Vector3 positionDifference;
	
	private bool racerFound = false;
	private bool racerStuck = false;
	
	private GameObject homedRacer;
	private GameObject stuckRacer;
	
	public Material stuckMaterial;
	
	TTSAIController AIUtil;
	
	// Use this for initialization
	void Start () {
		//sortRacers();
		
		randX = Random.Range(-10.0f, 10.0f);
		randY = Random.Range(1.0f, 10.0f);
		
		newPosition = new Vector3(transform.position.x + randX, transform.position.y + randY, transform.position.z);
		
		currentWaypoint = currentRacer.GetComponent<TTSRacer>().currentWaypoint;
		nextWaypoint = currentRacer.GetComponent<TTSRacer>().nextWaypoint;
		destinationPosition = nextWaypoint.gameObject.transform.position;
		
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
		this.rigidbody.velocity = currentRacer.rigidbody.velocity;
		transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);
	}
	
	private void doHoming(){
		
		if(racerStuck){
			positionDifference = TTSUtils.RotateAround(positionDifference, Vector3.zero, new Vector3(Random.Range(10.0f, 20.0f), Random.Range(10.0f, 20.0f), Random.Range(10.0f, 20.0f)));
			transform.position = stuckRacer.transform.position + positionDifference;
		}else{
			this.rigidbody.velocity = (destinationPosition - this.transform.position).normalized * leechVelocity;
			findDestination();
		}
	}
	
	private void LeechExplosion(){
		Vector3 explosionPos = transform.position;
	    Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
		
		Instantiate(explosion,this.transform.position,this.transform.rotation);
		
	    foreach (Collider hit in colliders) {
	        if (hit && hit.rigidbody)
	            hit.rigidbody.AddExplosionForce(explosionPower, explosionPos, explosionRadius, -3.0F);
			if(hit.GetComponent<TTSRacer>())
				//slow down racer if it is a racer
				hit.GetComponent<TTSRacer>().DamageRacer(100.0f);
	    }
		
		Destroy (this.gameObject);
		Destroy (this);
	}
	
	private void ActivateHoming() {
		isInitiated = true;
		randomizeTarget();
		Invoke("LeechExplosion", 12);
	}
	
	private void findDestination(){
		if(!racerFound){
			Collider[] colliders = Physics.OverlapSphere(this.transform.position, homingRadius);
		    foreach (Collider hit in colliders) {
		        if (hit.GetComponent<TTSRacer>() && hit.gameObject != currentRacer.gameObject){
					Debug.Log("Leech - Racer Found");
					racerFound = true;
					homedRacer = hit.gameObject;
		            destinationPosition = hit.transform.position;
					break;
				}
		    }
		}
		
		if(racerFound && !racerStuck){
			//check to see if leech should attach itself to the racer
			Collider[] colliders2 = Physics.OverlapSphere(this.transform.position, stickRadius);
			foreach (Collider hit in colliders2) {
				if (hit.GetComponent<TTSRacer>()){
					this.rigidbody.velocity = new Vector3(0,0,0);
					stuckRacer = hit.gameObject;
					positionDifference = this.transform.position - stuckRacer.transform.position;
					racerStuck = true;
					Invoke("LeechExplosion", 3);
					this.GetComponentInChildren<MeshRenderer>().material = stuckMaterial;
					break;
				}
			}
			leechVelocity = homedRacer.rigidbody.velocity.magnitude + 10.0f;
			destinationPosition = homedRacer.transform.position;
			
		}
		
	}
	
	public void OnWaypoint(TTSWaypoint hit) {
		if(!racerFound && !racerStuck && previousWaypoint != currentWaypoint){
			previousWaypoint = currentWaypoint;
			currentWaypoint = hit;
		
			if(AIUtil == null)
				AIUtil = gameObject.AddComponent<TTSAIController>();	
				
			//this must be done for the player as well so that we can get the distance of all racers from the finish line
			nextWaypoint = AIUtil.getClosestWaypoint(currentWaypoint.nextWaypoints, this.transform.position);
				
			//this.destinationPosition = nextWaypoint.gameObject.transform.position;
			randomizeTarget();
		}
	}
	
	private void randomizeTarget(){
		this.destinationPosition = nextWaypoint.getPointOn(Random.Range(0f,1.0f)) + nextWaypoint.transform.up * (Random.Range(-0.5f,0.5f) * nextWaypoint.boxHeight);
		//this.destinationPosition.z -= 10.0f;
	}
	
	public void OnDrawGizmos(){
		Gizmos.DrawLine(transform.position, destinationPosition);
	}
	
	/*private void sortRacers(){
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
	}*/
}
