using UnityEngine;
using System.Collections;



public class TTSHud : MonoBehaviour {
	
	public enum HudMode {NO_HUD, TIME_TRIAL, RACE};
	
	public Font font;
	
	public Vector2 offset = new Vector2(0,0);
	
	private float SCREEN_PADDING = 20.0f;
	
	private Vector3 lastEulerAngles;
	private Vector3 lastPosition;
	
	public GUISkin skin;
	
	/* LIKELY TO BE TRASHED COMPLETELY */
	
	// Update is called once per frame
	void OnGUI () {
		
		CalculateOffset();
		
		GUI.skin = skin;
		
		//Start with the container
		GUI.BeginGroup(new Rect(-offset.x,-offset.y,Screen.width,Screen.height));
		
		GUIStyle headerTextStyle = skin.label;
		headerTextStyle.alignment = TextAnchor.UpperCenter;
		GUI.Label(new Rect(SCREEN_PADDING,SCREEN_PADDING,Screen.width - SCREEN_PADDING,Screen.height*0.1f - (SCREEN_PADDING*2)),GameObject.Find("Time Manager").GetComponent<TTSTimeManager>().GetCurrentTimeString(), headerTextStyle);

		
		GUI.EndGroup();
		
			
	}
	
	void CalculateOffset() {
		
		
		
		offset.x = Mathf.Lerp(offset.x, (transform.localEulerAngles.y - lastEulerAngles.y)*90, 0.01f);
		offset.y = Mathf.Lerp(offset.y, (lastPosition.y - transform.position.y)*200, 0.1f);
		
		Vector2.ClampMagnitude(offset,50);
		
		lastEulerAngles = transform.localEulerAngles;
		lastPosition = transform.position;
	}
}
