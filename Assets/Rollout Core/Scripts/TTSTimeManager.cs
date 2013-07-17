using UnityEngine;
using System.Collections;

public class TTSTimeManager : MonoBehaviour {
	
	
	private float timeInMillis = 0;
	private float startTime = 0;
	
	private bool running = false;
	
	void Start () {
		
	}
	
	void Update () {
		if(running)
			timeInMillis = Time.time - startTime;
	}
	
	public string GetCurrentTimeString(){
		int d = (int)(timeInMillis * 100.0f);
    	int minutes = d / (60 * 100);
    	int seconds = (d % (60 * 100)) / 100;
    	int hundredths = (d % 10);
    	return string.Format("{0:00}:{1:00}", minutes, seconds);
	}
	
	public void StartTimer() {
		running = true;
		startTime = Time.time;
	}
	
	public void StopTimer() {
		running = false;
	}
}
