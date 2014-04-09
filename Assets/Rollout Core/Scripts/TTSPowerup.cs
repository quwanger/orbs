using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
	using XInputDotNetPure;
#endif

public class TTSPowerup : TTSBehaviour
{

	public bool debug = false;
	public int DebugTier = 1;

	public PowerupType AvailablePowerup;
	public PowerupType PreviouslyAvailablePowerup = PowerupType.None;
	public PowerupType ActivePowerup;
	public int tier = 0;

	public GameObject hudPowerup;

	public string consumeButton;
	public string powerupShootBackwardsButton;

	public int ammo = 0;
	
	public GameObject myCamera{
		get {
			return GetComponent<TTSRacer>().myCamera;
		}
	}
	public TTSCameraEffects vfx{
		get {
			return GetComponent<TTSRacer>().vfx;
		}
	}

	//Perk Stuff
	public PowerupType pp2;
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
	public GameObject HelixPrefabTier3;
	#endregion

	#region Tier3 Specialty Variables
	public int numberOfEntropyCannonsAvailable = 0;
	public int numberOfHelixCannonsAvailable = 0;
	#endregion

	// Powerup Launch Positions
	public List<Transform> ppFront;
	public List<Transform> ppBack;

	//XInput
	#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		GamePadState state;
		PlayerIndex playerIndex;
	#endif

	void Start() {
		ppFront = this.GetComponent<TTSRacer>().CurrentRig.powerupPositionsFront;
		ppBack = this.GetComponent<TTSRacer>().CurrentRig.powerupPositionsBack;

		pp2 = this.GetComponent<TTSPerkManager>().equiptPerkPool2;

		#if UNITY_STANDALONE_WIN || UNITY_EDITOR
			CheckControllerWindows();
		#endif
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
		if (AvailablePowerup != PowerupType.None) {
			#if UNITY_STANDALONE_WIN || UNITY_EDITOR

				CheckControllerWindows();

				if (state.Buttons.A == ButtonState.Pressed){
					ConsumePowerup();
				}
			#endif


			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR

				CheckControllerMac();

				if (Input.GetKeyDown(consumeButton)) 
					ConsumePowerup();
			#endif

			if (this.gameObject.GetComponent<TTSRacer>().playerNum == 1) {

				if (level.useKeyboard) {
					Debug.Log("Keyboard");
					if (Input.GetKeyDown("space")) {
						ConsumePowerup();
					}				
				}else{
					#if UNITY_STANDALONE_WIN
					if( state.Buttons.A == ButtonState.Pressed) {
						ConsumePowerup();
					}
					#endif
				}
			}
		}

		//changes the powerup in the hud when a new powerup is picked up
		if (AvailablePowerup != PreviouslyAvailablePowerup && this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)
			hudPowerup.GetComponent<TTSHudPowerup>().UpdateHudPowerup(AvailablePowerup, tier);

		PreviouslyAvailablePowerup = AvailablePowerup;
	}
	#endregion

