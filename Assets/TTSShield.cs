using UnityEngine;
using System.Collections;

public class TTSShield : TTSPerishingBehaviour {
	
	public void DeployShield(int level){
		if(level == 1){
		}
		if(level == 2){
		}
		if(level == 3){
		}
	}
	
	protected override void Kill(){
		Destroy(this);
		Destroy(this.gameObject);
	}
}
