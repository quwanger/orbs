using UnityEngine;
using System.Collections;

public class TTSMenuItemRig : TTSBehaviour {
	
	/* Rig:			S, A, H,  O,   D
	 * Scorpion: 	7, 6, 8, 1.2, 0.8
	 * 7 and 3
	 * Dragon:		6, 8, 7, 0.9, 1.1
	 * 6 and 4
	 * Rhino:		4, 10, 7, 0.5, 1.5
	 * 3 and 7
	 * Spider:		9, 4, 8. 1.7, 0.3
	 * 8 and 2
	 * Next-Gen:	8, 8, 5, 1.1, 0.9
	 * 6 and 4
	 * Antique:		7, 7, 7, 1.0, 1.0
	 * 5 and 5
	 */

	public bool isSelected = false;
	public RigType rig;
	public int index;
	public int speed;
	public int acceleration;
	public int handling;
	public int offense;
	public int defense;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
