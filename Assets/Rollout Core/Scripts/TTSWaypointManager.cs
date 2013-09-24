using UnityEngine;
using System.Collections.Generic;

public class TTSWaypointManager : MonoBehaviour {
	
	
	public List<GameObject> waypoints = new List<GameObject>();
	
	// Use this for initialization
	void Start() {
		
		GameObject[] tempWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
		
		foreach (GameObject waypoint in tempWaypoints) {
			waypoints.Add(waypoint);
			foreach(GameObject wp in waypoints){
				if(wp.GetComponent<TTSWaypoint>().index == waypoint.GetComponent<TTSWaypoint>().index){
					wp.GetComponent<TTSWaypoint>().hasSibling = waypoint.GetComponent<TTSWaypoint>().hasSibling = true;
					waypoint.GetComponent<TTSWaypoint>().AddSibling(wp);
					wp.GetComponent<TTSWaypoint>().AddSibling(waypoint);
				}
			}
		}
		//Assign unique indexes
		int lastIndex = 0;
		foreach (GameObject waypoint in tempWaypoints) {
			waypoint.GetComponent<TTSWaypoint>().index = lastIndex;
			lastIndex++;
		}
	}
	
	void OnDrawGizmos() {
	}
}
