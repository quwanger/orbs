using UnityEngine;
using System.Collections.Generic;

public class TTSRacerSpeedBoost : TTSPerishingBehaviour {

	public float TargetForce = 100.0f;
	public bool owner = true;
	private GameObject go;
	private List<TrailRenderer> trailRenderers = new List<TrailRenderer>();
	private bool isPlatform;
	private float _power;

	//tier 3 boost
	public bool isTier3 = false;
	public float homingRadius = 25.0F;
	
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

		int counter = 0;

		foreach(TrailRenderer tr in trailRenderers){
			tr.gameObject.transform.position = this.gameObject.GetComponent<TTSPowerup>().GetSpecificBackPP(counter).position;
			counter++;
		}

		if(isTier3){
			//handle tier 3 shizzzzz
			Collider[] colliders = Physics.OverlapSphere(this.transform.position, homingRadius);
		    foreach (Collider hit in colliders) {
		        if (hit.GetComponent<TTSRacer>() && hit.gameObject != this.gameObject){
		        	//push racer
		        	Vector3 directionVector = (hit.gameObject.transform.position - this.gameObject.transform.position).normalized;
		        	hit.gameObject.rigidbody.AddForce(directionVector * 5.0f);
				}
		    }
		}
	}
	
	public void FireBoost(GameObject booster){
		isPlatform = false;
		go = (GameObject) Instantiate(booster);
		go.transform.parent = this.gameObject.transform;
		go.transform.position = this.gameObject.transform.position;
		
		int counter = 0;

		foreach(Transform child in go.transform){
			if(child.gameObject.GetComponent<TrailRenderer>()){
				trailRenderers.Add(child.gameObject.GetComponent<TrailRenderer>());
				child.gameObject.transform.position = this.gameObject.GetComponent<TTSPowerup>().GetSpecificBackPP(counter).position;
				counter++;
			}
		}
	}
	
	public void FireBoost(GameObject booster, float power){
		isPlatform = true;
		_power = power;
		go = (GameObject) Instantiate(booster);
		go.transform.parent = transform;
		go.transform.position = this.gameObject.transform.position;
		
		int counter = 0;

		foreach(Transform child in go.transform){
			if(child.gameObject.GetComponent<TrailRenderer>()){
				trailRenderers.Add(child.gameObject.GetComponent<TrailRenderer>());
				child.gameObject.transform.position = this.gameObject.GetComponent<TTSPowerup>().GetSpecificBackPP(counter).position;
				counter++;
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
