using UnityEngine;
using System.Collections;

public class TTSPowerupPlatform : MonoBehaviour {
	
	public enum Powerup {boost, missiles, shield, shockwave, moretime};
	
	public Powerup currentPowerup;
	public float respawnTime = 5.0f;
	public AudioClip clip;
	private float rotationSpeed = 50.0f;
	private bool pickedUp = false;
	private Collider collidedRacer;
	
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
		currentPowerup = getRandomPowerup();
		displayPowerup();
	}
	
	void Update () {
		//rotate the powerup
		foreach (Transform childTransform in this.transform) {
				childTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
		}
	}
	
	void OnTriggerEnter(Collider collider){	
		if(pickedUp==false){
			if(collider.name == "Racer 2.0"){
				StartCoroutine(handlePickup());
				collidedRacer = collider;
			}
		}
	}
	
	private IEnumerator handlePickup(){
		pickedUp = true;
		//play sound
		 audio.PlayOneShot(clip);
		//remove powerup
		foreach (Transform childTransform in this.transform) {
    		Destroy(childTransform.gameObject);
		}
		//wait 5 second for respawn
		yield return new WaitForSeconds(respawnTime);
		//set new powerup
		currentPowerup = getRandomPowerup();
		displayPowerup();
		pickedUp = false;
	}
	
	private Powerup getRandomPowerup () {
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
	
	private void displayPowerup(){
		//display the current powerup as a child of the platform
		switch (currentPowerup){
    	case (Powerup.boost):
			GameObject w1 = Instantiate(powerupBoost, new Vector3(transform.position.x,transform.position.y+2.0f, transform.position.z), transform.rotation) as GameObject;
			w1.transform.parent = this.transform;
			break;
	    case (Powerup.missiles):
			GameObject w2 = Instantiate(powerupMissiles, new Vector3(transform.position.x,transform.position.y+2.0f, transform.position.z), transform.rotation) as GameObject;
			w2.transform.parent = this.transform;
			break;
	    case (Powerup.shield):
			GameObject w3 = Instantiate(powerupShield, new Vector3(transform.position.x,transform.position.y+2.0f, transform.position.z), transform.rotation) as GameObject;
			w3.transform.parent = this.transform;
			break;
	    case (Powerup.shockwave):
			GameObject w4 = Instantiate(powerupShockwave, new Vector3(transform.position.x,transform.position.y+2.0f, transform.position.z), transform.rotation) as GameObject;
			w4.transform.parent = this.transform;
			break;
	    case (Powerup.moretime):
			GameObject w5 = Instantiate(powerupMoreTime, new Vector3(transform.position.x,transform.position.y+2.0f, transform.position.z), transform.rotation) as GameObject;
			w5.transform.parent = this.transform;
			break;
		}
	}
}
