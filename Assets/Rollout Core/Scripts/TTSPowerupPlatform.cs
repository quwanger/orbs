using UnityEngine;
using System.Collections;

public class TTSPowerupPlatform : TTSBehaviour {
	public PowerupType currentPowerup;
	public float respawnTime = 5.0f;
	public AudioClip clip;
	private float rotationSpeed = 50.0f;
	private bool pickedUp = false;
	private Collider collidedRacer;
	public bool isRandom = false;
	public GameObject powerupBoost;
	public GameObject powerupShield;
	public GameObject powerupShockwave;
	public GameObject powerupMoreTime;
	public GameObject powerupMissiles;
	public GameObject powerupDrezzStone;
	public GameObject powerupHelix;
	public GameObject powerupLeech;
	
	public ParticleSystem EntropyParticleSystem;
	public ParticleSystem SuperCParticleSystem;
	public ParticleSystem TimeBonusParticleSystem;
	public ParticleSystem ShieldParticleSystem;
	public ParticleSystem DrezzStoneParticleSystem;
	public ParticleSystem ShockwaveParticleSystem;
	public ParticleSystem HelixParticleSystem;
	public ParticleSystem LeechParticleSystem;
	
	public int numParticlesToEmitOnPickup = 1000;
	
	public GameObject powerupMesh;
	public ParticleSystem pickupParticleSystem;
	
	
	void Start () {
		audio.volume = 0.75f;

		//set an initial powerup for the platform
		if(isRandom)
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
						case PowerupType.EntropyCannon:
						pickupParticleSystem = Instantiate(EntropyParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case PowerupType.SuperC:
						pickupParticleSystem = Instantiate(SuperCParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case PowerupType.TimeBonus:
						pickupParticleSystem = Instantiate(TimeBonusParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case PowerupType.Shield:
						pickupParticleSystem = Instantiate(ShieldParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case PowerupType.DrezzStones:
						pickupParticleSystem = Instantiate(DrezzStoneParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case PowerupType.Helix:
						pickupParticleSystem = Instantiate(HelixParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case PowerupType.Leech:
						pickupParticleSystem = Instantiate(LeechParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case PowerupType.Shockwave:
						pickupParticleSystem = Instantiate(ShockwaveParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
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
		if(isRandom)
			currentPowerup = getRandomPowerup();
		displayPowerup();
		pickedUp = false;
	}
	
	private PowerupType getRandomPowerup () {
	
		PowerupType powerup = GetRandomEnum<PowerupType>();
		
		if(level.currentGameType != TTSLevel.Gametype.TimeTrial){
			while(powerup == PowerupType.TimeBonus || powerup == PowerupType.None || powerup == PowerupType.Lottery) {
				powerup = GetRandomEnum<PowerupType>();
			}
		}else{
			//make sure that the random powerup isnt none or lottery
			while(powerup == PowerupType.None || powerup == PowerupType.Lottery) {
				powerup = GetRandomEnum<PowerupType>();
			}
		}
		
		return powerup;
	}
	
	private void displayPowerup(){
		//display the current powerup as a child of the platform
		switch (currentPowerup){
    	case (PowerupType.SuperC):
			powerupMesh = Instantiate(powerupBoost, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
	    case (PowerupType.EntropyCannon):
			powerupMesh = Instantiate(powerupMissiles, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
			
		case (PowerupType.TimeBonus):
			powerupMesh = Instantiate(powerupMoreTime, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
		case (PowerupType.DrezzStones):
			powerupMesh = Instantiate(powerupDrezzStone, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
		case (PowerupType.Shield):
			powerupMesh = Instantiate(powerupShield, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
		case (PowerupType.Helix):
			powerupMesh = Instantiate(powerupHelix, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
		case (PowerupType.Leech):
			powerupMesh = Instantiate(powerupLeech, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
		case (PowerupType.Shockwave):
			powerupMesh = Instantiate(powerupShockwave, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
			
		default:
			getRandomPowerup();
			break;
		}
	}
}
