using UnityEngine;
using System.Collections;

public class TTSBlur : TTSBehaviour {
	
	private float topBlur = 0.3f;
	private BlurEffect blur;
	private bool fadingOut = false;
	
	// Use this for initialization
	void Awake () {
		blur = (BlurEffect) this.gameObject.AddComponent<BlurEffect>(); 
		blur.blurShader = Shader.Find("Hidden/BlurEffectConeTap");
	}
	
	void Update() {
		if(!fadingOut) {
			blur.blurSpread = Mathf.Lerp(blur.blurSpread, topBlur, 0.01f);
		} else {
			blur.blurSpread = Mathf.Lerp(blur.blurSpread, -0.3f, 0.03f);
		}
		
		if(blur.blurSpread < 0.0f) {
			Destroy(blur);
			Destroy (this);
		}
	}
	
	public void Kill() {
		fadingOut = true;		
	}
}
