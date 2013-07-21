using UnityEngine;
using System.Collections.Generic;

public class TTSDrezzStone : MonoBehaviour {
	
	public List<GameObject> Emitters;
	public GameObject EmitterToSpawn;
	public GameObject RegisteredTargets;
	
	void OnTriggerEnter(Collider other) {
		if(!other.gameObject.isStatic) {
			GameObject newemitter = (GameObject) Instantiate(EmitterToSpawn, this.transform.position, this.transform.rotation);
			newemitter.transform.parent = this.transform;
			newemitter.GetComponent<LightningBolt>().target = other.gameObject.transform;
			Emitters.Add(newemitter);
		}
	}
	
	void OnTriggerExit(Collider other) {
		if(!other.gameObject.isStatic) {
		
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
