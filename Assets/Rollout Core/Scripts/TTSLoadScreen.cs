using UnityEngine;
using System.Collections;

public class TTSLoadScreen : MonoBehaviour {

	public string levelToLoad;

	private AudioSource loadingAudioSource;
	public AudioClip loadingSound;

	AsyncOperation op;
	
	void Start() {

		this.GetComponent<TTSCameraFade>().SetScreenOverlayColor(new Color(0, 0, 0, 1.0f));
		this.GetComponent<TTSCameraFade>().StartFade(new Color(0,0,0,0), 2.0f);
		
		loadingAudioSource = gameObject.AddComponent<AudioSource>();
		loadingAudioSource.clip = loadingSound;
		loadingAudioSource.volume = 0.5f;
		loadingAudioSource.loop = true;
		loadingAudioSource.Play();

		GameObject dataToPass = GameObject.Find("DataToPass");
		if(dataToPass == null){
			Debug.Log("NO DATATOPASS GAMEOBJECT DETECTED, LOADING CITY1-1");
			levelToLoad = "city1-1";
		}else{
			levelToLoad = dataToPass.GetComponent<TTSDataToPass>().levelToLoad;
		}

       Invoke("BeginLoading", 5.0f);
    }

    void BeginLoading(){
    	op = Application.LoadLevelAsync (levelToLoad);
		op.allowSceneActivation = false;
    }

    void Update(){
    	if(op!=null){
	    	if(op.progress >= 0.9f){
	    		this.GetComponent<TTSCameraFade>().StartFade(new Color(0,0,0,1.0f), 2.0f);
	    		Invoke("BeginLevel", 2.0f);
	    	}
   		}
    }

    private void BeginLevel(){
		op.allowSceneActivation = true;
	}

	/*void Start(){

		this.GetComponent<TTSCameraFade>().SetScreenOverlayColor(new Color(0, 0, 0, 1.0f));
		this.GetComponent<TTSCameraFade>().StartFade(new Color(0,0,0,0), 2.0f);
		
		loadingAudioSource = gameObject.AddComponent<AudioSource>();
		loadingAudioSource.clip = loadingSound;
		loadingAudioSource.volume = 0.5f;
		loadingAudioSource.loop = true;
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

		
	}*/
	
}
