using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Basic camera to be centered on a target
/// </summary>
public class TTSCameraController : MonoBehaviour {

	public float distance = 10.0f;
	
	public float xSpeed = 250.0f;
	public float ySpeed = 120.0f;
	
	public float yMinLimit = -20.0f;
	public float yMaxLimit = 80.0f;
	
	public Transform target;
	
	private float x = 0.0f;
	private float y = 0.0f;
	
	
	void Start() {
	    Vector3 angles = transform.eulerAngles;
	    x = angles.y;
	    y = angles.x;
	    
	    target = GameObject.FindGameObjectWithTag("Player").transform;
	   
	
		// Make the rigid body not change rotation
	   	if (rigidbody)
			rigidbody.freezeRotation = true;
	}
	
	void LateUpdate() {
	    if (target) {
	        x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
	        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
	 		
	 		y = ClampAngle(y, yMinLimit, yMaxLimit);
	 		       
	        Quaternion rotation = Quaternion.Euler(y, x, 0);
	        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
	        
	        transform.rotation = rotation;
	        transform.position = position;
	    }
	}
	
	static float ClampAngle (float angle, float min, float max) {
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
}