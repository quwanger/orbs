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

public class ColoredBulletExampleCS : MonoBehaviour {
	
		// The prefab which contains the DS_Decals script with already set material and
		// uv rectangles.
	public GameObject decalsPrefab;
	
		// The reference to the instantiated prefab's DS_Decals instance.
	private DS_Decals m_Decals;
	private Matrix4x4 m_WorldToDecalsMatrix;
	
		// All the projectors that were created at runtime.
	private List <DecalProjector> m_DecalProjectors = new List <DecalProjector> ();
	
		// Intermediate mesh data. Mesh data is added to that one for a specific projector
		// in order to perform the cutting.
	private DecalsMesh m_DecalsMesh;
	private DecalsMeshCutter m_DecalsMeshCutter;
	
		// Vertex colors.
	private List <Color> m_VertexColors = new List <Color> ();
	
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
	private int m_UVRectangleIndex = 0;
	
		// Color iterator.
	private int m_ColorIndex = 0;
	
		// Move on to the next uv rectangle index.
	private void NextUVRectangleIndex () {
		m_UVRectangleIndex = m_UVRectangleIndex + 1;
		if (m_UVRectangleIndex >= m_Decals.uvRectangles.Length) {
			m_UVRectangleIndex = 0;
		}
	}
	
		// Move on to the next vertex color.
	private void NextColorIndex () {
		m_ColorIndex = m_ColorIndex + 1;
		if (m_ColorIndex > 2) {
			m_ColorIndex = 0;
		}
	}
	
	private Color CurrentColor {
		get {
			Color l_Color;
			if (m_ColorIndex == 0) {
				l_Color = Color.red;
			} else if (m_ColorIndex == 1) {
				l_Color = Color.green;
			} else {
				l_Color = Color.blue;
			}
			return (l_Color);
		}
	}
	
	private void Start () {
		
			// Instantiate the prefab and get its decals instance.
		GameObject l_Instance = Instantiate (decalsPrefab) as GameObject;
		m_Decals = l_Instance.GetComponentInChildren <DS_Decals> ();
		
		if (m_Decals == null) {
			Debug.LogError ("The 'decalsPrefab' does not contain a 'DS_Decals' instance!");
		} else {
			
				// Create the decals mesh (intermediate mesh data) for our decals instance.
				// Further we need a decals mesh cutter instance and the world to decals matrix.
			m_DecalsMesh = new DecalsMesh (m_Decals);
			m_DecalsMeshCutter = new DecalsMeshCutter ();
			m_WorldToDecalsMatrix = m_Decals.CachedTransform.worldToLocalMatrix;
		}
	}
	
