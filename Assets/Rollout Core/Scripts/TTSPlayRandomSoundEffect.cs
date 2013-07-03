using UnityEngine;
using System.Collections;

public class TTSPlayRandomSoundEffect : MonoBehaviour {
	
	public AudioClip[] sounds;
	private AudioSource audioSource;
	private float MAX_VOLUME = 0.6f;
	
	void Start() {
		if (sounds.Length < 1) {
			Debug.LogWarning("PlayRandomSoundEffect needs at least one sound to play.");	
		}
		
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null) {
			Debug.LogWarning("You need an audiosource component on this prefab.");	
		}
		
		audioSource.loop = false;
	}

	void OnCollisionEnter(Collision collision) {
		
		if (collision != null) {
			audioSource.volume = TTSUtils.Remap(collision.relativeVelocity.magnitude,0f,10,0f,MAX_VOLUME, true);
		}else {
			audioSource.volume = MAX_VOLUME;
		}
		audioSource.PlayOneShot(sounds[Mathf.FloorToInt(Random.value * sounds.Length)]);
	}
}
