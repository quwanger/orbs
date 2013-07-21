using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TTSFloatHud : MonoBehaviour {
	
	public TextMesh timeDisplay; 
	public Transform boundCamera;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 transcam = boundCamera.position + (boundCamera.forward * 10f);
		transform.position = Vector3.Lerp(transform.position, transcam,0.9f);
		transform.rotation = Quaternion.Lerp(transform.rotation, boundCamera.rotation, 0.7f);
		timeDisplay.text = GameObject.Find("TTSLevel").GetComponent<TTSTimeManager>().GetCurrentTimeString();
	}
}
