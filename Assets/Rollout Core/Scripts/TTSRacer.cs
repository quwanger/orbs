using UnityEngine;
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
	public GameObject CurrentRig;
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
	#endregion
	
	public bool hasShield;
	private float smooth;
	private float stopSpeed = 0.05f;

	#region Direction/Wrong way
	public TTSWaypoint currentWaypoint;
	public TTSWaypoint previousWaypoint;
	public TTSWaypoint debugStart;
	public bool goingWrongWay = false;
	#endregion

	// AI
	private TTSWaypoint lastWaypoint;
	private TTSWaypoint nextWaypoint;
	TTSAIUtils AIUtil;

	void Awake() {
		AIUtil = new TTSAIUtils(5);

		level.RegisterRacer(gameObject);
		//Get the body via tag.
		foreach (Transform child in transform) {
			if (child.gameObject.tag == "RacerDisplayMesh") {
				displayMeshComponent = child;
			}
		}

		if (player == PlayerType.AI) {
			//AIControl = new TTSRacerAI(allWaypoints, rigidbody.velocity, waypointManager);
		}

		lastForward = TTSUtils.FlattenVector(displayMeshComponent.forward).normalized;

		if (!level.PerksEnabled) {
			this.GetComponent<TTSPerkManager>().enabled = false;
		}

		if (displayMeshComponent == null) {
			Debug.LogException(new UnassignedReferenceException());
		}
		IdleForwardVector = transform.forward;


		//Setup Audio Channel for Pitched Racer Sounds
		RacerSounds = gameObject.AddComponent<AudioSource>();
		RacerSounds.clip = RollingSound;
		RacerSounds.loop = true;
		RacerSounds.rolloffMode = AudioRolloffMode.Linear;
		RacerSounds.Play();

		//Setup Audio Channel for SFX
		RacerSfx = gameObject.AddComponent<AudioSource>();
		RacerSfx.rolloffMode = AudioRolloffMode.Linear;
		RacerSfx.volume = 0.5f;

		//Apply Attributes
		TopSpeed = TopSpeed + (100.0f * CurrentRig.GetComponent<TTSRig>().rigSpeed);
		Acceleration = Acceleration + (100.0f * CurrentRig.GetComponent<TTSRig>().rigAcceleration);
		Handling = Handling + (100.0f * CurrentRig.GetComponent<TTSRig>().rigHandling);
		
		Offense = CurrentRig.GetComponent<TTSRig>().rigOffense;
		Defense = CurrentRig.GetComponent<TTSRig>().rigDefense;
	}
	
	void FixedUpdate () {
		if(!level.raceHasFinished){
			if(IsPlayerControlled){ 
				CalculateInputForces();
			}
		}else{
			SlowToStop();
		}

		CalculateBodyOrientation();

		resultAccel = Mathf.Lerp(resultAccel, rigidbody.velocity.magnitude - PreviousVelocity.magnitude, 0.01f);
		PreviousVelocity = rigidbody.velocity;
	}

	private void CalculateInputForces() {
		if (finished && !level.DebugMode) // No input when race is finished
			return;

		if (player == PlayerType.Player) {
			vInput = Input.GetAxis("Vertical");
			hInput = Input.GetAxis("Horizontal");

		}
		else if (player == PlayerType.Multiplayer) {

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

	public void WrongWay() {
		if (goingWrongWay == true) Debug.Log("WRONG WAY");
		else Debug.Log("RIGHT WAY");
	}

	public void OnWaypoint(TTSWaypoint hit) {
		previousWaypoint = currentWaypoint;
		currentWaypoint = hit;
		if (previousWaypoint == currentWaypoint) {
			if (goingWrongWay == true) {
				goingWrongWay = false;
				WrongWay();
			}
			else {
				goingWrongWay = true;
				WrongWay();
			}
		}

		if(!waypointManager.EndPoints.Contains(currentWaypoint))
			nextWaypoint = AIUtil.getClosestWaypoint(currentWaypoint.nextWaypoints, position);
	}

	public void SlowToStop() {
		rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, new Vector3(0, 0, 0), stopSpeed);
	}

	public void StopRacer() {
		rigidbody.velocity = new Vector3(0, 0, 0);
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
			Gizmos.DrawLine(transform.position, destination);
		}

		//if (AIControl != null)
		//	AIControl.drawGizmos();
	}

	Vector3 destination;
	public void AIInput(){
		float debugVInput = Input.GetAxis("Vertical"), debugHInput = Input.GetAxis("Horizontal");

		if (nextWaypoint == null){
			if (!waypointManager.EndPoints.Contains(currentWaypoint))
				nextWaypoint = waypointManager.SpawnZone;
			else {
				finished = true;
				return;
			}
		}

		float turnAmount = AIUtil.turnCurveAmount(lastForward, nextWaypoint);

		//destination = AIUtil.getVisibleWaypointPos(nextWaypoint, position);

		if (turnAmount > AIUtil.HARD_TURN_AMOUNT) // Check to see if there needs to be a hard turn
			destination = AIUtil.getVisibleWaypointPos(nextWaypoint, position);
		else
			destination = AIUtil.hardTurnManeuver(position, nextWaypoint);

		Vector3 steerDir = TTSUtils.FlattenVector(destination - position);

		if (!level.DebugMode || level.racers.Length > 1 || (debugVInput == 0.0f && debugHInput == 0.0f)) {

			vInput = AIUtil.forwardSpeed(vInput, turnAmount, nextWaypoint, position, rigidbody.velocity);
			hInput = TTSUtils.Remap(TTSUtils.GetRelativeAngle(lastForward, steerDir)*2, -90.0f, 90.0f, -1.0f, 1.0f, true);
		}

		Debug.DrawLine(position, destination);
		//nextWaypoint.nextWaypoints[0].getDistanceFrom(position, nextWaypoint); // Distance from end is screwed up.
	}
}

