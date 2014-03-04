using UnityEngine;
using System.Collections;

public class TTSIndicator : MonoBehaviour {

	public GameObject racerToIndicate;
	
	public Vector3 screenPosition;
	
	public Camera myCamera;
	
	public float myScale = 0.1f;
	
	float ratioW = Screen.width;
	float ratioH = Screen.height;
	float ratio;
	
	// Use this for initialization
	void Start () {
		ratio =  ratioW/ratioH;
	}
	
	// Update is called once per frame
	void Update () {
		if(racerToIndicate){
			screenPosition = myCamera.WorldToScreenPoint(racerToIndicate.transform.position);
			
			if(screenPosition.z > 0){
				
				ratioW = Screen.width;
				ratioH = Screen.height;
				ratio = ratioW/ratioH;
				
				//Debug.Log(screenPosition.z);
				
				float scaleFactor = screenPosition.z;
				
				Rect newInset = new Rect(screenPosition.x, screenPosition.y, 1f, 1f);
				
				if(scaleFactor<1.0f)
					scaleFactor = 1.0f;
				else if(scaleFactor > 300.0f)
					scaleFactor = 300.0f;
				
				//this is to increase the size to make it visible
				scaleFactor /= 100.0f;
				
				Vector3 newPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
				
				this.transform.position = newPosition;
				this.transform.localScale = new Vector3((myScale/ratio)/scaleFactor, myScale/scaleFactor, 1f);
				
				this.GetComponent<GUITexture>().pixelInset = newInset;
			}
		}
	}
	
	public void PlaceIndicator(Vector3 screenPosition){
		
	}
}
