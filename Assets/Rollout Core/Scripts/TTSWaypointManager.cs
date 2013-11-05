using UnityEngine;
using System.Collections.Generic;

public class TTSWaypointManager : MonoBehaviour {


	public List<TTSWaypoint> allWaypoints = new List<TTSWaypoint>();
	public List<List<TTSWaypoint>> waypointLevels = new List<List<TTSWaypoint>>();
	
	// Use this for initialization
	void Start() {
		SortWaypoints(GameObject.FindGameObjectsWithTag("Waypoint"));
	}
	
	void OnDrawGizmos() {
	}

	public TTSWaypoint getClosestWP(int index, Vector3 pos) {
		List<TTSWaypoint> waypoints = waypointLevels[index];
		TTSWaypoint closest = null;

		foreach (TTSWaypoint waypoint in waypoints) {
			if (closest == null || (pos - waypoint.GetClosestPoint(pos)).magnitude < (pos - closest.GetClosestPoint(pos)).magnitude) {
				closest = waypoint;
			}
		}

		return closest;
	}

	public TTSWaypoint getFarthestSeen(int index, Vector3 pos) {
		List<TTSWaypoint> waypoints = waypointLevels[index];
		TTSWaypoint farthest = null;

		foreach (TTSWaypoint waypoint in waypoints) {
			//if (farthest == null || ) {
			//	farthest = waypoint;
			//}
		}

		return farthest;
	}

	// Sorting
	private void AddWP(GameObject wp) {
		TTSWaypoint newWP = wp.GetComponent<TTSWaypoint>();
		if (allWaypoints.Count > 0) {
			TTSWaypoint lastWP = allWaypoints[allWaypoints.Count - 1].GetComponent<TTSWaypoint>();

			// Check to see if they're siblings
			if (lastWP.index == newWP.index) {
				lastWP.hasSibling = newWP.hasSibling = true;
				newWP.NewSibling(lastWP);
			}
			// When they're not.
			else {
				waypointLevels.Add(new List<TTSWaypoint>());
				lastWP.AddNextWaypoint(newWP);
			}
		}
		else {
			waypointLevels.Add(new List<TTSWaypoint>());
		}
		waypointLevels[waypointLevels.Count - 1].Add(newWP);

		// Debug.Log("ADDING WP: " + waypointLevels.Count + "-" + waypointLevels[waypointLevels.Count - 1].Count);
		allWaypoints.Add(newWP);
	}

	private void SortWaypoints(GameObject[] original) {
		for (int i = 0; i < original.Length - 1; i++) {
			int index = 0;
			for (int j = 0; j < original.Length - i; j++) {
				// Find the highest index
				index = (CompareWP(original[index], original[j]) < 0) ? index : j;
			}

			GameObject temp = original[index];
			original[index] = original[original.Length - i - 1];
			original[original.Length - i - 1] = temp;

			AddWP(temp);
		}
	}
	private int CompareWP(GameObject one, GameObject two) {
		return (one.GetComponent<TTSWaypoint>().index - two.GetComponent<TTSWaypoint>().index);
	}
}
