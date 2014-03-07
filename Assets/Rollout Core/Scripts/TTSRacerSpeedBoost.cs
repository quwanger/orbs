using UnityEngine;
using System.Collections.Generic;

public class TTSRacerSpeedBoost : TTSPerishingBehaviour {

	public float TargetForce = 100.0f;
	private GameObject go;
	private List<TrailRenderer> trailRenderers = new List<TrailRenderer>();
	private bool isPlatform;
	private float _power;
	private GameObject racerBeingBoosted;
	
	void Awake(){
		destroyWhenLifecycleComplete = false; // Once duration has passed, the class will stop running and self-destruct
		useKillFunctionWhenComplete = true; // Execute the kill function once complete

		Debug.Log("Boost Activated!");
	}

	protected override void OnPerishingUpdate(float progress) {
		if(racerBeingBoosted != null){
			if(isPlatform){
				//racerBeingBoosted.rigidbody.AddForce(racerBeingBoosted.rigidbody.velocity.normalized * 2.0f);
				racerBeingBoosted.rigidbody.AddForce(racerBeingBoosted.GetComponent<TTSRacer>().displayMeshComponent.forward * _power);
				Debug.Log(racerBeingBoosted.GetComponent<TTSRacer>().player + " racer " + racerBeingBoosted.GetComponent<TTSRacer>().playerNum + " is receiving a boost.");
			}else{
				racerBeingBoosted.rigidbody.AddForce(racerBeingBoosted.GetComponent<TTSRacer>().displayMeshComponent.forward * Mathf.Lerp (TargetForce, 0.0f, progress));
				Debug.Log(racerBeingBoosted.GetComponent<TTSRacer>().player + " racer " + racerBeingBoosted.GetComponent<TTSRacer>().playerNum + " is receiving a boost.");
			}
		}
	}
	
	public void FireBoost(GameObject booster, GameObject effectedRacer){
		racerBeingBoosted = effectedRacer;
		isPlatform = false;
		go = (GameObject) Instantiate(booster);
		go.transform.parent = racerBeingBoosted.transform;
		go.transform.position = racerBeingBoosted.GetComponent<TTSRacer>().displayMeshComponent.position;
		
		foreach(Transform child in go.transform){
			if(child.gameObject.GetComponent<TrailRenderer>()){
				trailRenderers.Add(child.gameObject.GetComponent<TrailRenderer>());
			}
		}
	}
	
	public void FireBoost(GameObject booster, float power, GameObject effectedRacer){
		racerBeingBoosted = effectedRacer;
		isPlatform = true;
		_power = power;
		go = (GameObject) Instantiate(booster);
		go.transform.parent = racerBeingBoosted.transform;
		go.transform.position = racerBeingBoosted.GetComponent<TTSRacer>().displayMeshComponent.position;
		
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
			}else if(child.GetComponent<TrailRenderer>()){
				child.GetComponent<TrailRenderer>().enabled = false;
			}
		}
		go.transform.parent = null;
		go.transform.position = this.transform.position;
		Invoke("Cleanup", 5);
	}
	
	void Cleanup() {
		Destroy (this.go);
		Destroy (this);
	}
}
