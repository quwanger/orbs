using UnityEngine;
using System.Collections;

public class TTSPerkManager : TTSBehaviour {
	
	//general use variables
	public PerkType equiptPerkPool1;
	public PowerupType equiptPerkPool2;
	public GameObject hudPerkPool1;
	public GameObject hudPerkPool2;
	
	//photofinish variables
	private float distance;
	private bool hasBoosted = false;
	public GameObject BoostPrefab;
	public GameObject finishlineObject;
	
	//hot start variables
	private bool hot = false;
	
	void Awake(){
		switch(equiptPerkPool1) {
			case PerkType.Acceleration:
				doAcceleration();
				break;
			case PerkType.DiamondCoat:
				doDiamondCoat();
				break;
			case PerkType.Handling:
				doHandling();
				break;
			case PerkType.ManOWar:
				doManOWar();
				break;
			case PerkType.Evolution:
				doEvolution();
				break;
			case PerkType.Speed:
				doSpeed();
				break;
			default:
				break;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch(equiptPerkPool1) {
			case PerkType.PhotoFinish:
				doPhotoFinish();
				break;
			case PerkType.HotStart:
				if(!hot && level.raceHasStarted){
					doHotStart();
				}
			break;
			default:
				break;
		}
	}
	
	private void doPhotoFinish() {
		distance = Vector3.Distance(this.transform.position, finishlineObject.transform.position);
		
		if(distance < 170 && !hasBoosted){
			hasBoosted = true;
			TTSRacerSpeedBoost boost = gameObject.AddComponent<TTSRacerSpeedBoost>();	
			boost.FireBoost(BoostPrefab);
			boost.duration = 2.0f;
			boost.TargetForce = 80.0f;
			vfx.BoostEffect(1.0f);
		}
	}
	
	private void doHotStart() {
		hot = true;
		PowerupType p = getRandomPowerup();
		this.GetComponent<TTSPowerup>().GivePowerup(p);
	}
	
	private void doAcceleration(){
		this.GetComponent<TTSRacer>().Acceleration += (accelerationIncrease * 3);
	}
	
	private void doSpeed(){
		this.GetComponent<TTSRacer>().TopSpeed += (speedIncrease * 3);
	}
	
	private void doHandling(){
		this.GetComponent<TTSRacer>().Handling += (handlingIncrease * 3);
	}
	
	private void doManOWar(){
		this.GetComponent<TTSRacer>().Offense += offenseIncrease;
	}
	
	private void doEvolution(){
		this.GetComponent<TTSRacer>().Handling += handlingIncrease;
		this.GetComponent<TTSRacer>().TopSpeed += speedIncrease;
		this.GetComponent<TTSRacer>().Acceleration += accelerationIncrease;
	}
	
	private void doDiamondCoat(){
		this.GetComponent<TTSRacer>().Defense += defenseIncrease;
	}
	
	private PowerupType getRandomPowerup () {
	
		PowerupType powerup = GetRandomEnum<PowerupType>();
		
		while(powerup == PowerupType.None || powerup == PowerupType.TimeBonus) {
			powerup = GetRandomEnum<PowerupType>();
		}
		
		//Debug.Log (powerup);
		
		return powerup;
	}
	
}
