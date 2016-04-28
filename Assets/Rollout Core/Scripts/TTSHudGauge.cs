using UnityEngine;
using System.Collections;

public class TTSHudGauge : MonoBehaviour {
	
	public enum Type {SPEED};
	
	public Type data;
	public GameObject boundRacer;
	
	private float ratio = 0.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(data == Type.SPEED) {
			this.ratio = boundRacer.GetComponent<Rigidbody>().velocity.magnitude / boundRacer.GetComponent<TTSRacer>().TopSpeed;
			this.GetComponent<Renderer>().material.SetTextureOffset("_MainTex",new Vector2(0, ratio/2));
		}
	}
}
