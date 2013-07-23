using UnityEngine;
using System.Collections.Generic;

public class TTSRacerSpeedBoost : TTSPerishingBehaviour {

	public float TargetForce = 100.0f;
	private GameObject go;
	private List<TrailRenderer> trailRenderers = new List<TrailRenderer>();
	
	protected override void OnPerishingUpdate(float progress) {
		rigidbody.AddForce(GetComponent<TTSRacer>().displayMeshComponent.forward * Mathf.Lerp (TargetForce, 0.0f, progress));
		
		foreach(TrailRenderer tr in trailRenderers.ToArray()){
			tr.time = Mathf.Lerp(tr.time, 0.0f, progress/5.0f);
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
	
	protected override void Kill(){
		Destroy(this.go);
	}
}
