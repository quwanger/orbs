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

	void FixedUpdate () {
		// Early out if we don't have a target
		if (!target)
			return;
		
		// Calculate the current rotation angles
		float wantedRotationAngle = target.eulerAngles.y;
		float wantedHeight = target.position.y + height;
			
		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;
		
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

		// Set the height of the camera
		transform.position = new Vector3(transform.position.x,currentHeight,transform.position.z);
		
		// Always look at the target
		transform.LookAt (target);
		
		if(Input.GetKey(KeyCode.LeftShift)) {
			transform.RotateAround(Vector3.up,180);
		} 
		
		
		
		tiltAngle = Mathf.Lerp (tiltAngle, target.parent.GetComponent<TTSRacer>().GetTiltAngle(),0.05f);
		
		transform.RotateAround(transform.forward,tiltAngle);
		
		if(target.parent.GetComponent<TTSRacer>().rigidbody.velocity.sqrMagnitude > 3000) {
			transform.position += Random.insideUnitSphere * 0.06f;	
		}
		
		if(GetComponent<Vignetting>().chromaticAberration > 0.0f) {
			GetComponent<Vignetting>().chromaticAberration = Mathf.Lerp(GetComponent<Vignetting>().chromaticAberration, 0.0f, 0.07f);	
		}
	}
	
	public void DoDamageEffect() {
		GetComponent<Vignetting>().chromaticAberration = 100;
		transform.position += Random.onUnitSphere * 2;
	}
}