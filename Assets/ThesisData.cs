using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThesisData : MonoBehaviour {

	public int participantId = 0;
	public int thesisTrack = 0;
	public int thesisTrackVariation = 0;
	public int thesisScheme = 0;
	public int thesisTrial = 0;
	//events: 0=button press, 1=boost pickup, 2=boost use, 3=crash, 4=end of race
	public int thesisEvent = 0;

	public List<myData> currentDataPoints = new List<myData>();

	public class myData{
		public int pId;
		public int track;
		public int trackVar;
		public int scheme;
		public int trial;
		public int eventId;
		public int buttonPressed;
		public int falseInput;
		public int boostId;
		public int boostLevel;
		public float speed;
		public float timeStamp;

		public string dataAsString = "";

		public myData(int pId, int track, int trackVar, int scheme, int trial, int eventId, int buttonPressed, int falseInput, int boostId, int boostLevel, float speed, float timeStamp)
		{
			this.pId = pId;
			this.track = track;
			this.trackVar = trackVar;
			this.scheme = scheme;
			this.trial = trial;
			this.eventId = eventId;
			this.buttonPressed = buttonPressed;
			this.falseInput = falseInput;
			this.boostId = boostId;
			this.boostLevel = boostLevel;
			this.speed = speed;
			this.timeStamp = timeStamp;
		}

		public string dataToString()
		{
			dataAsString = pId.ToString () + "," + track.ToString () + "," + trackVar.ToString() + "," + scheme.ToString () + "," + trial.ToString () + "," + eventId.ToString () + "," + buttonPressed.ToString () + "," + falseInput.ToString () + "," + boostId.ToString () + "," + boostLevel.ToString () + "," + speed.ToString () + "," + timeStamp.ToString ();
			return dataAsString;
		}
	}

	void Awake()
	{
		DontDestroyOnLoad (this.transform.gameObject);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LogData(int pId, int track, int trackVar, int scheme, int trial, int eventId, int buttonPressed, int falseInput, int boostId, int boostLevel, float speed, float timeStamp)
	{
		//participantId, track, scheme, trial, event, button pressed, false input?, boost ID, boost level, speed, time
		myData tempData = new myData(pId, track, trackVar, scheme, trial, eventId, buttonPressed, falseInput, boostId, boostLevel, speed, timeStamp);
		currentDataPoints.Add (tempData);
	}

	public void SaveData()
	{
		string documentTitle = "data" + participantId.ToString() + thesisTrack.ToString() + thesisTrackVariation.ToString() + thesisScheme.ToString() + thesisTrial.ToString();
		List<string> lines = new List<string> ();
		foreach (myData md in currentDataPoints) {
			lines.Add(md.dataToString ());
		}
		System.IO.File.WriteAllLines (@"C:\Users\CIL-admin\Desktop\" + documentTitle + ".txt", lines.ToArray());
		currentDataPoints.Clear ();
	}
}