public class TTSAIUtils
{
	public float HARD_TURN_AMOUNT = 0.9f;

	public int resolution = 4;
	public int foresight = 3;
	public float foresightDistance = 300;
	public bool debugMode = false;
	public float AISlowDownDistance = 300.0f;
	public float hardTurnDistance = 100.0f;

	public float turnCautiousness = 4.0f;

	public int intelligence;

	public TTSAIUtils(int intelligence){
		this.intelligence = intelligence;

		if(intelligence > 1){
			hardTurnDistance = 75.0f;
		}
	}

	public void randomizeValues(){

	}

	public Vector3 hardTurnManeuver(Vector3 position, TTSWaypoint nextWP) {
		Vector3 destination = new Vector3();

		Vector3 point = Vector3.Project(waypointForwardForesight(foresight, nextWP).normalized, nextWP.colliderLine);

		Debug.DrawRay(nextWP.position, point * nextWP.boxWidth / 2);

		if (nextWP.getDistanceFrom(position) > hardTurnDistance)
			destination = nextWP.position - (point * nextWP.boxWidth / 2);
		else {
			destination = nextWP.position + (point.normalized * nextWP.boxWidth / 2);
		}

		return destination;
	}

	/// How similar the racer's forward and the waypoints forward are (0.0f -> 1.0f = hard -> no turn )
	public float turnCurveAmount(Vector3 racerForward, TTSWaypoint wp) {
		// Compare racer forward with waypoint forwards
		Vector3 tempForward = waypointForwardForesight(foresight, wp).normalized;
		float speed = Vector3.Project(tempForward, racerForward).magnitude;

		return Mathf.Sqrt(speed);
	}

	/// Takes distance into account for how fast the racer must go.
	public float forwardSpeed(float prevInput, float turnAmount, TTSWaypoint wp, Vector3 position, Vector3 velocity){
		// Get distance multiplier based on how far the racer is from the waypoint
		float distanceMultiplier = Mathf.Pow(Mathf.Min(1.0f, wp.getDistanceFrom(position) / AISlowDownDistance), turnCautiousness);

		if(intelligence > 2){ // Reverse if it's a hard turn
			if(turnAmount < HARD_TURN_AMOUNT &&	velocity.magnitude > (80.0f * turnAmount) && wp.getDistanceFrom(position) < ((1-turnAmount) * hardTurnDistance))
				return -1.0f;
		}

		float t = Mathf.Pow(turnAmount, 1-distanceMultiplier);

		return Mathf.Lerp(prevInput, t, 0.1f);
	}

