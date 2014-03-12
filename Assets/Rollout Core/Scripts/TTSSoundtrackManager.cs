using UnityEngine;
using System.Collections;

public class TTSSoundtrackManager : TTSBehaviour {
	
	
	//attach this to Main Camera
	
	public bool LOAD_RANDOM_TRACK = false;
	public AudioClip songToPlay;
	
	public Object[] tracks;
	
	Vector3 stLocation;
	Quaternion stRotation;
	
	void Start () {
		if(LOAD_RANDOM_TRACK || songToPlay == null) {
			//Object[] tracks = UnityEngine.Resources.LoadAll("Soundtrack");
			songToPlay = (AudioClip) tracks[Random.Range(0, tracks.Length)];
		}
		
		this.GetComponent<AudioSource>().clip = songToPlay;
	}
	
	
	void Update () {
		
		float tempX = 0;
		float tempY = 0;
		float tempZ = 0;
		float tempRotX = 0;
		float tempRotY = 0;
		float tempRotZ = 0;
		float tempRotW = 0;
		float numberOfRacers = 0;
		
		//get the average position of all non-ai racers to place the soundtrack (which has the one audio listener)
		foreach(GameObject racer in racers){
			if(racer.GetComponent<TTSRacer>().player != TTSRacer.PlayerType.AI){
				tempX += racer.transform.position.x;
				tempY += racer.transform.position.y;
				tempZ += racer.transform.position.z;
				tempRotX += racer.transform.rotation.x;
				tempRotY += racer.transform.rotation.y;
				tempRotZ += racer.transform.rotation.z;
				tempRotZ += racer.transform.rotation.w;
				numberOfRacers++;
			}
		}
		
		stLocation = new Vector3(tempX/numberOfRacers, tempY/numberOfRacers, tempZ/numberOfRacers);
		stRotation = new Quaternion(tempRotX/numberOfRacers, tempRotY/numberOfRacers, tempRotZ/numberOfRacers, tempRotW/numberOfRacers);
		
		this.transform.position = stLocation;
		this.transform.rotation = stRotation;
	}
	
	public void StartSoundtrack() {
		this.GetComponent<AudioSource>().Play();
	}
}
