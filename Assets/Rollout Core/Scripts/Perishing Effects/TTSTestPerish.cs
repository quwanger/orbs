using UnityEngine;
using System.Collections;

public class TTSTestPerish : TTSPerishingBehaviour {
	
	// Template to show how persishing behaviours work
	
	private float FovBy = 20.0f;
	private float Fov;
	
	void Awake() {
		duration = 15.0f;
		Fov = Camera.main.fov;
	}
	
	protected override void OnPerishingUpdate(float progress) {
		Camera.main.fov = Mathf.Lerp (Fov + FovBy, Fov, progress);
	}
}
