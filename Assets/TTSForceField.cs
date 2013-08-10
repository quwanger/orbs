using UnityEngine;
using System.Collections;

public class TTSForceField : MonoBehaviour {
	
	public GameObject effect;
	
	void OnCollisionEnter(Collision collision) {
		GameObject go = (GameObject) Instantiate(effect);
		go.transform.position = collision.contacts[0].point - collision.contacts[0].normal;
		go.transform.rotation = Quaternion.FromToRotation(Vector3.forward, collision.contacts[0].normal);
	}
}
