using UnityEngine;
using System.Collections.Generic;

public class TTSAIController : MonoBehaviour {
	
	public List<GameObject> waypointsCrossed = new List<GameObject>();
	
	public void seekWaypoint() {
		
		
		
		GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
		
		GameObject closestPoint = waypoints[0];
		
		foreach(GameObject waypoint in waypoints) {
			if(waypoint.GetComponent<TTSWaypoint>().isActive && Vector3.Distance(transform.position, waypoint.transform.position) < Vector3.Distance(transform.position, closestPoint.transform.position)) {
				//if(!Physics.Linecast(transform.position,waypoint.transform.position)) {
					closestPoint = waypoint;	
				//}
			}
		}
		
		if(!waypointsCrossed.Contains(closestPoint)) {
			waypointsCrossed.Add(closestPoint);
		}
		
		
		if(waypointsCrossed.Count > 10) {
			waypointsCrossed[0].GetComponent<TTSWaypoint>().isActive = true;
			waypointsCrossed.RemoveAt(0);	
		}
		
		//GetComponent<SteerForPoint>().TargetPoint = closestPoint.transform.position;
	}
}


