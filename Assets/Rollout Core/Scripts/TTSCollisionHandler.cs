using UnityEngine;
using System.Collections;

// Not sure if this is being used...
public class TTSCollisionHandler : MonoBehaviour {
	
	
	public bool colliding = false;
	
	// Use this for initialization
	void Start() {
	
	}
	
	// Update is called once per frame
	void Update() {
	
	}
	
	void OnCollisionEnter(Collision collision) {
		colliding = true;	
	}
		
	void OnCollisionStay() {
		colliding = true;	
	}
		
	void OnCollisionExit() {
		colliding = false;	
	}
}
