using UnityEngine;
using System.Collections.Generic;



public class MainMovement : MonoBehaviour {

	public Vector3 RealForward = new Vector3(0,0,1);
	public Vector3 FloorNormal = new Vector3(0,1,0);
	public float Acceleration = 1000.0f;
	public float Magnitude = 10.0f;
	public float Rotation = 0.0f;
	public float rotationSpeed = 0.02f;
	public float MaxAngularVelocity = 30.0f;
	public bool isPC = true;
	public bool onGround = true;
	
	
	void Start () {
		
		RealForward = this.transform.forward;
		this.rigidbody.maxAngularVelocity = MaxAngularVelocity;
	
		this.collider.material.staticFriction = 50;	
		this.collider.material.dynamicFriction = 20;
	}
	
	void FixedUpdate () {
		
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
			
			
			
		this.rigidbody.AddTorque(-Vector3.Normalize(torqueVector) * Magnitude);

	
	
	}
	
	void OnDrawGizmos() {
		//Debuggin
		 Gizmos.color = Color.red;
	     Vector3 direction  = RealForward*10;
	     Gizmos.DrawRay (transform.position, direction);
	     Gizmos.color = Color.blue;
	     Gizmos.DrawRay (transform.position, FloorNormal*10);
	     Gizmos.color = Color.magenta;
	     Gizmos.DrawRay (transform.position, Vector3.Cross(RealForward,FloorNormal)*Magnitude);
	
	}
	
}
	
