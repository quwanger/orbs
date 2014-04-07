using UnityEngine;
using System.Collections;

public class TTSRotatingGrid : MonoBehaviour {

	private float RotationSpeed = 1.0f;

	void Update () {
		transform.Rotate ( Vector3.up * ( RotationSpeed * Time.deltaTime ) );
	}
}
