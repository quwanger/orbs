using UnityEngine;
using System.Collections;

public class TTSHudPowerup : MonoBehaviour {
		
	public Texture[] powerupImages = new Texture[9];
	public TTSHudPowerupLevel hpl;
	public GameObject powerupFrame;
	public GameObject powerupLevel;
	
	public float frameScaleFactor = 1.05f;
	public float frameScaleTime = 0.3f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void UpdateHudPowerup(TTSBehaviour.PowerupType p, int t){
		switch(p) {
			case TTSBehaviour.PowerupType.EntropyCannon:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[0]);
			break;
			
			case TTSBehaviour.PowerupType.Shockwave:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[1]);
			break;
			
			case TTSBehaviour.PowerupType.Shield:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[2]);
			break;
			
			case TTSBehaviour.PowerupType.Leech:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[3]);
			break;
			
			case TTSBehaviour.PowerupType.Helix:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[4]);
			break;
			
			case TTSBehaviour.PowerupType.DrezzStones:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[5]);
			break;
			
			case TTSBehaviour.PowerupType.SuperC:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[6]);
			break;
			
			case TTSBehaviour.PowerupType.None:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[8]);
			break;
			
			default:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[0]);
			break;
		}
		
		if(t!=3){
			iTween.ScaleTo(powerupFrame,iTween.Hash("scale", new Vector3 (0.3f, 0.3f, 0.3f) ,"time", 0.1f, "oncomplete", "ReturnToOriginalSize", "oncompletetarget", this.gameObject));
			iTween.ScaleTo(powerupLevel,iTween.Hash("scale", new Vector3 (0.3f, 0.3f, 0.09231377f) ,"time", 0.1f, "oncomplete", "ReturnToOriginalSize", "oncompletetarget", this.gameObject));
			iTween.ScaleTo(this.gameObject,iTween.Hash("scale", new Vector3 (0.3f, 0.3f, 0.3f) ,"time", 0.1f, "oncomplete", "ReturnToOriginalSize", "oncompletetarget", this.gameObject));
			
			//powerupFrame.renderer.material.color = Color.white;
			//powerupLevel.renderer.material.color = Color.white;
			//this.gameObject.renderer.material.color = Color.white;
		}else{
			iTween.ScaleTo(powerupFrame,iTween.Hash("scale", new Vector3 (0.4f, 0.4f, 0.4f) ,"time", 0.1f, "oncomplete", "ReturnToOriginalSize", "oncompletetarget", this.gameObject));
			iTween.ScaleTo(powerupLevel,iTween.Hash("scale", new Vector3 (0.4f, 0.4f, 0.121854f) ,"time", 0.1f, "oncomplete", "ReturnToOriginalSize", "oncompletetarget", this.gameObject));
			iTween.ScaleTo(this.gameObject,iTween.Hash("scale", new Vector3 (0.4f, 0.4f, 0.4f) ,"time", 0.1f, "oncomplete", "ReturnToOriginalSize", "oncompletetarget", this.gameObject));
			
			//powerupFrame.renderer.material.color = Color.cyan;
			//powerupLevel.renderer.material.color = Color.cyan;
			//this.gameObject.renderer.material.color = Color.cyan;
		}
		
		hpl.UpdatePowerupLevel(t);
	}
	
	public void ReturnToOriginalSize(){
		iTween.ScaleTo(powerupFrame,iTween.Hash("scale", new Vector3 (0.4f,0.4f,0.4f) ,"time", frameScaleTime));
		iTween.ScaleTo(powerupLevel,iTween.Hash("scale", new Vector3 (0.2f,0.2f,0.06092709f) ,"time", frameScaleTime));
		iTween.ScaleTo(this.gameObject,iTween.Hash("scale", new Vector3 (0.2f,0.2f,0.2f) ,"time", frameScaleTime));
	}
}
