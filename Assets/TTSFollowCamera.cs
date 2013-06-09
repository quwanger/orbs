using UnityEngine;
using System.Collections.Generic;

public class TTSFollowCamera : MonoBehaviour {

	// The target we are following
	public Transform target;
	// The distance in the x-z plane to the target
	public float distance = 10.0f;
	// the height we want the camera to be above the target
	public float height = 5.0f;
	// How much we 
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	
	private float tiltAngle = 0.0f;

	void Update () {
		// Early out if we don't have a target
		if (!target)
			return;
		
		// Calculate the current rotation angles
		var wantedRotationAngle = target.eulerAngles.y;
		var wantedHeight = target.position.y + height;
			
		var currentRotationAngle = transform.eulerAngles.y;
		var currentHeight = transform.position.y;
		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
	
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
	
		// Convert the angle into a rotation
		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distance;
	
		
		
		
		// Always look at the target
		transform.LookAt (target);
		
		// Set the height of the camera
		transform.position.Set(transform.position.x,currentHeight,transform.position.z);
		
		tiltAngle = Mathf.Lerp (tiltAngle, target.parent.GetComponent<TTSRacer>().GetTiltAngle(),0.05f);
		
		transform.RotateAround(transform.forward,tiltAngle);
		
		if(target.parent.GetComponent<TTSRacer>().rigidbody.velocity.sqrMagnitude > 3000) {
			transform.position += Random.insideUnitSphere * 0.06f;	
		} 
	}
}