	private void Update () {
		if (Input.GetKeyDown (KeyCode.C)) {

				// Remove all projectors.
			while (m_DecalProjectors.Count > 0) {
				DecalProjector l_Projector = m_DecalProjectors [m_DecalProjectors.Count - 1];
				m_DecalsMesh.RemoveProjector (l_Projector);
				m_DecalProjectors.RemoveAt (m_DecalProjectors.Count - 1);
			}
			m_Decals.UpdateDecalsMeshes (m_DecalsMesh);
		}

		if (Input.GetButtonDown ("Fire1")) {
			Ray l_Ray = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.0f));
			RaycastHit l_RaycastHit;
			if (Physics.Raycast (l_Ray, out l_RaycastHit, Mathf.Infinity)) {
				
					// Collider hit.
				
					// Make sure there are not too many projectors.
				if (m_DecalProjectors.Count >= 50) {
					
						// If there are more than 50 projectors, we remove the first one from
						// our list and certainly from the decals mesh (the intermediate mesh
						// format). All the mesh data that belongs to this projector will
						// be removed.
					DecalProjector l_DecalProjector = m_DecalProjectors [0];
					
						// The vertex color list has to be updated as well.
					m_VertexColors.RemoveRange (0, l_DecalProjector.DecalsMeshUpperVertexIndex + 1);
					
					m_DecalProjectors.RemoveAt (0);
					m_DecalsMesh.RemoveProjector (l_DecalProjector);
				}
				
					// Calculate the position and rotation for the new decal projector.
				Vector3 l_ProjectorPosition = l_RaycastHit.point - (decalProjectorOffset * l_Ray.direction.normalized);
				Vector3 l_ForwardDirection = Camera.main.transform.up;
				Vector3 l_UpDirection = - Camera.main.transform.forward;
				Quaternion l_ProjectorRotation = Quaternion.LookRotation (l_ForwardDirection, l_UpDirection);
				
					// Randomize the rotation.
				Quaternion l_RandomRotation = Quaternion.Euler (0.0f, Random.Range (0.0f, 360.0f), 0.0f);
				l_ProjectorRotation = l_ProjectorRotation * l_RandomRotation;
				
				TerrainCollider l_TerrainCollider = l_RaycastHit.collider as TerrainCollider;
				if (l_TerrainCollider != null) {
					
						// Terrain collider hit.
					
					Terrain l_Terrain = l_TerrainCollider.GetComponent <Terrain> ();
					if (l_Terrain != null) {
						
							// Create the decal projector with all the required information.
						DecalProjector l_DecalProjector = new DecalProjector (l_ProjectorPosition, l_ProjectorRotation, decalProjectorScale, cullingAngle, meshOffset, m_UVRectangleIndex, m_UVRectangleIndex);
						
							// Add the projector to our list and the decals mesh, such that both are
							// synchronized. All the mesh data that is now added to the decals mesh
							// will belong to this projector.
						m_DecalProjectors.Add (l_DecalProjector);
						m_DecalsMesh.AddProjector (l_DecalProjector);
						
							// The terrain data has to be converted to the decals instance's space.
						Matrix4x4 l_TerrainToDecalsMatrix = Matrix4x4.TRS (l_Terrain.transform.position, Quaternion.identity, Vector3.one) * m_WorldToDecalsMatrix;
						
							// Pass the terrain data with the corresponding conversion to the decals mesh.
						m_DecalsMesh.Add (l_Terrain, l_TerrainToDecalsMatrix);
						
							// Cut the data in the decals mesh accoring to the size and position of the decal projector. Offset the
							// vertices afterwards and pass the newly computed mesh to the decals instance, such that it becomes
							// visible.
						m_DecalsMeshCutter.CutDecalsPlanes (m_DecalsMesh);
						m_DecalsMesh.OffsetActiveProjectorVertices ();
						m_Decals.UpdateDecalsMeshes (m_DecalsMesh);
						
							// Update the vertex colors too.
						Color l_VertexColor = CurrentColor;
						int l_VertexCount = l_DecalProjector.DecalsMeshUpperVertexIndex - l_DecalProjector.DecalsMeshLowerVertexIndex + 1;
						for (int i = 0; i < l_VertexCount; i = i + 1) {
							m_VertexColors.Add (l_VertexColor);
						}
						m_Decals.DecalsMeshRenderers [0].MeshFilter.mesh.colors = m_VertexColors.ToArray ();
						
							// For the next hit, use a new uv rectangle. Usually, you would select the uv rectangle
							// based on the surface you have hit.
						NextUVRectangleIndex ();
						NextColorIndex ();
					} else {
						Debug.Log ("Terrain is null!");
					}
					
				} else {
					
						// We hit a collider. Next we have to find the mesh that belongs to the collider.
						// That step depends on how you set up your mesh filters and collider relative to
						// each other in the game objects. It is important to have a consistent way in order
						// to have a simpler implementation.
					
					MeshCollider l_MeshCollider = l_RaycastHit.collider.GetComponent <MeshCollider> ();
					MeshFilter l_MeshFilter = l_RaycastHit.collider.GetComponent <MeshFilter> ();
					
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
							
								// Add the projector to our list and the decals mesh, such that both are
								// synchronized. All the mesh data that is now added to the decals mesh
								// will belong to this projector.
							m_DecalProjectors.Add (l_DecalProjector);
							m_DecalsMesh.AddProjector (l_DecalProjector);
							
								// Get the required matrices.
							Matrix4x4 l_WorldToMeshMatrix = l_RaycastHit.collider.renderer.transform.worldToLocalMatrix;
							Matrix4x4 l_MeshToWorldMatrix = l_RaycastHit.collider.renderer.transform.localToWorldMatrix;
							
								// Add the mesh data to the decals mesh, cut and offset it before we pass it
								// to the decals instance to be displayed.
							m_DecalsMesh.Add (l_Mesh, l_WorldToMeshMatrix, l_MeshToWorldMatrix);						
							m_DecalsMeshCutter.CutDecalsPlanes (m_DecalsMesh);
							m_DecalsMesh.OffsetActiveProjectorVertices ();
							m_Decals.UpdateDecalsMeshes (m_DecalsMesh);
							
								// Update the vertex colors too.
							Color l_VertexColor = CurrentColor;
							int l_VertexCount = l_DecalProjector.DecalsMeshUpperVertexIndex - l_DecalProjector.DecalsMeshLowerVertexIndex + 1;
							for (int i = 0; i < l_VertexCount; i = i + 1) {
								m_VertexColors.Add (l_VertexColor);
							}
							m_Decals.DecalsMeshRenderers [0].MeshFilter.mesh.colors = m_VertexColors.ToArray ();
							
								// For the next hit, use a new uv rectangle. Usually, you would select the uv rectangle
								// based on the surface you have hit.
							NextUVRectangleIndex ();
							NextColorIndex ();
						}
					}
				}
			}
		}
	}
}
