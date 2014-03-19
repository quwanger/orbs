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
	public float stickRadius = 3.0f;
	public float jumpRadius = 8.0f;
	public float explosionRadius = 8.0f;
	public float explosionPower = 1000.0F;
	
	private Vector3 currentTarget;
	public GameObject currentRacer;
	public TTSWaypoint currentWaypoint;
	public TTSWaypoint previousWaypoint;
	public TTSWaypoint nextWaypoint;
	public Vector3 destinationPosition;
	private float leechVelocity = 120.0f;
	
	private Vector3 positionDifference;
	
	private AudioSource leechSFX;
	public AudioClip beeping;
	
	private bool racerFound = false;
	public bool racerStuck = false;
	
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
		
		leechSFX = gameObject.AddComponent<AudioSource>();
		leechSFX.rolloffMode = AudioRolloffMode.Linear;
		leechSFX.minDistance = 25f;


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

			//handle tier 3 shield
			if(stuckRacer.GetComponent<TTSRacer>().hasShield){
				if(stuckRacer.GetComponentInChildren<TTSShield>().tier3){
					stuckRacer.GetComponent<TTSPowerup>().GivePowerup(Powerup.Leech);
					stuckRacer.GetComponentInChildren<TTSShield>().duration = 2.0f;
					Destroy(this.gameObject);
					Destroy(this);
				}
			}

			positionDifference = TTSUtils.RotateAround(positionDifference, Vector3.zero, new Vector3(Random.Range(10.0f, 20.0f), Random.Range(10.0f, 20.0f), Random.Range(10.0f, 20.0f)));
			transform.position = stuckRacer.transform.position + positionDifference;
			CheckForOtherRacers();
		}else{
			if(racerFound){
				this.rigidbody.velocity = (destinationPosition - this.transform.position).normalized * homedRacer.rigidbody.velocity.magnitude * 2.0f;
			}else{
				this.rigidbody.velocity = (destinationPosition - this.transform.position).normalized * leechVelocity;
			}
			findDestination();
		}
	}
	
	private void CheckForOtherRacers(){
		Collider[] colliders = Physics.OverlapSphere(this.transform.position, jumpRadius);
		    foreach (Collider hit in colliders) {
		        if (hit.GetComponent<TTSRacer>() && hit.gameObject != stuckRacer){
					racerStuck = false;
					currentRacer = stuckRacer;
					homedRacer = hit.gameObject;
					destinationPosition = homedRacer.transform.position;
					if(level.DebugMode)
						Debug.Log("Leech jump to: " + hit.gameObject);
				}
		    }
	}
	
	private void LeechExplosion(){
		Vector3 explosionPos = transform.position;
	    Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
		
		Instantiate(explosion,this.transform.position,this.transform.rotation);
		
	    foreach (Collider hit in colliders) {
	        if (hit && hit.rigidbody){
	        	if(hit.GetComponent<TTSRacer>()){
	        		if(!hit.GetComponent<TTSRacer>().hasShield){
	        			hit.GetComponent<TTSRacer>().DamageRacer(0.7f * currentRacer.GetComponent<TTSRacer>().Offense);
	        			hit.rigidbody.AddExplosionForce(explosionPower * currentRacer.GetComponent<TTSRacer>().Offense, explosionPos, explosionRadius * currentRacer.GetComponent<TTSRacer>().Offense, -3.0F);
	        		}
	        	}else{
	            	hit.rigidbody.AddExplosionForce(explosionPower * currentRacer.GetComponent<TTSRacer>().Offense, explosionPos, explosionRadius * currentRacer.GetComponent<TTSRacer>().Offense, -3.0F);
	            }
	        }
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
		        if (hit.GetComponent<TTSRacer>() && hit.gameObject != currentRacer){
					//Debug.Log("Leech - Racer Found");
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
				if (hit.gameObject == homedRacer){
					this.rigidbody.velocity = new Vector3(0,0,0);
					stuckRacer = hit.gameObject;
					positionDifference = this.transform.position - stuckRacer.transform.position;
					racerStuck = true;
					leechSFX.PlayOneShot(beeping);
					Invoke("LeechExplosion", 3);
					this.GetComponentInChildren<MeshRenderer>().material = stuckMaterial;
					break;
				}
			}
			leechVelocity = homedRacer.rigidbody.velocity.magnitude + 10.0f;
			destinationPosition = homedRacer.transform.position;
			
		}
		
		if(nextWaypoint.getDifferenceFromEnd(this.transform.position) < 7.0f){
			if(!racerFound && !racerStuck){
				resetWaypoints(nextWaypoint);
			}
		}
	}
	
	public void OnWaypoint(TTSWaypoint hit) {
		if(!racerFound && !racerStuck){
			resetWaypoints(hit);
		}
	}

	private void resetWaypoints(TTSWaypoint hit){
		previousWaypoint = currentWaypoint;
		currentWaypoint = hit;
		
		if(AIUtil == null)
			AIUtil = gameObject.AddComponent<TTSAIController>();	
				
		//this must be done for the player as well so that we can get the distance of all racers from the finish line
		nextWaypoint = AIUtil.getClosestWaypoint(currentWaypoint.nextWaypoints, this.transform.position);
				
		//this.destinationPosition = nextWaypoint.gameObject.transform.position;
		randomizeTarget();
	}
	
	private void randomizeTarget(){
		this.destinationPosition = nextWaypoint.getPointOn(Random.Range(0f,1.0f)) + nextWaypoint.transform.up * (Random.Range(-0.5f,0.5f) * nextWaypoint.boxHeight);
		//this.destinationPosition.z -= 10.0f;
	}
	
	public void OnDrawGizmos(){
		Gizmos.DrawLine(transform.position, destinationPosition);
	}
	
	#region networking
	TTSPowerupNetHandler netHandler;

	public void SetNetHandler(TTSPowerupNetHandler handler) {
		this.netHandler = handler;
	}
	#endregion
}
