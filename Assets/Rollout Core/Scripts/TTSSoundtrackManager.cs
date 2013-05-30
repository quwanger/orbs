using UnityEngine;
using System.Collections;

public class TTSSoundtrackManager : MonoBehaviour {
	
	
	//attach this to Main Camera
	
	public bool LOAD_RANDOM_TRACK = false;
	public AudioClip songToPlay;
	
	
	void Start () {
		if(LOAD_RANDOM_TRACK || songToPlay == null) {
			Object[] tracks = Resources.LoadAll("Soundtrack");
			songToPlay = (AudioClip) tracks[Mathf.FloorToInt(Random.value * tracks.Length)];
		}
		
		this.GetComponent<AudioSource>().clip = songToPlay;
		this.GetComponent<AudioSource>().Play();
	}
	
	
	void Update () {
	
	}
}
