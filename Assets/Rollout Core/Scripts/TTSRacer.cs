using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***********************
 * Racer.cs - Racer Prefab Builder and Motion Handler
 * 
 * This script has two primary roles, the first is to handle the connection (and instansiation [TODO])
 * of all the Objects embedded in the racer prefab. It also handles the motion of the racer, weather
 * it is player controlled or AI.
 * 
 * The main components of the prefab are the physics controller, which is an invisible rigidbody with
 * roughly the same size of the mesh. It is responsible for handling the simulation of physics on the 
 * character and provide realistic movement. It also is where the forces should be applied to to invoke
 * the characters movement. 
 * 
 * The body is the displayed mesh for the character, it simply needs to update its transform to match what
 * movment patterns we decide on, making movement much more realistic and predictable. It also needs to update 
 * It's position to match the rigidbodies.
 * 
 * The lights controlled contains the spotlights responsible for creating the light patterns on the ground.
 * It basically needs to update to match the transform of the body.
 * 
 * The current implementation relies on a vector called RealForward, it is the current desired direction of 
 * motion, the FloorNormal is the normal of the floor under the object. It should update and change the 'y'  
 * direction of the realforward vector.
 * 
 * The rest will be explained inline.
 * 
 * 
 * */

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TTSRacer : TTSBehaviour
{
	public GameObject minimapIconSmall;
	public GameObject minimapIconBig;

	#region serialized settings
	public bool IsPlayerControlled = true;
	#endregion

	#region Internal Components
	public Transform displayMeshComponent;
	private AudioSource RacerSounds;
	private AudioSource RacerSfx;
	public AudioClip RollingSound;
	#endregion


	#region Internal Movement Operators
	private Vector3 IdleForwardVector;
	private Vector3 PreviousVelocity = new Vector3(1, 0, 0);
	public bool onGround = true;
	private float TiltRecoverySpeed = 0.1f;
	private float TiltAngle = 0.0f;
	public AudioClip[] DamageSounds;
	public GameObject SparksEmitter;
	public TTSRig CurrentRig;
	public bool canMove = false;
	private float MinimumVelocityToAnimateSteering = 1.0f;

	private float resultAccel = 0.0f; // For sound calculation
	public float rpm = 0;
	private float vInput = 0.0f, hInput = 0.0f;
	private Vector3 lastForward;

	public Vector3 position{
		get{
			return transform.position;
		}
	}
	
	public bool calcOrientation = true;
	#endregion

	#region gameplay vars
	public float TopSpeed = 250.0f;
	public float Acceleration = 8000.0f;
	public float Handling = 11000.0f;
	
	public enum PlayerType { Player, AI, Multiplayer };
	public PlayerType player = PlayerType.Player;
	
	public float Defense;
	public float Offense;

	public bool finished = false;
	public float distanceToFinish;
	public float previousDistanceToFinish;
	public int place;
	
	public float respawnTime = 2.0f;
	public Vector3 respawnPoint;
	public Quaternion respawnRotation;
	
	public int playerNum;
	
	public int numHelix = 0;
	#endregion
	
	public bool hasShield;
	private float smooth;
	private float stopSpeed = 0.05f;
	
	public GameObject myCamera;

	private TTSPowerup powerupManager;

	#region Direction/Wrong way
	public TTSWaypoint currentWaypoint;
	public TTSWaypoint previousWaypoint;
	public TTSWaypoint debugStart;
	public bool goingWrongWay = false;
	#endregion

	// AI
	private TTSWaypoint lastWaypoint;
	public TTSWaypoint nextWaypoint;
	TTSAIController AIUtil;

	// Networking
	TTSRacerNetHandler netHandler;
	public int rigID;

	public struct RacerConfig
	{
		public float netID;
		public int Index;
		public int RigType;
		public int Perk1;
		public int Perk2;
		public string Name;

		/// <summary>
		/// Can either be Player, AI, or Multiplayer
		/// </summary>
		public int LocalControlType;

		/// <summary>
		/// Can be either Player or AI
		/// </summary>
		public int ControlType;

		public int CharacterType;
	}

	void Awake() {
		level.RegisterRacer(gameObject);
	}

	void Start() {
		//Get the body via tag.
		foreach (Transform child in transform) {
			if (child.gameObject.tag == "RacerDisplayMesh") {
				displayMeshComponent = child;
			}
		}

		//lastForward = TTSUtils.FlattenVector(displayMeshComponent.forward).normalized;

		if (!level.PerksEnabled) {
			this.GetComponent<TTSPerkManager>().enabled = false;
		}

		if (displayMeshComponent == null) {
			//Debug.LogException(new UnassignedReferenceException());
		}
		IdleForwardVector = transform.forward;


		//Setup Audio Channel for Pitched Racer Sounds
		RacerSounds = gameObject.AddComponent<AudioSource>();
		RacerSounds.clip = RollingSound;
		RacerSounds.volume = 0.5f;
		RacerSounds.loop = true;
		RacerSounds.rolloffMode = AudioRolloffMode.Linear;
		RacerSounds.Play();

		//Setup Audio Channel for SFX
		RacerSfx = gameObject.AddComponent<AudioSource>();
		RacerSfx.rolloffMode = AudioRolloffMode.Linear;
		RacerSfx.volume = 0.5f;

		//Apply Attributes
		TopSpeed = TopSpeed + (speedIncrease * CurrentRig.rigSpeed);
		Acceleration = Acceleration + (accelerationIncrease * CurrentRig.rigAcceleration);
		Handling = Handling + (handlingIncrease * CurrentRig.rigHandling);
		
		Offense = CurrentRig.rigOffense;
		Defense = CurrentRig.rigDefense;
		
		if(myCamera != null)
			myCamera.GetComponent<TTSCameraFade>().SetScreenOverlayColor(new Color(0,0,0,0));
	}

	// Runs after the racer is initialized with the rigs
	public void Initialized() {

		powerupManager = GetComponent<TTSPowerup>();

		if (player == PlayerType.Player) {
			SetNetHandler(new TTSRacerNetHandler(level.client, true, rigID));
		}
		else if (player == PlayerType.AI) {
			AIUtil = gameObject.AddComponent<TTSAIController>();
			SetNetHandler(new TTSRacerNetHandler(level.client, true, rigID));
		}
		else if (player == PlayerType.Multiplayer) {

		}

		if (AIUtil == null)
			AIUtil = gameObject.AddComponent<TTSAIController>();
	}
	
	void FixedUpdate () {
		if(!level.raceHasFinished){
			if(IsPlayerControlled){ 
				CalculateInputForces();
			}
		}else{
			SlowToStop();
		}

		if (player != PlayerType.Multiplayer)
			CalculateBodyOrientation();

		netHandler.UpdateRacer(position, displayMeshComponent.rotation.eulerAngles, rigidbody.velocity, vInput, hInput);

		resultAccel = Mathf.Lerp(resultAccel, rigidbody.velocity.magnitude - PreviousVelocity.magnitude, 0.01f);
		PreviousVelocity = rigidbody.velocity;
	}



	private void CalculateInputForces() {
		if (finished && !level.DebugMode) // No input when race is finished
			return;

		if (player == PlayerType.Player) {
			if (playerNum == 1) {
				if(level.useKeyboard){
					vInput = Input.GetAxis ("Key_YAxis");
					hInput = Input.GetAxis ("Key_XAxis");
				}else{
					vInput = Input.GetAxis("TriggersR_1");
					hInput = Input.GetAxis("L_XAxis_1");
				}
			} else if (playerNum == 2) {
				vInput = Input.GetAxis("TriggersR_2");
				hInput = Input.GetAxis("L_XAxis_2");
			} else if (playerNum == 3) {
				vInput = Input.GetAxis("TriggersR_3");
				hInput = Input.GetAxis("L_XAxis_3");
			} else if (playerNum == 4) {
				vInput = Input.GetAxis("TriggersR_4");
				hInput = Input.GetAxis("L_XAxis_4");
			}
		}
		else if (player == PlayerType.Multiplayer) {
			MultiplayerInput();
		}
		else if (player == PlayerType.AI) {
			AIInput();
		}
		else {
			Debug.LogError("PLAYER TYPE NOT SET");
		}

		#region Vertical Input
		if (onGround && rigidbody.velocity.magnitude < TopSpeed && canMove) {
			rigidbody.AddForce(displayMeshComponent.forward * vInput * Time.deltaTime * Acceleration);

			if (Mathf.Abs(rpm) > 15.0f) {
				rigidbody.AddForce(TTSUtils.FlattenVector(lastForward * rpm / 20.0f * Time.deltaTime * Acceleration));
				RacerSounds.pitch -= 0.05f;
				rpm *= 0.8f;
			}
			else rpm = 0.0f;
		}
		else if (!onGround) {
			float gravity = -30;
			rigidbody.AddForce(displayMeshComponent.up * gravity);

			if (Mathf.Abs(vInput) > 0.1f) {
				rpm = Mathf.Clamp(rpm +
					TTSUtils.Remap(rpm, -100.0f, 100.0f, 1.0f, 0, true) * vInput + ((vInput > 0) ? 0 : -1),
					-100.0f, 100.0f);
			}
			else
				rpm *= 0.99f;
		}
		#endregion

		// Horizontal Input
		if (canMove) {
			rigidbody.AddForce(displayMeshComponent.right * hInput * Time.deltaTime * Handling);
		}
	}

	public void DamageRacer(float dmgLevel){
		//dmgLevel should be a percentage (between 0.0f and 1.0f)
		if(!hasShield){
			if(Defense > dmgLevel){
				if((Defense - dmgLevel) < 1.0f){
					dmgLevel = Defense - dmgLevel;
				}else{
					dmgLevel = 1.0f;
				}
			}else{
				dmgLevel = 0.0f;
			}
			Vector3 damageVector = new Vector3(rigidbody.velocity.x * dmgLevel, rigidbody.velocity.y * dmgLevel, rigidbody.velocity.z * dmgLevel);
			rigidbody.velocity = damageVector;
		}
	}

	void CalculateBodyOrientation() {
		if(!calcOrientation)
			return;
		
		//Facing Direction...
		if (new Vector2(rigidbody.velocity.x, rigidbody.velocity.z).magnitude > MinimumVelocityToAnimateSteering) {
			//based on rigidbody velocity.
			displayMeshComponent.forward = rigidbody.velocity;

			TiltAngle = Mathf.Lerp(TiltAngle, TTSUtils.GetRelativeAngle(rigidbody.velocity, PreviousVelocity) / 2, TiltRecoverySpeed);

			displayMeshComponent.RotateAround(displayMeshComponent.forward, TiltAngle);
			//set the idle vec, so it doesnt get janky.
			IdleForwardVector = displayMeshComponent.forward;
		}
		else {
			displayMeshComponent.forward = IdleForwardVector;
		}

		//Debug.Log(TTSUtils.FlattenVector(displayMeshComponent.forward).magnitude);
		if (TTSUtils.FlattenVector(displayMeshComponent.forward).magnitude > 0.2f) {
			lastForward = TTSUtils.FlattenVector(displayMeshComponent.forward).normalized;
		}
	}
	
	public void ManualOrientation(Vector3 faceTowards){
		displayMeshComponent.forward = faceTowards - position;
		transform.LookAt(faceTowards);
	}
	
	public float GetTiltAngle() {
		return TiltAngle;
	}

	void LateUpdate() {
		int offset = (resultAccel <= 0) ? -10 : 15;

		if (onGround) {
			RacerSounds.pitch = Mathf.Max(Mathf.Lerp(RacerSounds.pitch, TTSUtils.Remap(rigidbody.velocity.magnitude + offset, 0f, 100.0f, 0.5f, 1.0f, false), 0.1f), 0);
			RacerSounds.volume = Mathf.Max(Mathf.Lerp(RacerSounds.volume, TTSUtils.Remap(rigidbody.velocity.magnitude + offset, 0f, 100.0f, 0.5f, 1f, false), 0.1f) * 1.5f, 0); // Needs Cleaning
		}
		else {
			RacerSounds.pitch = Mathf.Max(Mathf.Lerp(RacerSounds.pitch, TTSUtils.Remap(Mathf.Abs(vInput), 0.0f, 1.0f, 0.5f, 1.0f, false), 0.1f), 0);
			RacerSounds.volume = Mathf.Max(Mathf.Lerp(RacerSounds.volume, TTSUtils.Remap(Mathf.Abs(vInput), 0.0f, 1.0f, 0.5f, 1.0f, false) * 1.5f, 0), 0); // Needs cleaning
		}
	}

	void Update() {
		minimapIconSmall.transform.position = new Vector3(this.gameObject.transform.position.x, minimapIconSmall.transform.position.y, this.gameObject.transform.position.z);
		minimapIconBig.transform.position = new Vector3(this.gameObject.transform.position.x, minimapIconBig.transform.position.y, this.gameObject.transform.position.z);
	
		if(nextWaypoint)
			goingWrongWay = CheckWrongWay();
	}

	#region Events
	void OnCollisionEnter(Collision collision) {

		onGround = true;
		if (collision.relativeVelocity.magnitude > 10) {
			vfx.DamageEffect(100.0f);
			//RacerSfx.volume = collision.relativeVelocity.magnitude / TopSpeed / 1.5f;
			RacerSfx.volume = collision.relativeVelocity.magnitude / 100.0f / 1.5f;
			RacerSfx.PlayOneShot(DamageSounds[Mathf.FloorToInt(Random.value * DamageSounds.Length)]);
		}

		//spawn sparks (TODO: move this to a component script)
		GameObject sparkClone = (GameObject)Instantiate(SparksEmitter);
		sparkClone.transform.position = collision.contacts[0].point;
		sparkClone.particleEmitter.emit = true;
		sparkClone.transform.forward = displayMeshComponent.forward;
	}

	void OnCollisionStay(Collision collision) {
		onGround = true;
	}

	void OnCollisionExit(Collision collision) {
		onGround = false;
	}

	private bool CheckWrongWay(){

		Vector3 toWaypoint = nextWaypoint.getClosestPoint(position) - position;

		float angle = Vector3.Angle(toWaypoint, lastForward);

		return (angle>110.0f);
	}

	public void WrongWay() {
		if (goingWrongWay == true){
			//going the wrong way
		}else{
			//going the right way
		} 
	}

	public void OnWaypoint(TTSWaypoint hit) {
		previousWaypoint = currentWaypoint;
		currentWaypoint = hit;

		/*if (previousWaypoint == currentWaypoint) {
			if (goingWrongWay == true) {
				goingWrongWay = false;
				WrongWay();
			}
			else {
				goingWrongWay = true;
				WrongWay();
			}
		}*/
		

		if(AIUtil == null)
			AIUtil = gameObject.AddComponent<TTSAIController>();	
		
		//this must be done for the player as well so that we can get the distance of all racers from the finish line
		nextWaypoint = AIUtil.getClosestWaypoint(currentWaypoint.nextWaypoints, position);
		
	}

	public void SlowToStop() {
		rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, new Vector3(0, 0, 0), stopSpeed);
	}

	public void SlowToStopToPosition(GameObject pos)
    {
		rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, new Vector3(0, 0, 0), stopSpeed);
    	StartCoroutine(pause(pos));
    }
	
	// the pause for SlowToStopToPosition
	IEnumerator pause(GameObject pos)
    {
    	yield return new WaitForSeconds(2.0f);
		this.transform.position = new Vector3(pos.transform.position.x, this.transform.position.y, pos.transform.position.z);
    }

	public void StopRacer() {
		rigidbody.velocity = new Vector3(0, 0, 0);
	}
	
	public void DelayedRespawn() {
		if(myCamera != null)
			myCamera.GetComponent<TTSCameraFade>().StartFade(new Color(0,0,0,1.0f), respawnTime);
		
		Invoke("DelayedRespawnPart2", respawnTime);
	}
	
	private void DelayedRespawnPart2(){
		StopRacer();
		canMove = false;
		this.transform.rotation = respawnRotation;
		this.transform.position = respawnPoint;
		Invoke("AllowPlayerControl", respawnTime);
		if(myCamera != null)
			myCamera.GetComponent<TTSCameraFade>().StartFade(new Color(0,0,0,0), respawnTime);
	}
	
	private void AllowPlayerControl(){
		if(!IsPlayerControlled)
			IsPlayerControlled = true;
		if(!canMove)
			canMove = true;
	}
	
	#endregion

	public void OnDrawGizmos() {
		bool drawMovement = true;

		if (drawMovement) {
			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position, IdleForwardVector * 10);

			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(transform.position, displayMeshComponent.forward * 10);

			Gizmos.color = Color.magenta;
			Gizmos.DrawRay(transform.position, rigidbody.velocity * 5);

			Gizmos.color = Color.cyan;
			Gizmos.DrawRay(transform.position, lastForward * 5);

			Gizmos.color = Color.green;
			Gizmos.DrawCube(destination, Vector3.one);

			Gizmos.color = Color.white;
			Gizmos.DrawCube(netHandler.netPosition, Vector3.one);
		}

		//if (AIControl != null)
		//	AIControl.drawGizmos();
	}

	Vector3 destination;
	public void AIInput(){

		float debugVInput = 0.0f, debugHInput = 0.0f;
		
		if(AIUtil.debugMode){
			debugVInput = Input.GetAxis("Vertical");
			debugHInput = Input.GetAxis("Horizontal");
		}

		if (currentWaypoint == null) {
			vInput = hInput = 0.0f;
			return;
		}

		// If there's no next waypoint
		if (nextWaypoint == null){
			if (!waypointManager.EndPoints.Contains(currentWaypoint))
				nextWaypoint = waypointManager.SpawnZone;
			else {
				finished = true;
				return;
			}
		}

		destination = AIUtil.getDestination(lastForward, nextWaypoint, position);

		Vector3 steerDir = TTSUtils.FlattenVector(destination - position);

		if (!level.DebugMode || level.racers.Length > 1 || (debugVInput == 0.0f && debugHInput == 0.0f)) {
			vInput = AIUtil.verticalInput(vInput, nextWaypoint, position, rigidbody.velocity);
			hInput = TTSUtils.Remap(TTSUtils.GetRelativeAngle(lastForward, steerDir) * 2, -90.0f, 90.0f, -1.0f, 1.0f, true);
		}

		Debug.DrawLine(position, destination);
	}

	public void SetNetHandler(TTSRacerNetHandler handler) {
		this.netHandler = handler;
		powerupManager.SetNetHandler(netHandler);
	}

	public float GetNetworkID() {
		return this.netHandler.id;
	}

	public void MultiplayerInput() {
		if (netHandler.isNetworkUpdated) {
			if (netHandler.netPosition != Vector3.zero) {
				transform.position = Vector3.Lerp(transform.position, netHandler.netPosition, netHandler.networkInterpolation);
			}
		}
		displayMeshComponent.rotation = Quaternion.Lerp(displayMeshComponent.rotation, Quaternion.Euler(netHandler.netRotation), netHandler.networkInterpolation * 10);
		rigidbody.velocity = netHandler.netSpeed;

		vInput = Mathf.Lerp(vInput, netHandler.networkVInput, netHandler.networkInterpolation * 5);
		hInput = Mathf.Lerp(hInput, netHandler.networkHInput, netHandler.networkInterpolation * 5);

		netHandler.isNetworkUpdated = false;

		// Powerups
		foreach (TTSPowerupNetHandler handler in netHandler.receivedPowerups) {
			switch (handler.Type) {
				case TTSPowerupNetworkTypes.Shield:
					powerupManager.DeployShield(handler.Tier, false);
					break;

				case TTSPowerupNetworkTypes.Boost:
					powerupManager.SuperCBooster(handler.Tier, false);
					break;

				case TTSPowerupNetworkTypes.Shockwave:
					powerupManager.DeployShockwave(handler.Tier, false);
					break;

				case TTSPowerupNetworkTypes.TimeBonus:
					powerupManager.GiveTimeBonus(false);
					break;

				case TTSPowerupNetworkTypes.Entropy:
					powerupManager.FireEntropyCannon(false, handler);
					break;

				case TTSPowerupNetworkTypes.Helix:
					powerupManager.FireHelix(false, handler);
					break;

				case TTSPowerupNetworkTypes.Drezz:
					powerupManager.DropDrezzStone(false, handler);
					break;

				case TTSPowerupNetworkTypes.Leech:
					powerupManager.DeployLeech(false, handler);
					break;
			}
		}
		netHandler.receivedPowerups.Clear();
	}
}

