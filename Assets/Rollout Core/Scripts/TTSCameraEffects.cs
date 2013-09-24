using UnityEngine;
using System.Collections;

/*
 * This class is referenced in TTSBehaviour.cs (Also inherits from it)
 * Any class extending TTSBehaviour can reference the camera effects via this.vfx
 */
public class TTSCameraEffects : TTSBehaviour {
	public void DamageEffect(float intensity) {
		TTSDamageEffect[] des = (TTSDamageEffect[]) gameObject.GetComponents<TTSDamageEffect>().Clone();
		foreach(TTSDamageEffect de in des) {
			Destroy(de);
		}
		gameObject.AddComponent<TTSDamageEffect>();
	}
	
	public void BoostEffect(float duration) {
		TTSCameraBoostEffect bo = gameObject.AddComponent<TTSCameraBoostEffect>();
		bo.duration = 1.0f;
	}
}
