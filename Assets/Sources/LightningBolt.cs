/*
	This script is placed in public domain. The author takes no responsibility for any possible harm.
	Contributed by Jonathan Czeck
*/
using UnityEngine;
using System.Collections;

public class LightningBolt : MonoBehaviour
{
	public Transform target;
	public int zigs = 100;
	public float speed = 1f;
	public float scale = 1f;
	public GameObject startLight;
	public GameObject endLight;
	
	public LineRenderer linerenderer;
	private Vector3[] positions;
	
	Perlin noise;
	float oneOverZigs;
	
	private Particle[] particles;
	
	void Start()
	{
		positions = new Vector3[zigs];
		oneOverZigs = 1f / (float)zigs;
		linerenderer = GetComponent<LineRenderer>();
		linerenderer.SetVertexCount(zigs);
	}
	
	void Update ()
	{
		if(target != null) {
			if (noise == null)
				noise = new Perlin();
				
			float timex = Time.time * speed * 0.1365143f;
			float timey = Time.time * speed * 1.21688f;
			float timez = Time.time * speed * 2.5564f;
			
			for (int i=0; i < zigs; i++)
			{
				Vector3 position = Vector3.Lerp(transform.position, target.position, oneOverZigs * (float)i);
				Vector3 offset = new Vector3(noise.Noise(timex + position.x, timex + position.y, timex + position.z),
											noise.Noise(timey + position.x, timey + position.y, timey + position.z),
											noise.Noise(timez + position.x, timez + position.y, timez + position.z));
				position += (offset * scale * ((float)i * oneOverZigs));
				positions[i] = position;
				linerenderer.SetPosition(i, position);

			}
			
			if (startLight)
				startLight.transform.position = positions[0];
			if (endLight)
				endLight.transform.position = positions[positions.Length - 1];		

		}
	}	
}