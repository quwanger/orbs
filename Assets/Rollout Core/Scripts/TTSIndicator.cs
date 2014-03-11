using UnityEngine;
using System.Collections;

public class TTSIndicator : MonoBehaviour {

	public GameObject racerToIndicate;
	
	public Vector3 screenPosition;
	
	public Camera myCamera;
	
	public int myCameraNumber;
	
	public float myScale = 0.1f;
	
	float ratioW = Screen.width;
	float ratioH = Screen.height;
	float ratio;
	
	// Use this for initialization
	void Start () {
		ratio =  ratioW/ratioH;
		
		TTSFollowCamera tempCamera = myCamera.GetComponent<TTSFollowCamera>();
		
		myCameraNumber = tempCamera.cameraNumber;
		
		switch(tempCamera.cameraNumber){
		case 1:
			this.gameObject.layer = 18;
			tempCamera.camera.cullingMask &= ~(1 << 19);
			tempCamera.camera.cullingMask &= ~(1 << 20);
			tempCamera.camera.cullingMask &= ~(1 << 21);
			tempCamera.camera.cullingMask &= ~(1 << 22);
			tempCamera.camera.cullingMask &= ~(1 << 23);
			break;
		case 2:
			this.gameObject.layer = 19;
			tempCamera.camera.cullingMask &= ~(1 << 18);
			tempCamera.camera.cullingMask &= ~(1 << 20);
			tempCamera.camera.cullingMask &= ~(1 << 21);
			tempCamera.camera.cullingMask &= ~(1 << 22);
			tempCamera.camera.cullingMask &= ~(1 << 23);
			break;
		case 3:
			this.gameObject.layer = 20;
			tempCamera.camera.cullingMask &= ~(1 << 18);
			tempCamera.camera.cullingMask &= ~(1 << 19);
			tempCamera.camera.cullingMask &= ~(1 << 21);
			tempCamera.camera.cullingMask &= ~(1 << 22);
			tempCamera.camera.cullingMask &= ~(1 << 23);
			break;
		case 4:
			this.gameObject.layer = 21;
			tempCamera.camera.cullingMask &= ~(1 << 18);
			tempCamera.camera.cullingMask &= ~(1 << 19);
			tempCamera.camera.cullingMask &= ~(1 << 20);
			tempCamera.camera.cullingMask &= ~(1 << 22);
			tempCamera.camera.cullingMask &= ~(1 << 23);
			break;
		case 5:
			this.gameObject.layer = 22;
			break;
		case 6:
			this.gameObject.layer = 23;
			break;
		default:
			break;
		}
		
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
