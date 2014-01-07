//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.DecalSystem;

public class BulletLightmapExampleCS : MonoBehaviour {
	
		// The prefab which contains the DS_Decals script with already set material and
		// uv rectangles.
		//
		// IMPORTANT:
		// In our test scene, the lightmaps are baked to the uv2 of each mesh.
		// Thus it is necessary that this prefab's DS_Decals instance has the uv2 set to
		// TargetUV2.
	public GameObject decalsPrefab;
		
		// All the decals that were created at runtime.
	private List <GameObject> m_DecalsInstances = new List <GameObject> ();
	
		// Intermediate mesh data. Mesh data is added to that one for a specific projector
		// in order to perform the cutting.
	private DecalsMesh m_DecalsMesh;
	private DecalsMeshCutter m_DecalsMeshCutter;
	
		// The raycast hits a collider at a certain position. This value indicated how far we need to
		// go back from that hit point along the ray of the raycast to place the new decal projector. Set
		// this value to 0.0f to see why this is needed.
	public float decalProjectorOffset = 0.5f;
	
		// The size of new decal projectors.
	public Vector3 decalProjectorScale = new Vector3 (0.2f, 2.0f, 0.2f);
	public float cullingAngle = 90.0f;
	public float meshOffset = 0.001f;
	
		// We iterate through all the defined uv rectangles. This one indices which index we are using at
		// the moment.
		//
		// REMARK:
		// We use 7 as the first index because the previous ones are too dark and the lightmapping
		// is barely visible for them.
	private int m_UVRectangleIndex = 7;
	
		// Move on to the next uv rectangle index.
	private void NextUVRectangleIndex (DS_Decals a_Decals) {
		m_UVRectangleIndex = m_UVRectangleIndex + 1;
		
		if (m_UVRectangleIndex >= a_Decals.uvRectangles.Length) {
			m_UVRectangleIndex = 7;
		}
	}
	
	private void Start () {
		
			// Create the decals mesh (intermediate mesh data) for our decals instance.
			// Further we need a decals mesh cutter instance.
		m_DecalsMesh = new DecalsMesh ();
		m_DecalsMeshCutter = new DecalsMeshCutter ();
	}
	
	private void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			Ray l_Ray = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.0f));
			RaycastHit l_RaycastHit;
			
				// Terrains have no uv2, so we just skip them.
			if
				(Physics.Raycast (l_Ray, out l_RaycastHit, Mathf.Infinity) &&
				 l_RaycastHit.collider as TerrainCollider == null)
			{
				
					// Collider hit.
				
					// Make sure there are not too many decals instances.
				if (m_DecalsInstances.Count >= 50) {
					m_DecalsInstances.RemoveAt (0);
				}
				
				
					// Instantiate the prefab and get its decals instance.
				GameObject l_Instance = Instantiate (decalsPrefab) as GameObject;
				DS_Decals l_Decals = l_Instance.GetComponentInChildren <DS_Decals> ();
				
					// Reuse the decals mesh, but be sure to initialize it always for the current
					// decals instance.
				m_DecalsMesh.Initialize (l_Decals);
				
					// Calculate the position and rotation for the new decal projector.
				Vector3 l_ProjectorPosition = l_RaycastHit.point - (decalProjectorOffset * l_Ray.direction.normalized);
				Vector3 l_ForwardDirection = Camera.main.transform.up;
				Vector3 l_UpDirection = - Camera.main.transform.forward;
				Quaternion l_ProjectorRotation = Quaternion.LookRotation (l_ForwardDirection, l_UpDirection);
				
					// Randomize the rotation.
				Quaternion l_RandomRotation = Quaternion.Euler (0.0f, Random.Range (0.0f, 360.0f), 0.0f);
				l_ProjectorRotation = l_ProjectorRotation * l_RandomRotation;

				
					// We hit a collider. Next we have to find the mesh that belongs to the collider.
					// That step depends on how you set up your mesh filters and collider relative to
					// each other in the game objects. It is important to have a consistent way in order
					// to have a simpler implementation.
				
				MeshCollider l_MeshCollider = l_RaycastHit.collider.GetComponent <MeshCollider> ();
				MeshFilter l_MeshFilter = l_RaycastHit.collider.GetComponent <MeshFilter> ();
				MeshRenderer l_MeshRenderer = l_RaycastHit.collider.GetComponent <MeshRenderer> ();
				
				if (l_MeshCollider != null || l_MeshFilter != null) {
					Mesh l_Mesh = null;
					if (l_MeshCollider != null) {
						
							// Mesh collider was hit. Just use the mesh data from that one.
						l_Mesh = l_MeshCollider.sharedMesh;
					} else if (l_MeshFilter != null) {
						
							// Otherwise take the data from the shared mesh.
						l_Mesh = l_MeshFilter.sharedMesh;
					}
					
					if (l_Mesh != null) {
						
							// Create the decal projector.
						DecalProjector l_DecalProjector = new DecalProjector (l_ProjectorPosition, l_ProjectorRotation, decalProjectorScale, cullingAngle, meshOffset, m_UVRectangleIndex, m_UVRectangleIndex);
						
							// All the mesh data that is now added to the decals mesh
							// will belong to this projector.
						m_DecalsMesh.AddProjector (l_DecalProjector);
						
							// Get the required matrices.
						Matrix4x4 l_WorldToMeshMatrix = l_RaycastHit.collider.renderer.transform.worldToLocalMatrix;
						Matrix4x4 l_MeshToWorldMatrix = l_RaycastHit.collider.renderer.transform.localToWorldMatrix;
						
							// Add the mesh data to the decals mesh, cut and offset it before we pass it
							// to the decals instance to be displayed.
						m_DecalsMesh.Add (l_Mesh, l_WorldToMeshMatrix, l_MeshToWorldMatrix);						
						m_DecalsMeshCutter.CutDecalsPlanes (m_DecalsMesh);
						m_DecalsMesh.OffsetActiveProjectorVertices ();
						l_Decals.UpdateDecalsMeshes (m_DecalsMesh);
						
							// Lightmapping
						l_Decals.DecalsMeshRenderers [0].MeshRenderer.lightmapIndex = l_MeshRenderer.lightmapIndex;
						l_Decals.DecalsMeshRenderers [0].MeshRenderer.lightmapTilingOffset = l_MeshRenderer.lightmapTilingOffset;
						
							// For the next hit, use a new uv rectangle. Usually, you would select the uv rectangle
							// based on the surface you have hit.
						NextUVRectangleIndex (l_Decals);
					}
				}
			}
		}
	}
}
