using UnityEngine;
using System.Collections;

public class TTSTimeManager : MonoBehaviour {
	
	
	private float timeInMillis = 0;
	private float startTime = 0;
	
	private bool running = false;
	
	void Start () {
	
	}
	
	void Update () {
		timeInMillis = Time.time - startTime;
	}
	
	public string GetCurrentTimeString(){
		int d = (int)(timeInMillis * 100.0f);
    	int minutes = d / (60 * 100);
    	int seconds = (d % (60 * 100)) / 100;
    	int hundredths = d % 100;
    	return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, hundredths);
	}
	
	public void StartTimer() {
		running = true;
		startTime = Time.time;
	}
	
	public void StopTimer() {
		running = false;
	}
}
