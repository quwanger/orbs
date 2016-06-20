using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThesisData : MonoBehaviour {

	public int participantId = 19;
	public int thesisTrack = 0;
	public int thesisTrackVariation = 0;
	public int thesisScheme = 0;
	public int thesisTrial = 0;
	//events: 0=button press, 1=boost pickup, 2=boost use, 3=crash, 4=end of race
	public int thesisEvent = 0;

	public string levelToLoad = "";

	public int collisions = 0;

	public List<myData> currentDataPoints = new List<myData>();

	public class myData{
		public int pId;
		public int track;
		public int trackVar;
		public int scheme;
		public int trial;
		public int eventId;
		public char buttonPressed;
		public bool falseInput;
		public Vector3 racerPosition;
		public int boostLevel;
		public float speed;
		public float timeStamp;

		public string dataAsString = "";

		public myData(int pId, int track, int trackVar, int scheme, int trial, int eventId, char buttonPressed, bool falseInput, Vector3 racerPosition, int boostLevel, float speed, float timeStamp)
		{
			this.pId = pId;
			this.track = track;
			this.trackVar = trackVar;
			this.scheme = scheme;
			this.trial = trial;
			this.eventId = eventId;
			this.buttonPressed = buttonPressed;
			this.falseInput = falseInput;
			this.racerPosition = racerPosition;
			this.boostLevel = boostLevel;
			this.speed = speed;
			this.timeStamp = timeStamp;
		}

		public string dataToString()
		{
			dataAsString = pId.ToString () + ";" + track.ToString () + ";" + trackVar.ToString() + ";" + scheme.ToString () + ";" + trial.ToString () + ";" + eventId.ToString () + ";" + buttonPressed.ToString () + ";" + falseInput.ToString () + ";" + racerPosition.x.ToString () + ";" + racerPosition.y.ToString () + ";"+ racerPosition.z.ToString () + ";"+ boostLevel.ToString () + ";" + speed.ToString () + ";" + timeStamp.ToString ();
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

		if (GameObject.FindObjectOfType<TTSRacer> ()) {
			TTSRacer racer = GameObject.FindObjectOfType<TTSRacer> ().GetComponent<TTSRacer> ();

			if (racer != null) {
				if (Input.GetKeyDown (KeyCode.W)) {
					if (thesisScheme == 1) {
						//action
						LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 'w', true, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
					} else {
						//navigation
						if (racer.GetComponent<TTSPowerup> ().AvailablePowerup != TTSBehaviour.PowerupType.None) {
							LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 'w', false, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
						} else {
							LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 'w', true, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
						}
					}
				} else if (Input.GetKeyDown (KeyCode.A)) {
					if (thesisScheme == 1) {
						//action
						LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 'a', false, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
					} else {
						//navigation
						LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 'a', true, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
					}
				} else if (Input.GetKeyDown (KeyCode.S)) {
					if (thesisScheme == 1) {
						//action
						LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 's', true, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
					} else {
						//navigation
						if (racer.GetComponent<TTSPowerup> ().AvailablePowerup != TTSBehaviour.PowerupType.None) {
							LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 's', false, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
						} else {
							LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 's', true, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
						}
					}
				} else if (Input.GetKeyDown (KeyCode.D)) {
					if (thesisScheme == 1) {
						//action
						LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 'd', false, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
					} else {
						//navigation
						LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 'd', true, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
					}
				} else if (Input.GetKeyDown (KeyCode.K)) {
					if (thesisScheme == 1) {
						//action
						if (racer.GetComponent<TTSPowerup> ().AvailablePowerup != TTSBehaviour.PowerupType.None) {
							LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 'k', false, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
						} else {
							LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 'k', true, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
						}
					} else {
						//navigation
						LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 'k', false, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
					}
				} else if (Input.GetKeyDown (KeyCode.L)) {
					if (thesisScheme == 1) {
						//action
						if (racer.GetComponent<TTSPowerup> ().AvailablePowerup != TTSBehaviour.PowerupType.None) {
							LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 'l', false, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
						} else {
							LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 'l', true, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
						}
					} else {
						//navigation
						LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, 'l', false, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
					}
				} else if (Input.GetKeyDown (KeyCode.Period)) {
					LogData (participantId, thesisTrack, thesisTrackVariation, thesisScheme, thesisTrial, 0, '.', false, GameObject.FindObjectOfType<TTSRacer>().transform.position, -1, GameObject.FindObjectOfType<TTSRacer>().GetComponent<Rigidbody>().velocity.magnitude, Time.time);
				}
			}
		}
	}

	public void LogData(int pId, int track, int trackVar, int scheme, int trial, int eventId, char buttonPressed, bool falseInput, Vector3 racerPosition, int boostLevel, float speed, float timeStamp)
	{
		TTSTimeManager tMan = GameObject.FindObjectOfType<TTSTimeManager> ().GetComponent<TTSTimeManager>();

		float _time = 0f;

		if (tMan != null)
			_time = tMan.timeInMillis;
		
		//participantId, track, scheme, trial, event, button pressed, false input?, boost ID, boost level, speed, time
		myData tempData = new myData(pId, track, trackVar, scheme, trial, eventId, buttonPressed, falseInput, racerPosition, boostLevel, speed, _time);
		currentDataPoints.Add (tempData);

		if (eventId == 3) {
			collisions++;
		}
	}

	public void SaveData()
	{
		string documentTitle = "data" + participantId.ToString() + thesisTrack.ToString() + thesisScheme.ToString();
		List<string> lines = new List<string> ();
		foreach (myData md in currentDataPoints) {
			lines.Add(md.dataToString ());
		}
		System.IO.File.WriteAllLines (@"C:\Users\CIL-admin\Dropbox\Paden\Study 2 Info\Data\19\" + documentTitle + ".txt", lines.ToArray());
		currentDataPoints.Clear ();
		collisions = 0;
	}
}
