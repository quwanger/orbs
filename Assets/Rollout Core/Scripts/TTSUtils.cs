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

	public static Vector3 FlattenVector(Vector3 vec) {
		return new Vector3(vec.x, 0.0f, vec.z);
	}

	public static int LayerMask(int[] layers) {
		int mask = (1 << layers[0]);

		for (int i = 1; i < layers.Length; i++) {
			mask |= (1 << layers[i]);
		}

		return ~mask;
	}

	public static int LayerMask(int layer) {
		return ~(1 << layer);
	}
}
