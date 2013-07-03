using UnityEngine;
using System.Collections;

public class TTSLoadScreen : MonoBehaviour {
	
	void Awake() {
		GetComponent<TTSSceneTransition>().transitionToSceneWithLoad(GameObject.Find("TTSPM").GetComponent<TTSPersistenceManager>().NextScene);	
	}
	
	void Update() {
		Vector3 temp = transform.position;
		temp.x += 0.5f * Time.deltaTime;
		transform.position.Set (temp.x,temp.y,temp.z);
	}
}
