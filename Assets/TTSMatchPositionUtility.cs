using UnityEngine;
using System.Collections;

public class TTSMatchPositionUtility : MonoBehaviour {
	
	
	public GameObject target;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.position = target.transform.position;
	}
}
