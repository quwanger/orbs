using UnityEngine;
using System.Collections;

public class TTSFinishline : TTSBehaviour {
	
	public GUIText levelName;
	public GUIText placement;
	public GUIText suffix;
	public GUIText rigName;
	public GUIText perksName;
	public GUIText thisTime;
	public GUIText bestTime;
	public GUIText worldRecordTime;
	public GUITexture panel;
	public GUITexture aButton;
	public GUIText replayText;
	
	public bool isVisible = false;
	
	public TTSFinishedRacer myFinishedRacer;
	
	// 0 = 1st place
	// 1 = 2nd place
	// 2 = 3rd place
	// etc.
	public GUIText[] positions;
	public GUIText[] rigs;
	public GUIText[] times;
	public GUIText[] playernames;
	public GUIText[] colours;
	
	
	//populate gui stuff with the information stored in myFinishedRacer;
	
	public void PopulatePanel(){
		//levelName
		PopulateTrackName();
		
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

		//hax
		GameObject levelObj = level.gameObject;
		bestTime.text = level.fakeBestTime;
		worldRecordTime.text = level.fakeWorldRecord;
		
		if(level.currentGameType == TTSLevel.Gametype.MultiplayerOnline){
			aButton.active = false;
			replayText.active = false;
		}
		
		//best time
		
		//world time
	}
	
	public void PopulateTrackName(){
		switch(Application.loadedLevelName){
			case("city1-1"):
				levelName.text = "Backroad Blitz";
			break;
			
			case("city1-2"):
				levelName.text = "Downtown Domination";
			break;

			case("rural1-1"):
				levelName.text = "Night Fright";
			break;
			
			case("cliffsidechoas"):
				levelName.text = "Cliffside Chaos";
			break;
			
			case("future1-1"):
				levelName.text = "Digital Palace";
			break;

			case("future1-2"):
				levelName.text = "Vindiciae";
			break;	
		}
	}
}
