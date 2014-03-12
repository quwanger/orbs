using UnityEngine;
using System.Collections.Generic;

public class TTSFollowCamera : TTSBehaviour
{

	public enum cameraModes
	{
		THIRD_PERSON,
		FIRST_PERSON
	};

	public cameraModes CameraMode;
	
	public int cameraNumber;

	// The target we are following
	public Transform target;

	// The distance in the x-z plane to the target
	public float distance = 5.0f; 

	// the height we want the camera to be above the target
	//private float height = 4.0f;  			
	public float heightDamping = 2.0f;			
	public float rotationDamping = 3.0f;		
	
	private float racerTopSpeed = -1.0f;

	// Camera tilt
	private float tiltAngle = 0.0f;

	// Shakiness factor
	public float shakiness = 0.15f;

	// Camera Effects handles
	public float baseDistance = 5.0f; // 							
	public float maxDistance = 7.0f; //							
	public float Base_FOV = 72.9f;
	public float Max_FOV = 80.0f;

	// Start effect percentages
	public float distanceStartEffect = 0.0f;
	public float heightStartEffect = 0.6f;
	public float fovStartEffect = 0.6f;
	public float shakeStartEffect = 0.7f;

	//Width, height, x and y of camera
	public float fadeWidth;
	public float fadeHeight;
	public float fadeX;
	public float fadeY;

	void Start ()
	{
		racer = GetComponent<TTSRacer>();
		racerTopSpeed = target.parent.GetComponent<TTSRacer>().TopSpeed;
		cameraToTarget = target.forward * cameraDistance;

		level.RegisterCamera(this.camera, false);
	}

	void FixedUpdate ()
	{
		if (CameraMode == cameraModes.THIRD_PERSON) {
			ThirdPersonv2();
		}
		if (CameraMode == cameraModes.FIRST_PERSON) {
			this.transform.position = target.transform.position + transform.forward * 2;
			this.transform.rotation = target.transform.rotation;

			if (target.parent.GetComponent<TTSRacer>().rigidbody.velocity.sqrMagnitude > 3000) {
				transform.position += Random.insideUnitSphere * 0.06f;
			}
		}
		prevVelocity = racerVelocity;
	}

	public TTSRacer racer;
	public float cameraDistance = 7.0f;
	public float cameraBaseDistance = 7.0f; // 							
	public float cameraMinDistance = 5.0f; //						
	public float cameraMaxDistance = 9.0f; //	

	public float height = 2.0f;
	public float baseHeight = 2.3f;
	public float minHeight = 1.75f;
	public float heightFromTarget = 0.5f; // Height parented

	Vector3 position {
		get {
			return transform.position;
		}
		set {
			transform.position = value;
		}
	}
	Vector3 targetPos {
		get {
			return target.position;
		}
	}
	Vector3 racerVelocity {
		get {
			return target.parent.rigidbody.velocity;
		}
	}
	Vector3 prevVelocity;
	float racerAccel {
		get {
			return racerVelocity.magnitude - prevVelocity.magnitude;
		}
	}

	Vector3 posBehindTarget;

	Vector3 cameraToTarget;

	Vector3 hitPos;
	void ThirdPersonv2() {
		cameraToTarget = Vector3.Lerp(cameraToTarget, target.forward * cameraDistance, 0.7f);
		Vector3 temp = targetPos - cameraToTarget;
		posBehindTarget.x = Mathf.Lerp(posBehindTarget.x, temp.x, 0.8f);
		posBehindTarget.y = Mathf.Lerp(posBehindTarget.y, temp.y, 0.1f);
		posBehindTarget.z = Mathf.Lerp(posBehindTarget.z, temp.z, 0.8f);
		position = posBehindTarget;

		transform.up = Vector3.Lerp(Vector3.up, target.up, 0.3f);
		transform.LookAt(target);
		position = position + target.up * heightFromTarget + Vector3.up * height;

		tiltAngle = Mathf.Lerp(tiltAngle, Mathf.Clamp(GetTargetTilt() * 0.7f, -Mathf.PI / 2, Mathf.PI / 2), 0.05f);
		transform.RotateAround(transform.forward, tiltAngle);

		//Linecast and solve camera clipping.
		RaycastHit hit = new RaycastHit();
		if (Physics.Linecast(targetPos, position, out hit, TTSUtils.ExceptLayerMask(new int[]{10, 2}))) {
			if (hit.collider.gameObject.rigidbody == null) {
				Vector3 buffer = (hit.point - position).normalized * 2.5f;
				position = hit.point;
			}
		}

		// Camera Distance according to accel
		cameraDistance = Mathf.Lerp(cameraDistance, Mathf.Clamp(cameraBaseDistance + racerAccel * 2.0f, cameraMinDistance, cameraMaxDistance), 0.05f);

		height = Mathf.Lerp(height, Mathf.Max(baseHeight - racerVelocity.magnitude * 0.01f, minHeight), 0.1f);

		// Camera FOV according to speed
		camera.fov = Mathf.Lerp(camera.fov, TTSUtils.Remap(racerVelocity.magnitude, 40, racerTopSpeed * fovStartEffect, Base_FOV, Max_FOV, true), 0.05f);

		if (GetComponent<Vignetting>().chromaticAberration > 0.0f) {
			GetComponent<Vignetting>().chromaticAberration = Mathf.Lerp(GetComponent<Vignetting>().chromaticAberration, 0.0f, 0.07f);
		}
	}

