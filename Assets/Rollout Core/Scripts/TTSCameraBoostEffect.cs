using UnityEngine;
using System.Collections;

public class TTSCameraBoostEffect : TTSPerishingBehaviour {
	
	private float startingFov;
	private float fromFov;
	private float multiplier = 1.9f;
	

	void Awake() {
		startingFov = camera.fov;
		fromFov = camera.fov * multiplier;
	}
	
	protected override void OnPerishingUpdate(float progress) {
		camera.fov = Mathf.Lerp(camera.fov, Mathf.Lerp(fromFov, startingFov, progress), 0.05f);
	}
}
