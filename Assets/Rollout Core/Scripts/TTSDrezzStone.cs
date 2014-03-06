using UnityEngine;
using System.Collections.Generic;

public class TTSDrezzStone : MonoBehaviour {
	
	public List<GameObject> Emitters;
	public GameObject EmitterToSpawn;
	public GameObject RegisteredTargets;
	
	public float offensiveMultiplier;
	
	private bool isActive = false;
	
	void Start(){
		Invoke("ActivateDrezzstone", 0.5f);
	}
	
	private void ActivateDrezzstone(){
		isActive = true;
	}
	
	void OnTriggerEnter(Collider other) {
		
		if(isActive){
		
			if(offensiveMultiplier == null)
				offensiveMultiplier = 1.0f;
			
			if(!other.gameObject.isStatic && !other.gameObject.GetComponent<TTSWaypoint>() && other.gameObject.tag != "Track") {
				GameObject newemitter = (GameObject) Instantiate(EmitterToSpawn, this.transform.position, this.transform.rotation);
				newemitter.transform.parent = this.transform;
				newemitter.GetComponent<LightningBolt>().target = other.gameObject.transform;
				if(other.gameObject.GetComponent<TTSRacer>())
					other.GetComponent<TTSRacer>().DamageRacer(0.5f + (0.5f*offensiveMultiplier));
				Emitters.Add(newemitter);
			}
			
		}
	}
	
	void OnTriggerExit(Collider other) {
		
		if(isActive){
		
			if(!other.gameObject.isStatic && !other.gameObject.GetComponent<TTSWaypoint>() && other.gameObject.tag != "Track") {
			
				List<GameObject> EmittersClone = Emitters;
				foreach(GameObject go in EmittersClone.ToArray()) {
					if(go != null && go.GetComponent<LightningBolt>() != null && go.GetComponent<LightningBolt>().target != null) {
						if(other.gameObject == go.GetComponent<LightningBolt>().target.gameObject) {
							Emitters.Remove(go);
							Destroy(go);
							break;
						}
					}else{
						Emitters.Remove(go);
					
					}
				}
			}
			
		}
	}
}
