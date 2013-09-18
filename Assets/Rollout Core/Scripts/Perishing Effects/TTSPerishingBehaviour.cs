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

/// <summary>
/// This class is used for any effect/object that will only last a set duration
/// </summary>
public class TTSPerishingBehaviour : TTSBehaviour {

	/// <summary> If true, the class will stop running and self-destruct at end of lifecycle </summary>
	public bool destroyWhenLifecycleComplete = true;

	/// <summary> Execute the kill function once complete </summary>
	public bool useKillFunctionWhenComplete = false;

	/// <summary> Set this to true if you want to use FixedUpdate() instead of Update() </summary>
	public bool useFixedUpdate = true;

	/// <summary> Lifecycle duration in seconds. Default 5.0f </summary>
	public float duration = 5.0f;

	/// <summary> Give milliseconds since birth </summary>
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
	
	/// <summary>
	/// Runs every frame for perishing behaviour
	/// </summary>
	/// <param name="progress">A float between 0 and 1 indicating progress</param>
	protected virtual void OnPerishingUpdate(float progress) {
        // Any code you want to run every frame
	}
	
	/// <summary>
	/// Runs when lifecyle complete and either useKillFunctionWhenComplete.
	/// </summary>
	protected virtual void Kill() {
		// Override this function to handle destruction by yourself. Such as letting particle systems bleed out.
	}
}
