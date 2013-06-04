using UnityEngine;
using System.Collections.Generic;

public class TTSAIController : MonoBehaviour {
	
	public float AI_STEERING_ABILITY = 0.02f;
	public int AI_ANTI_TRAP_DISTANCE = 20;
	public List<int> waypointsCrossed = new List<int>();
	GameObject closestGameObject;	// Use this for initialization
	public GameObject UnitySteerComponent;
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	GameObject findClosestGO(GameObject[] objects) {
	
		closestGameObject = objects[0];

		foreach(GameObject gobject in objects) {
			if(Vector3.Distance(gobject.transform.position,this.GetComponent<TTSRacer>().body.transform.position) < AI_ANTI_TRAP_DISTANCE && !this.waypointsCrossed.Contains(gobject.GetComponent<TTSWaypoint>().index)) {
				
				if(gobject.GetComponent<TTSWaypoint>().isLastWaypoint) {
					waypointsCrossed.Clear();
				}
				
				this.waypointsCrossed.Add(gobject.GetComponent<TTSWaypoint>().index);
				
			}
			if(Vector3.Distance(gobject.transform.position,this.GetComponent<TTSRacer>().body.transform.position) < Vector3.Distance(closestGameObject.transform.position,this.GetComponent<TTSRacer>().body.transform.position) && !this.waypointsCrossed.Contains(gobject.GetComponent<TTSWaypoint>().index)) {
				RaycastHit hit = new RaycastHit();	
				if(!Physics.Raycast(gobject.transform.position,gobject.transform.position - gobject.GetComponent<TTSWaypoint>().transform.position,out hit,100.0f)) {
					closestGameObject = gobject;
				}
				
				
				
			}
			
		}
		
		if(waypointsCrossed.Count > 10) {
			this.waypointsCrossed.RemoveAt(0);
		}
		
		
	
		return closestGameObject;
	}
	
	public Vector3 doAISeek() {
		UnitySteerComponent.GetComponent<SteerForPoint>().TargetPoint = findClosestGO(GameObject.FindGameObjectsWithTag("Waypoint")).transform.position;
		return UnitySteerComponent.transform.forward;
	}
	
	void OnDrawGizmos() {
		if(closestGameObject != null) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(this.GetComponent<TTSRacer>().body.transform.position,closestGameObject.transform.position);
		}
	}
	
	
}


