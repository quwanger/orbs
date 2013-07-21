using UnityEngine;
using System.Collections;

public class TTSDamageEffect : TTSPerishingBehaviour {
	
	private float fromValue = 25.0f;
	private float baseValue;
	
	void Awake() {
		duration = 3.0f;
		baseValue = GetComponent<Vignetting>().chromaticAberration;
	}
	
	protected override void OnPerishingUpdate(float progress) {
		GetComponent<Vignetting>().chromaticAberration = Mathf.Lerp(fromValue, baseValue, progress);
	}
}
