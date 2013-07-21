using UnityEngine;
using System.Collections;

public class TTSPowerupPlatform : TTSBehaviour {
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
	
	public ParticleSystem EntropyParticleSystem;
	public ParticleSystem DrezzStoneParticleSystem;
	
	public int numParticlesToEmitOnPickup = 1000;
	
	public GameObject powerupMesh;
	
	
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
		if(!pickedUp){
			foreach(GameObject racer in racers) {
				if(collider.gameObject == racer) {
					collider.gameObject.GetComponent<TTSPowerup>().GivePowerup(this.currentPowerup);
					switch(currentPowerup) {
						case Powerup.EntropyCannon:
						EntropyParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case Powerup.DrezzStones:
						DrezzStoneParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						default:
						break;
					}
					StartCoroutine("respawnPickup");
				}
			}
		}
	}
	
	private IEnumerator respawnPickup(){
		pickedUp = true;
		//play sound
		 audio.PlayOneShot(clip);
		//remove powerup
		Destroy (powerupMesh);
		powerupMesh = null;
		//wait 5 second for respawn
		yield return new WaitForSeconds(respawnTime);
		//set new powerup
		currentPowerup = getRandomPowerup();
		displayPowerup();
		pickedUp = false;
	}
	
	private Powerup getRandomPowerup () {
	
		Powerup powerup = GetRandomEnum<Powerup>();
		
		while(powerup == Powerup.None) {
			powerup = GetRandomEnum<Powerup>();
		}
		return powerup;
	}
	
	private void displayPowerup(){
		//display the current powerup as a child of the platform
		switch (currentPowerup){
    	case (Powerup.DrezzStones):
			powerupMesh = Instantiate(powerupBoost, new Vector3(transform.position.x,transform.position.y+2.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
	    case (Powerup.EntropyCannon):
			powerupMesh = Instantiate(powerupMissiles, new Vector3(transform.position.x,transform.position.y+2.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
			
		default:
			getRandomPowerup();
			break;
		}
	}
}
