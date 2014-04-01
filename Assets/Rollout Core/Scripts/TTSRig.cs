using UnityEngine;
using System.Collections.Generic;

public class TTSRig : MonoBehaviour {

	public float rotationSpeed;
	
	//attributes
	public float rigAcceleration;
	public float rigSpeed;
	public float rigHandling;
	
	public float rigOffense = 1.0f;
	public float rigDefense = 1.0f;

	public string rigName;

	public List<Transform> powerupPositionsFront = new List<Transform>();
	public List<Transform> powerupPositionsBack = new List<Transform>();
	public TTSBehaviour.RigType rigType;
	
	void Update () {
		
		//this rig is nested under display mesh so that it keeps the same position
		//below the velocity of the racer 2.0 is accessed in order to determine the rotation speed
		//rotationSpeed = transform.parent.parent.rigidbody.velocity.magnitude*50f;
		//transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
	}
}
