using UnityEngine;
using System.Collections;

public class TTSPerkManager : TTSBehaviour {
	
	//general use variables
	public PerksPool1 equiptPerkPool1;
	public Powerup equiptPerkPool2;
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
			case PerksPool1.Acceleration:
				doAcceleration();
				break;
			case PerksPool1.DiamondCoat:
				doDiamondCoat();
				break;
			case PerksPool1.Handling:
				doHandling();
				break;
			case PerksPool1.HotStart:
				doHotStart();
				break;
			case PerksPool1.ManOWar:
				doManOWar();
				break;
			case PerksPool1.Evolution:
				doEvolution();
				break;
			case PerksPool1.Speed:
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
			case PerksPool1.PhotoFinish:
				doPhotoFinish();
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
			boost.FireBoost(BoostPrefab, this.gameObject);
			boost.duration = 2.0f;
			boost.TargetForce = 80.0f;
			vfx.BoostEffect(1.0f);
		}
	}
	
	private void doHotStart() {
		if(!hot && level.raceHasStarted){
			//Debug.Log("Hot Start");
			hot = true;
			Powerup p = getRandomPowerup();
			this.GetComponent<TTSPowerup>().GivePowerup(p);
		}
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
	
	private Powerup getRandomPowerup () {
	
		Powerup powerup = GetRandomEnum<Powerup>();
		
		while(powerup == Powerup.None || powerup == Powerup.TimeBonus) {
			powerup = GetRandomEnum<Powerup>();
		}
		
		//Debug.Log (powerup);
		
		return powerup;
	}
	
}
