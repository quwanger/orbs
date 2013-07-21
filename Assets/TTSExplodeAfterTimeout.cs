using UnityEngine;
using System.Collections;

public class TTSExplodeAfterTimeout : MonoBehaviour {

	public float Timeout = 5.0f;
	public GameObject Explosion;
	
	void Start () {
		StartCoroutine("DoTimeout");
	}
	
	private IEnumerator DoTimeout() {
		yield return new WaitForSeconds(Timeout);
		Instantiate(Explosion, this.transform.position, this.transform.rotation);
		Destroy(this.gameObject);
	}
	
	
}
