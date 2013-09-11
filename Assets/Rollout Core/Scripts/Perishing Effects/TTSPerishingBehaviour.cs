using UnityEngine;
using System.Collections;

/*
 * ============= TTSPerishingBehaviour, extends TTSBehaviour =================
 * This class is used for any effect/object that will only last a set duration
 * 
 * Any class that extends this class will implement either/or
 *  - OnPerishingUpdate(float progress)
 *  - Kill()
 *  
 * You can use the public values to set how the update/kill functions will run
 */

public class TTSPerishingBehaviour : TTSBehaviour {
	
	public float duration = 5.0f; // Override this with your own values
	public bool destroyWhenLifecycleComplete = true; // Once duration has passed, the class will stop running and self-destruct
	public bool useKillFunctionWhenComplete = false; // Execute the kill function once complete
	public bool useFixedUpdate = true; // Set this to true if you want to use FixedUpdate() instead of Update()
	
	private float birth;
	private float _progress = 0.0f;
	
	private void Start () {
		birth = Time.time;
	}
	
	private void Update () {
		if(!useFixedUpdate) {
			Tick();
		}
	}
	
	private void FixedUpdate() {
		if(useFixedUpdate) {
			Tick();
		}
	}
	
	private void Tick() {
		
		_progress = Mathf.Lerp(0,1.0f,(Time.time - birth)/duration);
		
		OnPerishingUpdate(_progress);

        if ((Time.time - birth) / duration > 1.0f) {
            if (destroyWhenLifecycleComplete) {
                Kill();
                Destroy(this);
            }
            if (useKillFunctionWhenComplete)
                Kill();
		}
	}
	
	//override this method to do your stuff.
	protected virtual void OnPerishingUpdate(float progress) {
        // Any code you want to run every frame
	}
	
	
	protected virtual void Kill() {
		//Override this function to handle destruction by yourself. Such as letting particle systems bleed out.
	}
}