	#region donation methods
	public void GivePowerup(PowerupType powerup) {
		if (powerup == AvailablePowerup) {
			if (tier < 3) {
				tier++;
			}
			//deal with tier 3 entropy
			if(powerup == PowerupType.EntropyCannon && tier == 3){
				numberOfEntropyCannonsAvailable = 10;
			}
		} else if(powerup == PowerupType.TimeBonus) {
			if(AvailablePowerup == TTSBehaviour.PowerupType.None)
				tier = 0;
			GiveTimeBonus(true);
		}
		else {
			ammo = 0;
			AvailablePowerup = powerup;
			if (pp2 == AvailablePowerup) {
				tier = 2;
			}
			else if (pp2 == PowerupType.Lottery) {
				//deals with the lottery perk
				float temp = Random.Range(0, 1.0f);
				if (temp <= lotteryChance) {
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

		if(powerup == PowerupType.Helix){
			switch(tier){
			case 1:
				numberOfHelixCannonsAvailable = 1;
				ammo = 1;
				break;
			case 2:
				numberOfHelixCannonsAvailable = 3;
				ammo = 3;
				break;
			case 3:
				numberOfHelixCannonsAvailable = 1;
				ammo = 1;
				break;
			default:
				break;
			}
		}else if(powerup == PowerupType.DrezzStones){
			switch(tier){
			case 1:
				ammo = 1;
				break;
			case 2:
				ammo = 3;
				break;
			case 3:
				ammo = 6;
				break;
			default:
				break;
			}
		}else if(powerup == PowerupType.EntropyCannon){
			switch(tier){
			case 1:
				ammo = 3;
				break;
			case 2:
				ammo = 8;
				break;
			case 3:
				ammo = 10;
				break;
			default:
				break;
			}
		}else if(powerup == PowerupType.Leech){
			switch(tier){
			case 1:
				ammo = 2;
				break;
			case 2:
				ammo = 5;
				break;
			case 3:
				ammo = 10;
				break;
			default:
				break;
			}
		}
		//only update the hud if they are a human racer (as AI do not have HUDs)
		if (this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)
			hudPowerup.GetComponent<TTSHudPowerup>().UpdateHudPowerup(AvailablePowerup, tier);
	}

	public void ConsumePowerup() {

		switch (AvailablePowerup) {
			case PowerupType.EntropyCannon:
				EntropyCannon(tier);
				break;

			case PowerupType.DrezzStones:
				DrezzStone(tier);
				break;

			case PowerupType.SuperC:
				SuperCBooster(tier, true);
				break;

			case PowerupType.Shield:
				Shield(tier);
				break;

			case PowerupType.Shockwave:
				DeployShockwave(tier, true);
				break;

			case PowerupType.Leech:
				Leech(tier);
				break;

			case PowerupType.Helix:
				Helix(tier);
				break;

			default:
				//Play a sound?
				break;
		}

		this.ActivePowerup = this.AvailablePowerup;

		if(this.AvailablePowerup == PowerupType.EntropyCannon && tier == 3){
			if(numberOfEntropyCannonsAvailable <= 0){
				this.AvailablePowerup = PowerupType.None;
				this.tier = 0;
			}
		}else if(this.AvailablePowerup == PowerupType.Helix){
			if(numberOfHelixCannonsAvailable <= 0){
				this.AvailablePowerup = PowerupType.None;
				this.tier = 0;
			}
		}else{
			this.AvailablePowerup = PowerupType.None;
			this.tier = 0;
			ammo = 0;
		}
		
		if(this.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)
			hudPowerup.GetComponent<TTSHudPowerup>().UpdateHudPowerup(this.AvailablePowerup, this.tier);
	}

	#endregion


	#region public methods
	public void CheckControllerWindows(){
		#if UNITY_STANDALONE_WIN || UNITY_EDITOR
			switch(this.GetComponent<TTSRacer>().playerControllerNum){
				case(1):
					state = GamePad.GetState(PlayerIndex.One);
					break;
				case(2):
					state = GamePad.GetState(PlayerIndex.Two);
					break;
				case(3):
					state = GamePad.GetState(PlayerIndex.Three);
					break;
				case(4):
					state = GamePad.GetState(PlayerIndex.Four);
					break;
				default:
					break;
			}
		#endif
	}

	public void CheckControllerMac(){
		#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			switch(this.GetComponent<TTSRacer>().playerControllerNum){
				case(1):
					consumeButton = "joystick 1 button 16";
					break;
				case(2):
					consumeButton = "joystick 2 button 16";
					break;
				case(3):
					consumeButton = "joystick 3 button 16";
					break;
				case(4):
					consumeButton = "joystick 4 button 16";
					break;
				default:
					break;
			}
		#endif
	}

	public void CheckControllerMacAnalog(){
		#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			switch(this.GetComponent<TTSRacer>().playerControllerNum){
				case(1):
					powerupShootBackwardsButton = "R_YAxis_1";
					break;
				case(2):
					powerupShootBackwardsButton = "R_YAxis_2";
					break;
				case(3):
					powerupShootBackwardsButton = "R_YAxis_3";
					break;
				case(4):
					powerupShootBackwardsButton = "R_YAxis_4";
					break;
				default:
					break;
			}
		#endif
	}

	public void Leech(int _tier) {

		//int leechesPerRacer = Mathf.CeilToInt(ammo/(this.gameObject.GetComponent<TTSRacer>().place-1));

		for (int i = 0; i < ammo; i++) {
			DeployLeech(true);
		}
	}

	public void DrezzStone(int _tier) {
		for (int i = 0; i < ammo; i++) {
			Invoke("DrezzMid", i * 0.3f);
		}
	
		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.PowerupType.None;
	}

	public void EntropyCannon(int _tier) {
		if (_tier == 1) {
			for (int i = 0; i < ammo; i++) {
				Invoke("EntropyMid", i * 0.075f);
			}
		}

		if (_tier == 2) {
			for (int i = 0; i < ammo; i++) {
				Invoke("EntropyMid", i * 0.075f);
			}
		}

		if(_tier == 3){
			EntropyMid();
			numberOfEntropyCannonsAvailable--;
			ammo--;
		}

		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.PowerupType.None;
	}

	public void Helix(int _tier) {

		if(_tier == 1 || _tier == 2 || _tier == 3) {
			HelixMid(_tier);
			numberOfHelixCannonsAvailable--;
			ammo--;
		}

		//it is only active while firing
		this.ActivePowerup = TTSBehaviour.PowerupType.None;
	}

	#region timed handler functions
	private void DrezzMid() {
		DropDrezzStone(true);
	}

	private void EntropyMid() {
		FireEntropyCannon(true);
	}

	private void HelixMid(int t) {
		if(t==3){
			FireHelixTier3(true);
		}else{	
			FireHelix(true);
		}
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
			if (owner && myCamera != null) vfx.BoostEffect(1.0f);
		}
		if (tier == 2.0f) {
			boost.duration = 1.5f * this.GetComponent<TTSRacer>().Offense;
			boost.TargetForce = 90.0f * this.GetComponent<TTSRacer>().Offense;
			if (owner && myCamera != null) vfx.BoostEffect(1.0f);
		}
		if (tier == 3.0f) {
			boost.duration = 3.0f * this.GetComponent<TTSRacer>().Offense;
			boost.TargetForce = 100.0f * this.GetComponent<TTSRacer>().Offense;
			boost.isTier3 = true;
			if (owner && myCamera != null) vfx.BoostEffect(1.0f);
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

	public Transform GetFrontPP(){
		Transform t = ppFront[Random.Range(0, ppFront.Count)];
		return t;
	}

	public Transform GetBackPP(){
		Transform t = ppBack[Random.Range(0, ppBack.Count)];
		return t;
	}

	public Transform GetSpecificBackPP(int gunNumber){
		
		Transform t;

		if(ppBack.Count<2)
			t = ppBack[0];
		else
			t = ppBack[gunNumber];
		return t;
	}

	public GameObject DropDrezzStone(bool owner, TTSPowerupNetHandler handle) {
		//GameObject go = (GameObject)Instantiate(DrezzStonePrefab, GetBackPP().position - GetComponent<TTSRacer>().displayMeshComponent.forward * 2.0f, this.gameObject.transform.rotation);
		
		GameObject go;

		if(CheckForwardAnalog()){
			go = (GameObject)Instantiate(DrezzStonePrefab, GetBackPP().position, this.gameObject.transform.rotation);
			go.GetComponent<TTSDrezzStone>().offensiveMultiplier = this.gameObject.GetComponent<TTSRacer>().Offense;
			go.rigidbody.AddForce(Random.insideUnitCircle * 50f);
		}else{
			go = (GameObject)Instantiate(DrezzStonePrefab, GetFrontPP().position, this.gameObject.transform.rotation);
			go.GetComponent<TTSDrezzStone>().offensiveMultiplier = this.gameObject.GetComponent<TTSRacer>().Offense;
			go.rigidbody.velocity = this.rigidbody.velocity * 1.5f;
		}

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

		if(CheckForwardAnalog()){
			//shoot forward
			go.transform.position = GetFrontPP().position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
			go.GetComponent<TTSEntropyCannonProjectile>().ProjectileDirectionVector = this.rigidbody.velocity.normalized;
			go.GetComponent<TTSEntropyCannonProjectile>().ProjectileStartVelocity = Random.Range(this.gameObject.rigidbody.velocity.magnitude + 50.0f, this.gameObject.rigidbody.velocity.magnitude + 150.0f);
			//go.rigidbody.velocity = this.rigidbody.velocity.normalized * Random.Range(this.gameObject.rigidbody.velocity.magnitude + 50.0f, this.gameObject.rigidbody.velocity.magnitude + 150.0f);
		}else{
			//shoot backwards
			go.transform.position = GetBackPP().position + GetComponent<TTSRacer>().displayMeshComponent.forward * -3.5f;
			go.GetComponent<TTSEntropyCannonProjectile>().ProjectileDirectionVector = this.rigidbody.velocity.normalized * -1.0f;
			go.GetComponent<TTSEntropyCannonProjectile>().ProjectileStartVelocity = Random.Range(this.gameObject.rigidbody.velocity.magnitude + 50.0f, this.gameObject.rigidbody.velocity.magnitude + 150.0f);
			//go.rigidbody.velocity = this.rigidbody.velocity.normalized * Random.Range(rigidbody.velocity.magnitude + 50.0f, rigidbody.velocity.magnitude + 150.0f) * -1.0f;
		}


		if (owner) { SendPowerupDeploy(TTSPowerupNetworkTypes.Entropy, go); }
		else {
			go.GetComponent<TTSEntropyCannonProjectile>().SetNetHandler(handle);
		}

		return go;
	}

	public GameObject FireHelix(bool owner) {
		return FireHelix(owner, null);
	}

	public GameObject FireHelixTier3(bool owner) {
		return FireHelixTier3(owner, null);
	}

	public GameObject FireHelix(bool owner, TTSPowerupNetHandler handle) {

		GameObject go = (GameObject)Instantiate(HelixPrefab);

		TTSHelixProjectile helix = go.GetComponent<TTSHelixProjectile>();
		TTSRacer racer = GetComponent<TTSRacer>();

		helix.offensiveMultiplier = racer.Offense;
		helix.currentRacer = racer;
		helix.racersInFront = (racer.place - 1);

		go.transform.rotation = racer.displayMeshComponent.transform.rotation;
		go.transform.position = GetFrontPP().position + racer.displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = racer.rigidbody.velocity + (racer.rigidbody.velocity.normalized * 100.0f);

		if (owner) { SendPowerupDeploy(TTSPowerupNetworkTypes.Helix, go); }
		else {
			helix.SetNetHandler(handle);
		}
		return go;
	}

	public GameObject FireHelixTier3(bool owner, TTSPowerupNetHandler handle) {
		
		GameObject go = (GameObject)Instantiate(HelixPrefabTier3);

		TTSHelixProjectileTier3 helix = go.GetComponent<TTSHelixProjectileTier3>();
		TTSRacer racer = GetComponent<TTSRacer>();

		helix.offensiveMultiplier = racer.Offense;
		helix.currentRacer = racer;
		helix.racersInFront = (racer.place - 1);

		go.transform.rotation = racer.displayMeshComponent.transform.rotation;
		go.transform.position = GetFrontPP().position + racer.displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = racer.rigidbody.velocity + (racer.rigidbody.velocity.normalized * 100.0f);

		if (owner) { SendPowerupDeploy(TTSPowerupNetworkTypes.Helix, go); }
		else {
			helix.SetNetHandler(handle);
		}
		return go;
	}

	public GameObject GiveTimeBonus(bool owner) {
		level.time.GiveTimeBonus(1.0f);
		GameObject go = (GameObject)Instantiate(TimeBonusPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
		go.GetComponent<TTSTimeBonusPrefab>().target = this.gameObject.GetComponent<TTSRacer>().displayMeshComponent.gameObject;

		if (owner) { SendStaticPowerupDeploy(TTSPowerupNetworkTypes.TimeBonus, 0.0f); }
		return go;
	}

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

		GameObject go = (GameObject) Instantiate(ShockwavePrefab, transform.position, Quaternion.identity);
		go.GetComponent<ParticleSystem>().startSize = startSize;
		go.GetComponent<TTSShockwave>().power = go.GetComponent<TTSShockwave>().power * 2.0f * GetComponent<TTSRacer>().Offense;
		go.GetComponent<TTSShockwave>().radius = go.GetComponent<TTSShockwave>().radius * power * GetComponent<TTSRacer>().Offense;
		//go.GetComponent<TTSShockwave>().upwardsForce = go.GetComponent<TTSShockwave>().upwardsForce * power * GetComponent<TTSRacer>().Offense;
		go.GetComponent<TTSShockwave>().upwardsForce = 0;
		go.GetComponent<TTSShockwave>().Activate(this.gameObject);
		go.transform.parent = this.transform;

		if (owner) { SendStaticPowerupDeploy(TTSPowerupNetworkTypes.Shockwave, tier); }
		return go;
	}

	public GameObject DeployLeech(bool owner) {
		return DeployLeech(owner, null);
	}

	public GameObject DeployLeech(bool owner, TTSPowerupNetHandler handle) {
		GameObject go = (GameObject)Instantiate(LeechPrefab, GetFrontPP().position, Quaternion.identity);
		go.GetComponent<TTSLeech>().currentRacer = this.gameObject;
		go.GetComponent<TTSLeech>().leechesInPack = ammo;
		go.GetComponent<TTSLeech>().racersAheadOfLeechOwner = this.gameObject.GetComponent<TTSRacer>().place - 1;
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
	
	public bool CheckForwardAnalog(){

		#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		CheckControllerWindows();
		/*if(state.ThumbSticks.Left.Y == -1.0f)
			return false;*/
		if(state.Buttons.LeftShoulder == 0)
			return false;

		Debug.Log(state.Buttons.LeftShoulder);
		#endif

		#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			CheckControllerMacAnalog();
			if(Input.GetAxis(powerupShootBackwardsButton) == -1.0f) {
				return false;
			}
		#endif

		if(this.gameObject.GetComponent<TTSRacer>().playerNum == 1) {
			if(level.useKeyboard){
				if(level.DebugMode){
					if(Input.GetKey (KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
						return false;
					}
				}else{
					//do controller stuff that I can't remember right now
				}
			}
		}

		return true;
	}

}

public class TTSPowerupNetworkTypes
{
	// Static
	public const int TimeBonus = 1;
	public const int Shockwave = 2;
	public const int Shield = 3;
	public const int Boost = 4;

	// Updated
	public const int Leech = 5;
	public const int Drezz = 6;
	public const int Entropy = 7;
	public const int Helix = 8;

	public static bool isStaticType(int type) {
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
		type = "Static Powerup";
		// For static powerups only. Class is used only for storage.
	}

	public TTSPowerupNetHandler(TTSClient Client, bool Owner, int PowerupType, float racerID) {
		type = "Powerup";
		Type = PowerupType;
		owner = Owner;
		RacerID = racerID;
		registerCommand = TTSCommandTypes.PowerupRegister;
		client = Client;
		inGameRegistration = true;
		client.LocalObjectRegister(this);
	}

	public TTSPowerupNetHandler(TTSClient Client, bool Owner, float ID, int PowerupType, float racerID) { // For multiplayer
		type = "Powerup";
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
