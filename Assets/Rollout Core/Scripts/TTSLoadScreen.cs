using UnityEngine;
using System.Collections;

public class TTSLoadScreen : MonoBehaviour {

	private bool canLoad = false;
	public string levelToLoad;
	
	void Awake() {
		//GetComponent<TTSSceneTransition>().transitionToSceneWithLoad(GameObject.Find("TTSPM").GetComponent<TTSPersistenceManager>().NextScene);
		//GetComponent<TTSSceneTransition>().transitionToSceneWithLoad(PlayerPrefs.GetString("Next Scene"));

		GameObject dataToPass = GameObject.Find("DataToPass");

		levelToLoad = dataToPass.GetComponent<TTSDataToPass>().levelToLoad;

		Invoke("BeginLevel", 5.0f);
	}
	
	void Update() {
		Vector3 temp = transform.position;
		temp.x += 0.5f * Time.deltaTime;
		transform.position.Set (temp.x,temp.y,temp.z);

		if(Application.GetStreamProgressForLevel(levelToLoad) ==1 && canLoad){
   			Application.LoadLevel(levelToLoad);
		}

		
	}

	private void BeginLevel(){
		canLoad = true;
	}
}
