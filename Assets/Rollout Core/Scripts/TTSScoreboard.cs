using UnityEngine;
using System.Collections;

public class TTSScoreboard : TTSBehaviour {
	
	private Perlin noise;
	private float speed = 0.001f;
	private bool startedDeath = false;
	
	private TTSBlur blur;
	private TTSScoreboardContent content;
	
	
	// Use this for initialization
	void Start () {
		blur = this.gameObject.AddComponent<TTSBlur>();
		content = this.gameObject.AddComponent<TTSScoreboardContent>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey (KeyCode.R) && !startedDeath) {
			StartCoroutine("Kill");
			Application.LoadLevel(Application.loadedLevel);
		}
	}
	
	public IEnumerator Kill() {
		startedDeath = true;
		blur.Kill();
		content.Kill();
		yield return new WaitForSeconds(2.0f);
		level.StartCountdown();
		Destroy (this);
		
	}
	
	
}
