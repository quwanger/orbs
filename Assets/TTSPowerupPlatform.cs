using UnityEngine;
using System.Collections;

public class TTSPowerupPlatform : MonoBehaviour {
	
	public enum Powerup {boost, missiles, shield, shockwave, moretime};
	
	public Powerup currentPowerup;
	
	private bool taken = false;
	private bool isVisible = true;
	private float timeTaken = 0.0f;
	
	//weights for each of the powerups (to chance their chances of appearing)
	private float weightBoost = 100.0f;
	private float weightMissiles = 100.0f;
	private float weightShield = 100.0f;
	private float weightShockwave = 100.0f;
	private float weightMoreTime = 100.0f;
	
	public GameObject powerupBoost;
	public GameObject powerupShield;
	public GameObject powerupShockwave;
	public GameObject powerupMoreTime;
	public GameObject powerupMissiles;
	
	void Start () {
		//set an initial powerup for the platform
		currentPowerup = getPowerup();
		displayPowerup();
	}
	
	void Update () {
		if(taken == true){
			if(isVisible == true){
				timeTaken = Time.time;
				isVisible = false;
				//remove powerup
				foreach (Transform childTransform in this.transform) {
    				Destroy(childTransform.gameObject);
				}
			}
			if(Time.time > timeTaken + 5.0f){
				//set new powerup
				currentPowerup = getPowerup();
				displayPowerup();
				taken = false;
				isVisible = true;
			}
		}else{	
			//animate (spin)
			foreach (Transform childTransform in this.transform) {
				childTransform.Rotate(Vector3.up, 50f * Time.deltaTime);
			}
		}
	}
	
	void OnCollisionEnter(Collision collision){
		taken = true;
	}
	
	Powerup getPowerup () {
		float temp = Random.Range(0.0f, 1.0f);
		float total = weightBoost + weightMissiles + weightMoreTime + weightShield + weightShockwave;
		
		Powerup tempPowerup;
		
		//checks to see which powerup to return based on their probability weight
		if(temp < weightBoost/total) tempPowerup = Powerup.boost;
		else if(temp < (weightBoost + weightMissiles)/total) tempPowerup = Powerup.missiles;
		else if(temp < (weightBoost + weightMissiles + weightMoreTime)/total) tempPowerup = Powerup.moretime;
		else if(temp < (weightBoost + weightMissiles + weightMoreTime + weightShield)/total) tempPowerup = Powerup.shield;
		else tempPowerup = Powerup.shockwave;
			
		return tempPowerup;
	}
	
	void displayPowerup(){
		//display the current powerup as a child of the platform
		switch (currentPowerup){
    	case (Powerup.boost):
			GameObject w1 = Instantiate(powerupBoost, transform.position, transform.rotation) as GameObject;
			w1.transform.parent = this.transform;
			break;
	    case (Powerup.missiles):
			GameObject w2 = Instantiate(powerupMissiles, transform.position, transform.rotation) as GameObject;
			w2.transform.parent = this.transform;
			break;
	    case (Powerup.shield):
			GameObject w3 = Instantiate(powerupShield, transform.position, transform.rotation) as GameObject;
			w3.transform.parent = this.transform;
			break;
	    case (Powerup.shockwave):
			GameObject w4 = Instantiate(powerupShockwave, transform.position, transform.rotation) as GameObject;
			w4.transform.parent = this.transform;
			break;
	    case (Powerup.moretime):
			GameObject w5 = Instantiate(powerupMoreTime, transform.position, transform.rotation) as GameObject;
			w5.transform.parent = this.transform;
			break;
		}
	}
}
