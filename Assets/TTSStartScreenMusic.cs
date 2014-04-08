using UnityEngine;
using System.Collections;

public class TTSStartScreenMusic : MonoBehaviour {

	////attach this to Main Camera
	
	public bool LOAD_RANDOM_TRACK = false;
	public AudioClip songToPlay;
	
	public Object[] tracks;
		
	void Start () {
		if(LOAD_RANDOM_TRACK || songToPlay == null) {
			//Object[] tracks = UnityEngine.Resources.LoadAll("Soundtrack");
			songToPlay = (AudioClip) tracks[Random.Range(0, tracks.Length)];
		}
		
		this.GetComponent<AudioSource>().clip = songToPlay;

		StartSoundtrack();
	}
	
	public void StartSoundtrack() {
		this.GetComponent<AudioSource>().Play();
	}
}
