using System;
using UnityEngine;

public class ChunkGameObject : MonoBehaviour
{
	private void Awake()
	{
		this._MeshFilter = base.gameObject.GetComponent<MeshFilter>();
		if (this._MeshFilter == null)
		{
			this._MeshFilter = base.gameObject.AddComponent<MeshFilter>();
		}
		this._MeshCollider = base.gameObject.GetComponent<MeshCollider>();
		if (this._MeshCollider == null)
		{
			this._MeshCollider = base.gameObject.AddComponent<MeshCollider>();
		}
		this._MeshRenderer = base.gameObject.GetComponent<MeshRenderer>();
		if (this._MeshRenderer.material.mainTexture == null)
		{
			this._MeshRenderer.material.mainTexture = WorldGameObjectX.Instance.WorldTextureAtlas;
		}
		this._MeshRenderer.material.SetFloat("_TimeOfDay", TimeOfDay.NormalizedTime);
	}

	public void CreateFromChunk(Chunk.MeshData data)
	{
		PMeshData pmeshData = PMeshData.AcquireFor(data);
		pmeshData.ApplyTo(this._MeshFilter.mesh);
		this._MeshCollider.sharedMesh = null;
		this._MeshCollider.sharedMesh = this._MeshFilter.mesh;
		this._MeshFilter.mesh.RecalculateNormals();
		data.m_Vertices.Clear();
		data.m_Uvs.Clear();
		data.m_Colors.Clear();
		data.m_Indices.Clear();
	}

	private MeshFilter _MeshFilter;

	private MeshCollider _MeshCollider;

	private MeshRenderer _MeshRenderer;
}
