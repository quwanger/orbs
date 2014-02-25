using UnityEngine;
using System.Collections;

public class TTSMinimap : MonoBehaviour {

	public Transform player;
	public RenderTexture minimapTexture;
	public Material minimapMaterial;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
		//this.transform.rotation = new Quaternion(this.transform.rotation.x, player.transform.rotation.y, this.transform.rotation.z, 1.0f);
	}

	void OnGUI() {
		if(Event.current.type == EventType.Repaint) 
			Graphics.DrawTexture(new Rect(20, Screen.height - 220, 200, 200), minimapTexture, minimapMaterial);
	}
}