	public Vector3 getVisibleWaypointPos(TTSWaypoint wp, Vector3 from) {
		int resolution = 5;
		Vector3 closest = wp.getClosestPoint(from);

		if (!Physics.Linecast(from, closest, TTSUtils.LayerMask(10))){
			return closest;
		}
		else if (debugMode)
			Debug.Log("Can't see closest point");

		closest = wp.position;
		closest.y = from.y;
		Vector3 pnt = new Vector3();

		closest = wp.position;

		// So that we make as many checks as resolutions;
		for (float i = 0; i < resolution; i++) { // Start from right to left.
			pnt = wp.getPointOn(i / (resolution-1));
			pnt.y = from.y;
			
			if(debugMode)
				Debug.DrawLine(from, pnt);

			if (!Physics.Linecast(from, pnt, TTSUtils.LayerMask(10)) && Vector3.Distance(from, pnt) < Vector3.Distance(from, closest)) {
				closest = pnt;
			}
		}

		return closest;
	}

	public TTSWaypoint getClosestWaypoint(List<TTSWaypoint> wp, Vector3 from){
		TTSWaypoint closest = null;

		foreach (TTSWaypoint waypoint in wp) {
			if (closest == null) {
				closest = waypoint;
			}
			else {
				if(waypoint.getDistanceFrom(from) < closest.getDistanceFrom(from))
					closest = waypoint;
			}
		}

		return closest;
	}

	private Vector3 waypointForwardForesight(int foresight, TTSWaypoint wp) {
		Vector3 forward = wp.forwardLine * foresight;

		if(foresight != 1){
			foresight--;

			foreach (TTSWaypoint next in wp.nextWaypoints) {
				forward += waypointForwardForesight(foresight, next);
			}
		}

		return forward;
	}
}

/*
public class TTSRacerAI {
	// Waypoints
	private List<TTSWaypoint> waypoints;
	private TTSWaypointManager wpManager;
	public int nextWaypoint = 0;
	public int foresight = 3;

	// Racer Vars
	private Vector3 rForward;
	private Vector3 rSpeed;
	private Vector3 rPosition;

	// Movement Vars
	private Vector3 destination;
	private Vector3 nextWaypointDir = new Vector3();

	// Input
	public float vInput = 0.0f;
	public float hInput = 0.0f;

	/// <summary> 
	/// 
	/// </summary> 
	/// <param name="waypointList">List of waypoints needed</param> 
	/// <param name="rSpeed">Reference to racer speed</param> 
	public TTSRacerAI(List<TTSWaypoint> waypointList, Vector3 racerSpeed, TTSWaypointManager waypointManager)
	{
		waypoints = waypointList;
		wpManager = waypointManager;
		rSpeed = racerSpeed;
	}

	public void update(Vector3 position, Vector3 forward) {
		rPosition = position;
		rForward = forward;
		update();
	}

	public void update() {
		destination = waypoints[nextWaypoint].getClosestSeenPoint(rPosition, 7);
		nextWaypointDir = TTSUtils.FlattenVector(destination - rPosition);

		float sensitivity = 90.0f;

		vInput = 1.0f;
		hInput = TTSUtils.Remap(TTSUtils.GetRelativeAngle(rForward, nextWaypointDir), -sensitivity, sensitivity, -1.0f, 1.0f, true);
	}

	public void lookForward(){
		//Debug.DrawRay()
	}

	public void drawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(rPosition, destination);

		Gizmos.color = Color.cyan;
		Gizmos.DrawCube(destination, new Vector3(0.5f, 0.5f, 0.5f));
	}
}
*/