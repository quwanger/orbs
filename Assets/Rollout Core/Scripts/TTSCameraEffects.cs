using UnityEngine;
using System.Collections;

public class TTSCameraEffects : TTSBehaviour {
	public void DamageEffect(float intensity) {
		TTSDamageEffect[] des = (TTSDamageEffect[]) gameObject.GetComponents<TTSDamageEffect>().Clone();
		foreach(TTSDamageEffect de in des) {
			Destroy(de);
		}
		gameObject.AddComponent<TTSDamageEffect>();
	}
}
