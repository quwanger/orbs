using UnityEngine;
using System.Collections;

// Is this being used anywhere? (Check gizmos)

public class TTSDrawForward : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDrawGizmos () {
		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position,transform.forward*100);
	}
}
