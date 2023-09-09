using System;
using System.Collections.Generic;
using System.Text;
using hd;
using UnityEngine;

public class PMeshData : HDPoolableObject
{
	private PMeshData(int alignedQuadCount)
	{
		this.QuadCount = alignedQuadCount;
		this.m_indices = new int[this.QuadCount * 6];
		this.m_uvs = new Vector2[this.QuadCount * 4];
		this.m_vertices = new Vector3[this.QuadCount * 4];
		this.m_colors = new Color[this.QuadCount * 4];
	}

	private static int AlignQuadCount(int quadCount)
	{
		return (quadCount % 64 != 0) ? (quadCount / 64 * 64 + 64) : quadCount;
	}

	public static PMeshData AcquireFor(Chunk.MeshData md)
	{
		PMeshData pmeshData = PMeshData.Acquire(md.m_Colors.Count / 4);
		pmeshData.AssignFrom(md);
		return pmeshData;
	}

	public static PMeshData Acquire(int quadCount)
	{
		quadCount = PMeshData.AlignQuadCount(quadCount);
		if (!PMeshData.pools.ContainsKey(quadCount))
		{
			PMeshData.pools.Add(quadCount, new HDPool<PMeshData>());
		}
		PMeshData pmeshData = PMeshData.pools[quadCount].Acquire();
		if (pmeshData != null && pmeshData.isAcquired)
		{
			throw new Exception("Object is already acquired!");
		}
		if (pmeshData == null)
		{
			pmeshData = new PMeshData(quadCount);
		}
		pmeshData.isAcquired = true;
		return pmeshData;
	}

	public void AssignFrom(Chunk.MeshData md)
	{
		if (md.m_Colors.Count > this.m_colors.Length)
		{
			throw new Exception("Invalid PMeshData size");
		}
		int num = md.m_Vertices.Count;
		md.m_Indices.CopyTo(this.m_indices, 0);
		int i = md.m_Indices.Count;
		while (i < this.m_indices.Length)
		{
			this.m_indices[i] = num;
			this.m_indices[i + 1] = num + 1;
			this.m_indices[i + 2] = num + 2;
			this.m_indices[i + 3] = num + 2;
			this.m_indices[i + 4] = num + 3;
			this.m_indices[i + 5] = num;
			i += 6;
			num += 4;
		}
		md.m_Uvs.CopyTo(this.m_uvs, 0);
		for (int j = md.m_Uvs.Count; j < this.m_uvs.Length; j++)
		{
			this.m_uvs[j] = Vector2.zero;
		}
		md.m_Vertices.CopyTo(this.m_vertices, 0);
		for (int k = md.m_Vertices.Count; k < this.m_vertices.Length; k++)
		{
			this.m_vertices[k] = md.m_Vertices[0];
		}
		md.m_Colors.CopyTo(this.m_colors, 0);
		for (int l = md.m_Colors.Count; l < this.m_colors.Length; l++)
		{
			this.m_colors[l] = new Color(0f, 0f, 0f, 0f);
		}
	}

	public void ApplyTo(Mesh mesh)
	{
		PMeshData pmeshData = null;
		if (PMeshData.inUse.TryGetValue(mesh, out pmeshData))
		{
			PMeshData.inUse.Remove(mesh);
			pmeshData.Release();
		}
		else if (mesh.colors != null && mesh.colors.Length > 0)
		{
			hd.Debug.LogError("PMeshData: Invalid data in ApplyTo");
		}
		PMeshData.inUse.Add(mesh, this);
		mesh.Clear();
		mesh.vertices = this.m_vertices;
		mesh.uv = this.m_uvs;
		mesh.colors = this.m_colors;
		mesh.triangles = this.m_indices;
	}

	public static void Reset()
	{
		hd.Debug.LogWarning("PMeshData.Reset");
		foreach (PMeshData pmeshData in PMeshData.inUse.Values)
		{
			pmeshData.Release();
		}
		PMeshData.inUse.Clear();
	}

	public override void Release()
	{
		if (!this.isAcquired)
		{
			throw new Exception("Object is not acquired!");
		}
		if (!PMeshData.pools.ContainsKey(this.QuadCount))
		{
			hd.Debug.LogError("PMeshData.Release called, but pools does not contain element for quadCount=" + this.QuadCount);
		}
		else
		{
			this.isAcquired = false;
			PMeshData.pools[this.QuadCount].Release(this);
		}
	}

	public static string GetDebugInfo()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<int, HDPool<PMeshData>> keyValuePair in PMeshData.pools)
		{
			stringBuilder.AppendFormat("PMeshData[{0}] InUse={1} Cached={2}", keyValuePair.Key, keyValuePair.Value.PoolCount, keyValuePair.Value.CachedCount);
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}

	private const int SizeStep = 64;

	private static readonly Dictionary<int, HDPool<PMeshData>> pools = new Dictionary<int, HDPool<PMeshData>>();

	private static readonly Dictionary<Mesh, PMeshData> inUse = new Dictionary<Mesh, PMeshData>();

	public readonly int QuadCount;

	private readonly int[] m_indices;

	private readonly Vector2[] m_uvs;

	private readonly Vector3[] m_vertices;

	private readonly Color[] m_colors;

	private bool isAcquired;
}
