using UnityEngine;
using System.Collections;

public class TTSMinimap : TTSBehaviour {

	public Transform player;
	public RenderTexture minimapTexture;
	public Material minimapMaterial;

	private int numPlayers;
	
	public Rect minimapWindow;
	
	public int minimapID;

	// Use this for initialization
	void Start () {
		//levelGO = GameObject.Find("TTSLevel");
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
		//this.transform.rotation = new Quaternion(this.transform.rotation.x, player.transform.rotation.y, this.transform.rotation.z, 1.0f);
	}

	/*void OnGUI() {
		/*if(levelGO.GetComponent<TTSInitRace>().levelInitialized){
			if(Event.current.type == EventType.Repaint)	{
				Debug.Log ("Minimap ID: " + minimapID + ", Rect: " + minimapWindow + ", Rig: " + player);
				Graphics.DrawTexture(minimapWindow, minimapTexture, minimapMaterial);
			}
		}
		if(minimapInitialized){
			if(Event.current.type == EventType.Repaint) {
				Debug.Log ("NumPlayers: " + numPlayers);
				Debug.Log ("Minimap ID: " + minimapID);
				
				switch(numPlayers){
					case 2:
						if(minimapID==0) {
							Debug.Log ("Cam one draw");
							Graphics.DrawTexture(new Rect(10, (Screen.height / 2) - 110, 100, 100), minimapTexture, minimapMaterial);
						} else {
						Debug.Log ("Cam two draw");
							Graphics.DrawTexture(new Rect(10, (Screen.height - 110) , 100, 100), minimapTexture, minimapMaterial);
						}
						break;
	
					case 3:
						if(minimapID==0)
							Graphics.DrawTexture(new Rect(10, (Screen.height / 2) - 110, 100, 100), minimapTexture, minimapMaterial);
						else if(minimapID==1)
							Graphics.DrawTexture(new Rect(10, Screen.height - 110, 100, 100), minimapTexture, minimapMaterial);
						else
							Graphics.DrawTexture(new Rect((Screen.width / 2) + 10, Screen.height - 110, 100, 100), minimapTexture, minimapMaterial);
						break;
	
					case 4:
						if(minimapID==0)
							Graphics.DrawTexture(new Rect(10, (Screen.height / 2) - 110, 100, 100), minimapTexture, minimapMaterial);
						else if(minimapID==1)
							Graphics.DrawTexture(new Rect((Screen.width / 2) + 10, (Screen.height / 2) - 110, 100, 100), minimapTexture, minimapMaterial);
						else if(minimapID==2)
							Graphics.DrawTexture(new Rect(10, Screen.height - 110, 100, 100), minimapTexture, minimapMaterial);
						else
							Graphics.DrawTexture(new Rect((Screen.width / 2) + 10, Screen.height - 110, 100, 100), minimapTexture, minimapMaterial);
						break;
	
					default:
						Graphics.DrawTexture(new Rect(10, Screen.height - 210, 200, 200), minimapTexture, minimapMaterial);
					break;
				}
			}
		}
	}*/
}
