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
		
		//Sanity check for indexes
		
		List<int> indeces = new List<int>();
		List<GameObject> tempwaypoints = new List<GameObject>();
		
		
		
		foreach(GameObject waypoint in tempwaypoints) {
			
			//Check for dupes
			
			foreach(int index in indeces) {
				if (waypoint.GetComponent<Waypoint>().index == index) {
					Debug.LogError("Duplicate indeces in waypoints. see index: " + index);
				}
			}
			
			//add the current index
			indeces.Add(waypoint.GetComponent<Waypoint>().index);
			waypoints.Insert(waypoint.GetComponent<Waypoint>().index,waypoint);
		}
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		
		
		
		for(int i = 0; i < waypoints.Count - 1; i++) {
			Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);
		}
	}
	
	
}
