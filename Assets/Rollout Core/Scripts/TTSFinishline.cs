using UnityEngine;
using System.Collections;

public class TTSFinishline : MonoBehaviour {
	
	public GUIText levelName;
	public GUIText placement;
	public GUIText suffix;
	public GUIText rigName;
	public GUIText perksName;
	public GUIText thisTime;
	public GUIText bestTime;
	public GUIText worldRecordTime;
	public GUITexture panel;
	
	public bool isVisible = false;
	
	public TTSFinishedRacer myFinishedRacer;
	
	// 0 = 1st place
	// 1 = 2nd place
	// 2 = 3rd place
	// etc.
	public GUIText[] positions;
	public GUIText[] rigs;
	public GUIText[] times;
	
	//populate gui stuff with the information stored in myFinishedRacer;
	
	public void PopulatePanel(){
		//levelName
		
		placement.text = myFinishedRacer.place.ToString();
		
		switch(myFinishedRacer.place){
			case 1:
				suffix.text = "st";
			break;
			
			case 2:
				suffix.text = "nd";
			break;
			
			case 3:
				suffix.text = "rd";
			break;
			
			case 4:
			case 5:
			case 6:
				suffix.text = "th";
			break;
		}
		
		rigName.text = myFinishedRacer.rig.ToString();
		
		perksName.text = myFinishedRacer.perkpool1.ToString() + " & " + myFinishedRacer.perkpool2.ToString();
		
		thisTime.text = myFinishedRacer.time; //already a string
		
		//best time
		
		//world time
	}
}
