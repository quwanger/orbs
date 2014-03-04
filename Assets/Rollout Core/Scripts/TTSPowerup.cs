using UnityEngine;
using System.Collections;

public class TTSPowerup : TTSBehaviour
{

	public bool debug = false;
	public int DebugTier = 2;

	public Powerup AvailablePowerup;
	public Powerup PreviouslyAvailablePowerup = Powerup.None;
	public Powerup ActivePowerup;
	public int tier = 0;

	public GameObject hudPowerup;

	//Perk Stuff
	public Powerup pp2;
	private float lotteryChance = 0.2f;

	#region Projectile Prefab Assignment
	public GameObject DrezzStonePrefab;
	public GameObject EntropyCannonPrefab;
	public GameObject BoostPrefab;
	public GameObject TimeBonusPrefab;
	public GameObject ShieldPrefab;
	public GameObject ShockwavePrefab;
	public GameObject LeechPrefab;
	public GameObject HelixPrefab;
	#endregion

	void Awake() {
		pp2 = this.GetComponent<TTSPerkManager>().equiptPerkPool2;
	}

	#region monobehaviour methods
	void Update() {
		if (level.DebugMode) {
			if (Input.GetKeyDown("1")) { if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  DrezzStone(DebugTier); }
			if (Input.GetKeyDown("2")) { if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  EntropyCannon(DebugTier); }
			if (Input.GetKeyDown("3")) { if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  SuperCBooster(DebugTier, true); }
			if (Input.GetKeyDown("4")) { if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  TimeBonus(); }
			if (Input.GetKeyDown("5")) { if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  Shield(DebugTier); }
			if (Input.GetKeyDown("6")) { if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  Shockwave(DebugTier); }
			if (Input.GetKeyDown("7")) { if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  Leech(DebugTier); }
			if (Input.GetKeyDown("8")) { if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  Helix(DebugTier); }

			if (Input.GetKeyDown("9")) {
				this.GetComponent<TTSRacer>().DamageRacer(0.9f);
			}
		}

		//if you hit space or the 'a' button on an Xbox controller
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0")) ConsumePowerup();

		//changes the powerup in the hud when a new powerup is picked up
		if (AvailablePowerup != PreviouslyAvailablePowerup && this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)
			hudPowerup.GetComponent<TTSHudPowerup>().UpdateHudPowerup(AvailablePowerup, tier);

		PreviouslyAvailablePowerup = AvailablePowerup;
	}
	#endregion

