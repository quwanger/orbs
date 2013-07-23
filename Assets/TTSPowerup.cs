using UnityEngine;
using System.Collections;

public class TTSPowerup : TTSBehaviour {
	
	public bool debug = false;
	public int DebugTier = 2;
	
	public Powerup AvailablePowerup;
	public int tier = 1;
	
	#region Prefab Assignment
	public GameObject DrezzStonePrefab;
	public GameObject EntropyCannonPrefab;
	public GameObject TimeBonusPrefab;
	#endregion
	
	#region monobehaviour methods
	void Update() {
		if(Input.GetKeyDown("1")) DrezzStone(DebugTier);
		if(Input.GetKeyDown("2")) EntropyCannon(DebugTier);
		if(Input.GetKeyDown("4")) TimeBonus();
		
		if(Input.GetKeyDown(KeyCode.Space)) ConsumePowerup();
	
	}
	#endregion
	
	#region donation methods
	public void GivePowerup(Powerup powerup) {
		if(powerup == AvailablePowerup && tier < 3) {
			tier++;
		} else if(powerup == Powerup.TimeBonus) {
			TimeBonus();
		} else {
			AvailablePowerup = powerup;
			tier = 1;
		}
	}   
	
	public void ConsumePowerup() {
		
		switch(AvailablePowerup) {
			case Powerup.EntropyCannon:
			EntropyCannon(tier);
			break;
			
			case Powerup.DrezzStones:
			DrezzStone(tier);
			break;
			
			
			default:
			//Play a sound?
			break;
		}
		
		this.AvailablePowerup = Powerup.None;
		this.tier = 1;
		
		
	}
	
	#endregion
	
	
	#region public methods
	public void DrezzStone(int _tier) {
		if(_tier == 1) {
			DropDrezzStone();
		}
		
		if(_tier == 2) {
			DropDrezzStone();
			Invoke("DropDrezzStone", 0.5f);
		}
		if(_tier == 3) {
			for(int i = 0; i < 5; i++) {
				Invoke("DropDrezzStone", i * 0.5f);
			}
		}
		
	}
	
	public void EntropyCannon(int _tier) {
		if(_tier == 1) {
			FireEntropyCannon();
		}
		
		if(_tier == 2) {
			for(int i = 0; i < 5; i++) {
				Invoke("FireEntropyCannon", i * 0.1f);
			}
		}
		if(_tier == 3) {
			for(int i = 0; i < 10; i++) {
				Invoke("FireEntropyCannon", i * 0.1f);
			}
		}
	}
	
	public void TimeBonus() {
		GiveTimeBonus();
	}
	
	
	#endregion
	
	#region internal methods
	private void DropDrezzStone() {
		GameObject go = (GameObject) Instantiate(DrezzStonePrefab, this.transform.position - GetComponent<TTSRacer>().displayMeshComponent.forward * 3.0f, this.transform.rotation);
		go.rigidbody.AddForce(Random.insideUnitCircle * 50f);
	}
	
	private void FireEntropyCannon() {
		GameObject go = (GameObject) Instantiate(EntropyCannonPrefab);
		go.transform.rotation = GetComponent<TTSRacer>().displayMeshComponent.transform.rotation;
		go.transform.position = this.transform.position + GetComponent<TTSRacer>().displayMeshComponent.forward * 3.5f;
		go.rigidbody.velocity = this.rigidbody.velocity.normalized * go.GetComponent<TTSEntropyCannonProjectile>().ProjectileStartVelocity;
	}
	
	private void GiveTimeBonus() {
		level.time.GiveTimeBonus(1.0f);
		GameObject go = (GameObject) Instantiate(TimeBonusPrefab, this.transform.position, this.transform.rotation);
		go.GetComponent<TTSTimeBonusPrefab>().target = this.GetComponent<TTSRacer>().displayMeshComponent.gameObject;
	}
	#endregion
	
}
