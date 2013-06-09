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
//[RequireComponent(typeof(TTSInputController))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
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
	#endregion
	
	
	#region NEEDS EDITOR
	private float MinimumVelocityToAnimateSteering = 1.0f;
	#endregion
	
	
	#region gameplay vars
	public float TopSpeed = 100.0f;
	public float Acceleration = 50.0f;
	public float Handling = 50.0f;
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
		
	}
	
	
	void Start () {
		
		
		
	}
	
	void FixedUpdate () {
		
		
		
		if(IsPlayerControlled) {
			CalculateInputForces();	
		}
		
		CalculateBodyOrientation();
		
		PreviousVelocity = rigidbody.velocity;
	}
	
	
	void CalculateInputForces() {
		
		
		
		//add acceleration forces...
		if(onGround && rigidbody.velocity.magnitude < TopSpeed) {
			this.rigidbody.AddForce(displayMeshComponent.forward * Input.GetAxis("Vertical") * Time.deltaTime * Acceleration);
			this.rigidbody.AddForce(displayMeshComponent.right * Input.GetAxis("Horizontal") * Time.deltaTime * Acceleration);
		}
		
		
	
		
		
		
	}
	
	void OnEnterCollision(Collider other) {
		onGround = false;	
	}
	
	void OnExitCollision(Collider other) {
		onGround = true;	
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
		
		//low priority movement first...
		TiltAngle = Mathf.Lerp (TiltAngle, TTSUtils.GetRelativeAngle(rigidbody.velocity,PreviousVelocity)/2, TiltRecoverySpeed);
		
		displayMeshComponent.RotateAround(displayMeshComponent.forward,TiltAngle);
		
		PreviousVelocity = rigidbody.velocity;
		
		
	}
	
	void OnDrawGizmos() {
	
	}
	
	public float GetTiltAngle() {
		return TiltAngle;
	}
	
	
}

	
 