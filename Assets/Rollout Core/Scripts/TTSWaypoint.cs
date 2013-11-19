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
	//public Transform transform;

	public Vector3 closestPoint = new Vector3();

	public List<TTSWaypoint> siblings = new List<TTSWaypoint>();
	public List<TTSWaypoint> nextWaypoints = new List<TTSWaypoint>();
	public List<TTSWaypoint> prevWaypoints = new List<TTSWaypoint>();

	private Vector3 colliderLine;
	public Vector3 forwardLine;
	public float boxWidth = 0.0f;
	public float boxHeight = 0.0f;
	public Vector3 position;

	public float distanceFromEnd = 0.0f;

	void Start () {
		boxWidth = boxCollider.size.x;
		boxHeight = boxCollider.size.y;

		colliderLine = boxCollider.transform.right;
		forwardLine = -boxCollider.transform.forward;
		position = transform.position;
	}

	void Awake() {
		boxCollider = GetComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		//transform = GetComponent<Transform>();
	}

	public void addPreviousWaypoint(TTSWaypoint prev){
		prevWaypoints.Add(prev);
	}
	
	void OnTriggerEnter(Collider other) {
		this.isActive = false;	
		
		foreach(GameObject racer in racers) {
			if(other.gameObject == racer) {
				racer.GetComponent<TTSRacer>().OnWaypoint(this);
			}
		}
	}

	#region Vector Calculations
	/// <summary>
	/// </summary>
	public Vector3 getPointOn(float b){
		b -= 0.5f; // correct for start from left edge
		return colliderLine * (b * boxWidth) + position;
	}

	public Vector3 racerPos = new Vector3();
	public Vector3 closestPointPos = new Vector3();

	public Vector3 getClosestPoint(Vector3 from) {

		Vector3 pnt = Vector3.ClampMagnitude(Vector3.Project(from - position, colliderLine), boxWidth / 2) + position;

		pnt.y = Mathf.Clamp(from.y, position.y - boxHeight / 2, position.y + boxHeight / 2);

		// convert point to local space
		return closestPointPos = pnt;
	}

	public float getDistanceFrom(Vector3 from) {
		return (getClosestPoint(from) - from).magnitude;
	}
	
	/// <summary>
	/// Returns transform.position if no points seen.
	/// </summary>
	/// <param name="from"></param>
	/// <param name="resolution"></param>
	/// <returns>Finds the closest seen point on the collider</returns>
	public Vector3 getClosestSeenPoint(Vector3 from, int resolution) {
		Vector3 closest = getClosestPoint(from);

		if(!Physics.Linecast(from, closest, TTSUtils.LayerMask(10)))
			return closest;
		else
			Debug.Log("Can't see closest point");

		closest = position;
		closest.y = Mathf.Clamp(from.y, position.y - boxHeight / 2, position.y - boxHeight / 2);
		Vector3 pnt = new Vector3();

		resolution--; // So that we make as many checks as resolutions;
		for (float i = 0; i < resolution+1; i++) { // Start from right to left.
			pnt = getPointOn(i / resolution);
			pnt.y = Mathf.Clamp(from.y, position.y - boxHeight / 2, position.y - boxHeight / 2);

			if (!Physics.Linecast(from, pnt, TTSUtils.LayerMask(10)) && Vector3.Distance(from, pnt) < Vector3.Distance(from, closest)) {
				closest = pnt;
			}
		}

		return closest;
	}

	public bool visibleFrom(Vector3 from) {
		return visibleFrom(from, 5);
	}

	public bool visibleFrom(Vector3 from, int resolution) {
		Vector3 pnt = getPointOn(1.0f); // Left most
		pnt.y = Mathf.Clamp(from.y, position.y - boxHeight / 2, position.y - boxHeight / 2);

		if (!Physics.Linecast(from, pnt, TTSUtils.LayerMask(10)))
			return true;

		resolution--; // So that we make as many checks as resolutions;
		for (float i = 0; i < resolution+1; i++) { // Start from right to left.
			pnt = getPointOn(i / resolution);

			if (!Physics.Linecast(from, pnt, TTSUtils.LayerMask(10)))
				return true;
		}
		return false;
	}
	#endregion

	public void OnDrawGizmos() {
		if (transform == null)
			return;

		if (closestPointPos.magnitude == 0) {
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position, closestPointPos);
		}

		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, colliderLine);

		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, forwardLine);

		Gizmos.color = Color.magenta;
		foreach(TTSWaypoint prev in prevWaypoints)
			Gizmos.DrawLine(position, prev.position);
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
