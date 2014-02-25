using UnityEngine;
using System.Collections;

public class TTSMinimapRotation : MonoBehaviour {
	public Transform player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.rotation = new Quaternion(this.transform.rotation.x, this.transform.rotation.y, player.rotation.y, 1.0f);
	}
}
