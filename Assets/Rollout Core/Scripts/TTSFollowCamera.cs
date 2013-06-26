using UnityEngine;
using System.Collections.Generic;

public class TTSFollowCamera : MonoBehaviour {
	
	public enum cameraModes {THIRD_PERSON, FIRST_PERSON};
	
	public cameraModes CameraMode;
	
	// The target we are following
	public Transform target;

	// The distance in the x-z plane to the target
    private float baseDistance = 4.0f;
	public float distance = 10.0f;
	// the height we want the camera to be above the target
	public float baseHeight = 4.0f;
    private float height = 4.0f;
	// How much we 
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;

    // Camera tilt
    private float tiltAngle = 0.0f;

    // FOV of the camera with it's set limits
    private float Base_FOV = 72.9f;
    private float Max_FOV = 95.0f;

    // Shakiness factor
    public float shakiness = 5.0f;

	void FixedUpdate () {
		if(CameraMode == cameraModes.THIRD_PERSON) {
			ThirdPerson ();	
		}
		if(CameraMode == cameraModes.FIRST_PERSON)
		{
			this.transform.position = target.transform.position + transform.forward*2;
			this.transform.rotation = target.transform.rotation;
			
			if(target.parent.GetComponent<TTSRacer>().rigidbody.velocity.sqrMagnitude > 3000) {
				transform.position += Random.insideUnitSphere * 0.06f;	
			}
		
			if(GetComponent<Vignetting>().chromaticAberration > 0.0f) {
				GetComponent<Vignetting>().chromaticAberration = Mathf.Lerp(GetComponent<Vignetting>().chromaticAberration, 0.0f, 0.07f);	
			}
		}
	}
	
	void ThirdPerson() {
		// Early out if we don't have a target
		if (!target)
            return;

        // Speed squared magnitude
        float speedSqrMag = target.parent.GetComponent<TTSRacer>().rigidbody.velocity.sqrMagnitude;
		
		// Calculate the current rotation angles
		float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        // Change the height that the camera should be above the ball
        height = Mathf.Lerp(height, baseHeight - (Mathf.Clamp((speedSqrMag - 1300) / 300, 0, baseHeight-0.5f)), 0.03f);

        float wantedRotationAngle = target.eulerAngles.y;
        float wantedHeight = target.position.y + height;
		
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
		
		if(Input.GetKey(KeyCode.LeftShift)) {
			transform.RotateAround(Vector3.up,180);
		} 
		
		// Set the height of the camera
        //transform.position.Set(transform.position.x, currentHeight, transform.position.z);
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        transform.position -= currentRotation * Vector3.forward * distance;
        //transform.position += Vector3.up * 3;

		tiltAngle = Mathf.Lerp (tiltAngle, target.parent.GetComponent<TTSRacer>().GetTiltAngle(),0.05f);

        transform.RotateAround(transform.forward, tiltAngle);

        distance = Mathf.Lerp(distance, baseDistance + Mathf.Max(0, speedSqrMag - 1000) / 800, 0.008f);

        camera.fov = Mathf.Lerp(camera.fov, Base_FOV + Mathf.Max(0, speedSqrMag - 1500) / 50, 0.05f);

        if (speedSqrMag > 1500) {

            transform.position += Random.insideUnitSphere * 0.02f * shakiness * (Mathf.Max(0, speedSqrMag - 1500) / 1000);

            // Always look at the target
            transform.LookAt(target);

            transform.position += Random.insideUnitSphere * 0.01f * shakiness * (Mathf.Max(0, speedSqrMag - 1500) / 1000);
        }
        else {

            // Always look at the target
            transform.LookAt(target);
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