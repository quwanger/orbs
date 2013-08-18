#pragma strict

var LowBeep : AudioClip;
var HighBeep : AudioClip;

function playLowBeep() {
	audio.PlayOneShot(LowBeep);
}

function go() {
	audio.PlayOneShot(HighBeep);
	GameObject.Find("TTSLevel").SendMessage("StartRace");
}