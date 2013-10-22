using UnityEngine;
using System.Collections;

public class TTSShield : TTSPerishingBehaviour {
	
	private bool dead = false;
	
	void Awake(){
		destroyWhenLifecycleComplete = false; // Once duration has passed, the class will stop running and self-destruct
		useKillFunctionWhenComplete = true; // Execute the kill function once complete
	}
	
	public void DeployShield(int level, float def){
		this.duration = 4.0f * level * def;
	}
	
	protected override void Kill(){
		
		if(dead == false){
			foreach(Transform child in this.transform){
				if(child.GetComponent<ParticleSystem>().playOnAwake == false){
					child.GetComponent<ParticleSystem>().Play();
				}else{
					child.GetComponent<ParticleSystem>().Stop();
				}
			}
			dead = true;
		}
		Invoke("Cleanup", 5);
	}
	
	void Cleanup() {
		Destroy (this.gameObject);
		Destroy (this);
	}
}