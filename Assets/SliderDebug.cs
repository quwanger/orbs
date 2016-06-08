using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderDebug : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.GetComponent<Slider> ().value = GameObject.FindObjectOfType<ArduinoManager> ().GetComponent<ArduinoManager> ().stickValue;
	}
}
