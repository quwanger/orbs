using UnityEngine;
using System.Collections;

public class TTSEntropyCannonProjectile : MonoBehaviour {
	
#region internal fields
	private float birth;
#endregion

#region configuration fields
	public GameObject explosion;
	public AudioClip fire;
	public float Timeout = 5.0f;
	public float ProjectileAcceleration = 10.0f;
	public float ProjectileStartVelocity = 100.0f;
	
	public float offensiveMultiplier;
	
	private float initialDistanceToGround;
#endregion

	
#region unity functions
	void Start () {
		birth = Time.time;
		audio.PlayOneShot(fire);
		
		RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0F)){
            initialDistanceToGround = checkDistanceToGround();
			//this is how you get the item is collides with
			//Debug.Log (hit.collider);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - birth > Timeout) {
			Explode(false);
		}

		if (netHandler == null || netHandler.owner) {

			if(checkDistanceToGround()!=initialDistanceToGround){
				float newY = (this.gameObject.transform.position.y - checkDistanceToGround()) + initialDistanceToGround;
				this.transform.position = new Vector3(this.transform.position.x, newY, this.transform.position.z);
			}

			if(netHandler != null)
				netHandler.UpdatePowerup(transform.position, transform.rotation.eulerAngles, rigidbody.velocity);
		}
		else if(!netHandler.owner) {
			GetNetworkUpdate();
		}
		
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

	private float checkDistanceToGround(){
		RaycastHit hit;
		float distanceToGround;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0F)){
            distanceToGround = hit.distance;
			return distanceToGround;
		}else{
			return 0.0f;
		}
	}
	
	void OnCollisionEnter(Collision other) {
		//damage racer if racer is hit
		if(other.gameObject.GetComponent<TTSRacer>()){
			if(other.gameObject.GetComponent<TTSRacer>().hasShield){
				if(other.gameObject.GetComponentInChildren<TTSShield>().tier3){
					other.gameObject.GetComponent<TTSPowerup>().GivePowerup(TTSBehaviour.Powerup.EntropyCannon);
					other.gameObject.GetComponentInChildren<TTSShield>().duration = 2.0f;
					other.gameObject.GetComponentInChildren<TTSShield>().absorbEffect.Play();
					Explode(false);
				}
			}else{
				other.gameObject.GetComponent<TTSRacer>().DamageRacer(offensiveMultiplier * 0.7f);
				Explode(true);
			}
		}else{
			Explode(true);
		}
	}
#endregion

	#region networking
	TTSPowerupNetHandler netHandler;

	public void SetNetHandler(TTSPowerupNetHandler handler) {
		this.netHandler = handler;
	}
	#endregion

	private void Explode(bool actually) {
		if (netHandler != null) {
			netHandler.DeregisterFromClient();
			netHandler = null;
		}

		if (actually) {
			Instantiate(explosion, this.transform.position, this.transform.rotation);
		}
			
		foreach(Transform child in transform) {
				Destroy(child.gameObject);
		}
		//stop motion so the trail can end and destroy the parent GO.
		this.GetComponent<SphereCollider>().enabled = false;
		this.rigidbody.velocity = new Vector3(0f,0f,0f);
	}

}
