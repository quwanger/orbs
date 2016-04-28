using UnityEngine;
using System.Collections.Generic;

public class TTSDrezzStone : MonoBehaviour
{
	
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
				if(other.gameObject.GetComponent<TTSRacer>()){
					if(other.gameObject.GetComponent<TTSRacer>().hasShield){
						if(other.gameObject.GetComponentInChildren<TTSShield>().tier3){
							other.gameObject.GetComponent<TTSPowerup>().GivePowerup(TTSBehaviour.PowerupType.DrezzStones);
							other.gameObject.GetComponentInChildren<TTSShield>().duration = 2.0f;
							other.gameObject.GetComponentInChildren<TTSShield>().absorbEffect.Play();
							Destroy(this.gameObject);
							Destroy(this);
						}
					}else{
						other.GetComponent<TTSRacer>().DamageRacer(0.5f + (0.5f*offensiveMultiplier));
					}
				}
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

	void Update() {
		if (netHandler != null && netHandler.owner) {
			netHandler.UpdatePowerup(transform.position, transform.rotation.eulerAngles, GetComponent<Rigidbody>().velocity);
		}
		else if (netHandler != null) {
			GetNetworkUpdate();
		}
	}

	void OnDestroy() {
		if (netHandler != null) {
			netHandler.DeregisterFromClient();
			netHandler = null;
		}
	}

	#region networking

	TTSPowerupNetHandler netHandler;

	public void SetNetHandler(TTSPowerupNetHandler handler) {
		this.netHandler = handler;
	}

	private void GetNetworkUpdate() {
		if (netHandler.isNetworkUpdated) {
			if (netHandler.netPosition != Vector3.zero) {
				transform.position = Vector3.Lerp(transform.position, netHandler.netPosition, netHandler.networkInterpolation);
			}
			transform.rotation = Quaternion.Euler(netHandler.netRotation);
			GetComponent<Rigidbody>().velocity = netHandler.netSpeed;

			netHandler.isNetworkUpdated = false;
			netHandler.framesSinceNetData = 0;
		}
		else {
			netHandler.framesSinceNetData++;
			if (netHandler.framesSinceNetData >= TTSPowerupNetHandler.ExplodeTimeout) {
				//Explode(true);
			}
		}
	}
	#endregion
}
