using UnityEngine;
using System.Collections;

public class TTSTimeBonusPrefab : TTSPerishingBehaviour {
	
	public GameObject target;
	public GameObject display;
	
	public ParticleSystem system;
	
	public AudioClip[] audio;
	
	void Awake () {
		this.duration = 2.0f;
		this.useKillFunctionWhenComplete = true;
		sfx.PlayOneShot(audio[Random.Range(0,audio.Length)]);
		hud.FlashTimeForBonus();
	}
	
	protected override void OnPerishingUpdate (float progress){
		transform.position = Vector3.Lerp(transform.position, target.transform.position,0.5f);
		transform.rotation = Quaternion.Slerp(transform.rotation, target.transform.rotation,0.1f);
	}
	
	protected override void Kill() {
		Destroy(display);
		system.Emit(1000);
		Invoke("Cleanup", 1.0f);
	}
	
	void Cleanup() {
		Destroy(gameObject);
		
	}
}
