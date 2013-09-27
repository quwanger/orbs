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
		
		//gets the next waypoint gameobject for each waypoint in the scene
		foreach (GameObject wp in waypoints){
			foreach(GameObject wp2 in waypoints){
				if(wp2.GetComponent<TTSWaypoint>().index == (wp.GetComponent<TTSWaypoint>().index + 1)){
					wp.GetComponent<TTSWaypoint>().nextWaypoint = wp2;
					break;
				}
			}
		}
	}
	
	void OnDrawGizmos() {
	}
}
