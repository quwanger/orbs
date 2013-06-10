using UnityEngine;
using System.Collections;

public static class TTSUtils {
	
	public static float GetRelativeAngle(Vector3 firstVector, Vector3 secondVector) {
		
		float angle = Vector3.Angle(firstVector, secondVector);
		
		Vector3 cross = Vector3.Cross(firstVector, secondVector);
    	
		if (cross.y < 0) angle = -angle;
		
		return angle;
		
	}
	
	public static float Remap (float value, float from1, float to1, float from2, float to2) {

    	return (value - from1) / (to1 - from1) * (to2 - from2) + from2;

	}


}
