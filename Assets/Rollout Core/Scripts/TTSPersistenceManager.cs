using UnityEngine;
using System.Collections;

public class TTSPersistenceManager : MonoBehaviour {
	
	public string NextScene;
	public string FirstScene;
	
	// Use this for initialization
	void Awake () {
		Object.DontDestroyOnLoad(gameObject);
		Application.LoadLevel(FirstScene);
	}
	
}
