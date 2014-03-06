using UnityEngine;
using System.Collections;

public class TTSCountdownManager : MonoBehaviour {

	public GameObject localOrigin;

	// Use this for initialization
	void Start () {
		transform.position = localOrigin.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = new Vector3(localOrigin.transform.position.x, transform.position.y, localOrigin.transform.position.z);
	}
}
