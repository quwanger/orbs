using UnityEngine;
using System.Collections.Generic;

public class TTSFloatHud : TTSBehaviour {
	
	public TextMesh timeDisplay; 
	public Transform boundCamera;
	
	public List<GameObject> ammo = new List<GameObject>();
	private int previousAmmo = -1;

	public GameObject racerToFollow;
	
	private Color initialBackingColor;
	public Color flashColor;
	
	public float frameScaleTime = 0.3f;
	
	#region hudcomponents
	public GameObject displayBacking;
	public GameObject bonusTime;
	public GameObject position;
	public GameObject wrongway;
	
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
		
		if(racerToFollow.GetComponent<TTSRacer>().goingWrongWay){
			if(!wrongway.active)
				wrongway.active = true;
		}else{
			if(wrongway.active)
				wrongway.active = false;
		}	

		if(previousPlace != racerToFollow.GetComponent<TTSRacer>().place){
			//if the racer is in a new position
			position.GetComponent<TextMesh>().text = (racerToFollow.GetComponent<TTSRacer>().place).ToString();
			iTween.ScaleTo(position,iTween.Hash("scale", new Vector3 (1.5f, 1.5f, 1.5f) ,"time", 0.1f, "oncomplete", "ReturnToOriginalSize", "oncompletetarget", this.gameObject));
		}else{
		}
			
		previousPlace = racerToFollow.GetComponent<TTSRacer>().place;

		if(level.GetComponent<TTSLevel>().DebugMode == false){
			if(racerToFollow.GetComponent<TTSPowerup>().ammo != previousAmmo){
				DisableAllAmmo();
				DisplayAmmo();
				previousAmmo = racerToFollow.GetComponent<TTSPowerup>().ammo;
			}
		}

	}

	public void DisableAllAmmo(){
		foreach(GameObject _ammo in ammo){
			if(_ammo.active)
				_ammo.active = false;
		}
	}

	public void DisplayAmmo(){
		for(int i=0; i<racerToFollow.GetComponent<TTSPowerup>().ammo; i++){
			ammo[i].active = true;
		}
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
