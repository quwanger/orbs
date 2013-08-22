using UnityEngine;
using System.Collections;

public class TTSDisclaimerMessage : TTSBehaviour {
	
	private float alpha = 0.0f;
	private bool fadingOut = false;
	private Texture2D texture;
	
	void Awake() {
		texture = new Texture2D(1,1);
		texture.SetPixel(0,0,new Color(1f,1f,1f,1f));
		texture.Apply();
		
	}
	
	void OnGUI () {		
		GUI.color = new Color(0f,0f,0f,alpha*0.8f);
		GUI.skin.box.normal.background = texture;
		GUI.Box(new Rect(0,0, Screen.width, Screen.height), "");
		GUI.color = new Color(1F,1F,1F,alpha);
		GUI.skin.label.alignment = TextAnchor.UpperCenter;
		GUI.skin.label.fontSize = 25;
		GUI.skin.label.fontStyle = FontStyle.Bold;

		var message = " Welcome to the 0.1.0 Alpha release of Studio236's ORBS." +
			"\n\n" +
			" This is an extremely early release so please keep in mind that only about 5% of the game is implemented at the moment." +
			" You will be trying out a short level in a game mode called 'Sprint'. The goal of Sprint is to make it to the end as fast as possible, utilizing boosts and time bonus powerups as you go." +
			"\n\n" +
			" Please fill out the feedback form that will appear at the end of your experience. All feedback is greatly appreciated and will help us to develop a fun and exciting game for gamers alike." +
			"\n\n" +
			" Thank you for playing!" +
			"\n\n" +
			" Use an Xbox controller or the keyboard using WASD or the arrow keys to control your GREYHND, and the 'space' bar or the 'a' button on the Xbox controller to activate your boost powerup.";		
		message.ToUpper ();
		
		GUI.Label (new Rect(Screen.width/4,Screen.height/9,Screen.width/2,Screen.height), message);
	}
	
	void Update() {
		if(!fadingOut) {
			alpha = Mathf.Lerp(alpha, 1.0f, 0.01f);
		} else {
			alpha = Mathf.Lerp(alpha, -0.1f, 0.03f);
		}
		
		if(alpha < 0.0f) {
			Destroy (this);
		}
	}
	
	
	
	public void Kill() {
		fadingOut = true;		
	}
}
