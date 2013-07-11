using UnityEngine;
using System.Collections;

public class TTSRig : MonoBehaviour {

	public float turnSpeed;
	
	// Update is called once per frame
	void Update () {
		//turnSpeed = transform.parent.parent.rigidbody.velocity.magnitude*50f;
		turnSpeed = transform.parent.parent.rigidbody.velocity.magnitude*50f;
		transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
	}
}
