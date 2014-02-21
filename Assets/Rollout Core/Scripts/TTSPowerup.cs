using UnityEngine;
using System.Collections;

public class TTSPowerup : TTSBehaviour {
	
	public bool debug = false;
	public int DebugTier = 2;
	
	public Powerup AvailablePowerup;
	public Powerup PreviouslyAvailablePowerup = Powerup.None;
	public Powerup ActivePowerup;
	public int tier = 0;
	
	public GameObject hudPowerup;
	
	public Powerup pp2;

	private TTSRacer racer;
	
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
		racer = this.GetComponent<TTSRacer>();
		pp2 = this.GetComponent<TTSPerkManager>().equiptPerkPool2;
	}
	
	#region monobehaviour methods
	void Update() {
		if(level.DebugMode){
			if(Input.GetKeyDown("1")) DrezzStone(DebugTier);
			if(Input.GetKeyDown("2")) EntropyCannon(DebugTier);
			if(Input.GetKeyDown("3")) SuperCBooster(DebugTier);
			if(Input.GetKeyDown("4")) TimeBonus();
			if(Input.GetKeyDown("5")) Shield(DebugTier);
			if(Input.GetKeyDown("6")) Shockwave(DebugTier);
			if(Input.GetKeyDown("7")) Leech(DebugTier);
			if(Input.GetKeyDown("8")) Helix(DebugTier);
			
			if(Input.GetKeyDown("9")){
				racer.DamageRacer(0.9f);
			}
		}

		//if you hit space or the 'a' button on an Xbox controller
		if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0")) ConsumePowerup();
		
		//changes the powerup in the hud when a new powerup is picked up
		if(AvailablePowerup != PreviouslyAvailablePowerup && racer.player == TTSRacer.PlayerType.Player)
			hudPowerup.GetComponent<TTSHudPowerup>().UpdateHudPowerup(AvailablePowerup, tier);
		
		PreviouslyAvailablePowerup = AvailablePowerup;
	}
	#endregion
	
	#region donation methods
	public void GivePowerup(Powerup powerup) {
		if(powerup == AvailablePowerup) {
			if(tier < 3){
				tier++;
			}
		} else if(powerup == Powerup.TimeBonus) {
			if(AvailablePowerup == TTSBehaviour.Powerup.None)
				tier = 0;
			TimeBonus();
		} else {
			AvailablePowerup = powerup;
			if(pp2 == AvailablePowerup)
				tier = 2;
			else
				tier = 1;
		}
		
		//only update the hud if they are a human racer (as AI do not have HUDs)
		if(racer.player == TTSRacer.PlayerType.Player)
			hudPowerup.GetComponent<TTSHudPowerup>().UpdateHudPowerup(AvailablePowerup, tier);
	} 
	
	public void ConsumePowerup() {
		
		switch(AvailablePowerup) {
			case Powerup.EntropyCannon:
			EntropyCannon(tier);
			break;
			
			case Powerup.DrezzStones:
			DrezzStone(tier);
			break;
			
			case Powerup.SuperC:
			SuperCBooster(tier);
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
		
		if(racer.player == TTSRacer.PlayerType.Player)
			hudPowerup.GetComponent<TTSHudPowerup>().UpdateHudPowerup(this.AvailablePowerup, this.tier);
	}
	
	#endregion
	
	
	#region public methods
	public void Leech(int Tier) {
		int deployNum = 0;

		switch (Tier) {
			case 1:
				deployNum = 2;
				break;
			case 2:
				deployNum = 5;
				break;
			case 3:
				deployNum = 15;
				break;
		}

		for(int i=0; i<deployNum; i++){
			DeployLeech();
		}
	}
	
	public void Shockwave(int Tier) {
		DeployShockwave(Tier);
	}
	
	public void DrezzStone(int Tier) {
		if (Tier == 1) {
			DropDrezzStone();
		}

		if (Tier == 2) {
			DropDrezzStone();
			Invoke("DropDrezzStone", 0.5f);
		}
		if (Tier == 3) {
			for(int i = 0; i < 5; i++) {
				Invoke("DropDrezzStone", i * 0.5f);
			}
		}
		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.Powerup.None;
	}

	public void EntropyCannon(int Tier) {
		if (Tier == 1) {
			FireEntropyCannonAsOwner();
		}

		if (Tier == 2) {
			for(int i = 0; i < 5; i++) {
				Invoke("FireEntropyCannonAsOwner", i * 0.1f);
			}
		}
		if (Tier == 3) {
			for(int i = 0; i < 10; i++) {
				Invoke("FireEntropyCannonAsOwner", i * 0.1f);
			}
		}
		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.Powerup.None;
	}

	// Whenever current racer fires entropy cannon.
	private void FireEntropyCannonAsOwner() {
		TTSEntropyCannonProjectile temp = FireEntropyCannon().GetComponent<TTSEntropyCannonProjectile>();
		temp.netOwner = true;
		temp.netRacerID = racer.racerID;
	}

	public void FireEntropyCannonAsClient(float ID) {
		Debug.Log("Firing");
		TTSEntropyCannonProjectile temp = FireEntropyCannon().GetComponent<TTSEntropyCannonProjectile>();
		temp.netOwner = false;
		temp.netID = ID;
	}

	public void Helix(int Tier) {
		if (Tier == 1) {
			FireHelix();
		}

		if (Tier == 2) {
			for(int i = 0; i < 5; i++) {
				Invoke("FireHelix", i * 0.1f);
			}
		}
		if (Tier == 3) {
			for(int i = 0; i < 10; i++) {
				Invoke("FireHelix", i * 0.1f);
			}
		}
		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.Powerup.None;
	}

	public void SuperCBooster(int _tier) {
		TTSRacerSpeedBoost boost = gameObject.AddComponent<TTSRacerSpeedBoost>();
		boost.FireBoost(BoostPrefab);
		
		//offense adds to the boost duration and target force at every level
		if(_tier == 1) {
			boost.duration = 1.0f * racer.Offense;
			boost.TargetForce = 80.0f * racer.Offense;
			vfx.BoostEffect(1.0f);
		}
		if(_tier == 2) {
			boost.duration = 1.5f * racer.Offense;
			boost.TargetForce = 90.0f * racer.Offense;
			vfx.BoostEffect(1.0f);
		}
		if(_tier == 3) {
			boost.duration = 3.0f * racer.Offense;
			boost.TargetForce = 100.0f * racer.Offense;
			vfx.BoostEffect(1.0f);
		}
	}

	public void TimeBonus() {
		GiveTimeBonus();
	}
	
	public void Shield(int _tier){
		DeployShield(_tier);
	}

	#endregion
	
	#region internal methods
	private void DeployShield(int level){
		racer.hasShield = true;
		GameObject go = (GameObject) Instantiate(ShieldPrefab, this.transform.position, Quaternion.identity);
		go.transform.parent = this.transform;
		//defense stat effects to the duration of the shield
		go.GetComponent<TTSShield>().DeployShield(level, racer.Defense, racer);
	}
	
	private void DropDrezzStone() {
		GameObject go = (GameObject) Instantiate(DrezzStonePrefab, this.transform.position - GetComponent<TTSRacer>().displayMeshComponent.forward * 7.0f, this.transform.rotation);
		go.GetComponent<TTSDrezzStone>().offensiveMultiplier = racer.Offense;
		go.rigidbody.AddForce(Random.insideUnitCircle * 50f);
	}
	
	private GameObject FireEntropyCannon() {
		GameObject go = (GameObject) Instantiate(EntropyCannonPrefab);
		go.GetComponent<TTSEntropyCannonProjectile>().offensiveMultiplier = racer.Offense;
		go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
		go.transform.position = this.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = TTSUtils.FlattenVector(racer.displayForward) * (this.rigidbody.velocity.magnitude + go.GetComponent<TTSEntropyCannonProjectile>().ProjectileStartVelocity);
		return go;
	}
	
	private void FireHelix() {
		GameObject go = (GameObject) Instantiate(HelixPrefab);
		go.GetComponent<TTSHelixProjectile>().offensiveMultiplier = racer.Offense;
		go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
		go.transform.position = this.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = this.rigidbody.velocity.normalized * (this.rigidbody.velocity.magnitude + go.GetComponent<TTSHelixProjectile>().ProjectileStartVelocity);
	}
	
	private void GiveTimeBonus() {
		level.time.GiveTimeBonus(1.0f);
		GameObject go = (GameObject) Instantiate(TimeBonusPrefab, this.transform.position, this.transform.rotation);
		go.GetComponent<TTSTimeBonusPrefab>().target = racer.displayMeshComponent.gameObject;
	}
	
	private void DeployShockwave(float _power){
		GameObject go = (GameObject) Instantiate(ShockwavePrefab, this.transform.position, Quaternion.identity);
		go.GetComponent<TTSExplosiveForce>().power = go.GetComponent<TTSExplosiveForce>().power * _power * racer.Offense;
		go.GetComponent<TTSExplosiveForce>().radius = go.GetComponent<TTSExplosiveForce>().radius * _power * racer.Offense;
		go.GetComponent<TTSExplosiveForce>().Activate();
	}
	
	private void DeployLeech() {
		GameObject go = (GameObject) Instantiate(LeechPrefab, this.transform.position, Quaternion.identity);
		go.GetComponent<TTSLeech>().currentRacer = racer;
		go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
		go.transform.position = this.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = this.rigidbody.velocity;
	}
	#endregion
	
}

public class TTSPowerupNetworkHandle : UniGoNetworkHandle
{
	public const int PowerupType_EntropyCannon = 6;

	TTSClient client;
	public int powerupType;
	public float racerID;
	public bool explode;

	public TTSPowerupNetworkHandle(TTSClient Client, int PowerupType, bool Owner, float ID, float RacerID) {
		owner = Owner;
		powerupType = PowerupType;
		client = Client;
		id = ID;
		racerID = RacerID;
		client.RegisterPowerup(this);
	}
	
	public void Death() {
		client.DeregisterPowerup(this);
	}
}