public class TTSRacerNetHandler : TTSNetworkHandle
{
	// Racer Configuration
	public int Index = 0;
	public int Rig = 3;
	public int Perk1 = 0;
	public int Perk2 = 0;
	public string Name = "Bob";
	public int ControlType = 0;

	public Vector3 position, rotation, speed;
	public float vInput, hInput;
	// public int powerUpType, powerUpTier;

	// Receivers (Read data from here)
	public Vector3 netPosition, netRotation, netSpeed;
	public float networkVInput, networkHInput;
	//public int networkPowerUpType, networkPowerUpTier;

	// Powerup
	public List<TTSPowerupNetHandler> receivedPowerups = new List<TTSPowerupNetHandler>();

	public TTSRacerNetHandler(TTSClient Client, bool Owner, int rigID) {
		registerCommand = TTSCommandTypes.RacerRegister;
		owner = Owner;
		client = Client;
		Rig = rigID;
		Client.LocalRacerRegister(this);
	}

	public TTSRacerNetHandler(TTSClient Client, bool Owner, float ID) { // For multiplayer players
		id = ID;
		registerCommand = TTSCommandTypes.RacerRegister;
		owner = Owner;
		client = Client;
		Client.LocalRacerRegister(this);
	}

	public override byte[] GetNetworkRegister() {
		writer.ClearData();
		writer.AddData(registerCommand);
		writer.AddData(id);
		writer.AddData(-1); // Index
		writer.AddData(Rig);
		writer.AddData(Perk1);
		writer.AddData(Perk2);
		writer.AddData(Name);
		writer.AddData(ControlType);
		return writer.GetMinimizedData();
	}

