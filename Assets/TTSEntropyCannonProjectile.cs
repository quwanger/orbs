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
#endregion

	
#region unity functions
	void Start () {
		birth = Time.time;
		audio.PlayOneShot(fire);
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - birth > Timeout) {
			Explode(false);
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
