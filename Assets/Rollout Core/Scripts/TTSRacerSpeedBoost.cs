using UnityEngine;
using System.Collections.Generic;

public class TTSRacerSpeedBoost : TTSPerishingBehaviour {

	public float TargetForce = 100.0f;
	public bool owner = true;
	private GameObject go;
	private List<TrailRenderer> trailRenderers = new List<TrailRenderer>();
	private bool isPlatform;
	private float _power;
	
	void Awake(){
		destroyWhenLifecycleComplete = false; // Once duration has passed, the class will stop running and self-destruct
		useKillFunctionWhenComplete = true; // Execute the kill function once complete

		TTSRacer racer = GetComponent<TTSRacer>();
		if(racer.player == TTSRacer.PlayerType.Player)
			racer.myCamera.GetComponent<TTSCameraEffects>().BoostEffect(1.4f);

	}

	protected override void OnPerishingUpdate(float progress) {
		if (!owner)	return;

		if(isPlatform){
			//racerBeingBoosted.rigidbody.AddForce(racerBeingBoosted.rigidbody.velocity.normalized * 2.0f);
			this.rigidbody.AddForce(GetComponent<TTSRacer>().displayMeshComponent.forward * _power);
		}else{
			this.rigidbody.AddForce(GetComponent<TTSRacer>().displayMeshComponent.forward * Mathf.Lerp(TargetForce, 0.0f, progress));
		}
	}
	
	public void FireBoost(GameObject booster){
		isPlatform = false;
		go = (GameObject) Instantiate(booster);
		go.transform.parent = this.gameObject.transform;
		go.transform.position = this.gameObject.GetComponent<TTSRacer>().displayMeshComponent.position;
		
		foreach(Transform child in go.transform){
			if(child.gameObject.GetComponent<TrailRenderer>()){
				trailRenderers.Add(child.gameObject.GetComponent<TrailRenderer>());
			}
		}
	}
	
	public void FireBoost(GameObject booster, float power, GameObject effectedRacer){
		isPlatform = true;
		_power = power;
		go = (GameObject) Instantiate(booster);
		go.transform.parent = transform;
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
