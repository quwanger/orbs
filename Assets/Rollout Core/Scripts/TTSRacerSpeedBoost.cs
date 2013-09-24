using UnityEngine;
using System.Collections.Generic;

public class TTSRacerSpeedBoost : TTSPerishingBehaviour {

	public float TargetForce = 100.0f;
	public float boostDuration = -1.0f;
	private GameObject go;
	private List<TrailRenderer> trailRenderers = new List<TrailRenderer>();
	private bool isPlatform;
	private float _power;
	
	private bool generateParticles = true;
	
	void Awake () {
		this.useKillFunctionWhenComplete = true;
	}
	protected override void OnPerishingUpdate(float progress) {
		/*if(progressSinceBirth < boostDuration){
			rigidbody.AddForce(GetComponent<TTSRacer>().displayMeshComponent.forward * Mathf.Lerp (TargetForce, 0.0f, progress));
		}
		else if(generateParticles == true){
			stopParticles();
			generateParticles = false;
		}*/

		if(isPlatform){
			rigidbody.AddForce(GetComponent<TTSRacer>().displayMeshComponent.forward * _power);
			Debug.Log("IsPlatform!");
		}else{
			rigidbody.AddForce(GetComponent<TTSRacer>().displayMeshComponent.forward * Mathf.Lerp (TargetForce, 0.0f, progress));
		}
	}
	
	public void FireBoost(GameObject booster){
		isPlatform = false;
		go = (GameObject) Instantiate(booster);
		go.transform.parent = GetComponent<TTSRacer>().transform;
		go.transform.position = GetComponent<TTSRacer>().displayMeshComponent.position;
		
		foreach(Transform child in go.transform){
			if(child.gameObject.GetComponent<TrailRenderer>()){
				trailRenderers.Add(child.gameObject.GetComponent<TrailRenderer>());
			}
		}
	}
	
	public void FireBoost(GameObject booster, float power){
		isPlatform = true;
		_power = power;
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
