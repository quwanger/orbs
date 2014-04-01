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
	
	public void InitializePerkPool1(TTSBehaviour.PerkType p){
		switch(p) {
			case TTSBehaviour.PerkType.Acceleration:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[0]);
			break;
			
			case TTSBehaviour.PerkType.DiamondCoat:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[1]);
			break;
			
			case TTSBehaviour.PerkType.Evolution:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[2]);
			break;
			
			case TTSBehaviour.PerkType.Handling:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[3]);
			break;
			
			case TTSBehaviour.PerkType.HotStart:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[4]);
			break;
			
			case TTSBehaviour.PerkType.ManOWar:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[5]);
			break;
			
			case TTSBehaviour.PerkType.None:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[6]);
			break;
			
			case TTSBehaviour.PerkType.PhotoFinish:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[7]);
			break;
			
			case TTSBehaviour.PerkType.Speed:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[8]);
			break;
			
			default:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool1Images[6]);
			break;
		}
	}
	
	public void InitializePerkPool2(TTSBehaviour.PowerupType p){
		switch(p) {
			case TTSBehaviour.PowerupType.EntropyCannon:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[0]);
			break;
			
			case TTSBehaviour.PowerupType.Shockwave:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[1]);
			break;
			
			case TTSBehaviour.PowerupType.Shield:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[2]);
			break;
			
			case TTSBehaviour.PowerupType.Leech:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[3]);
			break;
			
			case TTSBehaviour.PowerupType.Helix:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[4]);
			break;
			
			case TTSBehaviour.PowerupType.DrezzStones:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[5]);
			break;
			
			case TTSBehaviour.PowerupType.SuperC:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[6]);
			break;
			
			case TTSBehaviour.PowerupType.Lottery:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[7]);
			break;
			
			case TTSBehaviour.PowerupType.None:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[8]);
			break;
			
			default:
			this.gameObject.renderer.material.SetTexture("_MainTex", perkPool2Images[8]);
			break;
		}
	}
	
}
