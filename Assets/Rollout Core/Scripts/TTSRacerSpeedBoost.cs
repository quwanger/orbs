using UnityEngine;
using System.Collections.Generic;

public class TTSRacerSpeedBoost : TTSPerishingBehaviour {

	public float TargetForce = 100.0f;
	public float boostDuration = -1.0f;
	private GameObject go;
	private List<TrailRenderer> trailRenderers = new List<TrailRenderer>();
	
	private bool generateParticles = true;
	
	void Awake () {
		this.useKillFunctionWhenComplete = true;
	}
	protected override void OnPerishingUpdate(float progress) {
		if(progressSinceBirth < boostDuration){
			rigidbody.AddForce(GetComponent<TTSRacer>().displayMeshComponent.forward * Mathf.Lerp (TargetForce, 0.0f, progress));
		}
		else if(generateParticles == true){
			stopParticles();
			generateParticles = false;
		}
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
	
	private void stopParticles(){
		foreach(Transform child in go.transform) {
			if(child.GetComponent<ParticleSystem>()) {
				child.GetComponent<ParticleSystem>().Stop();
			}
		}
		go.transform.parent = null;
		go.transform.position = this.transform.position;
		Invoke("Cleanup", 5.0f);
	}
	
	protected override void Kill(){
		
		Destroy(go);
	}
	
	void Cleanup() {
		Destroy (this.go);
		Destroy (this);
	}
}
