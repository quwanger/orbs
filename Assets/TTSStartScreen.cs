using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;

public class TTSStartScreen : MonoBehaviour {
	private GamePadState state;
	private PlayerIndex playerIndex;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		state = GamePad.GetState(PlayerIndex.One);

		if(state.Buttons.Start == ButtonState.Pressed) {
			Application.LoadLevel("hub-world");
		}
	}
}
