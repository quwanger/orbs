using UnityEngine;
using System.Collections;

public class TTSHudPerk : MonoBehaviour {
	
	public Texture[] perkPool1Images = new Texture[9];
	public Texture[] perkPool2Images = new Texture[9];
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void InitializePerkPool1(TTSBehaviour.PerksPool1 p){
		switch(p) {
			case TTSBehaviour.PerksPool1.Acceleration:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[0]);
			break;
			
			case TTSBehaviour.PerksPool1.DiamondCoat:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[1]);
			break;
			
			case TTSBehaviour.PerksPool1.Evolution:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[2]);
			break;
			
			case TTSBehaviour.PerksPool1.Handling:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[3]);
			break;
			
			case TTSBehaviour.PerksPool1.HotStart:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[4]);
			break;
			
			case TTSBehaviour.PerksPool1.ManOWar:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[5]);
			break;
			
			case TTSBehaviour.PerksPool1.None:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[6]);
			break;
			
			case TTSBehaviour.PerksPool1.PhotoFinish:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[8]);
			break;
			
			case TTSBehaviour.PerksPool1.Speed:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[9]);
			break;
			
			default:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[6]);
			break;
		}
	}
	
	public void InitializePerkPool2(TTSBehaviour.Powerup p){
		switch(p) {
			case TTSBehaviour.Powerup.EntropyCannon:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[0]);
			break;
			
			case TTSBehaviour.Powerup.Shockwave:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[1]);
			break;
			
			case TTSBehaviour.Powerup.Shield:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[2]);
			break;
			
			case TTSBehaviour.Powerup.Leech:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[3]);
			break;
			
			case TTSBehaviour.Powerup.Helix:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[4]);
			break;
			
			case TTSBehaviour.Powerup.DrezzStones:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[5]);
			break;
			
			case TTSBehaviour.Powerup.SuperC:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[6]);
			break;
			
			case TTSBehaviour.Powerup.Lottery:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[7]);
			break;
			
			case TTSBehaviour.Powerup.None:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[8]);
			break;
			
			default:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[8]);
			break;
		}
	}
	
}
