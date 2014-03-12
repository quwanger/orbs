using UnityEngine;
using System.Collections;
public class TTSHelixProjectile : TTSBehaviour
{
	#region internal fields
	private float birth;
	#endregion

	#region configuration fields
	public GameObject explosion;
	public AudioClip fire;
	public float Timeout = 15.0f;
	public float ProjectileAcceleration = 10.0f;
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

	TTSAIController AIUtil;
	#endregion


	#region unity functions
	void Start() {

		ProjectileStartVelocity = Random.Range(80.0f, 140.0f);

		birth = Time.time;
		audio.PlayOneShot(fire);

		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0F)) {
			initialDistanceToGround = hit.distance;
			//this is how you get the item is collides with
			//Debug.Log (hit.collider);
		}

		currentWaypoint = currentRacer.GetComponent<TTSRacer>().currentWaypoint;
		nextWaypoint = currentRacer.GetComponent<TTSRacer>().nextWaypoint;
		destinationPosition = nextWaypoint.gameObject.transform.position;
	}

	// Update is called once per frame
	void Update() {
		if (Time.time - birth > Timeout) {
			Explode(false);
		}

		if (netHandler == null || netHandler.owner) {
			float distanceToGround = checkDistanceToGround();

			if (distanceToGround < initialDistanceToGround) {
				//while(distanceToGround < initialDistanceToGround){
				float tempY = this.transform.position.y;
				tempY += (initialDistanceToGround - distanceToGround);
				this.transform.position = new Vector3(this.transform.position.x, tempY, this.transform.position.z);
				//distanceToGround = checkDistanceToGround();
				//}
			}
			else if (distanceToGround > initialDistanceToGround) {
				//while(distanceToGround > initialDistanceToGround){
				float tempY = this.transform.position.y;
				tempY -= (distanceToGround - initialDistanceToGround);
				this.transform.position = new Vector3(this.transform.position.x, tempY, this.transform.position.z);
				//distanceToGround = checkDistanceToGround();
				//}
			}
			//move the projectile
			this.rigidbody.velocity = (destinationPosition - this.transform.position).normalized * ProjectileStartVelocity;
			//check for the projectile's next destination
			FindDestination();

			if (netHandler != null && netHandler.owner)
				netHandler.UpdatePowerup(transform.position, transform.rotation.eulerAngles, rigidbody.velocity);
		}
		else if (!netHandler.owner) {
			GetNetworkUpdate();
		}
	}

	private void FindDestination() {
		if (!racerFound) {
			Collider[] colliders = Physics.OverlapSphere(this.transform.position, homingRadius);
			foreach (Collider hit in colliders) {
				if (hit.GetComponent<TTSRacer>() && hit.gameObject != currentRacer.gameObject) {
					if (hit.GetComponent<TTSRacer>().numHelix < Mathf.Ceil(helixInBatch / racersInFront)) {
						Debug.Log("Helix - Racer Found");
						racerFound = true;
						homedRacer = hit.gameObject;
						homedRacer.GetComponent<TTSRacer>().numHelix++;
						destinationPosition = hit.transform.position;
						break;
					}
				}
			}
		}
		else {
			destinationPosition = homedRacer.transform.position;
		}

		if (nextWaypoint.getDifferenceFromEnd(this.transform.position) < 7.0f) {
			resetWaypoints(nextWaypoint);
		}
	}

	private float checkDistanceToGround() {
		RaycastHit hit;
		float distanceToGround;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0F)) {
			distanceToGround = hit.distance;
			return distanceToGround;
		}
		else {
			return 0.0f;
		}
	}

	void OnCollisionEnter(Collision other) {
		//damage racer if racer is hit
		if (other.gameObject.GetComponent<TTSWaypoint>() || other.gameObject.GetComponent<TTSHelixProjectile>()) {
			//do nothing if it's a waypoint
		}
		else {
			if (other.gameObject.GetComponent<TTSRacer>()) {
				other.gameObject.GetComponent<TTSRacer>().DamageRacer(offensiveMultiplier * 0.7f);
			}
			Explode(true);
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

		destinationPosition = nextWaypoint.transform.position;

		//randomizeTarget();
	}

	private void Explode(bool actually) {
		if (netHandler != null) {
			netHandler.DeregisterFromClient();
			netHandler = null;
		}

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

	private void randomizeTarget() {
		this.destinationPosition = nextWaypoint.getPointOn(Random.Range(0f, 1.0f)) + nextWaypoint.transform.up * (Random.Range(-0.5f, 0.5f) * nextWaypoint.boxHeight);
		//this.destinationPosition.z -= 10.0f;
	}

}
