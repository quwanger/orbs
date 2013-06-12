using UnityEngine;
using System.Collections;

	
public class TTSSceneTransition : MonoBehaviour {
	
	private float alphaFadeValue = 1.0f;
	private float transitionTimeIn = 6.0f;
	private float transitionTimeOut = 6.0f;
	private bool fadeIn = true;
	private bool fadeOut = false;
	private AsyncOperation loader;
	public Texture Overlay;
	private bool transitioning = false;
	
	void Awake() {
		AudioListener.volume = 0.0f;
	}
	
	void OnGUI() {
		if(fadeIn && alphaFadeValue > 0.0f) {
			alphaFadeValue -= Mathf.Clamp01(Time.deltaTime / transitionTimeIn);
			AudioListener.volume = (1.0f - alphaFadeValue);	
		} else if(fadeIn) {
			fadeIn = false;
			alphaFadeValue = 0.0f;
		}
		
		if(loader != null) {
			if(!fadeIn && fadeOut && alphaFadeValue < 1.0f && loader.progress > 0.7f) {
				alphaFadeValue += Mathf.Clamp01(Time.deltaTime / transitionTimeOut);
				AudioListener.volume = (1.0f - alphaFadeValue);	
			} else if(!fadeIn && fadeOut && loader.progress > 0.7f) {
				Debug.Log ("Got Here");
				loader.allowSceneActivation = true;
			}
		}
		
		GUI.color = new Color(0, 0, 0, alphaFadeValue);
		GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height ), Overlay);
	}
	
	
	public void transitionToSceneWithLoad(string scene) {
		Debug.Log ("called.");
		if(!transitioning) {
			transitioning = true;
			Debug.Log ("Started async...");
			loader = Application.LoadLevelAsync (scene);
			fadeOut = true;
			loader.allowSceneActivation = false;
		}
	}
}
