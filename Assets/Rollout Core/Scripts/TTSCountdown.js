#pragma strict

var countGO : AudioClip;

var count3 : AudioClip;
var count2 : AudioClip;
var count1 : AudioClip;


function playCount3() {
	GetComponent.<AudioSource>().PlayOneShot(count3);

}


function playCount2() {
	GetComponent.<AudioSource>().PlayOneShot(count2);

}


function playCount1() {
	GetComponent.<AudioSource>().PlayOneShot(count1);

}




function go() {
	GetComponent.<AudioSource>().PlayOneShot(countGO);
	GameObject.Find("TTSLevel").SendMessage("StartRace");
}