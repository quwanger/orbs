using UnityEngine;
using System.Collections.Generic;

public class TTSRacerSpeedBoost : TTSPerishingBehaviour {

	public float TargetForce = 100.0f;
	private GameObject go;
	private List<TrailRenderer> trailRenderers = new List<TrailRenderer>();
	
	protected override void OnPerishingUpdate(float progress) {
		rigidbody.AddForce(GetComponent<TTSRacer>().displayMeshComponent.forward * Mathf.Lerp (TargetForce, 0.0f, progress));
		
	}
	
	public void FireBoost(GameObject booster){
		go = (GameObject) Instantiate(booster);
		go.transform.parent = GetComponent<TTSRacer>().transform;
		go.transform.position = GetComponent<TTSRacer>().displayMeshComponent.position;
		
		foreach(Transform child in go.transform){
			if(child.gameObject.GetComponent<TrailRenderer>()){
				trailRenderers.Add(child.gameObject.GetComponent<TrailRenderer>());
			}
		}
	}
	
	protected override void Kill(){
		foreach(Transform child in go.transform) {
			if(child.GetComponent<ParticleSystem>()) {
				child.GetComponent<ParticleSystem>().Stop();
			}
		}
		go.transform.parent = null;
		go.transform.position = this.transform.position;
		Invoke("Cleanup", 5.0f);
		
	}
	
	void Cleanup() {
		Destroy (this.go);
		Destroy (this);
	}
}
