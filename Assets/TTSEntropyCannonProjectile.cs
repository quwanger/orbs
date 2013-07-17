using UnityEngine;
using System.Collections;

public class TTSEntropyCannonProjectile : MonoBehaviour {
	
	
	private float birth;
	public GameObject explosion;
	public AudioClip fire;
	private int numberOfBounces = 0;
	
	// Use this for initialization
	void Start () {
		birth = Time.time;
		audio.PlayOneShot(fire);
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Time.time - birth > 5) {
			foreach(Transform child in transform) {
				Destroy(child.gameObject);
			}
			this.rigidbody.velocity = new Vector3(0f,0f,0f);
			this.GetComponent<SphereCollider>().enabled = false;
		}
	}
	
	void FixedUpdate() {
		this.rigidbody.AddForce(this.transform.forward.normalized*10f);
	}
	
	void OnCollisionEnter(Collision other) {

		GameObject go = (GameObject) Instantiate(explosion);
		go.transform.position = this.transform.position;
		foreach(Transform child in transform) {
				Destroy(child.gameObject);
		}
		this.GetComponent<SphereCollider>().enabled = false;
		this.rigidbody.velocity = new Vector3(0f,0f,0f);
		
	}
}
