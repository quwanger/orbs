#pragma strict

var target:Transform;

function Start () {

}

function Update () {
	this.transform.LookAt(target);
}