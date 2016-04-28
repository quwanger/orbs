using UnityEngine;
using System.Collections;

public class TTSCameraBoostEffect : TTSPerishingBehaviour {
	
	private float startingFov;
	private float fromFov;
	private float multiplier = 1.9f;
	

	void Awake() {
		startingFov = GetComponent<Camera>().fov;
		fromFov = Mathf.Clamp(GetComponent<Camera>().fov * multiplier, startingFov, 150.0f);
	}
	
	protected override void OnPerishingUpdate(float progress) {
		GetComponent<Camera>().fov = Mathf.Lerp(GetComponent<Camera>().fov, Mathf.Lerp(fromFov, startingFov, progress), 0.05f);
	}
}
