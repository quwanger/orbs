using UnityEngine;
using System.Collections;

public class TTSIndicator : MonoBehaviour {

	public GameObject racerToIndicate;
	
	public Vector3 screenPosition;
	
	public Camera myCamera;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(racerToIndicate){
			screenPosition = myCamera.WorldToScreenPoint(racerToIndicate.transform.position);
			
			if(screenPosition.z > 0){
				Rect newInset = new Rect(screenPosition.x, screenPosition.y, screenPosition.z, screenPosition.z);
				
				float scaleFactor = screenPosition.z/100.0f;
				
				this.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
				
				this.GetComponent<GUITexture>().pixelInset = newInset;
			}
		}
	}
	
	public void PlaceIndicator(Vector3 screenPosition){
		
	}
}
