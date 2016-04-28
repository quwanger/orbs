using UnityEngine;
using System.Collections;

public class TTSExplosiveForce : TTSPerishingBehaviour {

	public float radius = 15.0F;
    public float power = 1000.0F;
	
	public void Activate(bool effectCurrentRacer, GameObject currentRacer){
		Vector3 explosionPos = transform.position;
	    Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
	    foreach (Collider hit in colliders) {
	        if (hit && hit.GetComponent<Rigidbody>()){
				if(hit.GetComponent<TTSRacer>()){
					
					//handles self-explostion if necessary
					if(hit.gameObject == currentRacer){
						if(effectCurrentRacer){
							if(!hit.GetComponent<TTSRacer>().hasShield){
								hit.GetComponent<Rigidbody>().AddExplosionForce(power, explosionPos, radius, 3.0F);
							}
						}
					}else if(!hit.GetComponent<TTSRacer>().hasShield){
						hit.GetComponent<Rigidbody>().AddExplosionForce(power, explosionPos, radius, 3.0F);
					}
				}else{
	            	hit.GetComponent<Rigidbody>().AddExplosionForce(power, explosionPos, radius, 3.0F);
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
