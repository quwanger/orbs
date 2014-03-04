using UnityEngine;
using System.Collections;

public class TTSFloatHud : TTSBehaviour {
	
	public TextMesh timeDisplay; 
	public Transform boundCamera;
	
	public GameObject racerToFollow;
	
	private Color initialBackingColor;
	public Color flashColor;
	
	public float frameScaleTime = 0.3f;
	
	#region hudcomponents
	public GameObject displayBacking;
	public GameObject bonusTime;
	public GameObject position;
	
	public int previousPlace;
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
		
		if(previousPlace != racerToFollow.GetComponent<TTSRacer>().place){
			//if the racer is in a new position
			position.GetComponent<TextMesh>().text = (racerToFollow.GetComponent<TTSRacer>().place).ToString();
			iTween.ScaleTo(position,iTween.Hash("scale", new Vector3 (1.5f, 1.5f, 1.5f) ,"time", 0.1f, "oncomplete", "ReturnToOriginalSize", "oncompletetarget", this.gameObject));
		}else{
		}
			
		previousPlace = racerToFollow.GetComponent<TTSRacer>().place;
	}
	
	public void ReturnToOriginalSize(){
		iTween.ScaleTo(position,iTween.Hash("scale", new Vector3 (1.0f,1.0f,1.0f) ,"time", frameScaleTime));
	}
	
#region vfx
	public void FlashTimeForBonus() {
		displayBacking.renderer.material.color = flashColor;
		bonusTime.GetComponent<TextMesh>().text = "-" + time.bonusTime.ToString();
	}
#endregion
}
