using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Thesis_Button : MonoBehaviour {
	public int buttonId = 0;

	public bool speed = true;
	public bool mirrored = false;
	public bool reversed = false;

	public bool action = false;
	public bool navigation = false;

	public int trial = 0;

	public GameObject trials;
	public GameObject schemes;
	public GameObject tracks;

	public void onButtonPressed()
	{
		ThesisData td = GameObject.FindObjectOfType<ThesisData> ().GetComponent<ThesisData>();
		Text title = GameObject.Find("Title").GetComponent<Text>();
		ThesisStart ts = GameObject.FindObjectOfType<ThesisStart> ().GetComponent<ThesisStart> ();


		if (buttonId == 1) {
			ts.menuState = buttonId;
			if (action) {
				td.thesisScheme = 1;
			} else if (navigation) {
				td.thesisScheme = 2;
			}
			title.text = "Trial #";
			schemes.SetActive (false);
			trials.SetActive (true);
		} else if (buttonId == 2) {
			ts.menuState = buttonId;
			td.thesisTrial = trial;
			title.text = "Map";
			trials.SetActive (false);
			tracks.SetActive (true);
		} else if (buttonId == 3) {
			ts.menuState = buttonId;
			if (speed) {
				td.thesisTrack = 1;
				if (mirrored && reversed) {
					td.thesisTrackVariation = 4;
				} else if (mirrored) {
					td.thesisTrackVariation = 3;
				} else if (reversed) {
					td.thesisTrackVariation = 2;
				} else {
					td.thesisTrackVariation = 1;
					Application.LoadLevel ("Speed");
				}
			} else {
				td.thesisTrack = 2;
				if (mirrored && reversed) {
					td.thesisTrackVariation = 4;
				} else if (mirrored) {
					td.thesisTrackVariation = 3;
				} else if (reversed) {
					td.thesisTrackVariation = 2;
				} else {
					td.thesisTrackVariation = 1;
				}
			}
		} else if (buttonId == -1) {
			switch (ts.menuState) {
			case 0:
				break;
			case 1:
				trials.SetActive (false);
				schemes.SetActive (true);
				ts.menuState--;
				break;
			case 2:
				tracks.SetActive (false);
				trials.SetActive (true);
				ts.menuState--;
				break;
			}

		}
	}
}
