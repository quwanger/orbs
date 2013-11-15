using UnityEngine;
using System.Collections;

public class TTSHudTotalRacers : MonoBehaviour {
	
	private GameObject[] r;
	
	// Use this for initialization
	void Start () {
		r = GameObject.Find("TTSLevel").GetComponent<TTSLevel>().racers;
	}
	
	// Update is called once per frame
	void Update () {
		this.GetComponent<TextMesh>().text = "/" + (r.Length).ToString();
	}
}
