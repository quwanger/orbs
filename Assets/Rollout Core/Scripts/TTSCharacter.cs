using UnityEngine;
using System.Collections;

public class TTSCharacter : MonoBehaviour {
	
	public float rotationSpeed;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//rotationSpeed = transform.parent.rigidbody.velocity.magnitude*50f;
		//transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
	}
}
