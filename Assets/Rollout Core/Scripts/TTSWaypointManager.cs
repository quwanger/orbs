using UnityEngine;
using System.Collections.Generic;

public class TTSWaypointManager : MonoBehaviour {


	public List<TTSWaypoint> waypoints = new List<TTSWaypoint>();
	
	// Use this for initialization
	void Start() {
		
		GameObject[] tempWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
		SortWaypoints(tempWaypoints);
		
		//foreach (GameObject waypoint in tempWaypoints) {
		//	waypoints.Add(waypoint);
		//	Debug.Log(waypoint.GetComponent<TTSWaypoint>().index);
		//	foreach(GameObject wp in waypoints){
		//		if(wp.GetComponent<TTSWaypoint>().index == waypoint.GetComponent<TTSWaypoint>().index){
		//			wp.GetComponent<TTSWaypoint>().hasSibling = waypoint.GetComponent<TTSWaypoint>().hasSibling = true;
		//			waypoint.GetComponent<TTSWaypoint>().AddSibling(wp);
		//			wp.GetComponent<TTSWaypoint>().AddSibling(waypoint);
		//		}
		//	}
		//}
		
		////gets the next waypoint gameobject for each waypoint in the scene
		//foreach (GameObject wp in waypoints){
		//	foreach(GameObject wp2 in waypoints){
		//		if(wp2.GetComponent<TTSWaypoint>().index == (wp.GetComponent<TTSWaypoint>().index + 1)){
		//			wp.GetComponent<TTSWaypoint>().nextWaypoint = wp2;
		//			break;
		//		}
		//	}
		//}
	}
	
	void OnDrawGizmos() {
	}

	// Sorting
	private int CompareWP(GameObject one, GameObject two) {
		return (one.GetComponent<TTSWaypoint>().index - two.GetComponent<TTSWaypoint>().index);
	}

	private void AddWP(GameObject wp) {
		TTSWaypoint newWP = wp.GetComponent<TTSWaypoint>();
		if (waypoints.Count > 0) {
			TTSWaypoint lastWP = waypoints[waypoints.Count - 1].GetComponent<TTSWaypoint>();

			// Check to see if they're siblings
			if (lastWP.index == newWP.index) {
				lastWP.hasSibling = newWP.hasSibling = true;

				newWP.NewSibling(lastWP);
			}
			else {
				lastWP.AddNextWaypoint(newWP);
			}
		}

		waypoints.Add(newWP);
	}

	private void SortWaypoints(GameObject[] original) {
		for (int i = 0; i < original.Length - 1; i++) {
			int index = 0;
			for (int j = 0; j < original.Length - i; j++) {
				index = (CompareWP(original[index], original[j]) < 0) ? index : j;
			}

			GameObject temp = original[index];
			original[index] = original[original.Length - i - 1];
			original[original.Length - i - 1] = temp;

			AddWP(temp);
		}
	}
}
