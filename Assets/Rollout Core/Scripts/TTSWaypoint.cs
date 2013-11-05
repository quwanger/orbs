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

	void Start () {
	}

	void Awake() {
		boxCollider = GetComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		//transform = GetComponent<Transform>();

		colliderLine = boxCollider.transform.right;
	}
	
	void OnTriggerEnter(Collider other) {
		this.isActive = false;	
		
		foreach(GameObject racer in racers) {
			if(other.gameObject == racer) {
				racer.GetComponent<TTSRacer>().OnWaypoint(this);
			}
		}
	}

	public Vector3 racerPos = new Vector3();
	public Vector3 closestPointPos = new Vector3();

	public Vector3 GetClosestPoint(Vector3 position) {

		Vector3 pnt = Vector3.ClampMagnitude(Vector3.Project(position - transform.position, colliderLine), boxCollider.size.x / 2) + transform.position;

		pnt.y = position.y;

		// convert point to local space
		return closestPointPos = pnt;
	}
	
	/// <summary>
	/// Returns transform.position if no points seen.
	/// </summary>
	/// <param name="position"></param>
	/// <param name="resolution"></param>
	/// <returns>Finds the closest seen point on the collider</returns>
	public Vector3 GetClosestSeenPoint(Vector3 position, int resolution) {
		Vector3 closest = GetClosestPoint(position);
		RaycastHit hit;

		if(Physics.Raycast(position, (closest - position), out hit, (closest - position).magnitude))
			if (hit.collider.name == "Waypoint")//(!Physics.Linecast(position, closest, 10))
				return closest;
			else
				Debug.Log("Can't see closest point " + hit.collider.name);

		closest = transform.position;
		closest.y = position.y;
		Vector3 pnt = new Vector3();

		resolution--; // So that we make as many checks as resolutions;
		for (int i = 0; i < resolution+1; i++) { // Start from right to left.
			pnt = colliderLine * ((float)i / resolution * boxCollider.size.x - (boxCollider.size.x / 2)) + transform.position;
			pnt.y = position.y;

			Debug.DrawLine(position, pnt);
			if (Physics.Raycast(position, (pnt - position), out hit, (pnt - position).magnitude)) {
				if (hit.collider.name == "Waypoint" && Vector3.Distance(position, pnt) < Vector3.Distance(position, closest)) {
					closest = pnt;
				}
			}
		}

		return closest;
	}

	public bool canBeSeenFrom(Vector3 posistion) {
		return canBeSeenFrom(posistion, 5);
	}

	public bool canBeSeenFrom(Vector3 position, int resolution) {
		Vector3 pnt = colliderLine * -boxCollider.size.x / 2 + transform.position; // Left most
		pnt.y = position.y;

		if (Physics.Linecast(position, pnt))
			return true;

		resolution--; // So that we make as many checks as resolutions;
		for (int i = resolution; i > 0; i--) { // Start from right to left.
			pnt = colliderLine * (i / resolution * boxCollider.size.x - (boxCollider.size.x / 2)) + transform.position;
			if (Physics.Linecast(position, pnt))
				return true;
		}
		return false;
	}

	public void OnDrawGizmos() {
		if (transform == null)
			return;

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, closestPointPos);

		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, colliderLine);

		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(transform.position + Vector3.left * 3, transform.position + Vector3.right * 3);
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
