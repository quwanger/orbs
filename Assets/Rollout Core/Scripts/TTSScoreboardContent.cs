using UnityEngine;
using System.Collections;

public class TTSScoreboardContent : TTSBehaviour {
	
	private float alpha = 0.0f;
	private bool fadingOut = false;
	private Texture2D texture;
	
	private bool feedbackOpened = false;

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
		GUI.skin.label.fontSize = 35;
		GUI.skin.label.fontStyle = FontStyle.Bold;
		
		var message = "Your time was: " + 
			GameObject.Find("TTSLevel").GetComponent<TTSTimeManager>().GetCurrentTimeString() +
			"\n\n" + 
			"Pressing any key will take you to our feedback form!" +
			"\n\n" +
			"Press the 'r' key to play again!";
					
		message.ToUpper ();
		
		GUI.Label (new Rect(Screen.width/4,Screen.height/4,Screen.width/2,Screen.height), message);
		
		//GUI.color = Color.yellow;
		// only used for placing "labelRectangle"
		//GUI.Box(new Rect(385,160, 250, 20), "");
		//Rect labelRectangle = new Rect(385,160, 250, 20);
		
		//if (Event.current.type == EventType.MouseUp && labelRectangle.Contains(Event.current.mousePosition))
		if(Input.anyKeyDown && !feedbackOpened && !Input.GetKey(KeyCode.R)){
    		Application.OpenURL("https://docs.google.com/forms/d/1ZUsZfbeh9URL6a-cqio91h_echHk1yCslJDnxZZa8dA/viewform");
			feedbackOpened = true;
		}
	}
	
	void Update() {
		
		if(!fadingOut) {
			alpha = Mathf.Lerp(alpha, 1.0f, 0.1f);
		} else {
			alpha = Mathf.Lerp(alpha, -0.1f, 0.1f);
		}
		
		if(alpha < 0.0f) {
			Destroy (this);
		}
	}
	
	public void Kill() {
		fadingOut = true;		
	}
}
