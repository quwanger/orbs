using UnityEngine;
using System.Collections;

public class TTSHudPowerupLevel : MonoBehaviour {
	
	public Texture[] levelImages = new Texture[4];
	
	public void UpdatePowerupLevel(int tier){
		
		switch(tier) {
			case 0:
			this.gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", levelImages[0]);
			break;
			
			case 1:
			this.gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", levelImages[1]);
			break;
			
			case 2:
			this.gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", levelImages[2]);
			break;
			
			case 3:
			this.gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", levelImages[3]);
			break;
			
			default:
			this.gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", levelImages[0]);
			break;
		}
	}
}
