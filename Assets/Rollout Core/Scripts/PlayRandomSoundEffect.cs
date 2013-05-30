using UnityEngine;
using System.Collections;

public class PlayRandomSoundEffect : MonoBehaviour {
	
	public AudioClip[] sounds;
	private AudioSource audioSource;
	
	void Start () {
		if(sounds.Length < 1) {
			Debug.LogWarning("PlayRandomSoundEffect needs at least one sound to play.");	
		}
		
		audioSource = this.GetComponent<AudioSource>();
		if(audioSource == null) {
			Debug.LogWarning("You need an audiosource component on this prefab.");	
		}
		
		audioSource.loop = false;
		
	}

	void OnCollisionEnter(Collision collision) {
		
		if(collision != null) {
			audioSource.volume = 1*Mathf.Abs(collision.relativeVelocity.magnitude);
		}else{
			audioSource.volume = 1;
		}
		audioSource.PlayOneShot(sounds[Mathf.FloorToInt(Random.value * sounds.Length)]);
	
		
	}
	
	
}
