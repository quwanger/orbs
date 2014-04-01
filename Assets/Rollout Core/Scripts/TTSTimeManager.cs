using UnityEngine;
using System.Collections;

public class TTSTimeManager : MonoBehaviour {
	
	
	public float timeInMillis = 0;
	private float startTime = 0;
	
	private bool running = false;
	
	public float bonusTime {get; private set;}
	
	void Start () {
		bonusTime = 0.0f;
	}
	
	void Update () {
		if(running)
			timeInMillis = Time.time - startTime - bonusTime;
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
	
	public void StopTimer() {
		running = false;
	}
	
	public void GiveTimeBonus(float amount) {
		bonusTime += amount;
	}
}
