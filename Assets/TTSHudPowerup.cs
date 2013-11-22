using UnityEngine;
using System.Collections;

public class TTSHudPowerup : MonoBehaviour {
		
	public Texture[] powerupImages = new Texture[9];
	public TTSHudPowerupLevel hpl;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void UpdateHudPowerup(TTSBehaviour.Powerup p, int t){
		switch(p) {
			case TTSBehaviour.Powerup.EntropyCannon:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[0]);
			break;
			
			case TTSBehaviour.Powerup.Shockwave:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[1]);
			break;
			
			case TTSBehaviour.Powerup.Shield:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[2]);
			break;
			
			case TTSBehaviour.Powerup.Leech:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[3]);
			break;
			
			case TTSBehaviour.Powerup.Helix:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[4]);
			break;
			
			case TTSBehaviour.Powerup.DrezzStones:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[5]);
			break;
			
			case TTSBehaviour.Powerup.SuperC:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[6]);
			break;
			
			case TTSBehaviour.Powerup.None:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[8]);
			break;
			
			default:
			this.gameObject.renderer.material.SetTexture("_MainTex", powerupImages[0]);
			break;
		}
		
		hpl.UpdatePowerupLevel(t);
	}
}
