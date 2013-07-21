using UnityEngine;
using System.Collections;

public class TTSCameraEffects : TTSBehaviour {
	public void DamageEffect(float intensity) {
		if(gameObject.GetComponent<TTSDamageEffect>() != null) {
			gameObject.AddComponent<TTSDamageEffect>();
		} else {
			Destroy(gameObject.GetComponent<TTSDamageEffect>());
			gameObject.AddComponent<TTSDamageEffect>();
		}
	}
}
