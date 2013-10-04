#pragma strict

var HighBeep : AudioClip;

var count3 : AudioClip;
var count2 : AudioClip;
var count1 : AudioClip;


function playCount3() {
	audio.PlayOneShot(count3);

}


function playCount2() {
	audio.PlayOneShot(count2);

}


function playCount1() {
	audio.PlayOneShot(count1);

}




function go() {
	audio.PlayOneShot(HighBeep);
	GameObject.Find("TTSLevel").SendMessage("StartRace");
}