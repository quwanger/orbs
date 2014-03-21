using UnityEngine;
using System.Collections;

public class TTSPowerupPlatform : TTSBehaviour {
	public Powerup currentPowerup;
	public float respawnTime = 5.0f;
	public AudioClip clip;
	private float rotationSpeed = 50.0f;
	private bool pickedUp = true;
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

	#region networking
	private TTSPowerupPlatformNetworkHandler netHandle;
	#endregion
	
	
	void Start () {
		audio.volume = 0.75f;

		//set an initial powerup for the platform
		if(isRandom)
			currentPowerup = getRandomPowerup();
		displayPowerup();

		if (level.client.isMultiplayer) {
			netHandle = new TTSPowerupPlatformNetworkHandler(level.client, transform.position);
		}
		else {
			pickedUp = false;
		}
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
						pickupParticleSystem = Instantiate(EntropyParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case Powerup.SuperC:
						pickupParticleSystem = Instantiate(SuperCParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case Powerup.TimeBonus:
						pickupParticleSystem = Instantiate(TimeBonusParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case Powerup.Shield:
						pickupParticleSystem = Instantiate(ShieldParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case Powerup.DrezzStones:
						pickupParticleSystem = Instantiate(DrezzStoneParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case Powerup.Helix:
						pickupParticleSystem = Instantiate(HelixParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case Powerup.Leech:
						pickupParticleSystem = Instantiate(LeechParticleSystem, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;
						
						case Powerup.Shockwave:
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
	
	private Powerup getRandomPowerup () {
	
		Powerup powerup = GetRandomEnum<Powerup>();
		
		if(level.currentGameType != TTSLevel.Gametype.TimeTrial){
			while(powerup == Powerup.TimeBonus || powerup == Powerup.None || powerup == Powerup.Lottery) {
				powerup = GetRandomEnum<Powerup>();
			}
		}else{
			//make sure that the random powerup isnt none or lottery
			while(powerup == Powerup.None || powerup == Powerup.Lottery) {
				powerup = GetRandomEnum<Powerup>();
			}
		}
		
		return powerup;
	}
	
	private void displayPowerup(){
		//display the current powerup as a child of the platform
		switch (currentPowerup){
    	case (Powerup.SuperC):
			powerupMesh = Instantiate(powerupBoost, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
	    case (Powerup.EntropyCannon):
			powerupMesh = Instantiate(powerupMissiles, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
			
		case (Powerup.TimeBonus):
			powerupMesh = Instantiate(powerupMoreTime, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
		case (Powerup.DrezzStones):
			powerupMesh = Instantiate(powerupDrezzStone, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
		case (Powerup.Shield):
			powerupMesh = Instantiate(powerupShield, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
		case (Powerup.Helix):
			powerupMesh = Instantiate(powerupHelix, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
		case (Powerup.Leech):
			powerupMesh = Instantiate(powerupLeech, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
		case (Powerup.Shockwave):
			powerupMesh = Instantiate(powerupShockwave, new Vector3(transform.position.x,transform.position.y+3.0f, transform.position.z), transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
			break;
			
		default:
			getRandomPowerup();
			break;
		}
	}
}

public class TTSPowerupPlatformNetworkHandler : TTSNetworkHandle
{
	public TTSBehaviour.Powerup PowerupType = TTSBehaviour.Powerup.None;
	//public bool pickedUp = false;

	public TTSPowerupPlatformNetworkHandler(TTSClient Client, Vector3 StartingPosition) {
		type = "Powerup Platform";

		canForfeitControl = true;
		inGameRegistration = true;
		owner = true; // Until the already registered command comes back.
		registerCommand = TTSCommandTypes.PowerupPlatformRegister;

		id = StartingPosition.x * StartingPosition.y * StartingPosition.z;

		client = Client;
		client.LocalObjectRegister(this);
	}

	public override void ReceiveNetworkData(TTSPacketReader reader, int command) {

		if (command == TTSCommandTypes.PowerupPlatformSpawn) {
			PowerupType = (TTSBehaviour.Powerup)reader.ReadInt32();
			isNetworkUpdated = true;
		}
		else if (command == TTSCommandTypes.PowerupPlatformPickedUp) {
			//pickedUp = true;
		}
		else if (command == TTSCommandTypes.PowerupPlatformRegisterOK) {
			//startPowerup = true;
			isNetworkUpdated = true;
		}
	}

}
