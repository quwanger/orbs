using UnityEngine;
using System.Collections;

public class TTSForceField : TTSBehaviour {
	
	public GameObject effect;
	
	void OnCollisionEnter(Collision collision) {
		GameObject go = (GameObject) Instantiate(effect);
		go.transform.position = collision.contacts[0].point - collision.contacts[0].normal;
		go.transform.rotation = Quaternion.FromToRotation(Vector3.forward, collision.contacts[0].normal);
	}
	
	void OnTriggerEnter(Collider collider) {
			if(collider.gameObject.tag == "Player") {
			
				Vector3 newPosition;
				newPosition.x = collider.gameObject.GetComponent<TTSRacer>().currentWaypoint.transform.position.x;
				newPosition.y = collider.gameObject.GetComponent<TTSRacer>().currentWaypoint.transform.position.y - (collider.gameObject.GetComponent<TTSRacer>().currentWaypoint.GetComponent<BoxCollider>().size.y/2.0f) + (collider.gameObject.GetComponent<SphereCollider>().radius/2.0f);
				newPosition.z = collider.gameObject.GetComponent<TTSRacer>().currentWaypoint.transform.position.z;
			
				collider.gameObject.transform.position = newPosition;
			
				collider.gameObject.transform.rotation = collider.gameObject.GetComponent<TTSRacer>().currentWaypoint.transform.rotation;
			
				collider.gameObject.GetComponent<TTSRacer>().StopRacer();
			}
	}
}
