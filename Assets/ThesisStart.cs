using UnityEngine;
using System.Collections;

public class ThesisStart : MonoBehaviour {

	public GameObject myDataObject;

	public int menuState = 0;

	// Use this for initialization
	void Start () {
		if (GameObject.FindObjectOfType<ThesisData>() == null) {
			Instantiate(myDataObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
