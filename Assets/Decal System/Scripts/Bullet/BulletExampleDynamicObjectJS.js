//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://edelweissinteractive.com)
//

#pragma strict

import System.Collections.Generic;
import Edelweiss.DecalSystem;

public class BulletExampleDynamicObjectJS extends MonoBehaviour {

		// The prefab which contains the DS_Decals script with already set material and
		// uv rectangles.
	var decalsPrefab : GameObject;
	
		// The reference to the instantiated prefab's DS_Decals instance.
	private var m_Decals : DS_Decals;
	private var m_WorldToDecalsMatrix : Matrix4x4;

		// All the projectors that were created at runtime.
	private var m_DecalProjectors : List.<DecalProjector> = List.<DecalProjector> ();
	
		// Intermediate mesh data. Mesh data is added to that one for a specific projector
		// in order to perform the cutting.
	private var m_DecalsMesh : DecalsMesh;
	private var m_DecalsMeshCutter : DecalsMeshCutter;
	
		// The raycast hits a collider at a certain position. This value indicated how far we need to
		// go back from that hit point along the ray of the raycast to place the new decal projector. Set
		// this value to 0.0f to see why this is needed.
	var decalProjectorOffset : float = 0.5;
	
		// The size of new decal projectors.
	var decalScale : Vector3 = Vector3 (0.2, 2.0, 0.2);
	var cullingAngle : float = 180.0;
	var meshOffset : float = 0.001;
	
		// We iterate through all the defined uv rectangles. This one indices which index we are using at
		// the moment.
	private var m_UVRectangleIndex : int = 0;

		// Move on to the next uv rectangle index.
	private function NextUVRectangleIndex () {
		m_UVRectangleIndex = m_UVRectangleIndex + 1;
		if (m_UVRectangleIndex >= m_Decals.uvRectangles.Length) {
			m_UVRectangleIndex = 0;
		}
	}
	
	function Start () {
	
			// Instantiate the prefab and get its decals instance.
		var l_Instance = UnityEngine.Object.Instantiate (decalsPrefab);
		m_Decals = l_Instance.GetComponentInChildren.<DS_Decals> ();
		
			// Make sure the decals move with this dynamic object.
		m_Decals.transform.parent = transform;
		m_Decals.transform.localPosition = Vector3.zero;
		m_Decals.transform.localScale = Vector3.one;
		
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

	function AddDecalProjector (a_Ray : Ray, a_RaycastHit : RaycastHit) {
			// Make sure there are not too many projectors.
		if (m_DecalProjectors.Count >= 50) {
		
				// If there are more than 50 projectors, we remove the first one from
				// our list and certainly from the decals mesh (the intermediate mesh
				// format). All the mesh data that belongs to this projector will
				// be removed.
			var l_DecalProjectorForRemoval = m_DecalProjectors [0];
			m_DecalProjectors.RemoveAt (0);
			m_DecalsMesh.RemoveProjector (l_DecalProjectorForRemoval);
		}
		
			// Calculate the position and rotation for the new decal projector.
		var l_ProjectorPosition = a_RaycastHit.point - (decalProjectorOffset * a_Ray.direction.normalized);
		var l_ForwardDirection = Camera.main.transform.up;
		var l_UpDirection = - Camera.main.transform.forward;
		var l_ProjectorRotation = Quaternion.LookRotation (l_ForwardDirection, l_UpDirection);
		
			// Randomize the rotation.
		var l_RandomRotation = Quaternion.Euler (0.0f, Random.Range (0.0f, 360.0f), 0.0f);
		l_ProjectorRotation = l_ProjectorRotation * l_RandomRotation;
		
		
			// We hit a collider. Next we have to find the mesh that belongs to the collider.
			// That step depends on how you set up your mesh filters and collider relative to
			// each other in the game objects. It is important to have a consistent way in order
			// to have a simpler implementation.
		
		var l_MeshCollider = a_RaycastHit.collider.GetComponent.<MeshCollider> ();
		var l_MeshFilter = a_RaycastHit.collider.GetComponent.<MeshFilter> ();
		
		if (l_MeshCollider != null || l_MeshFilter != null) {
			var l_Mesh : Mesh = null;
			if (l_MeshCollider != null) {
			
					// Mesh collider was hit. Just use the mesh data from that one.
				l_Mesh = l_MeshCollider.sharedMesh;
			} else if (l_MeshFilter != null) {
			
					// Otherwise take the data from the shared mesh.
				l_Mesh = l_MeshFilter.sharedMesh;
			}
			
			if (l_Mesh != null) {
			
					// Create the decal projector.
				var l_DecalProjector = DecalProjector (l_ProjectorPosition, l_ProjectorRotation, decalScale, cullingAngle, meshOffset, m_UVRectangleIndex, m_UVRectangleIndex);
				
					// Add the projector to our list and the decals mesh, such that both are
					// synchronized. All the mesh data that is now added to the decals mesh
					// will belong to this projector.
				m_DecalProjectors.Add (l_DecalProjector);
				m_DecalsMesh.AddProjector (l_DecalProjector);
				
					// Get the required matrices.
				var l_WorldToMeshMatrix = a_RaycastHit.collider.renderer.transform.worldToLocalMatrix;
				var l_MeshToWorldMatrix = a_RaycastHit.collider.renderer.transform.localToWorldMatrix;
				
					// Add the mesh data to the decals mesh, cut and offset it before we pass it
					// to the decals instance to be displayed.
				m_DecalsMesh.Add (l_Mesh, l_WorldToMeshMatrix, l_MeshToWorldMatrix);						
				m_DecalsMeshCutter.CutDecalsPlanes (m_DecalsMesh);
				m_DecalsMesh.OffsetActiveProjectorVertices ();
				m_Decals.UpdateDecalsMeshes (m_DecalsMesh);
				
				NextUVRectangleIndex ();
			}
		}
	}
}
