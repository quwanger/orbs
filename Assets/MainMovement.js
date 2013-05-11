#pragma strict

var RealForward:Vector3 = Vector3(0,0,1);
var FloorNormal:Vector3 = Vector3(0,1,0);
var Acceleration:float = 1000;
var Magnitude:float = 10;
var Rotation:float = 0;
var rotationSpeed:float = 0.02;
var MaxAngularVelocity:float = 30;
var isPC:boolean = true;
var target:MainMovement;

function Start () {
	
	RealForward = this.transform.forward;
	this.rigidbody.maxAngularVelocity = MaxAngularVelocity;

	this.collider.material.staticFriction = 10000;	
}

function FixedUpdate () {
	
	  //Raycast to find the normal of the floor
	  var hit : RaycastHit;
      if (Physics.Raycast (transform.position, -Vector3.up, hit, 100.0)) {
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
	 	
	 	Magnitude = 1000;
		RealForward = Vector3.RotateTowards(RealForward,target.RealForward,10,0);
	 
	 }
	 
	
	 
	  go();
		
}


function go() {
	
	//The vector to calculate
	var torqueVector:Vector3 = Vector3(0,0,0);
	
	
	//Torque is perpendicular to the forward vector of the player and the normal of the floor
	torqueVector = Vector3.Cross(RealForward,FloorNormal);
	
	
	
	this.rigidbody.AddTorque(Vector3.Normalize(torqueVector) * Magnitude);


}

function OnDrawGizmos() {
	//Debuggin
	 Gizmos.color = Color.red;
     var direction : Vector3 = RealForward*10;
     Gizmos.DrawRay (transform.position, direction);
      Gizmos.color = Color.blue;
     Gizmos.DrawRay (transform.position, FloorNormal*10);
      Gizmos.color = Color.magenta;
     Gizmos.DrawRay (transform.position, Vector3.Cross(RealForward,FloorNormal)*Magnitude);

}