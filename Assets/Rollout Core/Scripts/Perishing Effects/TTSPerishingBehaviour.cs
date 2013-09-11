using UnityEngine;
using System.Collections;

public class TTSPerishingBehaviour : TTSBehaviour {
	
	public float duration = 5.0f;
	public bool destroyWhenLifecycleComplete = true;
	public bool useKillFunctionWhenComplete = false;
	public bool useFixedUpdate = true;
	public float progressSinceBirth = 0.0f;
	
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
		
		progressSinceBirth = Time.time - birth;
		_progress = Mathf.Lerp(0,1.0f, progressSinceBirth/duration);
		
		OnPerishingUpdate(_progress);
		
		if(progressSinceBirth/duration > 1.0f) {
			if(destroyWhenLifecycleComplete) {
				Kill();
				Destroy(this);				
			}
			
			if(useKillFunctionWhenComplete) {
				Kill ();
			}
		}
	}
	
	//override this method to do your stuff.
	protected virtual void OnPerishingUpdate(float progress) {
	}
	
	
	protected virtual void Kill() {
		//Override this function to handle destruction by yourself. Such as letting particle systems bleed out.
	}
}