	// Command and ID already read in packet
	public override void ReceiveNetworkData(TTSPacketReader reader, int command) {
		if (command == TTSCommandTypes.RacerUpdate) {
			netPosition = reader.ReadVector3();
			netRotation = reader.ReadVector3();
			netSpeed = reader.ReadVector3();

			networkVInput = reader.ReadFloat();
			networkHInput = reader.ReadFloat();

			isNetworkUpdated = true;
		}
		else if (command == TTSCommandTypes.PowerupStaticRegister) {
			int powerupType = reader.ReadInt32();
			float powerupTier = reader.ReadFloat();
			TTSPowerupNetHandler handler = new TTSPowerupNetHandler();
			handler.Type = powerupType;
			handler.Tier = powerupTier;

			if (TTSPowerupNetworkTypes.isStaticType(powerupType)) {
				receivedPowerups.Add(handler);
			}
		}
		else if (command == TTSCommandTypes.PowerupRegister) {
			float powerupID = reader.ReadFloat();
			int powerupType = reader.ReadInt32();

			TTSPowerupNetHandler handler = new TTSPowerupNetHandler(client, false, powerupID, powerupType, id);

			if (!TTSPowerupNetworkTypes.isStaticType(powerupType)) {
				receivedPowerups.Add(handler);
			}
		}
	}

	public void UpdateRacer(Vector3 Pos, Vector3 Rot, Vector3 Speed, float VInput, float HInput) {
		if (owner && isServerRegistered) { // Only send data if it's the owner

			if (!isWriterUpdated) writer.ClearData();
			isWriterUpdated = true;

			writer.AddData(TTSCommandTypes.RacerUpdate);
			writer.AddData(id);
			writer.AddData(Pos);
			writer.AddData(Rot);
			writer.AddData(Speed);
			writer.AddData(VInput);
			writer.AddData(HInput);
		}
	}

	public void SendStaticPowerup(int powerupType, float powerupTier) {
		if (owner && isServerRegistered) { // Only send data if it's the owner
			if (!isWriterUpdated) writer.ClearData();
			isWriterUpdated = true;

			writer.AddData(TTSCommandTypes.PowerupStaticRegister);
			writer.AddData(id);
			writer.AddData(powerupType);
			writer.AddData(powerupTier);
		}
	}
}
