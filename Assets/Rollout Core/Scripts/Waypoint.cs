using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {
	
	/* Waypoint.cs
	 * Jake Cataford
	 * 
	 * This class generates a waypoint and displays it properly.
	 * The prefab of this object is flagged as a waypoint and the 
	 * scene's waypoint manager consolidates them. It requires an
	 * index to be set, where the index is the number in which they
	 * are to be followed.
	 * 
	 * */
	
	
	public int index = 0;
	public bool isLastWaypoint = false;
	
	
	// Use this for initialization
	void Start () {
		
		Debug.Log("Waypoint Script Initialized.");
		
		if(index == 0) {
			Debug.LogError("Please set the index of your waypoints to a number greater than 0, in the order that they should be connected");
		}
	}
	
	void update() {
		
	}
	
	void OnDrawGizmos() {
		
		Gizmos.color = Color.black;
		Gizmos.DrawIcon(this.transform.position,"waypoint-icon.png");
		
	}
}
