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
	private TTSClient client;
	private TTSPowerupPlatformNetworkHandler netHandle;
	#endregion

	void Start() {
		audio.volume = 0.75f;

		client = level.client;

		if (client.isMultiplayer) {
			netHandle = new TTSPowerupPlatformNetworkHandler(client, transform.position);
		}
		else {
			pickedUp = false;

			//set an initial powerup for the platform
			if (isRandom)
				currentPowerup = getRandomPowerup();

			spawnPowerup(currentPowerup);
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
				Vector3 pos = transform.position + Vector3.up * 3.0f;

				switch (currentPowerup) {
					case Powerup.EntropyCannon:
						pickupParticleSystem = Instantiate(EntropyParticleSystem, pos, powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;

					case Powerup.SuperC:
						pickupParticleSystem = Instantiate(SuperCParticleSystem, pos, powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;

					case Powerup.TimeBonus:
						pickupParticleSystem = Instantiate(TimeBonusParticleSystem, pos, powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;

					case Powerup.Shield:
						pickupParticleSystem = Instantiate(ShieldParticleSystem, pos, powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;

					case Powerup.DrezzStones:
						pickupParticleSystem = Instantiate(DrezzStoneParticleSystem, pos, powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;

					case Powerup.Helix:
						pickupParticleSystem = Instantiate(HelixParticleSystem, pos, powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;

					case Powerup.Leech:
						pickupParticleSystem = Instantiate(LeechParticleSystem, pos, powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;

					case Powerup.Shockwave:
						pickupParticleSystem = Instantiate(ShockwaveParticleSystem, pos, powerupMesh.transform.rotation) as ParticleSystem;
						pickupParticleSystem.Emit(numParticlesToEmitOnPickup);
						break;

					default:
						break;
				}

				pickedUp = true;

				//play sound
				audio.PlayOneShot(clip);

				//remove powerup
				Destroy(powerupMesh);

				powerupMesh = null;
				//currentPowerup = Powerup.None;

				if (netHandle != null && netHandle.owner) {
					StartCoroutine("respawnPickup");
				}
			}
		}
	}

	private IEnumerator respawnPickup() {
		//wait 5 second for respawn
		yield return new WaitForSeconds(respawnTime);

		spawnPowerup(getRandomPowerup());
	}

	private void spawnPowerup(Powerup powerup) {
		if (powerupMesh != null) {
			//remove powerup
			Destroy(powerupMesh);
		}

		if (!client.isMultiplayer || (netHandle != null && netHandle.owner)) {
			//set new powerup
			if (isRandom)
				currentPowerup = powerup;

			displayPowerup();

			pickedUp = false;

			if (netHandle != null) {
				netHandle.SpawnPowerup(currentPowerup);
			}
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

		//display the current powerup as a child of the platform
		switch (currentPowerup) {
			case (Powerup.SuperC):
				powerupMesh = Instantiate(powerupBoost, pos, transform.rotation) as GameObject;
				powerupMesh.transform.parent = this.transform;
				break;
			case (Powerup.EntropyCannon):
				powerupMesh = Instantiate(powerupMissiles, pos, transform.rotation) as GameObject;
				powerupMesh.transform.parent = this.transform;
				break;

			case (Powerup.TimeBonus):
				powerupMesh = Instantiate(powerupMoreTime, pos, transform.rotation) as GameObject;
				powerupMesh.transform.parent = this.transform;
				break;
			case (Powerup.DrezzStones):
				powerupMesh = Instantiate(powerupDrezzStone, pos, transform.rotation) as GameObject;
				powerupMesh.transform.parent = this.transform;
				break;
			case (Powerup.Shield):
				powerupMesh = Instantiate(powerupShield, pos, transform.rotation) as GameObject;
				powerupMesh.transform.parent = this.transform;
				break;
			case (Powerup.Helix):
				powerupMesh = Instantiate(powerupHelix, pos, transform.rotation) as GameObject;
				powerupMesh.transform.parent = this.transform;
				break;
			case (Powerup.Leech):
				powerupMesh = Instantiate(powerupLeech, pos, transform.rotation) as GameObject;
				powerupMesh.transform.parent = this.transform;
				break;
			case (Powerup.Shockwave):
				powerupMesh = Instantiate(powerupShockwave, pos, transform.rotation) as GameObject;
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
	public TTSBehaviour.Powerup PowerupType = 0;
	public bool pickedUp = false;

	public bool startPowerup = false;

	public TTSPowerupPlatformNetworkHandler(TTSClient Client, Vector3 StartingPosition) {
		type = "Powerup Platform";
		canForfeitControl = true;
		inGameRegistration = true;
		owner = true;
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
			pickedUp = true;
		}
		else if (command == TTSCommandTypes.PowerupPlatformRegisterOK) {
			startPowerup = true;
			isNetworkUpdated = true;
		}
	}
}