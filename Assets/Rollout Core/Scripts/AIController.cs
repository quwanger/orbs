using UnityEngine;
using System.Collections.Generic;

public class AIController : MonoBehaviour {
	
	public float AI_STEERING_ABILITY = 0.02f;
	public int AI_ANTI_TRAP_DISTANCE = 10;
	public List<int> waypointsCrossed = new List<int>();
	GameObject closestGameObject;	// Use this for initialization
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	GameObject findClosestGO(GameObject[] objects) {
	
		closestGameObject = objects[0];

		foreach(GameObject gobject in objects) {
			if(Vector3.Distance(gobject.transform.position,this.transform.position) < AI_ANTI_TRAP_DISTANCE && !this.waypointsCrossed.Contains(gobject.GetComponent<Waypoint>().index)) {
				
				if(gobject.GetComponent<Waypoint>().isLastWaypoint) {
					waypointsCrossed.Clear();
				}
				
				this.waypointsCrossed.Add(gobject.GetComponent<Waypoint>().index);
				
			}
			if(Vector3.Distance(gobject.transform.position,this.transform.position) < Vector3.Distance(closestGameObject.transform.position,this.transform.position) && !this.waypointsCrossed.Contains(gobject.GetComponent<Waypoint>().index)) {
					closestGameObject = gobject;
			}
			
		}
		
		if(waypointsCrossed.Count > 10) {
			this.waypointsCrossed.RemoveAt(0);
		}
		
		
	
		return closestGameObject;
	}
	
	public Vector3 doAISeek() {
		GameObject target = findClosestGO(GameObject.FindGameObjectsWithTag("Waypoint"));
	 	this.GetComponent<MainMovement>().Magnitude = 1000;
		return Vector3.RotateTowards(this.GetComponent<MainMovement>().RealForward,target.transform.position - this.transform.position,AI_STEERING_ABILITY,0.0f);	
	}
	
	void OnDrawGizmos() {
		if(closestGameObject != null) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(this.transform.position,closestGameObject.transform.position);
		}
	}
	
	
}


