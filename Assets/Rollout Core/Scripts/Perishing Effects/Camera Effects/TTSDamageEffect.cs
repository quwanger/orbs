using UnityEngine;
using System.Collections;

public class TTSDamageEffect : TTSPerishingBehaviour {
	
	private float fromValue = 20.0f;
	private float baseValue;

	private float fromBlur = 2.0f;
	private float baseBlur = 0.14f;
	
	void Awake() {
		duration = 0.5f;
		baseValue = GetComponent<Vignetting>().chromaticAberration;
		//baseBlur = GetComponent<Vignetting>().blur;
	}
	
	protected override void OnPerishingUpdate(float progress) {
		GetComponent<Vignetting>().chromaticAberration = Mathf.Lerp(fromValue, baseValue, progress);// *(Random.value * 0.5f + 1.0f);
		GetComponent<Vignetting>().blur = Mathf.Lerp(fromBlur, baseBlur, progress);
	}
}
