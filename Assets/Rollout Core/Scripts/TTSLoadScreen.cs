using UnityEngine;
using System.Collections;

public class TTSLoadScreen : MonoBehaviour {

	public string levelToLoad;

	private AudioSource loadingAudioSource;
	public AudioClip loadingSound;
	
	void Start(){
		
		loadingAudioSource = gameObject.AddComponent<AudioSource>();
		loadingAudioSource.clip = loadingSound;
		loadingAudioSource.volume = 0.5f;
		loadingAudioSource.Play();

		GameObject dataToPass = GameObject.Find("DataToPass");
		if(dataToPass == null){
			Debug.Log("NO DATATOPASS GAMEOBJECT DETECTED, LOADING CITY1-1");
			levelToLoad = "city1-1";
		}else{
			levelToLoad = dataToPass.GetComponent<TTSDataToPass>().levelToLoad;
		}
	}
	
	void Update() {

		if(Application.GetStreamProgressForLevel(levelToLoad) ==1){
			//this.GetComponent<TTSCameraFade>().StartFade(new Color(0,0,0,1.0f), 2.0f);
   			Invoke("BeginLevel", 5.0f);
		}

		
	}

	private void BeginLevel(){
		Application.LoadLevel(levelToLoad);
	}
}
