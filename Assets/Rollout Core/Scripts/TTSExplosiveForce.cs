using UnityEngine;
using System.Collections;

public class TTSExplosiveForce : TTSPerishingBehaviour {

	public float radius = 15.0F;
    public float power = 1000.0F;
	
	public void Activate(){
		Vector3 explosionPos = transform.position;
	    Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
	    foreach (Collider hit in colliders) {
	        if (hit && hit.rigidbody)
	            hit.rigidbody.AddExplosionForce(power, explosionPos, radius, 3.0F);
	    }
		
		Invoke("Cleanup", 2);
	}
	
	void Cleanup() {
		Destroy (this.gameObject);
		Destroy (this);
	}
}
