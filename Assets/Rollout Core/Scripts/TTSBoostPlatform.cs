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
				Debug.Log(racer.GetComponent<TTSRacer>().player + " racer " + racer.GetComponent<TTSRacer>().playerNum + " hit a boost platform.");
				boost.duration = 1.0f;
				boost.TargetForce = 35.0f;

				boost.FireBoost(BoostPrefab, BoostPower, racer);
			}
		}
	}
}
