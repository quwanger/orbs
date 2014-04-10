using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class TTSPauseMenu : TTSBehaviour {
	public int changer = 0;
	public GUITexture[] highlighters = new GUITexture[3];
	public bool paused = false;
	public bool isTweening = false;
	private bool joystickY = false;
	
	PlayerIndex playerIndex;
	GamePadState state;
	
	// Use this for initialization
	void Start () {
		highlighters[0].active = true;
		highlighters[1].active = false;
		highlighters[2].active = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 controlDirection = GetControlDirection(1);
		
		//if(Input.GetKeyDown(KeyCode.Escape))
		if(GetButtonDown(1, "start"))
		{
			//Game started
			if(!paused){
				isTweening = true;
				iTween.MoveTo(this.gameObject, iTween.Hash("x", 0.5, "time", 0, "easeType", iTween.EaseType.easeOutCirc, "onComplete", "enteredMenu", "onCompleteTarget", gameObject));
			}
			
		}
		
		//Inside menu
		if(paused){
			if(GetButtonDown(1, "b")) {
				isTweening = true;
				iTween.MoveTo(this.gameObject, iTween.Hash("x", -5, "time", 0, "easeType", iTween.EaseType.easeOutCirc, "onComplete", "leavingMenu", "onCompleteTarget", gameObject));
			}
		}
	
		//if(Input.GetKeyDown(KeyCode.Return) && paused)
		if(GetButtonDown(1, "a") && paused)
		{
			if(changer == 0)
			{
				isTweening = true;
				iTween.MoveTo(this.gameObject, iTween.Hash("x", -5, "time", 0, "easeType", iTween.EaseType.easeOutCirc, "onComplete", "leavingMenu", "onCompleteTarget", gameObject));
			}
			
			if(changer == 1)
			{
				isTweening = true;
				iTween.MoveTo(this.gameObject, iTween.Hash("x", -5, "time", 0, "easeType", iTween.EaseType.easeOutCirc, "onComplete", "leavingMenu", "onCompleteTarget", gameObject));
				Application.LoadLevel(Application.loadedLevel);
			}
			
			if(changer == 2)
			{
				isTweening = true;
				iTween.MoveTo(this.gameObject, iTween.Hash("x", -5, "time", 0, "easeType", iTween.EaseType.easeOutCirc, "onComplete", "leavingMenu", "onCompleteTarget", gameObject));
				Application.LoadLevel("hub-world");
			}
		}

		//if(Input.GetKeyDown(KeyCode.W) && paused){
		if(controlDirection.y > 0.5f && paused && !joystickY) {
			joystickY = true;
			if(changer == 0){
			}
			
			else if(changer == 1 || changer == 2){
				foreach(GUITexture h in highlighters){
					h.active = false;
				}
				changer--;
				highlighters[changer].active = true;
			}
		}
		
		else if(controlDirection.y < -0.5f && paused && !joystickY){
			joystickY = true;
			if(changer == 2){
			}
			
			else if(changer == 0 || changer == 1){
				foreach(GUITexture h in highlighters){
					h.active = false;
				}
				changer++;
				highlighters[changer].active = true;
				
			}
		}else if(controlDirection.y == 0 && joystickY){
			joystickY = false;
		}
	}	
	public void enteredMenu(){
		isTweening = false;
		paused = true;
		//set canMove to false for all racers
				if(racers != null){
					foreach(GameObject racer in racers){
						if(racer.GetComponent<TTSRacer>().playerNum == 1 && racer.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player){
							racer.GetComponent<TTSRacer>().myCamera.camera.rect = new Rect(0, 0, 1.0f, 1.0f);
						}else{
							if(racer.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)
								racer.GetComponent<TTSRacer>().myCamera.active = false;
						}
						racer.GetComponent<TTSRacer>().canMove = false;
						racer.GetComponent<TTSRacer>().pausedVelocity = racer.rigidbody.velocity;
						racer.rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
					}
					if(level != null){
						level.GetComponent<TTSTimeManager>().StopTimer();
					}
				}
	}
	
	public void leavingMenu(){
		isTweening = false;
		paused = false;
		if(racers != null){
					foreach(GameObject racer in racers){
						if(racer.GetComponent<TTSRacer>().playerNum == 1 && racer.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player){
							switch(level.initRace.numHumanPlayers){
							case 1:
								racer.GetComponent<TTSRacer>().myCamera.camera.rect = new Rect(0, 0, 1.0f, 1.0f);
								break;
							case 2:
								racer.GetComponent<TTSRacer>().myCamera.camera.rect = new Rect(0, 0.5f, 1.0f, 0.5f);
								break;
							case 3:
								racer.GetComponent<TTSRacer>().myCamera.camera.rect = new Rect(0, 0.5f, 1.0f, 0.5f);
								break;
							case 4:
								racer.GetComponent<TTSRacer>().myCamera.camera.rect = new Rect(0, 0.5f, 0.5f, 1.0f);
								break;
							}
						}else{
							if(racer.GetComponent<TTSRacer>().player == TTSRacer.PlayerType.Player)
								racer.GetComponent<TTSRacer>().myCamera.active = active;
						}
						racer.GetComponent<TTSRacer>().canMove = true;
						racer.rigidbody.constraints = RigidbodyConstraints.None;
						racer.rigidbody.velocity = racer.GetComponent<TTSRacer>().pausedVelocity;
					}
					if(level != null){
						level.GetComponent<TTSTimeManager>().ResumeTimer();
					}
				}
	}
	
	Vector2 GetControlDirection(int player){
		PlayerIndex playerIndex = PlayerIndex.One;

		switch(player){
			case 1:
				playerIndex = PlayerIndex.One;
				break;

			case 2:
				playerIndex = PlayerIndex.Two;
				break;
			
			case 3:
				playerIndex = PlayerIndex.Three;
				break;
			
			case 4:
				playerIndex = PlayerIndex.Four;
				break;
		}

		state = GamePad.GetState(playerIndex);

		float VInput = (state.DPad.Up == ButtonState.Pressed) ? 1 : ((state.DPad.Down == ButtonState.Pressed)? -1 : 0);
		float HInput = (state.DPad.Right == ButtonState.Pressed) ? 1 : ((state.DPad.Left == ButtonState.Pressed)? -1 : 0);

		return new Vector2(HInput, VInput);
	}
	
	bool GetButtonDown(int player, string button){
		PlayerIndex playerIndex = PlayerIndex.One;

		switch(player){
			case 1:
				playerIndex = PlayerIndex.One;
				break;

			case 2:
				playerIndex = PlayerIndex.Two;
				break;
			
			case 3:
				playerIndex = PlayerIndex.Three;
				break;
			
			case 4:
				playerIndex = PlayerIndex.Four;
				break;
		}

		state = GamePad.GetState(playerIndex);

		switch(button){
			case "A":
			case "a":
				return (state.Buttons.A == ButtonState.Pressed) ? true : false;
			
			case "B":
			case "b":
				return (state.Buttons.B == ButtonState.Pressed) ? true : false;

			case "Start":
			case "start":
				return (state.Buttons.Start == ButtonState.Pressed) ? true : false;
		}

		return false;
	}
}
