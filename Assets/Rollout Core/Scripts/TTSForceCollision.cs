using UnityEngine;
using System.Collections;

public class TTSForceCollision : TTSPerishingBehaviour {
	
	public Projector proj;
	private Color init;

	void Awake () {
		this.duration = 10.0f;
		proj = GetComponent<Projector>();
		this.destroyWhenLifecycleComplete = true;
		this.useKillFunctionWhenComplete = true;
	}
	
	override protected void OnPerishingUpdate(float progress) {
		transform.position -= transform.forward *0.3f;
	}
	
	override protected void Kill() {
		Destroy (this.gameObject);
	}
}
