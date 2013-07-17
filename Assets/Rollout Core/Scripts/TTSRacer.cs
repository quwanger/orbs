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

[RequireComponent(typeof(TTSAIController))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(AudioSource))]
public class TTSRacer: MonoBehaviour {
	
	
	#region serialized settings
	public bool IsPlayerControlled = true;
	#endregion
	
	#region Internal Component
	private Transform displayMeshComponent;
	#endregion
	
	
	#region internal operators
	private Vector3 IdleForwardVector;
	private Vector3 PreviousVelocity = new Vector3(1,0,0);
	public bool onGround = true;
	private float TiltRecoverySpeed = 0.1f;
	private float TiltAngle = 0.0f;
	public AudioClip[] DamageSounds;
	public GameObject SparksEmitter;
	public GameObject entropyCannonDebug;
	#endregion
	
	
	#region NEEDS EDITOR
	private float MinimumVelocityToAnimateSteering = 1.0f;
	#endregion
	
	
	#region gameplay vars
	public float TopSpeed = 100.0f;
	public float Acceleration = 2000.0f;
	public float Handling = 3000.0f;
	#endregion
	
	 
	
	void Awake() {
		
		//Get the body via tag.
		foreach(Transform child in transform){
    		if(child.gameObject.tag == "RacerDisplayMesh"){
        		displayMeshComponent = child;
    		}
		}
		
		if(displayMeshComponent == null) {
			Debug.LogException(new UnassignedReferenceException());	
		}
		
		IdleForwardVector = transform.forward;
		//SparksParticleSystem = GameObject.Find("SparksEmitter");
		
	}
	
	
	void Start () {
		
		
		
	}
	
	void FixedUpdate () {
		if(IsPlayerControlled) {
			CalculateInputForces();	
		} else {
			CalculateAiForces();	
		}
		
		CalculateBodyOrientation();
		
		PreviousVelocity = rigidbody.velocity;
		
		
	}
	
	
	void CalculateInputForces() {
		//add acceleration forces...
		if(onGround && rigidbody.velocity.magnitude < TopSpeed) {
			this.rigidbody.AddForce(displayMeshComponent.forward * Input.GetAxis("Vertical") * Time.deltaTime * Acceleration);
			this.rigidbody.AddForce(displayMeshComponent.right * Input.GetAxis("Horizontal") * Time.deltaTime * Handling);
		}
		
		if (Input.GetKeyDown ("space")) {
			EntropyCannon(1);
		}
		
	}
	
	
	void OnCollisionEnter(Collision collision) {

		onGround = true;
		if(collision.relativeVelocity.magnitude > 10) {
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TTSFollowCamera>().DoDamageEffect();
			GetComponent<AudioSource>().PlayOneShot(DamageSounds[Mathf.FloorToInt(Random.value * DamageSounds.Length)]);
		}
		
		
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
			//set the idle vec, so it doesnt get janky.
			IdleForwardVector = displayMeshComponent.forward;
		}else{
			displayMeshComponent.forward = IdleForwardVector;	
		}
		
		TiltAngle = Mathf.Lerp (TiltAngle, TTSUtils.GetRelativeAngle(rigidbody.velocity,PreviousVelocity)/2, TiltRecoverySpeed);
		
		displayMeshComponent.RotateAround(displayMeshComponent.forward,TiltAngle);
		
		//sound
		GetComponent<AudioSource>().pitch = TTSUtils.Remap(rigidbody.velocity.magnitude, 0f, TopSpeed, 0.5f, 1.3f, false);
		GetComponent<AudioSource>().volume = TTSUtils.Remap(rigidbody.velocity.magnitude, 0f, TopSpeed, 0.5f, 1f, false);
		
		
	}
	
	void OnDrawGizmos() {
	
	}
	
	public float GetTiltAngle() {
		return TiltAngle;
	}
	
	private void CalculateAiForces() {
		//GetComponent<Biped>().MaxSpeed = TopSpeed;
		//GetComponent<Biped>().MaxForce = Acceleration;
		GetComponent<TTSAIController>().seekWaypoint();
	}
	
	#region powerup functions
	public void EntropyCannon(int tier){
		if (tier == 1) {
            GameObject go = (GameObject) Instantiate(entropyCannonDebug);
            go.transform.rotation = displayMeshComponent.transform.rotation;
            go.transform.position = this.transform.position + displayMeshComponent.forward * 3.5f;
			go.rigidbody.velocity = this.rigidbody.velocity.normalized * 100f;
		}
   }
   
   #endregion
	
	
}

	
 