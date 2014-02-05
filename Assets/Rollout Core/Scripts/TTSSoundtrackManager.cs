using UnityEngine;
using System.Collections;

public class TTSSoundtrackManager : MonoBehaviour {
	
	
	//attach this to Main Camera
	
	public bool LOAD_RANDOM_TRACK = false;
	public AudioClip songToPlay;
	
	public Object[] tracks;
	
	
	void Start () {
		if(LOAD_RANDOM_TRACK) {
			songToPlay = (AudioClip) tracks[Random.Range (0, tracks.Length)];
		}
		this.GetComponent<AudioSource>().clip = songToPlay;
	}
	
	
	void Update () {
	
	}
	
	public void StartSoundtrack() {
		this.GetComponent<AudioSource>().Play();
	}
}
