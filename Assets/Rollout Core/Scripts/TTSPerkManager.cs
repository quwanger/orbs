using UnityEngine;
using System.Collections;

public class TTSPerkManager : TTSBehaviour {
	
	public float distance;
	public GameObject finishlineObject;
	public bool hasBoosted = false;
	public GameObject BoostPrefab;
	
	public Perks EquiptPerks;
	
	public bool hot = false;
	
	// Update is called once per frame
	void Update ()
	{
		if(!hot) { 
			hot = true;
			HotStart();
		}
		
		distance = Vector3.Distance(this.transform.position, finishlineObject.transform.position);
		
		if(distance < 170 && !hasBoosted){
			hasBoosted = true;
			PhotoFinish();		
		}
		
		//IncreasedAcceleration();
	}
	
	public void PhotoFinish() {
		TTSRacerSpeedBoost boost = gameObject.AddComponent<TTSRacerSpeedBoost>();	
		boost.FireBoost(BoostPrefab);
		
		//GetComponent<TTSPowerup>().SuperCBooster(2);	
		
		boost.duration = 2.0f;
		boost.TargetForce = 80.0f;
		vfx.BoostEffect(1.0f);
	}
	
	public void HotStart() {
		
		var randomNum = Random.Range(0,2);
		
		switch(randomNum) {
			case 0:
			this.GetComponent<TTSPowerup>().AvailablePowerup = Powerup.EntropyCannon;
			break;
			
			case 1:
			this.GetComponent<TTSPowerup>().AvailablePowerup = Powerup.DrezzStones;
			break;
			
			case 2:
			this.GetComponent<TTSPowerup>().AvailablePowerup = Powerup.SuperC;
			break;
			
			default:
			//Play a sound?
			break;
		}
	}
	
	/*public void IncreasedAcceleration() {
		this.GetComponent<TTSRacer>().Acceleration = 3000.0f;
	}*/
	
}

