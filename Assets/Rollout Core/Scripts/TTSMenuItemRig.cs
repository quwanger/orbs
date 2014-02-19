using UnityEngine;
using System.Collections;

public class TTSMenuItemRig : TTSMenuEnums {
	
	/* Rig:			S, A, H,  O,   D
	 * Scorpion: 	7, 6, 8, 1.2, 0.8
	 * Dragon:		6, 8, 7, 0.9, 1.1
	 * Rhino:		4, 10, 7, 0.5, 1.5
	 * Spider:		9, 4, 8. 1.7, 0.3
	 * Next-Gen:	8, 8, 5, 1.1, 0.9
	 * Antique:		7, 7, 7, 1.0, 1.0
	 */

	public bool isSelected = false;
	public RigMenuItem rig;
	public int index;
	public int speed;
	public int acceleration;
	public int handling;
	public float offense;
	public float defense;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
