using UnityEngine;
using System.Collections;

public class TTSPowerup : TTSBehaviour {
	
	public bool debug = false;
	public int DebugTier = 2;
	
	public Powerup AvailablePowerup;
	public Powerup ActivePowerup;
	public int tier = 0;
	
	public GameObject hudPowerup;
	
	#region Projectile Prefab Assignment
	public GameObject DrezzStonePrefab;
	public GameObject EntropyCannonPrefab;
	public GameObject BoostPrefab;
	public GameObject TimeBonusPrefab;
	public GameObject ShieldPrefab;
	public GameObject ShockwavePrefab;
	#endregion
	
	
	#region monobehaviour methods
	void Update() {
		if(level.DebugMode){
			if(Input.GetKeyDown("1")) DrezzStone(DebugTier);
			if(Input.GetKeyDown("2")) EntropyCannon(DebugTier);
			if(Input.GetKeyDown("3")) SuperCBooster(DebugTier);
			if(Input.GetKeyDown("4")) TimeBonus();
			if(Input.GetKeyDown("5")) Shield(DebugTier);
			if(Input.GetKeyDown("6")) Shockwave(DebugTier);
			
			if(Input.GetKeyDown("9")){
				this.GetComponent<TTSRacer>().DamageRacer(0.9f);
			}
		}

		//if you hit space or the 'a' button on an Xbox controller
		if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0")) ConsumePowerup();
	
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
			tier = 1;
		}
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
			
			default:
			//Play a sound?
			break;
		}
		
		this.ActivePowerup = this.AvailablePowerup;
		this.AvailablePowerup = Powerup.None;
		
		this.tier = 0;
		hudPowerup.GetComponent<TTSHudPowerup>().UpdateHudPowerup(this.AvailablePowerup, this.tier);
		this.tier = 1;
	}
	
	#endregion
	
	
	#region public methods
	public void Shockwave(int _tier) {
		if(_tier==1){
			DeployShockwave(1.0f);
		}else if(_tier==2){
			DeployShockwave(2.0f);
		}else if(_tier==3){
			DeployShockwave(3.0f);
		}
	}
	
	public void DrezzStone(int _tier) {
		if(_tier == 1) {
			DropDrezzStone();
		}
		
		if(_tier == 2) {
			DropDrezzStone();
			Invoke("DropDrezzStone", 0.5f);
		}
		if(_tier == 3) {
			for(int i = 0; i < 5; i++) {
				Invoke("DropDrezzStone", i * 0.5f);
			}
		}
		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.Powerup.None;
	}
	
	public void EntropyCannon(int _tier) {
		if(_tier == 1) {
			FireEntropyCannon();
		}
		
		if(_tier == 2) {
			for(int i = 0; i < 5; i++) {
				Invoke("FireEntropyCannon", i * 0.1f);
			}
		}
		if(_tier == 3) {
			for(int i = 0; i < 10; i++) {
				Invoke("FireEntropyCannon", i * 0.1f);
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
			boost.duration = 1.0f * this.GetComponent<TTSRacer>().Offense;
			boost.TargetForce = 80.0f * this.GetComponent<TTSRacer>().Offense;
			vfx.BoostEffect(1.0f);
		}
		if(_tier == 2) {
			boost.duration = 1.5f * this.GetComponent<TTSRacer>().Offense;
			boost.TargetForce = 90.0f * this.GetComponent<TTSRacer>().Offense;
			vfx.BoostEffect(1.0f);
		}
		if(_tier == 3) {
			boost.duration = 3.0f * this.GetComponent<TTSRacer>().Offense;
			boost.TargetForce = 100.0f * this.GetComponent<TTSRacer>().Offense;
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
		this.GetComponent<TTSRacer>().hasShield = true;
		GameObject go = (GameObject) Instantiate(ShieldPrefab, this.transform.position, Quaternion.identity);
		go.transform.parent = this.transform;
		//defense stat effects to the duration of the shield
		go.GetComponent<TTSShield>().DeployShield(level, this.GetComponent<TTSRacer>().Defense, this.GetComponent<TTSRacer>());
	}
	
	private void DropDrezzStone() {
		GameObject go = (GameObject) Instantiate(DrezzStonePrefab, this.transform.position - GetComponent<TTSRacer>().displayMeshComponent.forward * 7.0f, this.transform.rotation);
		go.GetComponent<TTSDrezzStone>().offensiveMultiplier = this.GetComponent<TTSRacer>().Offense;
		go.rigidbody.AddForce(Random.insideUnitCircle * 50f);
	}
	
	private void FireEntropyCannon() {
		GameObject go = (GameObject) Instantiate(EntropyCannonPrefab);
		go.GetComponent<TTSEntropyCannonProjectile>().offensiveMultiplier = this.GetComponent<TTSRacer>().Offense;
		go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
		go.transform.position = this.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = this.rigidbody.velocity.normalized * go.GetComponent<TTSEntropyCannonProjectile>().ProjectileStartVelocity;
	}
	
	private void GiveTimeBonus() {
		level.time.GiveTimeBonus(1.0f);
		GameObject go = (GameObject) Instantiate(TimeBonusPrefab, this.transform.position, this.transform.rotation);
		go.GetComponent<TTSTimeBonusPrefab>().target = this.GetComponent<TTSRacer>().displayMeshComponent.gameObject;
	}
	
	private void DeployShockwave(float _power){
		GameObject go = (GameObject) Instantiate(ShockwavePrefab, this.transform.position, Quaternion.identity);
		go.GetComponent<TTSExplosiveForce>().power = go.GetComponent<TTSExplosiveForce>().power * _power * this.GetComponent<TTSRacer>().Offense;
		go.GetComponent<TTSExplosiveForce>().radius = go.GetComponent<TTSExplosiveForce>().radius * _power * this.GetComponent<TTSRacer>().Offense;
		go.GetComponent<TTSExplosiveForce>().Activate();
	}
	#endregion
	
}
