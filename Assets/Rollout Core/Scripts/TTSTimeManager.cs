using UnityEngine;
using System.Collections;

public class TTSTimeManager : MonoBehaviour {
	
	
	public float timeInMillis = 0;
	public float startTime = 0;
	private float pauseTime;
	private float resumeTime;
	private float timeSpentPaused = 0;
	
	private bool running = false;
	
	public float bonusTime {get; private set;}
	
	void Start () {
		bonusTime = 0.0f;
	}
	
	void Update () {
		if(running)
			timeInMillis = Time.time - startTime - bonusTime - timeSpentPaused;
	}
	
	public string GetCurrentTimeString(){
		int d = (int)(timeInMillis * 100.0f);
    	int minutes = d / (60 * 100);
    	int seconds = (d % (60 * 100)) / 100;
    	int hundredths = (d % 100);
    	return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, hundredths);
	}
	
	public void StartTimer() {
		running = true;
		startTime = Time.time;
	}
	
	public void ResumeTimer() {
		resumeTime = Time.time;
		timeSpentPaused = timeSpentPaused + (resumeTime - pauseTime);
		Debug.Log ("TimeSpentPaused: " + timeSpentPaused);
		running = true;
	}
	
	public void StopTimer() {
		pauseTime = Time.time;
		running = false;
	}
	
	public void GiveTimeBonus(float amount) {
		bonusTime += amount;
	}
}
