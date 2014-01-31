using UnityEngine;
using System.Collections;

public class TTSDisclaimerCamera : TTSBehaviour {
	
	private Perlin noise;
	//private float speed = 0.001f;
	private bool startedDeath = false;
	
	private TTSBlur blur;
	private TTSDisclaimerMessage message;
	
	
	// Use this for initialization
	void Start () {
		blur = this.gameObject.AddComponent<TTSBlur>();
		message = this.gameObject.AddComponent<TTSDisclaimerMessage>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.anyKeyDown && !startedDeath) {
			StartCoroutine("Kill");
		}
	}
	
	public IEnumerator Kill() {
		startedDeath = true;
		blur.Kill();
		message.Kill();
		yield return new WaitForSeconds(2.0f);
		level.StartCountdown();
		Destroy (this);
		
	}
	
	
}
