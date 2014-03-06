using UnityEngine;
using System.Collections;

public class TTSShockwave : TTSPerishingBehaviour {

	public float radius = 40.0F;
    public float power = 1000.0F;
	public float upwardsForce = 3.0f;
	
	public void Activate(GameObject currentRacer){
		Vector3 explosionPos = transform.position;
	    Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
	    foreach (Collider hit in colliders) {
	        if (hit && hit.rigidbody){
				if(hit.GetComponent<TTSRacer>()){
					if(hit.gameObject != currentRacer){
						if(!hit.GetComponent<TTSRacer>().hasShield){
							hit.rigidbody.AddExplosionForce(power*5, explosionPos, radius, upwardsForce);
						}
					}
				}else{
	            	hit.rigidbody.AddExplosionForce(power, explosionPos, radius, 3.0F);
				}
			}
	    }
		
		Invoke("Cleanup", 2);
	}
	
	void Cleanup() {
		Destroy (this.gameObject);
		Destroy (this);
	}
}
