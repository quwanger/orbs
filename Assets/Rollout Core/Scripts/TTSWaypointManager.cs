using UnityEngine;
using System.Collections.Generic;

public class TTSWaypointManager : MonoBehaviour {

	/// <summary>
	/// Set in unity editor
	/// </summary>
	public TTSWaypoint SpawnZone;

	// Populated through TTSWaypoint Manager
	public List<TTSWaypoint> EndPoints = new List<TTSWaypoint>();

	void Awake(){

	}
	
	// Use this for initialization
	void Start() {
		if (SpawnZone == null) {
			Debug.LogError("You must set the starting waypoint under the TTSWaypointManager");
			return;
		}

		parseWaypoints(SpawnZone);
		foreach (TTSWaypoint end in EndPoints) {
			parseDistances(end);
		}
	}
	
	void OnDrawGizmos() {
	}

	private void parseWaypoints(TTSWaypoint waypoint) {
		// Reached the end
		if (waypoint.nextWaypoints.Count == 0) {
			EndPoints.Add(waypoint);
			return;
		}

		// Go through each of the next waypoints
		foreach (TTSWaypoint next in waypoint.nextWaypoints) {
			next.addPreviousWaypoint(waypoint);
			parseWaypoints(next);
		}
	}

	private void parseDistances(TTSWaypoint end) {

		// end point
		if (end.nextWaypoints.Count == 0) {
			end.distanceFromEnd = 0.0f;
		}
		// Get distance from the end
		else {
			// First find the farthest distance

			foreach(TTSWaypoint next in end.nextWaypoints){
				float temp = next.distanceFromEnd + Vector3.Distance(end.position, next.position);
				if (temp > end.distanceFromEnd)
					end.distanceFromEnd = temp;
			}
		}
		
		// Reached the start point
		if (end.prevWaypoints.Count == 0)
			return;

		// Parse previous
		foreach (TTSWaypoint prev in end.prevWaypoints) {
			parseDistances(prev);
		}
	}

	#region Finding
	/*
	public TTSWaypoint getClosestWP(int index, Vector3 pos) {
		List<TTSWaypoint> waypoints = waypointLevels[index];
		TTSWaypoint closest = null;

		foreach (TTSWaypoint waypoint in waypoints) {
			if (closest == null || (pos - waypoint.getClosestPoint(pos)).magnitude < (pos - closest.getClosestPoint(pos)).magnitude) {
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
	 */
	#endregion


	private int CompareWP(GameObject one, GameObject two) {
		return (one.GetComponent<TTSWaypoint>().index - two.GetComponent<TTSWaypoint>().index);
	}
}
