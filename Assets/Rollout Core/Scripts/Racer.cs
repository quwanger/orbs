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

public class Racer: MonoBehaviour {
	
	public Vector3 RealForward = new Vector3(0,0,1); //The direction that force is applied to move the racer.
	public Vector3 FloorNormal = new Vector3(0,1,0); //The normal vector of the floor that the object is over.
	
	public float ACCELERATION = 100.0f; //The constant force to apply to the rigidbody over every frame when moving forward.
	public float currentAcceleration = 0.0f; //The actual force being applied, is 0 if no button is pressed.
	
	public float Rotation = 0.0f; //The current y axis rotation of RealForward 
	public float rotationSpeed = 0.02f; //The speed at which to adjust rotation when the control is invoked
	
	public float MaxAngularVelocity = 30.0f; //The maximum rolling speed of the rigid body, change it if the rigid body is misbehaving (skidding)
	
	public bool isPC = true; //Is it using the controller or is it AI controlled?
	public bool onGround = true; //Is it on the ground? TODO: needs implementation
	
	public float bankingAngle = 0.0f; //The andgle to 'bank' the body when cornering
	public float MAXIMUM_BANK = 0.2f; //The maximum bank [TODO: implement clamp on banking angle]
	
	public GameObject physicsController; //The rigidbody to control movement
	public GameObject body; //The displayed mesh
	public GameObject lights; //Lights prefab
	
	public float SPEED_BANKING_DAMPENING = 100; //The damper for the banking based on speed.
	
	public float TOP_SPEED = 40.0f;
	
	void Start () {
		
		if(!physicsController || !body) {
			Debug.LogError("Please define a body and pc for the racer prefab."); //If these objects are not linked, throw an error	
		}
		
		RealForward = this.transform.forward; //Quickly set up forward vector
		physicsController.rigidbody.maxAngularVelocity = MaxAngularVelocity; //Set the angular velocity.
		physicsController.AddComponent<CollisionHandler>();
		
	}
	
	void LateUpdate () {
		
		//We use late update to avoid jittering mesh
		
		
		//Raycast to find the normal of the floor
		RaycastHit hit = new RaycastHit();
	    if (Physics.Raycast (physicsController.transform.position, -Vector3.up, out hit)) {
	        FloorNormal = hit.normal; //Update the var
			
	    }
		
		//Always reset to zero before handling input or AI
		this.currentAcceleration = 0;
			
		if(isPC) {
			
			//Add a force if input is forward or back
			if(Input.GetAxis("Vertical") > 0) {
			 	this.currentAcceleration = this.ACCELERATION;
			}
			 
			if(Input.GetAxis("Vertical") < 0) {
			 	this.currentAcceleration = -this.ACCELERATION; 
			}
			
			//Update Rotation if D or A is down.
			if(Input.GetAxis("Horizontal") > 0) {
			 	this.Rotation += rotationSpeed;
			}
			 
			if(Input.GetAxis("Horizontal") < 0) {
			 	this.Rotation -= rotationSpeed;
			}
			 
			//calculate orientation of realforward, in a pre normalized way
		 	RealForward.Set(Mathf.Cos(Rotation),0,Mathf.Sin(Rotation));
			
		}else{
			//Use the AI controller to seek a waypoint
			RealForward = this.GetComponent<AIController>().doAISeek();
		 	
		}
		
		//Update the mesh...
		body.transform.position = physicsController.transform.position;
		body.transform.forward = RealForward;
		
		//Update the lights...
		lights.transform.position = physicsController.transform.position;
		lights.transform.forward = RealForward;
		
		//This next part is banking... its a little weird.
		
		//Trick to get negative angles instead of just the smallest (or what Vector2.angle() would give you...)
		bankingAngle = Vector2.Angle(new Vector2(RealForward.x,RealForward.z),new Vector2(physicsController.rigidbody.velocity.x,physicsController.rigidbody.velocity.z));
		Vector3 cross = Vector3.Cross(new Vector2(RealForward.x,RealForward.z),new Vector2(physicsController.rigidbody.velocity.x,physicsController.rigidbody.velocity.z));
 
		if (cross.z > 0)
   			bankingAngle = 360 - bankingAngle;
			
		//now that we have the angle, we need it in radians...
		bankingAngle = Mathf.Deg2Rad * bankingAngle;
		 
		//Adjust our bank...
		bankingAngle = MAXIMUM_BANK * Mathf.Sin(bankingAngle) * (physicsController.rigidbody.velocity.magnitude/SPEED_BANKING_DAMPENING);
		
		//Update the meshes...
		body.transform.Rotate(new Vector3(0,0,bankingAngle));
		
		 
		//Get the roll... (this is wrong atm)
		body.transform.RotateAround(body.transform.right,physicsController.transform.eulerAngles.x);
		 
		//lights...
		lights.transform.right = body.transform.right;
		
		//Now the tilt of the Realforward to conform with the floor...
		RealForward = Vector3.Cross(FloorNormal,body.transform.right);
		
		
		
			
	}
	
	void FixedUpdate() {
		onGround = physicsController.GetComponent<CollisionHandler>().colliding;
		
		if(onGround) {
			go ();
		}
	}
	
	void go() {
		if(physicsController.rigidbody.velocity.magnitude < TOP_SPEED) {
			physicsController.rigidbody.AddForce(Vector3.Normalize(RealForward) * currentAcceleration);
		}
	}
	
	void OnDrawGizmos() {
		//Debuggin
		Gizmos.color = Color.red;
	    Gizmos.DrawRay (physicsController.transform.position, RealForward*5);
	    Gizmos.color = Color.blue;
	    Gizmos.DrawRay (physicsController.transform.position, FloorNormal*5);
		Gizmos.color = Color.white;
		Gizmos.DrawRay(body.transform.position,physicsController.rigidbody.velocity);
	}
	
}

	
