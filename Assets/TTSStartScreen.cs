using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;

public class TTSStartScreen : MonoBehaviour {
	private GamePadState state;
	private PlayerIndex playerIndex;

	private AudioSource MusicPlayer;
	public AudioClip songToPlay;

	// Use this for initialization
	void Start () {
		this.GetComponent<TTSCameraFade>().SetScreenOverlayColor(new Color(0, 0, 0, 1.0f));
		this.GetComponent<TTSCameraFade>().StartFade(new Color(0,0,0,0), 2.0f);
	}
	
	// Update is called once per frame
	void Update () {
		state = GamePad.GetState(PlayerIndex.One);

		if(state.Buttons.Start == ButtonState.Pressed || Input.GetKeyDown("return")) {
			this.GetComponent<TTSCameraFade>().StartFade(new Color(0,0,0,1.0f), 2.0f);
			Invoke("LoadHubWorld", 2.0f);
		}
	}

	void LoadHubWorld(){
		Application.LoadLevel("hub-world");
	}
}
