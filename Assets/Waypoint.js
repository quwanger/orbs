#pragma strict

var width:float = 1;

function Start () {

}

function Update () {

}

function OnDrawGizmos() {
	   Gizmos.color = Color (1,0,0,.5);
        Gizmos.DrawCube (transform.position, Vector3 (1,1,1));
        Gizmos.color = Color (2,0,0,.5);
        Gizmos.DrawRay(this.transform.position,this.transform.right * width);
        Gizmos.DrawRay(this.transform.position,this.transform.right * -width);
}

