using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bam
{
	public class TerrainPropertyReader : MonoBehaviour
	{
		[SerializeField]
		private TerrainProperties.Properties_s[] m_properties;
		
		//Because we are not using 5.5 I can't use the nice new GetTriangles overload so instead gotta cache it manually. Grrr.
		private static Dictionary<Mesh, int[][]> s_meshTriangleCache;//First element in list<int[]> is all_triangle indicies for the mesh, then submeshes triangles indicies
		private const int SUBMESH_CACHE_OFFSET = 1; //Who likes magic numbers? no one.
		private const int ALL_INDICIES_CACHE_INDEX = 0; //^^
		
		public TerrainPropertyReader()
		{
			if (s_meshTriangleCache==null) s_meshTriangleCache = new Dictionary<Mesh, int[][]>();
		}

	
		/// <summary>
		/// This is a heafty operation so do it once and cache the properties!
		/// </summary>
		/// <param name="hits"></param>
		/// <param name="properties"></param>
		public void GetPropertiesFromRays(RaycastHit[] hits, ref List<TerrainProperties.Properties_s> properties)
		{
			properties.Clear();
			foreach (var hit in hits)
			{
				TerrainProperties.Properties_s p = new TerrainProperties.Properties_s();
				if (GetPropertyFromRay(hit, ref p))
				{
					properties.Add(p);
				}
			}
		}

		/// <summary>
		/// This is a heafty operation so do it once and cache the properties!
		/// </summary>
		/// <param name="hits"></param>
		/// <param name="properties"></param>
		public bool GetPropertyFromRay(RaycastHit hit, ref TerrainProperties.Properties_s property)
		{
			if (hit.collider == null) { return false; }

			Renderer renderer = hit.collider.GetComponent<Renderer>();
			var meshCollider = hit.collider as MeshCollider;
		
			if (renderer == null || renderer.sharedMaterial == null || meshCollider == null)
			{
				return false;
			}

			int materialIdx = -1;

			Mesh mesh = meshCollider.sharedMesh;
			if (!mesh) return false;

			int subMeshesNr = mesh.subMeshCount;
			bool newCacheEntry = false;
			if (s_meshTriangleCache.ContainsKey(mesh) == false)
			{
				int[][] array2D = new int[subMeshesNr + 1][];
				s_meshTriangleCache.Add(mesh, array2D);
				newCacheEntry = true;
				//Debug.Log("[Yams] Adding new mesh to TerrainPropertyReader triangle index cache.");
			}

			/*Converted to C#, and modified to be more efficient, from https://forum.unity3d.com/threads/get-material-from-raycast.53123/*/
			int[] tr;
			if (newCacheEntry)
			{
				tr = mesh.triangles;
				s_meshTriangleCache[mesh][ALL_INDICIES_CACHE_INDEX] = tr;
			}
			else
			{
				tr = s_meshTriangleCache[mesh][ALL_INDICIES_CACHE_INDEX];
			}

			int triangleIdx = hit.triangleIndex;
			int lookupIdx1 = tr[triangleIdx * 3];
			int lookupIdx2 = tr[triangleIdx * 3 + 1];
			int lookupIdx3 = tr[triangleIdx * 3 + 2];

			for (var i = 0; i < subMeshesNr; i++)
			{
				if (newCacheEntry)
				{
					tr = mesh.GetTriangles(i);
					s_meshTriangleCache[mesh][i + SUBMESH_CACHE_OFFSET] = tr;
				}
				else
				{
					tr = s_meshTriangleCache[mesh][i + SUBMESH_CACHE_OFFSET];
				}

				if (materialIdx == -1)
				{
					for (var j = 0; j < tr.Length; j += 3)
					{
						if (tr[j] == lookupIdx1 && tr[j + 1] == lookupIdx2 && tr[j + 2] == lookupIdx3)
						{
							materialIdx = i;
							break;
						}
					}
				}

				if (materialIdx != -1 && !newCacheEntry)
				{
					break;
				}
			}

			string name = null;
			if (TerrainProperties.GetName(renderer.sharedMaterials[materialIdx], ref name))
			{
				if (GetProperty(name, ref property))
				{
					return true;
				}
			}


			return false;
		}

		public bool GetProperty(string name, ref TerrainProperties.Properties_s property)
		{
			foreach (var p in m_properties)
			{
				if (p.m_friendlyName == name)
				{
					property = p;
					return true;
				}
			}
			return false;
		}

	}
}