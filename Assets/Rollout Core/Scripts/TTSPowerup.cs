using UnityEngine;
using System.Collections;

public class TTSPowerup : TTSBehaviour
{

	public bool debug = false;
	public int DebugTier = 1;

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
			if (Input.GetKeyDown("4")) { if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  GiveTimeBonus(true); }
			if (Input.GetKeyDown("5")) { if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  Shield(DebugTier); }
			if (Input.GetKeyDown("6")) { if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  DeployShockwave(DebugTier, true); }
			if (Input.GetKeyDown("7")) { if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  Leech(DebugTier); }
			if (Input.GetKeyDown("8")) { if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  Helix(DebugTier); }

			if (Input.GetKeyDown("9")) {
				this.GetComponent<TTSRacer>().DamageRacer(0.9f);
			}
		}

		//if you hit space or the 'a' button on an Xbox controller
		if(AvailablePowerup != Powerup.None){
			if(this.gameObject.GetComponent<TTSRacer>().playerNum == 1) {
				if(level.useKeyboard){
					if(Input.GetKeyDown("space")){
						ConsumePowerup();
					}
				}else if(Input.GetKeyDown("joystick 1 button 0") || Input.GetKeyDown("joystick 1 button 16")){
					Debug.Log("A pressed");
					ConsumePowerup();
				}
			} else if(this.gameObject.GetComponent<TTSRacer>().playerNum == 2) {
				if(Input.GetKeyDown("joystick 2 button 0") || Input.GetKeyDown("joystick 2 button 16")){Debug.Log("A pressed");	ConsumePowerup();}
			} else if(this.gameObject.GetComponent<TTSRacer>().playerNum == 3) {
				if(Input.GetKeyDown("joystick 3 button 0") || Input.GetKeyDown("joystick 3 button 16"))	ConsumePowerup();
			} else if(this.gameObject.GetComponent<TTSRacer>().playerNum == 4) {
				if(Input.GetKeyDown("joystick 4 button 0") || Input.GetKeyDown("joystick 4 button 16"))	ConsumePowerup();
			}
		}

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
			GiveTimeBonus(true);
		}
		else {
			AvailablePowerup = powerup;
			if (pp2 == AvailablePowerup) {
				tier = 2;
			}
			else if (pp2 == Powerup.Lottery) {
				//deals with the lottery perk
				float temp = Random.Range(0, 1.0f);
				if(temp <= lotteryChance){
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
				DeployShockwave(tier, true);
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
				DeployLeech(true);
			}
		}
		else if (_tier == 2) {
			for (int i = 0; i < 5; i++) {
				DeployLeech(true);
			}
		}
		else if (_tier == 3) {
			for (int i = 0; i < 15; i++) {
				DeployLeech(true);
			}
		}
	}

	public void DrezzStone(int _tier) {
		if (_tier == 1) {
			DrezzMid();
		}
		
		if(_tier == 2) {
			for(int i = 0; i < 3; i++) {
				Invoke("DrezzMid", i * 0.3f);
			}
		}
		if(_tier == 3) {
			for(int i = 0; i < 6; i++) {
				Invoke("DrezzMid", i * 0.3f);
			}
		}
		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.Powerup.None;
	}

	public void EntropyCannon(int _tier) {
		if(_tier == 1) {
			for(int i = 0; i < 3; i++) {
				Invoke("EntropyMid", i * 0.075f);
			}
		}
		
		if(_tier == 2) {
			for(int i = 0; i < 8; i++) {
				Invoke("EntropyMid", i * 0.075f);
			}
		}
		if(_tier == 3) {
			for(int i = 0; i < 20; i++) {
				Invoke("EntropyMid", i * 0.075f);
			}
		}
		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.Powerup.None;
	}

	public void Helix(int _tier) {
		if (_tier == 1) {
			HelixMid();
		}
		
		if(_tier == 2) {
			for(int i = 0; i < 3; i++) {
				Invoke("HelixMid", i * 0.1f);
			}
		}
		if(_tier == 3) {
			for(int i = 0; i < 8; i++) {
				Invoke("HelixMid", i * 0.1f);
			}
		}
		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.Powerup.None;
	}

	#region timed handler functions
	private void DrezzMid() {
		DropDrezzStone(true);
	}

	private void EntropyMid() {
		FireEntropyCannon(true);
	}

	private void HelixMid() {
		FireHelix(true);
	}

	#endregion

	public void SuperCBooster(float tier, bool owner) {
		TTSRacerSpeedBoost boost = this.gameObject.AddComponent<TTSRacerSpeedBoost>();
		boost.owner = owner;
		boost.FireBoost(BoostPrefab);
		//offense adds to the boost duration and target force at every level
		if (tier == 1.0f) {
			boost.duration = 1.0f * this.GetComponent<TTSRacer>().Offense;
			boost.TargetForce = 80.0f * this.GetComponent<TTSRacer>().Offense;
			if(owner) vfx.BoostEffect(1.0f);
		}
		if (tier == 2.0f) {
			boost.duration = 1.5f * this.GetComponent<TTSRacer>().Offense;
			boost.TargetForce = 90.0f * this.GetComponent<TTSRacer>().Offense;
			if (owner) vfx.BoostEffect(1.0f);
		}
		if (tier == 3.0f) {
			boost.duration = 3.0f * this.GetComponent<TTSRacer>().Offense;
			boost.TargetForce = 100.0f * this.GetComponent<TTSRacer>().Offense;
			if (owner) vfx.BoostEffect(1.0f);
		}

		if (owner) { SendStaticPowerupDeploy(TTSPowerupNetworkTypes.Boost, tier); }
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

		if (owner) { SendStaticPowerupDeploy(TTSPowerupNetworkTypes.Shield, tier); }

		return go;
	}

	public GameObject DropDrezzStone(bool owner) {
		return DropDrezzStone(owner, null);
	}

	public GameObject DropDrezzStone(bool owner, TTSPowerupNetHandler handle) {
		GameObject go = (GameObject)Instantiate(DrezzStonePrefab, this.gameObject.transform.position - GetComponent<TTSRacer>().displayMeshComponent.forward * 2.0f, this.gameObject.transform.rotation);
		go.GetComponent<TTSDrezzStone>().offensiveMultiplier = this.gameObject.GetComponent<TTSRacer>().Offense;
		go.rigidbody.AddForce(Random.insideUnitCircle * 50f);

		if (owner) { SendPowerupDeploy(TTSPowerupNetworkTypes.Drezz, go); }
		else {
			go.GetComponent<TTSDrezzStone>().SetNetHandler(handle);
		}
		return go;
	}

	public GameObject FireEntropyCannon(bool owner) {
		return FireEntropyCannon(owner, null);
	}

	public GameObject FireEntropyCannon(bool owner, TTSPowerupNetHandler handle) {
		GameObject go = (GameObject)Instantiate(EntropyCannonPrefab);
		go.GetComponent<TTSEntropyCannonProjectile>().offensiveMultiplier = this.gameObject.GetComponent<TTSRacer>().Offense;
		go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
		go.transform.position = this.gameObject.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = this.gameObject.rigidbody.velocity.normalized * (this.gameObject.rigidbody.velocity.magnitude + go.GetComponent<TTSEntropyCannonProjectile>().ProjectileStartVelocity);

		if (owner) { SendPowerupDeploy(TTSPowerupNetworkTypes.Entropy, go); }
		else {
			go.GetComponent<TTSEntropyCannonProjectile>().SetNetHandler(handle);
		}
		return go;
	}

	public GameObject FireHelix(bool owner) {
		return FireHelix(owner, null);
	}

	public GameObject FireHelix(bool owner, TTSPowerupNetHandler handle) {
		GameObject go = (GameObject)Instantiate(HelixPrefab);
		go.GetComponent<TTSHelixProjectile>().offensiveMultiplier = this.gameObject.GetComponent<TTSRacer>().Offense;
		go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
		go.transform.position = this.gameObject.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = this.gameObject.rigidbody.velocity.normalized * (this.gameObject.rigidbody.velocity.magnitude + go.GetComponent<TTSHelixProjectile>().ProjectileStartVelocity);

		if (owner) { SendPowerupDeploy(TTSPowerupNetworkTypes.Helix, go); }
		else {
			go.GetComponent<TTSHelixProjectile>().SetNetHandler(handle);
		}

		return go;
	}
	
	//public GameObject FireHelix(GameObject effectedRacer) {
	//	GameObject go = (GameObject) Instantiate(HelixPrefab);
	//	go.GetComponent<TTSHelixProjectile>().currentRacer = effectedRacer.GetComponent<TTSRacer>();
	//	go.GetComponent<TTSHelixProjectile>().offensiveMultiplier = effectedRacer.GetComponent<TTSRacer>().Offense;
	//	go.GetComponent<TTSHelixProjectile>().racersInFront = (this.GetComponent<TTSRacer>().place - 1);
	//	switch(tier){
	//		case 1:
	//			go.GetComponent<TTSHelixProjectile>().helixInBatch = 1;
	//			break;
	//		case 2:
	//			go.GetComponent<TTSHelixProjectile>().helixInBatch = 3;
	//			break;
	//		case 3:
	//			go.GetComponent<TTSHelixProjectile>().helixInBatch = 8;
	//			break;
	//		default:
	//			go.GetComponent<TTSHelixProjectile>().helixInBatch = 1;
	//			break;
	//	}
	//	go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
	//	go.transform.position = effectedRacer.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
	//	//go.rigidbody.velocity = effectedRacer.rigidbody.velocity.normalized * (effectedRacer.rigidbody.velocity.magnitude + go.GetComponent<TTSHelixProjectile>().ProjectileStartVelocity);
	//	go.rigidbody.velocity = effectedRacer.rigidbody.velocity;
	//	return go;
	//}

	public GameObject GiveTimeBonus(bool owner) {
		level.time.GiveTimeBonus(1.0f);
		GameObject go = (GameObject)Instantiate(TimeBonusPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
		go.GetComponent<TTSTimeBonusPrefab>().target = this.gameObject.GetComponent<TTSRacer>().displayMeshComponent.gameObject;

		if (owner) { SendStaticPowerupDeploy(TTSPowerupNetworkTypes.TimeBonus, 0.0f); }
		return go;
	}

	//public GameObject DeployShockwave(float tier, bool owner) {
	//	GameObject go = (GameObject)Instantiate(ShockwavePrefab, this.gameObject.transform.position, Quaternion.identity);
	//	TTSExplosiveForce explosion = go.GetComponent<TTSExplosiveForce>();

	//	explosion.power = explosion.power * tier * this.gameObject.GetComponent<TTSRacer>().Offense;
	//	explosion.radius = explosion.radius * tier * this.gameObject.GetComponent<TTSRacer>().Offense;
	//	go.GetComponent<TTSShockwave>().Activate(this.gameObject);

	//	if (owner) { SendStaticPowerupDeploy(TTSPowerupNetworkTypes.Shockwave, tier); }
	//	return go;
	//}

	public GameObject DeployShockwave(float tier, bool owner) {
		float power = 0, startSize = 0;

		if (tier == 1f) {
			power = 1.0f;
			startSize = 0.02f;
		}
		else if (tier == 2f) {
			power = 2.0f;
			startSize = 0.04f;
		}
		else if (tier == 3f) {
			power = 10.0f;
			startSize = 0.01f;
		}

		GameObject go = (GameObject) Instantiate(ShockwavePrefab, this.gameObject.transform.position, Quaternion.identity);
		go.GetComponent<ParticleSystem>().startSize = startSize;
		go.GetComponent<TTSShockwave>().power = go.GetComponent<TTSShockwave>().power * power * this.gameObject.GetComponent<TTSRacer>().Offense;
		go.GetComponent<TTSShockwave>().radius = go.GetComponent<TTSShockwave>().radius * power * this.gameObject.GetComponent<TTSRacer>().Offense;
		go.GetComponent<TTSShockwave>().upwardsForce = go.GetComponent<TTSShockwave>().upwardsForce * power * this.gameObject.GetComponent<TTSRacer>().Offense;
		go.GetComponent<TTSShockwave>().Activate(this.gameObject);
		go.transform.parent = this.transform;

		if (owner) { SendStaticPowerupDeploy(TTSPowerupNetworkTypes.Shockwave, tier); }
		return go;
	}

	public GameObject DeployLeech(bool owner) {
		return DeployLeech(owner, null);
	}

	public GameObject DeployLeech(bool owner, TTSPowerupNetHandler handle) {
		GameObject go = (GameObject)Instantiate(LeechPrefab, this.gameObject.transform.position, Quaternion.identity);
		go.GetComponent<TTSLeech>().currentRacer = this.gameObject.GetComponent<TTSRacer>();
		go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
		go.transform.position = this.gameObject.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = this.gameObject.rigidbody.velocity;

		if (owner) { SendPowerupDeploy(TTSPowerupNetworkTypes.Leech, go); }
		else {
			go.GetComponent<TTSLeech>().SetNetHandler(handle);
		}

		return go;
	}
	#endregion

	#region networking

	private TTSRacerNetHandler netHandler;

	public void SetNetHandler(TTSRacerNetHandler handler) {
		netHandler = handler;
	}

	public void SendStaticPowerupDeploy(int poweruptype, float tier) {
		if (!level.client.isMultiplayer) return;
		if (TTSPowerupNetworkTypes.isStaticType(poweruptype)) {
			// Send through racer's net handler
			netHandler.SendStaticPowerup(poweruptype, tier);
		}
	}

	public void SendPowerupDeploy(int poweruptype, GameObject powerup) {
		if (!level.client.isMultiplayer) return;

		if (!TTSPowerupNetworkTypes.isStaticType(poweruptype)) {
			// Create a nethandler and proceed

			TTSPowerupNetHandler handler = new TTSPowerupNetHandler(level.client, true, poweruptype, GetComponent<TTSRacer>().GetNetworkID());

			switch (poweruptype) {
				case TTSPowerupNetworkTypes.Entropy:
					powerup.GetComponent<TTSEntropyCannonProjectile>().SetNetHandler(handler);
					break;

				case TTSPowerupNetworkTypes.Helix:
					powerup.GetComponent<TTSHelixProjectile>().SetNetHandler(handler);
					break;

				case TTSPowerupNetworkTypes.Drezz:
					powerup.GetComponent<TTSDrezzStone>().SetNetHandler(handler);
					break;

				case TTSPowerupNetworkTypes.Leech:
					powerup.GetComponent<TTSLeech>().SetNetHandler(handler);
					break;
			}
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
	public Vector3 position, rotation, speed;
	public Vector3 netPosition, netRotation, netSpeed;

	public int framesSinceNetData = 0;
	public const int ExplodeTimeout = 10;

	public int Type = -1;
	public float Tier = -1.0f;

	public float RacerID = -1.0f;

	public TTSPowerupNetHandler() {
		// For static powerups only. Class is used only for storage.
	}

	public TTSPowerupNetHandler(TTSClient Client, bool Owner, int PowerupType, float racerID) {
		Type = PowerupType;
		owner = Owner;
		RacerID = racerID;
		registerCommand = TTSCommandTypes.PowerupRegister;
		client = Client;
		inGameRegistration = true;
		client.LocalObjectRegister(this);
	}

	public TTSPowerupNetHandler(TTSClient Client, bool Owner, float ID, int PowerupType, float racerID) { // For multiplayer
		Type = PowerupType;
		owner = Owner;
		id = ID;
		RacerID = racerID;
		client = Client;
		networkInterpolation = 0.5f;
		client.LocalObjectRegister(this);
	}

	public override byte[] GetNetworkRegister() {
		writer.AddData(registerCommand);
		writer.AddData(RacerID);
		writer.AddData(id);
		writer.AddData(Type);

		byte[] data = writer.GetMinimizedData();
		writer.ClearData();
		return data;
	}

	public override void ReceiveNetworkData(TTSPacketReader reader, int command) {
		netPosition = reader.ReadVector3();
		netRotation = reader.ReadVector3();
		netSpeed = reader.ReadVector3();

		isNetworkUpdated = true;
	}

	public void UpdatePowerup(Vector3 Pos, Vector3 Rot, Vector3 Speed) {
		if (owner && isServerRegistered) { // Only send data if it's the owner

			if (!isWriterUpdated) writer.ClearData();
			isWriterUpdated = true;

			writer.AddData(TTSCommandTypes.PowerupUpdate);
			writer.AddData(id);
			writer.AddData(Pos);
			writer.AddData(Rot);
			writer.AddData(Speed);
		}
	}

	public override byte[] GetNetworkUpdate() {
		isWriterUpdated = false;
		return writer.GetMinimizedData();
	}
}
