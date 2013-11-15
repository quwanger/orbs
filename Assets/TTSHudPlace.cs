using UnityEngine;
using System.Collections;

public class TTSHudPlace : MonoBehaviour {

	private GameObject[] r;
	private GameObject[] sortedRacers;
	private GameObject currentRacer;
	
	// Use this for initialization
	void Start () {
		r = GameObject.Find("TTSLevel").GetComponent<TTSLevel>().racers;
		currentRacer = this.transform.parent.GetComponent<TTSFloatHud>().racerToFollow;
	}
	
	// Update is called once per frame
	void Update () {
		
		int temp;
		
		for (int pass = 1; pass <= r.Length - 2; pass++)
		{
		    for (int i = 0; i <= r.Length - 2; i++)
		    {
		       if (r[i].GetComponent<TTSRacer>().currentWaypoint.index > r[i + 1].GetComponent<TTSRacer>().currentWaypoint.index)
		       {
		           temp = r[i + 1].GetComponent<TTSRacer>().currentWaypoint.index;
		           r[i + 1].GetComponent<TTSRacer>().currentWaypoint.index = r[i].GetComponent<TTSRacer>().currentWaypoint.index;
		           r[i].GetComponent<TTSRacer>().currentWaypoint.index = temp;
		       }
		
		    }
		
		}
		
		for(int i = 0; i < r.Length; i++){
			if(r[i] = currentRacer)
				this.GetComponent<TextMesh>().text = (i+1).ToString();
		}
	}
}
