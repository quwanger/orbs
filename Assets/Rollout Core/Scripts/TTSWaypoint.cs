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
	
	public bool hasSibling = false;
	public Transform transform;

	public Vector3 closestPoint = new Vector3();
	public Vector3 closestPointRotation = new Vector3();

	public List<TTSWaypoint> siblings = new List<TTSWaypoint>();
	public List<TTSWaypoint> nextWaypoints = new List<TTSWaypoint>();
	public List<TTSWaypoint> prevWaypoints = new List<TTSWaypoint>();
	
	void Start () {
	}

	void Awake() {
		boxCollider = GetComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		transform = GetComponent<Transform>();
	}
	
	void OnTriggerEnter(Collider other) {
		this.isActive = false;	
		
		foreach(GameObject racer in racers) {
			if(other.gameObject == racer) {
				racer.GetComponent<TTSRacer>().OnWaypoint(this);
			}
		}
	}

	public Vector3 GetClosestPoint(Vector3 position) {
		// convert point to local space
		return closestPointRotation = position;//boxCollider.ClosestPointOnBounds(position);
	}


	public void OnDrawGizmos() {
		Gizmos.color = Color.red;

		Gizmos.DrawWireCube(closestPoint, new Vector3(0.5f, 0.5f, 0.5f));

		Gizmos.color = Color.blue;

		Gizmos.DrawWireCube(closestPointRotation, new Vector3(0.5f, 0.5f, 0.5f));
	}

	#region initialize

	/// <summary>
	/// Will add new  sibling waypoint to all the siblings and itself
	/// </summary>
	/// <param name="sibling">New sibling</param>
	public void NewSibling(TTSWaypoint lastSibling) {
		// Add yourself to all the previous siblings
		siblings.Add(lastSibling);
		siblings.AddRange(lastSibling.siblings);

		// Add previous siblings to yourself
		foreach (TTSWaypoint sibling in siblings) {
			sibling.AddSibling(this);
		}

		// Get all the previous waypoints too
		prevWaypoints = lastSibling.prevWaypoints;

		// Add yourself as next to all the previous
		foreach (TTSWaypoint prevWaypoint in prevWaypoints) {
			prevWaypoint.AddNext(this);
		}
	}

	private void AddSibling(TTSWaypoint newSibling) {
		siblings.Add(newSibling);
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
		foreach (TTSWaypoint sibling in siblings) {
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
		prevWaypoints.AddRange(prevWaypoint.siblings);
	}
	#endregion
}
