using UnityEngine;
using System.Collections;

public class TTSBoostPlatform : TTSBehaviour {
	
	public GameObject BoostPrefab;
	public float BoostPower = 50.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider collider){	
		foreach(GameObject racer in racers) {
			if(collider.gameObject == racer) {
				TTSRacerSpeedBoost boost = racer.AddComponent<TTSRacerSpeedBoost>();
				boost.FireBoost(BoostPrefab, BoostPower);
		
				boost.duration = 0.4f;
				boost.TargetForce = 35.0f;
				vfx.BoostEffect(0.4f);
			}
		}
	}
}
