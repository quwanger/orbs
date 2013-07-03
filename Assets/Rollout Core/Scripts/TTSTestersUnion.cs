using UnityEngine;
using System.Collections;

public class TTSTestersUnion : MonoBehaviour {
	
	public string name;
	public string age;
	public string sex;
	public string token;
	
	public Time sessionStartTimeStamp;
	
	void Awake() {
		StartCoroutine(RegisterToken("dev_jake"));
	}
		
	public IEnumerator RegisterToken(string userToken) {
		token = userToken;
		WWW request = new WWW("http://studio236.ca/Testers-Union/token-resolver.php?token=" + userToken);
		yield return request;
		string[] paramSet = request.text.Split('|');
		name = paramSet[0];
		age = paramSet[1];
		sex = paramSet[2];
		
		StartCoroutine(logEvent("SESSION_START"));
		
	}
	
	public IEnumerator logEvent(string eventTitle, string syslogStyleParams = "") {
		WWWForm requestGen = new WWWForm();
		
		Hashtable headers = requestGen.headers;
		
		string url = "https://api.splunkstorm.com/1/inputs/http?index=eba281f0e2ba11e2a02d22000a1fd1d2&sourcetype=syslog";
		
		string basicAuthHeader = "eDpFTS1xYjd4WnpvdFdtQ2hXZlp5bmNuWi1YMEZWel9TVF94eDF2U0VGNjh3Z0V5TlZHc0VWVEZWRVNhekVfWTktcHFfUGdvMlRSdEU9";
		headers.Add("Authorization", "Basic " + basicAuthHeader);
		
		string requestBody = System.DateTime.Now.ToString();
		requestBody += " Token=" + token;
		requestBody += " Name=\"" + name + "\"";
		requestBody += " Age=" + age;
		requestBody += " Sex=" + sex;
		requestBody += " Debug=" + Application.isEditor.ToString();
		requestBody += " Event=" + eventTitle;
		requestBody += " " + syslogStyleParams;
		
		WWW request = new WWW(url, System.Text.Encoding.ASCII.GetBytes(requestBody), headers);
		yield return request;
		Debug.Log("Testers Union: Logged Event -> " + eventTitle);
	}
	
	public void OnApplicationQuit() {
		Application.CancelQuit();
		StartCoroutine(postQuit());
	}
	
	public IEnumerator postQuit() {
		Debug.Log("Testers Union: Sending Quit Log");
		yield return StartCoroutine(logEvent("SESSION_END", "Play_Time=" + Time.timeSinceLevelLoad.ToString() + " Level=" + Application.loadedLevelName));	
		Debug.Log ("done.");
		Application.Quit();
	}
	
}
