using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof (BoxCollider))]
public class TTSWaypoint : TTSBehaviour {
	
	/* Waypoint.cs
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
	public bool isActive = true;
	private BoxCollider boxCollider;
	
	public GameObject nextWaypoint;
	
	public bool hasSibling = false;
	private List<GameObject> siblings = new List<GameObject>();
	public Transform transform;

	public List<TTSWaypoint> _siblings = new List<TTSWaypoint>();
	public List<TTSWaypoint> nextWaypoints = new List<TTSWaypoint>();
	public List<TTSWaypoint> prevWaypoints = new List<TTSWaypoint>();
	
	void Start () {
		boxCollider = GetComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		transform = GetComponent<Transform>();
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.black;
		//Gizmos.DrawIcon(this.transform.position,"Rollout Core/Waypoints/waypoint-icon.png");
	}
	
	void OnTriggerEnter(Collider other) {
		this.isActive = false;	
		
		foreach(GameObject racer in racers) {
			if(other.gameObject == racer) {
				racer.GetComponent<TTSRacer>().OnWaypoint(this);
			}
		}
	}


	/// <summary>
	/// Will add new  sibling waypoint to all the siblings and itself
	/// </summary>
	/// <param name="sibling">New sibling</param>
	public void NewSibling(TTSWaypoint lastSibling) {
		// Add yourself to all the previous siblings
		_siblings.Add(lastSibling);
		_siblings.AddRange(lastSibling._siblings);

		// Add previous siblings to yourself
		foreach (TTSWaypoint sibling in _siblings) {
			sibling.AddSibling(this);
		}

		// Get all the previous waypoints too
		prevWaypoints = lastSibling.prevWaypoints;
	}

	private void AddSibling(TTSWaypoint newSibling) {
		_siblings.Add(newSibling);
	}
	
	/// <summary>
	/// Will handle the siblings too when adding next waypoint
	/// </summary>
	/// <param name="next">Next waypoint</param>
	/// Next waypoint is the newest waypoint being added
	/// This function is called only once when the index has gone up, otherwise, new sibling is called
	public void AddNextWaypoint(TTSWaypoint next) { 
		nextWaypoints.Add(next);

		// Add previous siblings to yourself
		foreach (TTSWaypoint sibling in _siblings) {
			sibling.AddNext(next);
		}

		// Add yourself and siblings to next wp
		next.AddPrevWaypoints(this);
	}
	private void AddNext(TTSWaypoint next) {
		nextWaypoints.Add(next);
	}

	public void AddPrevWaypoints(TTSWaypoint prevWaypoint) {
		prevWaypoints.Add(prevWaypoint);
		prevWaypoints.AddRange(prevWaypoint._siblings);
	}
}
