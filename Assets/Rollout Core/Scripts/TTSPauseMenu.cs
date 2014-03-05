using UnityEngine;
using System.Collections;

public class TTSPauseMenu : MonoBehaviour {
	public int changer = 1;
	public GameObject resumeHighlighter;
	public GameObject quitHighlighter;
	// Use this for initialization
	void Start () {
		this.guiTexture.enabled = false;
		quitHighlighter.active= false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			this.guiTexture.enabled = true;
			Time.timeScale = 0;	
		}
		
		if(Input.GetKeyDown(KeyCode.W)){
			if(changer == 2)
			{
				quitHighlighter.active = false;
				resumeHighlighter.active = true;
				changer = 1;
			}
		}

		if(Input.GetKeyDown(KeyCode.S)){
			if(changer == 1)
			{
				quitHighlighter.active = true;
				resumeHighlighter.active = false;
				changer = 2;
			}
		}
		
		if(Input.GetKeyDown(KeyCode.Return))
		{
			if(changer == 1)
			{
				this.guiTexture.enabled = false;
				Time.timeScale = 1;
			}
			else if(changer == 2)
				Debug.Log("Back to menu");
		}
	}
}
