using UnityEngine;
using System.Collections.Generic;



public class Racer: MonoBehaviour {

	public Vector3 RealForward = new Vector3(0,0,1);
	public Vector3 FloorNormal = new Vector3(0,1,0);
	public float Acceleration = 1000.0f;
	public float Magnitude = 10.0f;
	public float Rotation = 0.0f;
	public float rotationSpeed = 0.02f;
	public float MaxAngularVelocity = 30.0f;
	public bool isPC = true;
	public bool onGround = true;
	
	public float bankingAngle = 0.0f;
	public float MAXIMUM_BANK = 0.2f;
	
	public GameObject physicsController;
	public GameObject body;
	public GameObject lights;
	
	public float VELOCITY_DIG_FACTOR = 100;
	
	
	void Start () {
		
		if(!physicsController || !body) {
			Debug.LogError("Please define a body and pc for the racer prefab.");	
		}
		
		RealForward = this.transform.forward;
		physicsController.rigidbody.maxAngularVelocity = MaxAngularVelocity;
		
		
	}
	
	void LateUpdate () {
		
		  //Raycast to find the normal of the floor
		  RaycastHit hit = new RaycastHit();
	      if (Physics.Raycast (transform.position, -Vector3.up, out hit, 100.0f)) {
	            FloorNormal = hit.normal; //Update the var
	      }
	
		this.Magnitude = 0;
		 //update magnitude if W is down.
			
		if(isPC) {
			 if(Input.GetAxis("Vertical") > 0) {
			 	this.Magnitude = this.Acceleration; 
			 }
			 
			 if(Input.GetAxis("Vertical") < 0) {
			 	this.Magnitude -= this.Acceleration; 
			 }
			 //Update RealForward if D or A is down.
			 
			 if(Input.GetAxis("Horizontal") > 0) {
			 	this.Rotation += rotationSpeed;
			 }
			 
			  if(Input.GetAxis("Horizontal") < 0) {
			 	this.Rotation -= rotationSpeed;
			 }
			 
			  //calculate orientation of realforward, in a pre normalized way
		 RealForward.Set(Mathf.Cos(Rotation),0,Mathf.Sin(Rotation));
		 }else{
		 	RealForward = this.GetComponent<AIController>().doAISeek();
		 	
		 }
		
		  //Prefab animation update
		  body.transform.position = physicsController.transform.position;
		  body.transform.forward = RealForward;
		 
		 
		  lights.transform.position = physicsController.transform.position;
		  lights.transform.forward = RealForward;
		
		  //we calculate the angle and compare the cross product to find out if it is greater or less than the forward direction
		
		  bankingAngle = Vector2.Angle(new Vector2(RealForward.x,RealForward.z),new Vector2(physicsController.rigidbody.velocity.x,physicsController.rigidbody.velocity.z));
		  Vector3 cross = Vector3.Cross(new Vector2(RealForward.x,RealForward.z),new Vector2(physicsController.rigidbody.velocity.x,physicsController.rigidbody.velocity.z));
 
		  if (cross.z > 0)
   			 bankingAngle = 360 - bankingAngle;
		
		  bankingAngle = Mathf.Deg2Rad * bankingAngle;
		  bankingAngle = MAXIMUM_BANK * Mathf.Sin(bankingAngle) * (physicsController.rigidbody.velocity.magnitude/VELOCITY_DIG_FACTOR);
		  body.transform.Rotate(new Vector3(0,0,bankingAngle));
		
		  body.transform.RotateAround(body.transform.right,physicsController.transform.eulerAngles.x);
		  lights.transform.right = body.transform.right;
		  //lights.transform.Find("Left").transform.Rotate(body.transform.right);
		  //lights.transform.Find("Right").transform.Rotate(-body.transform.right);		  
		  
			
		  go();
			
	}
	
	void OnCollisionEnter(Collision collision) {
		onGround = true;
	}
	
	void OnCollisionExit(Collision collision) {
		onGround = false;
	}
	
	void go() {
		
		
		//The vector to calculate
		Vector3 torqueVector = new Vector3(0,0,0);
			
			
		//Torque is perpendicular to the forward vector of the player and the normal of the floor
		torqueVector = Vector3.Cross(RealForward,FloorNormal);
			
			
			
		physicsController.rigidbody.AddTorque(-Vector3.Normalize(torqueVector) * Magnitude);

	
	
	}
	
	void OnDrawGizmos() {
		//Debuggin
		 Gizmos.color = Color.red;
	     Vector3 direction  = RealForward*10;
	     Gizmos.DrawRay (physicsController.transform.position, direction);
	     Gizmos.color = Color.blue;
	     Gizmos.DrawRay (physicsController.transform.position, FloorNormal*10);
	     Gizmos.color = Color.magenta;
	     Gizmos.DrawRay (physicsController.transform.position, Vector3.Cross(RealForward,FloorNormal)*Magnitude); 
		 Gizmos.color = Color.white;
		 Gizmos.DrawRay(body.transform.position,physicsController.rigidbody.velocity);

		Gizmos.color = Color.green;
		 Gizmos.DrawRay(body.transform.position, body.transform.right*100);
	}
	
}
	
