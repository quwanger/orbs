using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Thesis_Button : MonoBehaviour {
	public int buttonId = 0;

	public bool speed = false;
	public bool precision = false;

	public bool mirrored = false;
	public bool reversed = false;

	public bool binary = false;
	public bool action = false;
	public bool navigation = false;

	public int trial = 0;

	public GameObject trials;
	public GameObject schemes;
	public GameObject tracks;
	public GameObject confirmation;

	public void ContinuePressed()
	{
		ThesisData td = GameObject.Find ("Data(Clone)").GetComponent<ThesisData> ();
		td.SaveData ();
		Application.LoadLevel ("thesis_mainmenu");
	}

	public void onButtonPressed()
	{
		ThesisData td = GameObject.FindObjectOfType<ThesisData> ().GetComponent<ThesisData>();
		Text title = GameObject.Find("Title").GetComponent<Text>();
		ThesisStart ts = GameObject.FindObjectOfType<ThesisStart> ().GetComponent<ThesisStart> ();


		if (buttonId == 1) {
			ts.menuState = buttonId+1;
			if (binary) {
				td.thesisScheme = 1;
				title.text = "Binary";
			} else if (action) {
				td.thesisScheme = 2;
				title.text = "Action";
			} else if (navigation) {
				td.thesisScheme = 3;
				title.text = "Navigation";
			}
			schemes.SetActive (false);
			tracks.SetActive (true);
		} else if (buttonId == 2) {
			ts.menuState = buttonId;
			td.thesisTrial = trial;
			title.text += "-Trial";
			title.text += td.thesisTrial;
			trials.SetActive (false);
			tracks.SetActive (true);
		} else if (buttonId == 3) {
			ts.menuState = buttonId;
			if (speed) {
				td.thesisTrack = 1;
				if (mirrored && reversed) {
					td.thesisTrackVariation = 4;
					td.levelToLoad = "SpeedReversedMirrored";
				} else if (mirrored) {
					td.thesisTrackVariation = 3;
					td.levelToLoad = "SpeedMirrored";
				} else if (reversed) {
					td.thesisTrackVariation = 2;
					td.levelToLoad = "SpeedReversed";
				} else {
					td.thesisTrackVariation = 1;
					td.levelToLoad = "Speed";
				}
			} else if (precision) {
				td.thesisTrack = 2;
				if (mirrored && reversed) {
					td.thesisTrackVariation = 4;
					td.levelToLoad = "PrecisionReversedMirrored";
				} else if (mirrored) {
					td.thesisTrackVariation = 3;
					td.levelToLoad = "PrecisionMirrored";
				} else if (reversed) {
					td.thesisTrackVariation = 2;
					td.levelToLoad = "PrecisionReversed";
				} else {
					td.thesisTrackVariation = 1;
					td.levelToLoad = "Precision";
				}
			} else {
				td.thesisTrackVariation = 1;
				td.levelToLoad = "Practice";
			}

			title.text += "-";
			title.text += td.levelToLoad;
			tracks.SetActive (false);
			confirmation.SetActive (true);
		}else if(buttonId == 99){
			ts.menuState = buttonId;
			Application.LoadLevel (td.levelToLoad);
		} else if (buttonId == -1) {
			switch (ts.menuState) {
			case 0:
				break;
			case 1:
				trials.SetActive (false);
				schemes.SetActive (true);
				ts.menuState--;
				title.text = "Scheme";
				break;
			case 2:
				tracks.SetActive (false);
				schemes.SetActive (true);
				ts.menuState--;
				title.text = "Scheme";
				break;
			case 3:
				confirmation.SetActive (false);
				tracks.SetActive (true);
				ts.menuState--;
				if(title.text.Contains("Binary"))
					title.text = "Binary";
				else if (title.text.Contains ("Action")) {
					title.text = "Action-";
				} else {
					title.text = "Navigation-";
				}
				//title.text += td.thesisTrial;
				break;
			default:
				break;
			}

		}
	}
}