	#region donation methods
	public void GivePowerup(Powerup powerup) {
		if (powerup == AvailablePowerup) {
			if (tier < 3) {
				tier++;
			}
		}
		else if (powerup == Powerup.TimeBonus) {
			if (AvailablePowerup == TTSBehaviour.Powerup.None)
				tier = 0;
			TimeBonus();
		}
		else {
			AvailablePowerup = powerup;
			if (pp2 == AvailablePowerup) {
				tier = 2;
			}
			else if (pp2 == Powerup.Lottery) {
				//deals with the lottery perk
				float temp = Random.Range(0, 1.0f);
				if (temp <= lotteryChance) {
					if (level.DebugMode == true)
						Debug.Log("Lottery Winner");
					tier = 2;
				}
				else {
					tier = 1;
				}
			}
			else {
				tier = 1;
			}
		}

		//only update the hud if they are a human racer (as AI do not have HUDs)
		if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)
			hudPowerup.GetComponent<TTSHudPowerup>().UpdateHudPowerup(AvailablePowerup, tier);
	}

	public void ConsumePowerup() {

		switch (AvailablePowerup) {
			case Powerup.EntropyCannon:
				EntropyCannon(tier);
				break;

			case Powerup.DrezzStones:
				DrezzStone(tier);
				break;

			case Powerup.SuperC:
				SuperCBooster(tier, true);
				break;

			case Powerup.Shield:
				Shield(tier);
				break;

			case Powerup.Shockwave:
				Shockwave(tier);
				break;

			case Powerup.Leech:
				Leech(tier);
				break;

			case Powerup.Helix:
				Helix(tier);
				break;

			default:
				//Play a sound?
				break;
		}

		this.ActivePowerup = this.AvailablePowerup;
		this.AvailablePowerup = Powerup.None;

		this.tier = 0;

		if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)
			hudPowerup.GetComponent<TTSHudPowerup>().UpdateHudPowerup(this.AvailablePowerup, this.tier);
	}

	#endregion


	#region public methods
	public void Leech(int _tier) {
		if (_tier == 1) {
			for (int i = 0; i < 2; i++) {
				DeployLeech(this.gameObject);
			}
		}
		else if (_tier == 2) {
			for (int i = 0; i < 4; i++) {
				DeployLeech(this.gameObject);
			}
		}
		else if (_tier == 3) {
			for (int i = 0; i < 8; i++) {
				DeployLeech(this.gameObject);
			}
		}
	}

	public void Shockwave(int _tier) {
		if (_tier == 1) {
			DeployShockwave(1.0f, this.gameObject);
		}
		else if (_tier == 2) {
			DeployShockwave(2.0f, this.gameObject);
		}
		else if (_tier == 3) {
			DeployShockwave(3.0f, this.gameObject);
		}
	}

	public void DrezzStone(int _tier) {
		if (_tier == 1) {
			DrezzMid();
		}

		if (_tier == 2) {
			DropDrezzStone(this.gameObject);
			Invoke("DrezzMid", 0.5f);
		}
		if (_tier == 3) {
			for (int i = 0; i < 5; i++) {
				Invoke("DrezzMid", i * 0.5f);
			}
		}
		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.Powerup.None;
	}

	private void DrezzMid() {
		DropDrezzStone(this.gameObject);
	}

	public void EntropyCannon(int _tier) {
		if (_tier == 1) {
			EntropyMid();
		}

		if (_tier == 2) {
			for (int i = 0; i < 5; i++) {
				Invoke("EntropyMid", i * 0.1f);
			}
		}
		if (_tier == 3) {
			for (int i = 0; i < 10; i++) {
				Invoke("EntropyMid", i * 0.1f);
			}
		}
		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.Powerup.None;
	}

	private void EntropyMid() {
		FireEntropyCannon(this.gameObject);
	}

	public void Helix(int _tier) {
		if (_tier == 1) {
			HelixMid();
		}

		if (_tier == 2) {
			for (int i = 0; i < 5; i++) {
				Invoke("HelixMid", i * 0.1f);
			}
		}
		if (_tier == 3) {
			for (int i = 0; i < 10; i++) {
				Invoke("HelixMid", i * 0.1f);
			}
		}
		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.Powerup.None;
	}

	private void HelixMid() {
		FireHelix(this.gameObject);
	}

	public void SuperCBooster(float tier, bool owner) {
		TTSRacerSpeedBoost boost = this.gameObject.AddComponent<TTSRacerSpeedBoost>();
		boost.owner = owner;
		boost.FireBoost(BoostPrefab);

		//offense adds to the boost duration and target force at every level
		if (tier == 1.0f) {
			boost.duration = 1.0f * this.GetComponent<TTSRacer>().Offense;
			boost.TargetForce = 80.0f * this.GetComponent<TTSRacer>().Offense;
			vfx.BoostEffect(1.0f);
		}
		if (tier == 2.0f) {
			boost.duration = 1.5f * this.GetComponent<TTSRacer>().Offense;
			boost.TargetForce = 90.0f * this.GetComponent<TTSRacer>().Offense;
			vfx.BoostEffect(1.0f);
		}
		if (tier == 3.0f) {
			boost.duration = 3.0f * this.GetComponent<TTSRacer>().Offense;
			boost.TargetForce = 100.0f * this.GetComponent<TTSRacer>().Offense;
			vfx.BoostEffect(1.0f);
		}

		if (owner) { SendPowerupDeploy(TTSPowerupNetworkTypes.Boost, tier); }
	}

	public void TimeBonus() {
		GiveTimeBonus(this.gameObject);
	}

	public void Shield(float tier) {
		DeployShield(tier, true);
	}

	#endregion

	#region internal methods
	public GameObject DeployShield(float tier, bool owner) {
		gameObject.GetComponent<TTSRacer>().hasShield = true;
		GameObject go = (GameObject)Instantiate(ShieldPrefab, gameObject.transform.position, Quaternion.identity);
		go.transform.parent = gameObject.transform;
		//defense stat effects to the duration of the shield
		go.GetComponent<TTSShield>().DeployShield(tier, gameObject.GetComponent<TTSRacer>().Defense, gameObject.GetComponent<TTSRacer>());

		if (owner) { SendPowerupDeploy(TTSPowerupNetworkTypes.Shield, tier); }

		return go;
	}

	public GameObject DropDrezzStone(GameObject effectedRacer) {
		GameObject go = (GameObject)Instantiate(DrezzStonePrefab, effectedRacer.transform.position - GetComponent<TTSRacer>().displayMeshComponent.forward * 7.0f, effectedRacer.transform.rotation);
		go.GetComponent<TTSDrezzStone>().offensiveMultiplier = effectedRacer.GetComponent<TTSRacer>().Offense;
		go.rigidbody.AddForce(Random.insideUnitCircle * 50f);
		return go;
	}

	public GameObject FireEntropyCannon(GameObject effectedRacer) {
		GameObject go = (GameObject)Instantiate(EntropyCannonPrefab);
		go.GetComponent<TTSEntropyCannonProjectile>().offensiveMultiplier = effectedRacer.GetComponent<TTSRacer>().Offense;
		go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
		go.transform.position = effectedRacer.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = effectedRacer.rigidbody.velocity.normalized * (effectedRacer.rigidbody.velocity.magnitude + go.GetComponent<TTSEntropyCannonProjectile>().ProjectileStartVelocity);
		return go;
	}

	public GameObject FireHelix(GameObject effectedRacer) {
		GameObject go = (GameObject)Instantiate(HelixPrefab);
		go.GetComponent<TTSHelixProjectile>().offensiveMultiplier = effectedRacer.GetComponent<TTSRacer>().Offense;
		go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
		go.transform.position = effectedRacer.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = effectedRacer.rigidbody.velocity.normalized * (effectedRacer.rigidbody.velocity.magnitude + go.GetComponent<TTSHelixProjectile>().ProjectileStartVelocity);
		return go;
	}

	public GameObject GiveTimeBonus(GameObject effectedRacer) {
		level.time.GiveTimeBonus(1.0f);
		GameObject go = (GameObject)Instantiate(TimeBonusPrefab, effectedRacer.transform.position, effectedRacer.transform.rotation);
		go.GetComponent<TTSTimeBonusPrefab>().target = effectedRacer.GetComponent<TTSRacer>().displayMeshComponent.gameObject;
		return go;
	}

	public GameObject DeployShockwave(float _power, GameObject effectedRacer) {
		GameObject go = (GameObject)Instantiate(ShockwavePrefab, effectedRacer.transform.position, Quaternion.identity);
		go.GetComponent<TTSExplosiveForce>().power = go.GetComponent<TTSExplosiveForce>().power * _power * effectedRacer.GetComponent<TTSRacer>().Offense;
		go.GetComponent<TTSExplosiveForce>().radius = go.GetComponent<TTSExplosiveForce>().radius * _power * effectedRacer.GetComponent<TTSRacer>().Offense;
		go.GetComponent<TTSExplosiveForce>().Activate();
		return go;
	}

	public GameObject DeployLeech(GameObject effectedRacer) {
		GameObject go = (GameObject)Instantiate(LeechPrefab, effectedRacer.transform.position, Quaternion.identity);
		go.GetComponent<TTSLeech>().currentRacer = effectedRacer.GetComponent<TTSRacer>();
		go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
		go.transform.position = effectedRacer.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = effectedRacer.rigidbody.velocity;
		return go;
	}
	#endregion

	#region networking

	private TTSRacerNetHandler netHandler;

	public void SetNetHandler(TTSRacerNetHandler handler) {
		netHandler = handler;
	}

	public void SendPowerupDeploy(int poweruptype, float tier) {
		if (!level.client.isMultiplayer) return;

		if (TTSPowerupNetworkTypes.isStaticType(poweruptype)) {
			// Send through racer's net handler
			netHandler.SendStaticPowerup(poweruptype, tier);
		}

		else {
			// Create a nethandler and proceed
		}
	}
	#endregion

}

public class TTSPowerupNetworkTypes
{
	// Static
	public const int TimeBonus	= 1;
	public const int Shockwave	= 2;
	public const int Shield		= 3;
	public const int Boost		= 4;

	// Updated
	public const int Leech		= 5;
	public const int Drezz		= 6;
	public const int Entropy	= 7;
	public const int Helix		= 8;

	public static bool isStaticType(int type){
		return (type == TimeBonus || type == Shockwave || type == Shield || type == Boost);
	}
}

public class TTSPowerupNetHandler : TTSNetworkHandle
{
	public int Type = -1;
	public float Tier = -1.0f;

	public TTSPowerupNetHandler() {
		// For static powerups only.
	}

	public TTSPowerupNetHandler(TTSClient Client, bool Owner) {
	}

	public TTSPowerupNetHandler(TTSClient Client, bool Owner, float ID) { // For multiplayer
	}

	public override void ReceiveNetworkData(TTSPacketReader reader, int command) {
		// Implement
	}
}