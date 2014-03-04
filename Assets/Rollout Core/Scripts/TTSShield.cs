using UnityEngine;
using System.Collections;

public class TTSShield : TTSPerishingBehaviour {
	
	private bool dead = false;
	private TTSRacer racer;
	
	void Awake(){
		destroyWhenLifecycleComplete = false; // Once duration has passed, the class will stop running and self-destruct
		useKillFunctionWhenComplete = true; // Execute the kill function once complete
	}
	
	public void DeployShield(float level, float def, TTSRacer tempRacer){
		this.duration = 4.0f * level * def;
		racer = tempRacer;
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
		racer.hasShield = false;
		racer.GetComponent<TTSPowerup>().ActivePowerup = TTSBehaviour.Powerup.None;
		Invoke("Cleanup", 5);
	}
	
	void Cleanup() {
		Destroy (this.gameObject);
		Destroy (this);
	}
}