using UnityEngine;
using System.Collections.Generic;

public class TTSRacerSpeedBoost : TTSPerishingBehaviour {

	public float TargetForce = 100.0f;
	private GameObject go;
	private List<TrailRenderer> trailRenderers = new List<TrailRenderer>();
	private bool isPlatform;
	private float _power;
	public bool destroyWhenLifecycleComplete = false; // Once duration has passed, the class will stop running and self-destruct
	public bool useKillFunctionWhenComplete = true; // Execute the kill function once complete
	

	protected override void OnPerishingUpdate(float progress) {
		Debug.Log("Running\n");
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
	
	protected override void Kill(){
		foreach(Transform child in go.transform) {
			if(child.GetComponent<ParticleSystem>()) {
				child.GetComponent<ParticleSystem>().Stop();
			}
		}
		go.transform.parent = null;
		go.transform.position = this.transform.position;
		Invoke("Cleanup", 5);
	}
	
	void Cleanup() {
		Debug.Log("Cleanup\n");
		Debug.Log(this.go);
		Destroy (this.go);
		Destroy (this);
	}
}
