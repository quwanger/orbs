using UnityEngine;
using System.Collections.Generic;

public class WaypointManager : MonoBehaviour {
	
	
	private List<GameObject> waypoints = new List<GameObject>();
	
	// Use this for initialization
	void Start () {
		
		GameObject[] tempWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");;
		
		foreach(GameObject waypoint in tempWaypoints) {
			
			waypoints.Add(waypoint);
			
		}
		
		//Assign unique indexes
		int lastIndex = 0;
		foreach(GameObject waypoint in tempWaypoints) {
			waypoint.GetComponent<Waypoint>().index = lastIndex;
			lastIndex++;
		}
		
		
	}
	
	void OnDrawGizmos() {
	}
	
	
}
