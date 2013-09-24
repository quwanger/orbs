using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof (BoxCollider))]
public class TTSWaypoint : MonoBehaviour {
	
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
	private List<GameObject> siblings = new List<GameObject>();
	public Transform transform;
	
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
	}
	
	public void AddSibling(GameObject sibling){
		siblings.Add(sibling);
	}
}
