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
public class TTSRacer: TTSBehaviour {
	
	
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
	private Vector3 PreviousVelocity = new Vector3(1,0,0);
	public bool onGround = true;
	private float TiltRecoverySpeed = 0.1f;
	private float TiltAngle = 0.0f;
	public AudioClip[] DamageSounds;
	public GameObject SparksEmitter;
	public GameObject CurrentRig;
	public bool canMove = false;
	private float MinimumVelocityToAnimateSteering = 1.0f;
	#endregion

	#region gameplay vars
	public float TopSpeed = 250.0f;
	public float Acceleration = 8000.0f;
	public float Handling = 11000.0f;
	
	public float Defense;
	public float Offense;
	#endregion
	
	public bool hasShield;
	private float smooth;
	private float stopSpeed = 0.05f;
	public GameObject currentWaypoint;
	public GameObject previousWaypoint;
	public GameObject startingWaypoint;
	public bool goingWrongWay = false;
	
	void Awake() {
		level.RegisterRacer(gameObject);
		//Get the body via tag.
		foreach(Transform child in transform){
    		if(child.gameObject.tag == "RacerDisplayMesh"){
        		displayMeshComponent = child;
    		}
		}
		
		if(!level.PerksEnabled){
			this.GetComponent<TTSPerkManager>().enabled = false;
		}
		
		if(displayMeshComponent == null) {
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
		
		PreviousVelocity = rigidbody.velocity;
	}
	
	public void WrongWay(){
		if(goingWrongWay == true) Debug.Log("WRONG WAY");
		else Debug.Log("RIGHT WAY");
	}
	
	void CalculateInputForces() {
		//add acceleration forces...
		if(onGround && rigidbody.velocity.magnitude < TopSpeed && canMove) {
			rigidbody.AddForce(displayMeshComponent.forward * Input.GetAxis("Vertical") * Time.deltaTime * Acceleration);	
		}
		
		if(canMove) {
			rigidbody.AddForce(displayMeshComponent.right * Input.GetAxis("Horizontal") * Time.deltaTime * Handling);
		}
	}
	
	public void SlowToStop(){
		rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, new Vector3(0, 0, 0), stopSpeed);
	}
	
	public void StopRacer(){
		rigidbody.velocity = new Vector3(0, 0, 0);
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
	
	void OnCollisionEnter(Collision collision) {

		onGround = true;
		if(collision.relativeVelocity.magnitude > 10) {
			vfx.DamageEffect(100.0f);
			//RacerSfx.volume = collision.relativeVelocity.magnitude / TopSpeed / 1.5f;
			RacerSfx.volume = collision.relativeVelocity.magnitude / 100.0f / 1.5f;
			RacerSfx.PlayOneShot(DamageSounds[Mathf.FloorToInt(Random.value * DamageSounds.Length)]);
		}
		
		//spawn sparks (TODO: move this to a component script)
		GameObject sparkClone = (GameObject) Instantiate(SparksEmitter);
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
	
	void CalculateBodyOrientation () {

		//Facing Direction...
		if(new Vector2(rigidbody.velocity.x,rigidbody.velocity.z).magnitude > MinimumVelocityToAnimateSteering) {
			//based on rigidbody velocity.
			displayMeshComponent.forward = rigidbody.velocity;
			TiltAngle = Mathf.Lerp (TiltAngle, TTSUtils.GetRelativeAngle(rigidbody.velocity,PreviousVelocity)/2, TiltRecoverySpeed);
		
			displayMeshComponent.RotateAround(displayMeshComponent.forward,TiltAngle);
			//set the idle vec, so it doesnt get janky.
			IdleForwardVector = displayMeshComponent.forward;
		}else{
			displayMeshComponent.forward = IdleForwardVector;	
		}
		
	}
	
	void LateUpdate() {
		//sound
		RacerSounds.pitch = Mathf.Lerp(RacerSounds.pitch,TTSUtils.Remap(rigidbody.velocity.magnitude, 0f, 100.0f, 0.5f, 1.0f, false),0.04f);
		RacerSounds.volume = Mathf.Lerp(RacerSounds.volume,TTSUtils.Remap(rigidbody.velocity.magnitude, 0f, 100.0f, 0.5f, 1f, false),0.04f) * 1.5f;
	}
	
	public float GetTiltAngle() {
		return TiltAngle;
	}
}

	
 