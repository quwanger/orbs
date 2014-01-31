using UnityEngine;
using System.Collections;

public class TTSLightFlicker : MonoBehaviour {
	
	//private float minFlickerSpeed = 0.3f;
	//private float maxFlickerSpeed = 1.0f;
	private float randomTemp;
	public float offChance = 0.1f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		randomTemp = Random.Range(0.0f, 1.0f);
		
		if(randomTemp < offChance) light.enabled = false;
		else light.enabled = true;
		

	}
}
