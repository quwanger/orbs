using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class TTSGateway : MonoBehaviour {
	
	public string SceneToLoad;
	
	// Use this for initialization
	void Start () {
		
		if(SceneToLoad == null) {
			Debug.LogError("Please assign a scene to the gateway.");	
		}
		
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.GetComponent<TTSRacer>().IsPlayerControlled) {
			GameObject.Find("TTSPM").GetComponent<TTSPersistenceManager>().NextScene = SceneToLoad;
			GameObject.Find("Main Camera").GetComponent<TTSSceneTransition>().transitionToSceneWithLoad("LoadingScene");
			Debug.Log("Transitioning to " + GameObject.Find("TTSPM").GetComponent<TTSPersistenceManager>().NextScene);
		}
	}

}
