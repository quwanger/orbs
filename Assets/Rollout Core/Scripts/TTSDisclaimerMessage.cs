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
		GUI.skin.label.font = level.fonts[0];
		GUI.skin.label.fontSize = 18;
		GUI.Label (new Rect (10,10,Screen.width/2,Screen.height), 
		" Welcome To the Pre-Alpha Release of Studio236's Codename Rollout. This is an extremely early release so please keep in mind that about 10% of the game is implemented at the moment." +
		" You will be trying out a short level in a game mode called 'Sprint'. The Goal of sprint is to make it to the end as fast as possible, utilizing boosts and time bonus powerups as you go." +
		" Please, leave us feedback at the end of the game to anything that you though was cool or implemented wrong. We will also be logging some anonymous info about the way you play this game to our servers" + 
		" Thank you for playing!" 
		);
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
