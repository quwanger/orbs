using UnityEngine;
using System.Collections;

public class TTSPowerup : TTSBehaviour {
	
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
		if(level.DebugMode){
			if(Input.GetKeyDown("1")){ if(this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  DrezzStone(DebugTier);}
			if(Input.GetKeyDown("2")){ if(this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  EntropyCannon(DebugTier);}
			if(Input.GetKeyDown("3")){ if(this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  SuperCBooster(DebugTier, this.gameObject);}
			if(Input.GetKeyDown("4")){ if(this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  TimeBonus();}
			if(Input.GetKeyDown("5")){ if(this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  Shield(DebugTier);}
			if(Input.GetKeyDown("6")){ if(this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  Shockwave(DebugTier);}
			if(Input.GetKeyDown("7")){ if(this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  Leech(DebugTier);}
			if(Input.GetKeyDown("8")){ if(this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)  Helix(DebugTier);}
			
			if(Input.GetKeyDown("9")){
				this.GetComponent<TTSRacer>().DamageRacer(0.9f);
			}
		}

		//if you hit space or the 'a' button on an Xbox controller
		if(this.gameObject.GetComponent<TTSRacer>().playerNum == 1) {
			if(level.useKeyboard){
				if(Input.GetKeyDown("space")){
					ConsumePowerup();
				}
			}else if(Input.GetKeyDown("joystick 1 button 0")){
				ConsumePowerup();
			}
		} else if(this.gameObject.GetComponent<TTSRacer>().playerNum == 2) {
			if(Input.GetKeyDown("joystick 2 button 0"))	ConsumePowerup();
		} else if(this.gameObject.GetComponent<TTSRacer>().playerNum == 3) {
			if(Input.GetKeyDown("joystick 3 button 0"))	ConsumePowerup();
		} else if(this.gameObject.GetComponent<TTSRacer>().playerNum == 4) {
			if(Input.GetKeyDown("joystick 4 button 0"))	ConsumePowerup();
		}
		
		//changes the powerup in the hud when a new powerup is picked up
		if(AvailablePowerup != PreviouslyAvailablePowerup && this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)
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
			if(pp2 == AvailablePowerup){
				tier = 2;
			}else if(pp2 == Powerup.Lottery){
				//deals with the lottery perk
				float temp = Random.Range(0, 1.0f);
				if(temp <= lotteryChance){
					if(level.DebugMode == true)
						Debug.Log("Lottery Winner");
					tier = 2;
				}else{
					tier = 1;
				}
			}else{
				tier = 1;
			}
		}
		
		//only update the hud if they are a human racer (as AI do not have HUDs)
		if(this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)
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
			SuperCBooster(tier, this.gameObject);
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
		
		if(this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)
			hudPowerup.GetComponent<TTSHudPowerup>().UpdateHudPowerup(this.AvailablePowerup, this.tier);
	}
	
	#endregion
	
	
	#region public methods
	public void Leech(int _tier) {
		if(_tier==1){
			for(int i=0; i<2; i++){
				DeployLeech(this.gameObject);
			}
		}else if(_tier==2){
			for(int i=0; i<5; i++){
				DeployLeech(this.gameObject);
			}
		}else if(_tier==3){
			for(int i=0; i<15; i++){
				DeployLeech(this.gameObject);
			}
		}
	}
	
	public void Shockwave(int _tier) {
		if(_tier==1){
			DeployShockwave(1.0f, 0.02f, this.gameObject);
		}else if(_tier==2){
			DeployShockwave(2.0f, 0.04f, this.gameObject);
		}else if(_tier==3){
			DeployShockwave(10.0f, 0.1f, this.gameObject);
		}
	}
	
	public void DrezzStone(int _tier) {
		if(_tier == 1) {
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
	
	private void DrezzMid(){
		DropDrezzStone(this.gameObject);
	}
	
	public void EntropyCannon(int _tier) {
		if(_tier == 1) {
			EntropyMid();
		}
		
		if(_tier == 2) {
			for(int i = 0; i < 5; i++) {
				Invoke("EntropyMid", i * 0.1f);
			}
		}
		if(_tier == 3) {
			for(int i = 0; i < 10; i++) {
				Invoke("EntropyMid", i * 0.1f);
			}
		}
		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.Powerup.None;
	}
	
	private void EntropyMid(){
		FireEntropyCannon(this.gameObject);
	}
	
	public void Helix(int _tier) {
		if(_tier == 1) {
			HelixMid();
		}
		
		if(_tier == 2) {
			for(int i = 0; i < 5; i++) {
				Invoke("HelixMid", i * 0.1f);
			}
		}
		if(_tier == 3) {
			for(int i = 0; i < 10; i++) {
				Invoke("HelixMid", i * 0.1f);
			}
		}
		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.Powerup.None;
	}
	
	private void HelixMid(){
		FireHelix(this.gameObject);
	}

	public void SuperCBooster(int _tier, GameObject effectedRacer) {
		TTSRacerSpeedBoost boost = effectedRacer.AddComponent<TTSRacerSpeedBoost>();
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
		GiveTimeBonus(this.gameObject);
	}
	
	public void Shield(int _tier){
		DeployShield(_tier, this.gameObject);
	}

	#endregion
	
	#region internal methods
	public GameObject DeployShield(int level, GameObject effectedRacer){
		effectedRacer.GetComponent<TTSRacer>().hasShield = true;
		GameObject go = (GameObject) Instantiate(ShieldPrefab, effectedRacer.transform.position, Quaternion.identity);
		go.transform.parent = effectedRacer.transform;
		//defense stat effects to the duration of the shield
		go.GetComponent<TTSShield>().DeployShield(level, effectedRacer.GetComponent<TTSRacer>().Defense, effectedRacer.GetComponent<TTSRacer>());
		return go;
	}
	
	public GameObject DropDrezzStone(GameObject effectedRacer) {
		GameObject go = (GameObject) Instantiate(DrezzStonePrefab, effectedRacer.transform.position - GetComponent<TTSRacer>().displayMeshComponent.forward * 2.0f, effectedRacer.transform.rotation);
		go.GetComponent<TTSDrezzStone>().offensiveMultiplier = effectedRacer.GetComponent<TTSRacer>().Offense;
		go.rigidbody.AddForce(Random.insideUnitCircle * 50f);
		return go;
	}
	
	public GameObject FireEntropyCannon(GameObject effectedRacer) {
		GameObject go = (GameObject) Instantiate(EntropyCannonPrefab);
		go.GetComponent<TTSEntropyCannonProjectile>().offensiveMultiplier = effectedRacer.GetComponent<TTSRacer>().Offense;
		go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
		go.transform.position = effectedRacer.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = effectedRacer.rigidbody.velocity.normalized * (effectedRacer.rigidbody.velocity.magnitude + go.GetComponent<TTSEntropyCannonProjectile>().ProjectileStartVelocity);
		return go;
	}
	
	public GameObject FireHelix(GameObject effectedRacer) {
		GameObject go = (GameObject) Instantiate(HelixPrefab);
		go.GetComponent<TTSHelixProjectile>().currentRacer = effectedRacer.GetComponent<TTSRacer>();
		go.GetComponent<TTSHelixProjectile>().offensiveMultiplier = effectedRacer.GetComponent<TTSRacer>().Offense;
		go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
		go.transform.position = effectedRacer.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
		//go.rigidbody.velocity = effectedRacer.rigidbody.velocity.normalized * (effectedRacer.rigidbody.velocity.magnitude + go.GetComponent<TTSHelixProjectile>().ProjectileStartVelocity);
		go.rigidbody.velocity = effectedRacer.rigidbody.velocity;
		return go;
	}
	
	public GameObject GiveTimeBonus(GameObject effectedRacer) {
		level.time.GiveTimeBonus(1.0f);
		GameObject go = (GameObject) Instantiate(TimeBonusPrefab, effectedRacer.transform.position, effectedRacer.transform.rotation);
		go.GetComponent<TTSTimeBonusPrefab>().target = effectedRacer.GetComponent<TTSRacer>().displayMeshComponent.gameObject;
		return go;
	}
	
	public GameObject DeployShockwave(float _power, float _startSize, GameObject effectedRacer){
		GameObject go = (GameObject) Instantiate(ShockwavePrefab, effectedRacer.transform.position, Quaternion.identity);
		go.GetComponent<ParticleSystem>().startSize = _startSize;
		go.GetComponent<TTSShockwave>().power = go.GetComponent<TTSShockwave>().power * _power * effectedRacer.GetComponent<TTSRacer>().Offense;
		go.GetComponent<TTSShockwave>().radius = go.GetComponent<TTSShockwave>().radius * _power * effectedRacer.GetComponent<TTSRacer>().Offense;
		go.GetComponent<TTSShockwave>().upwardsForce = go.GetComponent<TTSShockwave>().upwardsForce * _power * effectedRacer.GetComponent<TTSRacer>().Offense;
		go.GetComponent<TTSShockwave>().Activate(this.gameObject);
		go.transform.parent = this.transform;
		return go;
	}
	
	public GameObject DeployLeech(GameObject effectedRacer) {
		GameObject go = (GameObject) Instantiate(LeechPrefab, effectedRacer.transform.position, Quaternion.identity);
		go.GetComponent<TTSLeech>().currentRacer = effectedRacer.GetComponent<TTSRacer>();
		go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
		go.transform.position = effectedRacer.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = effectedRacer.rigidbody.velocity;
		return go;
	}
	#endregion
	
}
