using UnityEngine;
using System.Collections.Generic;
public class TTSHelixProjectileTier3 : TTSBehaviour
{
	#region internal fields
	private float birth;
	#endregion

	#region configuration fields
	public GameObject explosion;
	public AudioClip fire;
	public float Timeout = 15.0f;
	public float ProjectileAcceleration = 1.5f;
	public float ProjectileStartVelocity = 100.0f;

	public float offensiveMultiplier;

	private float initialDistanceToGround;

	public float homingRadius = 25.0F;

	private Vector3 currentTarget;
	public TTSRacer currentRacer;
	public TTSWaypoint currentWaypoint;
	public TTSWaypoint previousWaypoint;
	public TTSWaypoint nextWaypoint;
	public Vector3 destinationPosition;

	public int racersInFront;
	public int helixInBatch;

	private bool racerFound = false;
	private GameObject homedRacer;

	public GameObject ProjectileToSpawn;

	public List<GameObject> hitRacers = new List<GameObject>();

	TTSAIController AIUtil;
	#endregion


	#region unity functions
	void Start() {

		ProjectileStartVelocity = Random.Range(120.0f, 160.0f);

		birth = Time.time;
		audio.PlayOneShot(fire);

		RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0F)){
            initialDistanceToGround = hit.distance;
		}

		currentWaypoint = currentRacer.currentWaypoint;
		nextWaypoint = currentRacer.nextWaypoint;
		destinationPosition = nextWaypoint.gameObject.transform.position;
	}

	// Update is called once per frame
	void Update () {
		if (netHandler == null || netHandler.owner || !netHandler.isNetworkUpdated) {
			/*if(Time.time - birth > Timeout) {
				Explode(false);
			}*/
		
			//if the helix reaches the end, kill it
			if (nextWaypoint == null) {
				Explode(false);
				return;
			}

			//check for the projectile's next destination
			FindDestination();

			//move the projectile
			this.rigidbody.velocity = (destinationPosition - this.transform.position).normalized * ProjectileStartVelocity;
			
			if(racerFound){
				ProjectileStartVelocity = homedRacer.rigidbody.velocity.magnitude * 1.5f;
			}else{
				ProjectileStartVelocity = ProjectileStartVelocity + ProjectileAcceleration;
			}
			
			if (netHandler != null && netHandler.owner)
				netHandler.UpdatePowerup(transform.position, transform.rotation.eulerAngles, rigidbody.velocity);

			foreach (TTSPowerupNetHandler handler in netHandler.receivedPowerups) {
				if (handler.Type == TTSPowerupNetworkTypes.Helix) {
					SpawnNetHelix(handler);
				}
			}
			netHandler.receivedPowerups.Clear();
		}
		else if (!netHandler.owner) {
			GetNetworkUpdate();
		}
	}

	private void FindDestination() {
		if (!racerFound) {
			Collider[] colliders = Physics.OverlapSphere(this.transform.position, homingRadius);
		    foreach (Collider hit in colliders) {
		        if (hit.GetComponent<TTSRacer>() && hit.gameObject != currentRacer.gameObject){
		        	if(hit.GetComponent<TTSRacer>().place == 1){
		        		//only target the player in first place
						racerFound = true;
						homedRacer = hit.gameObject;
				        destinationPosition = hit.transform.position;
						break;
					}else{
						//do shock
						if(!hitRacers.Contains(hit.gameObject)){
							GameObject go = (GameObject)Instantiate(ProjectileToSpawn, this.transform.position, this.transform.rotation);
							go.GetComponent<TTSHelixProjectile>().offensiveMultiplier = offensiveMultiplier;
							go.GetComponent<TTSHelixProjectile>().currentRacer = currentRacer;
							go.rigidbody.velocity = (hit.transform.position - this.transform.position).normalized * ProjectileStartVelocity;
							
							SendHelixDeploy(go);
							
							hitRacers.Add(hit.gameObject);
						}
					}
				}
		    }

		    if(nextWaypoint.getDifferenceFromEnd(this.transform.position) < 7.0f){
				resetWaypoints(nextWaypoint);
			}
		}else{
			destinationPosition = homedRacer.transform.position;
		}
		
	}

	private void SpawnNetHelix(TTSPowerupNetHandler handler) {
		GameObject go = (GameObject)Instantiate(ProjectileToSpawn, this.transform.position, this.transform.rotation);
		go.GetComponent<TTSHelixProjectile>().offensiveMultiplier = offensiveMultiplier;
		go.GetComponent<TTSHelixProjectile>().currentRacer = currentRacer;

		go.GetComponent<TTSHelixProjectile>().SetNetHandler(handler);
	}

	private void SendHelixDeploy(GameObject helix) {
		if (!level.client.isMultiplayer) return;

		TTSPowerupNetHandler handler = new TTSPowerupNetHandler(level.client, true, TTSPowerupNetworkTypes.Helix, netHandler.id);
		helix.GetComponent<TTSHelixProjectile>().SetNetHandler(handler);
	}

	void OnTriggerEnter(Collider other) {
		//only collide with racer
		if(other.gameObject.GetComponent<TTSRacer>()){
			if(other.gameObject.GetComponent<TTSRacer>().hasShield){
				if(other.gameObject.GetComponentInChildren<TTSShield>().tier3){
					other.gameObject.GetComponent<TTSPowerup>().GivePowerup(PowerupType.Helix);
					other.gameObject.GetComponentInChildren<TTSShield>().duration = 2.0f;
					other.gameObject.GetComponentInChildren<TTSShield>().absorbEffect.Play();
					Explode(false);
				}
			}else{
				other.gameObject.GetComponent<TTSRacer>().DamageRacer(offensiveMultiplier * 1.0f);
				other.rigidbody.AddExplosionForce(25, this.transform.position, 10, -10);
				Explode(true);
			}
		}
	}
	#endregion

	#region networking

	TTSPowerupNetHandler netHandler;

	public void SetNetHandler(TTSPowerupNetHandler handler) {
		this.netHandler = handler;
	}

	private void GetNetworkUpdate() {
		if (netHandler.isNetworkUpdated) {
			if (netHandler.netPosition != Vector3.zero) {
				transform.position = Vector3.Lerp(transform.position, netHandler.netPosition, netHandler.networkInterpolation);
			}
			transform.rotation = Quaternion.Euler(netHandler.netRotation);
			rigidbody.velocity = netHandler.netSpeed;

			netHandler.isNetworkUpdated = false;
			netHandler.framesSinceNetData = 0;
		}
		else {
			netHandler.framesSinceNetData++;
			if (netHandler.framesSinceNetData >= TTSPowerupNetHandler.ExplodeTimeout) {
				Explode(true);
			}
		}
	}
	#endregion

	public void OnWaypoint(TTSWaypoint hit) {
		if (!racerFound) {
			resetWaypoints(hit);
		}
	}

	private void resetWaypoints(TTSWaypoint hit) {
		previousWaypoint = currentWaypoint;
		currentWaypoint = hit;

		if (AIUtil == null)
			AIUtil = gameObject.AddComponent<TTSAIController>();

		//this must be done for the player as well so that we can get the distance of all racers from the finish line
		nextWaypoint = AIUtil.getClosestWaypoint(currentWaypoint.nextWaypoints, this.transform.position);

		if (nextWaypoint == null)
			return;

		destinationPosition = nextWaypoint.transform.position;
	}

	private void Explode(bool actually) {
		if (actually) {
			Instantiate(explosion, this.transform.position, this.transform.rotation);
		}

		foreach (Transform child in transform) {
			Destroy(child.gameObject);
		}
		//stop motion so the trail can end and destroy the parent GO.
		this.GetComponent<SphereCollider>().enabled = false;
		this.rigidbody.velocity = new Vector3(0f, 0f, 0f);

		Destroy(this.gameObject);
		Destroy(this);
	}

	void OnDestroy() {
		if (netHandler != null) {
			netHandler.DeregisterFromClient();
			netHandler = null;
		}
	}

}