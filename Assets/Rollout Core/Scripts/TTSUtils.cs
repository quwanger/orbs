using UnityEngine;
using System.Collections;

public static class TTSUtils {
	
	public static float GetRelativeAngle(Vector3 firstVector, Vector3 secondVector) {
		
		float angle = Vector3.Angle(firstVector, secondVector);
		
		Vector3 cross = Vector3.Cross(firstVector, secondVector);
    	
		if (cross.y < 0) angle = -angle;
		
		return angle;
		
	}
	
	public static float Remap (float value, float from1, float to1, float from2, float to2, bool clamped) {
        float temp = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        temp = clamped ? Mathf.Clamp(temp, from2, to2) : temp;
        return temp;

	}

	public static Vector3 LerpVector(Vector3 from, Vector3 to, float t) {
		float x = Mathf.Lerp(from.x, to.x, t);
		float y = Mathf.Lerp(from.y, to.y, t);
		float z = Mathf.Lerp(from.z, to.z, t);

		return new Vector3(x, y, z);
	}

	public static Vector3 FlattenVector(Vector3 vec) {
		return new Vector3(vec.x, 0.0f, vec.z);
	}
}