	private float GetTargetTilt() {
		Vector3 right = TTSUtils.FlattenVector(target.right).normalized;
		float angle = Vector3.Angle(target.up, right) - 90.0f;
		return Mathf.Deg2Rad * angle;
	}

	public void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawCube(hitPos, Vector3.one);
	}

	void ThirdPerson ()
	{
		racerTopSpeed = target.parent.GetComponent<TTSRacer>().TopSpeed;
		
		// Early out if we don't have a target
		if (!target)
			return;

		// Speed squared magnitude
		float speed = target.parent.GetComponent<TTSRacer> ().rigidbody.velocity.magnitude;

		// Calculate the current rotation angles
		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;

		// Change the height that the camera should be above the ball
		height = Mathf.Lerp (height, baseHeight - TTSUtils.Remap (speed, racerTopSpeed * heightStartEffect, racerTopSpeed, 0, baseHeight - minHeight, true), 0.03f);

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

		if (Input.GetKey (KeyCode.LeftShift)) {
			transform.RotateAround (Vector3.up, 180);
		}

		// Set the height of the camera
		transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z);
		transform.position -= currentRotation * Vector3.forward * distance;

		//Linecast and solve camera clipping.
		RaycastHit hit = new RaycastHit ();
		if (Physics.Linecast (target.position, transform.position, out hit, TTSUtils.ExceptLayerMask(10))) {
			if (hit.collider.gameObject.isStatic) {
				transform.position = Vector3.Lerp (transform.position, hit.point, 0.5f);
			}
		}

		// Camera Distance according to speed
		distance = Mathf.Lerp (distance, TTSUtils.Remap (speed, racerTopSpeed * distanceStartEffect, racerTopSpeed, baseDistance, maxDistance, true), 0.02f);

		// Camera FOV according to speed
		camera.fov = Mathf.Lerp (camera.fov, TTSUtils.Remap (speed, 40, racerTopSpeed * fovStartEffect, Base_FOV, Max_FOV, true), 0.05f);

		// Camera Shake. (Note: If statement can be removed)
		if (speed > racerTopSpeed * shakeStartEffect) {

			transform.position += Random.insideUnitSphere * TTSUtils.Remap (speed, racerTopSpeed * shakeStartEffect, racerTopSpeed, 0, shakiness, true);

			transform.LookAt (target);

			transform.position += Random.insideUnitSphere * TTSUtils.Remap (speed, racerTopSpeed * shakeStartEffect, racerTopSpeed, 0, shakiness / 2, true);
		} else {
			transform.LookAt (target);
		}

		tiltAngle = Mathf.Lerp (tiltAngle, Mathf.Clamp (target.parent.GetComponent<TTSRacer> ().GetTiltAngle (), -Mathf.PI / 2, Mathf.PI / 2), 0.05f);

		transform.RotateAround (transform.forward, tiltAngle);

		if (GetComponent<Vignetting> ().chromaticAberration > 0.0f) {
			GetComponent<Vignetting> ().chromaticAberration = Mathf.Lerp (GetComponent<Vignetting> ().chromaticAberration, 0.0f, 0.07f);
		}


	}

	public void DoDamageEffect ()
	{
		GetComponent<Vignetting> ().chromaticAberration = 100;
		transform.position += Random.onUnitSphere * 2;
	}
}