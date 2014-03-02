using UnityEngine;
using System.Collections;

public class TTSMinimap : MonoBehaviour {

	public Transform player;
	public RenderTexture minimapTexture;
	public Material minimapMaterial;

	private int numPlayers;
	private int numRacers;

	// Use this for initialization
	void Start () {
		GameObject levelGO = GameObject.Find("TTSLevel");
		numPlayers = levelGO.GetComponent<TTSInitRace>().tempNumHumanPlayers;
		numRacers = levelGO.GetComponent<TTSInitRace>().numberOfRacers;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
		//this.transform.rotation = new Quaternion(this.transform.rotation.x, player.transform.rotation.y, this.transform.rotation.z, 1.0f);
	}

	void OnGUI() {
		if(Event.current.type == EventType.Repaint)	{
			for(int i = 0; i < numRacers; i++) {
				switch(numPlayers){
					case 2:
						if(i==0) {
							Graphics.DrawTexture(new Rect(10, (Screen.height / 2) - 110, 100, 100), minimapTexture, minimapMaterial);
						} else {
							Graphics.DrawTexture(new Rect(10, (Screen.height - 110) , 100, 100), minimapTexture, minimapMaterial);
						}
						break;

					case 3:
						if(i==0)
							Graphics.DrawTexture(new Rect(10, (Screen.height / 2) - 110, 100, 100), minimapTexture, minimapMaterial);
						else if(i==1)
							Graphics.DrawTexture(new Rect(10, Screen.height - 110, 100, 100), minimapTexture, minimapMaterial);
						else
							Graphics.DrawTexture(new Rect((Screen.width / 2) + 10, Screen.height - 110, 100, 100), minimapTexture, minimapMaterial);
						break;

					case 4:
						if(i==0)
							Graphics.DrawTexture(new Rect(10, (Screen.height / 2) - 110, 100, 100), minimapTexture, minimapMaterial);
						else if(i==1)
							Graphics.DrawTexture(new Rect((Screen.width / 2) + 10, (Screen.height / 2) - 110, 100, 100), minimapTexture, minimapMaterial);
						else if(i==2)
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
	}
}
