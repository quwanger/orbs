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
	public float ProjectileAccleration = 10.0f;
	public float ProjectileStartVelocity = 100.0f;
	
	private float initialDistanceToGround;
#endregion

	
#region unity functions
	void Start () {
		birth = Time.time;
		audio.PlayOneShot(fire);
		
		RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0F)){
            initialDistanceToGround = hit.distance;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - birth > Timeout) {
			Explode(false);
		}
		
		float distanceToGround = checkDistanceToGround();
		
		while(distanceToGround > initialDistanceToGround){
			float tempY = this.transform.position.y;
			tempY -= 0.05f;
			this.transform.position = new Vector3(this.transform.position.x, tempY, this.transform.position.z);
			distanceToGround = checkDistanceToGround();
		}
		
		while(distanceToGround < initialDistanceToGround){
			float tempY = this.transform.position.y;
			tempY += 0.05f;
			this.transform.position = new Vector3(this.transform.position.x, tempY, this.transform.position.z);
			distanceToGround = checkDistanceToGround();
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
		Explode(true);
	}
#endregion

	private void Explode(bool actually) {
		if(actually) {
			 Instantiate(explosion,this.transform.position,this.transform.rotation);
		}
			
			
		foreach(Transform child in transform) {
				Destroy(child.gameObject);
		}
		//stop motion so the trail can end and destroy the parent GO.
		this.GetComponent<SphereCollider>().enabled = false;
		this.rigidbody.velocity = new Vector3(0f,0f,0f);
	}

}
