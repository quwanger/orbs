using UnityEngine;
using System.Collections;

public class TTSHudObject : MonoBehaviour {
	
	public enum anchors {TOP_CENTER, BOTTOM_CENTER, TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT ,BOTTOM_RIGHT, RIGHT_CENTER, LEFT_CENTER};
	
	public anchors anchorMode;
	
	public Vector2 padding = new Vector2(0,0);
	
	public float depth = 0f;
	
	public Vector2 frustrumDimentions = new Vector2(0,0);

	void Update () {
		calculateFrustrumSize();
		if(anchorMode == anchors.BOTTOM_LEFT) {
			Vector3 newPosition = new Vector3((-frustrumDimentions.x/2) + padding.x, (-frustrumDimentions.y/2) + padding.y, depth);
			transform.localPosition = newPosition;
		}
		
		if(anchorMode == anchors.BOTTOM_RIGHT) {
			Vector3 newPosition = new Vector3((frustrumDimentions.x/2) - padding.x, (-frustrumDimentions.y/2) + padding.y, depth);
			transform.localPosition = newPosition;
		}
		
		if(anchorMode == anchors.TOP_LEFT) {
			Vector3 newPosition = new Vector3((-frustrumDimentions.x/2) + padding.x, (frustrumDimentions.y/2) - padding.y, depth);
			transform.localPosition = newPosition;
		}
		
		if(anchorMode == anchors.TOP_RIGHT) {
			Vector3 newPosition = new Vector3((frustrumDimentions.x/2) - padding.x, (frustrumDimentions.y/2) - padding.y, depth);
			transform.localPosition = newPosition;
		}
		
		if(anchorMode == anchors.TOP_CENTER) {
			Vector3 newPosition = new Vector3(padding.x, (frustrumDimentions.y/2) - padding.y, depth);
			transform.localPosition = newPosition;
		}
		
		if(anchorMode == anchors.BOTTOM_CENTER) {
			Vector3 newPosition = new Vector3(padding.x, (-frustrumDimentions.y/2) + padding.y, depth);
			transform.localPosition = newPosition;
		}
		
		if(anchorMode == anchors.RIGHT_CENTER) {
			Vector3 newPosition = new Vector3((frustrumDimentions.x/2) - padding.x, padding.y, depth);
			transform.localPosition = newPosition;
		}
		
		if(anchorMode == anchors.LEFT_CENTER) {
			Vector3 newPosition = new Vector3((frustrumDimentions.x/2) + padding.x, padding.y, depth);
			transform.localPosition = newPosition;
		}
	}
	
	void calculateFrustrumSize() {
		frustrumDimentions.y = 2.0f * 10f * Mathf.Tan(transform.parent.GetComponent<TTSFloatHud>().boundCamera.camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		frustrumDimentions.x = frustrumDimentions.y * transform.parent.GetComponent<TTSFloatHud>().boundCamera.camera.aspect;
		
	}
}
