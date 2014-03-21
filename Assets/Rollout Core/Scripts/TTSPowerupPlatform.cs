using UnityEngine;
using System.Collections;

public class TTSPowerupPlatform : TTSBehaviour
{
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


	void Start() {
		audio.volume = 0.75f;

		//set an initial powerup for the platform

		if (level.client.isMultiplayer) {
			netHandle = new TTSPowerupPlatformNetworkHandler(level.client, transform.position);
		}
		else {
			pickedUp = false;

			if (isRandom)
				currentPowerup = getRandomPowerup();

			displayPowerup();
		}
	}

	void Update() {
		//rotate the powerup
		foreach (Transform childTransform in this.transform) {
			childTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
		}

		if (netHandle != null && netHandle.isServerRegistered) {
			if (netHandle.owner) {
				// Execute only once after the platform is registered
				if (netHandle.isServerRegistered && netHandle.startPowerup) {
					if (isRandom)
						currentPowerup = getRandomPowerup();

					spawnPowerup(currentPowerup);

					netHandle.startPowerup = false;
				}
			}
			else if (!netHandle.owner && netHandle.isNetworkUpdated) {
				spawnPowerup(netHandle.PowerupType);

				netHandle.DoneReading();
			}
		}
	}

	void OnTriggerEnter(Collider collider) {
		if (pickedUp)
			return;

		foreach (GameObject racer in racers) {
			if (collider.gameObject == racer) {

				collider.gameObject.GetComponent<TTSPowerup>().GivePowerup(this.currentPowerup);

				// Play the pickup animation

				ParticleSystem tempPickupParticleSystem = null;

				#region currentPowerup -> pickupParticleSystem
				switch (currentPowerup) {
					case Powerup.EntropyCannon:
						tempPickupParticleSystem = EntropyParticleSystem;
						break;

					case Powerup.SuperC:
						tempPickupParticleSystem = SuperCParticleSystem;
						break;

					case Powerup.TimeBonus:
						tempPickupParticleSystem = TimeBonusParticleSystem;
						break;

					case Powerup.Shield:
						tempPickupParticleSystem = ShieldParticleSystem;
						break;

					case Powerup.DrezzStones:
						tempPickupParticleSystem = DrezzStoneParticleSystem;
						break;

					case Powerup.Helix:
						tempPickupParticleSystem = HelixParticleSystem;
						break;

					case Powerup.Leech:
						tempPickupParticleSystem = LeechParticleSystem;
						break;

					case Powerup.Shockwave:
						tempPickupParticleSystem = ShockwaveParticleSystem;
						break;

					default:
						break;
				}
				#endregion

				if (tempPickupParticleSystem != null) {
					Vector3 pos = transform.position + Vector3.up * 3.0f;
					pickupParticleSystem = Instantiate(tempPickupParticleSystem, pos, powerupMesh.transform.rotation) as ParticleSystem;
					pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
				}

				pickedUp = true;

				//play sound
				audio.PlayOneShot(clip);

				//remove powerup
				Destroy(powerupMesh);

				powerupMesh = null;

				if (netHandle == null || (netHandle != null && netHandle.owner)) {
					StartCoroutine("respawnPickup");
				}
			}
		}
	}

	private IEnumerator respawnPickup() {
		//wait 5 second for respawn
		yield return new WaitForSeconds(respawnTime);
		//set new powerup
		spawnPowerup(getRandomPowerup());
	}

	private void spawnPowerup(Powerup powerup) {
		if (powerupMesh != null) {
			//remove powerup
			Destroy(powerupMesh);
		}

		//set new powerup
		if (isRandom)
			currentPowerup = powerup;

		displayPowerup();

		pickedUp = false;

		if (netHandle != null && netHandle.owner) {
			netHandle.SpawnPowerup(currentPowerup);
		}
	}

	private Powerup getRandomPowerup() {

		Powerup powerup = GetRandomEnum<Powerup>();

		if (level.currentGameType != TTSLevel.Gametype.TimeTrial) {
			while (powerup == Powerup.TimeBonus || powerup == Powerup.None || powerup == Powerup.Lottery) {
				powerup = GetRandomEnum<Powerup>();
			}
		}
		else {
			//make sure that the random powerup isnt none or lottery
			while (powerup == Powerup.None || powerup == Powerup.Lottery) {
				powerup = GetRandomEnum<Powerup>();
			}
		}

		return powerup;
	}

	private void displayPowerup() {
		Vector3 pos = transform.position + Vector3.up * 3.0f;

		GameObject tempPowerupMesh = null;

		//display the current powerup as a child of the platform
		#region currentPowerup -> powerupMesh
		switch (currentPowerup) {
			case (Powerup.SuperC):
				tempPowerupMesh = powerupBoost;
				break;
			case (Powerup.EntropyCannon):
				tempPowerupMesh = powerupMissiles;
				break;
			case (Powerup.TimeBonus):
				tempPowerupMesh = powerupMoreTime;
				break;
			case (Powerup.DrezzStones):
				tempPowerupMesh = powerupDrezzStone;
				break;
			case (Powerup.Shield):
				tempPowerupMesh = powerupShield;
				break;
			case (Powerup.Helix):
				tempPowerupMesh = powerupHelix;
				break;
			case (Powerup.Leech):
				tempPowerupMesh = powerupLeech;
				break;
			case (Powerup.Shockwave):
				tempPowerupMesh = powerupShockwave;
				break;

			default:
				//getRandomPowerup();
				break;
		}
		#endregion

		if (tempPowerupMesh != null) {
			powerupMesh = Instantiate(tempPowerupMesh, pos, transform.rotation) as GameObject;
			powerupMesh.transform.parent = this.transform;
		}
	}
}

public class TTSPowerupPlatformNetworkHandler : TTSNetworkHandle
{
	public TTSBehaviour.Powerup PowerupType = TTSBehaviour.Powerup.None;
	public bool pickedUp = false;
	public bool startPowerup = false;

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
	public void SpawnPowerup(TTSBehaviour.Powerup powerupType) {
		if (owner) {
			writer.ClearData();
			writer.AddData(TTSCommandTypes.PowerupPlatformSpawn);
			writer.AddData(id);
			writer.AddData((int)powerupType);
			isWriterUpdated = true;
		}
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
			startPowerup = true;
			isNetworkUpdated = true;
		}
	}

}
