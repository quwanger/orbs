using UnityEngine;
using System.Collections;

public static class TTSUtils {

	public static System.Random Rand = new System.Random();

	public static float Random() {
		return (float)Rand.NextDouble();
	}
	
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

	public static Vector3 RotateAround(Vector3 point, Vector3 pivot, Vector3 angles) {
		Vector3 dir = point - pivot;
		dir = Quaternion.Euler(angles) * dir;
		point = dir + pivot;
		return point;
	}
	public static Vector3 RotateScaleAround(Vector3 point, Vector3 pivot, Vector3 angles, float scale) {
		Vector3 dir = point - pivot;
		dir *= scale;
		dir = Quaternion.Euler(angles) * dir;
		point = dir + pivot;
		return point;
	}

	public static Vector3 FlattenVector(Vector3 vec) {
		return new Vector3(vec.x, 0.0f, vec.z);
	}

	public static int ExceptLayerMask(int[] layers) {
		return ~LayerMask(layers);
	}

	public static int ExceptLayerMask(int layer) {
		return ~LayerMask(layer);
	}

	public static int LayerMask(int layer) {
		return (1 << layer);
	}

	public static int LayerMask(int[] layers) {
		int mask = (1 << layers[0]);

		for (int i = 1; i < layers.Length; i++) {
			mask |= (1 << layers[i]);
		}

		return mask;
	}

	public static int EnumToInt(object e){
		return System.Convert.ToInt32(e);
	}
}
