using UnityEngine;
using System.Collections;

public class TTSPowerup : MonoBehaviour {
	
	public bool debug = false;
	public int DebugTier = 2;
	
	#region Prefab Assignment
	public GameObject DrezzStonePrefab;
	public GameObject EntropyCannonPrefab;
	#endregion
	
	#region monobehaviour methods
	void Update() {
		if(Input.GetKeyDown("1")) DrezzStone(DebugTier);
		if(Input.GetKeyDown("2")) EntropyCannon(DebugTier);
	}
	#endregion
	
	
	#region public methods
	public void DrezzStone(int tier) {
		if(tier == 1) {
			DropDrezzStone();
		}
		
		if(tier == 2) {
			DropDrezzStone();
			Invoke("DropDrezzStone", 0.5f);
		}
		if(tier == 3) {
			for(int i = 0; i < 5; i++) {
				Invoke("DropDrezzStone", i * 0.5f);
			}
		}
		
	}
	
	public void EntropyCannon(int tier) {
		if(tier == 1) {
			FireEntropyCannon();
		}
		
		if(tier == 2) {
			for(int i = 0; i < 5; i++) {
				Invoke("FireEntropyCannon", i * 0.1f);
			}
		}
		if(tier == 3) {
			for(int i = 0; i < 10; i++) {
				Invoke("FireEntropyCannon", i * 0.1f);
			}
		}
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
	#endregion
	
}
