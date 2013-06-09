using UnityEngine;
using System.Collections;

public class TTSUtils {
	
	public static float GetRelativeAngle(Vector3 firstVector, Vector3 secondVector) {
		
		float angle = Vector3.Angle(firstVector, secondVector);
		
		Vector3 cross = Vector3.Cross(firstVector, secondVector);
    	
		if (cross.y < 0) angle = -angle;
		
		return angle;
		
	}

}
