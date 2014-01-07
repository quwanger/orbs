//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

public class ScreenLock : MonoBehaviour {

	private void Start () {
		Screen.lockCursor = true;
	}
	
	private void Update () {
		if(Screen.lockCursor == false) {
			if(Input.GetMouseButtonDown(0)) {
				Screen.lockCursor = true;
			}
		}
	}
}
