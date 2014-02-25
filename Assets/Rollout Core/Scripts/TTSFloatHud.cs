using UnityEngine;
using System.Collections;

public class TTSFloatHud : TTSBehaviour {
	
	public TextMesh timeDisplay; 
	public Transform boundCamera;
	
	public GameObject racerToFollow;
	
	private Color initialBackingColor;
	public Color flashColor;
	
	#region hudcomponents
	public GameObject displayBacking;
	public GameObject bonusTime;
	public GameObject position;
	#endregion
	
	// Use this for initialization
	void Start () {
		initialBackingColor = displayBacking.renderer.material.color;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 transcam = boundCamera.position + (boundCamera.forward * 10f);
		transform.position = Vector3.Lerp(transform.position, transcam,0.9f);
		transform.rotation = Quaternion.Lerp(transform.rotation, boundCamera.rotation, 0.7f);
		timeDisplay.text = GameObject.Find("TTSLevel").GetComponent<TTSTimeManager>().GetCurrentTimeString();
		
		displayBacking.renderer.material.color = Color.Lerp(displayBacking.renderer.material.color, initialBackingColor, 0.04f);
		
		position.GetComponent<TextMesh>().text = (racerToFollow.GetComponent<TTSRacer>().place).ToString();
	}
	
	
	
#region vfx
	public void FlashTimeForBonus() {
		displayBacking.renderer.material.color = flashColor;
		bonusTime.GetComponent<TextMesh>().text = "-" + time.bonusTime.ToString();
	}
#endregion
}
