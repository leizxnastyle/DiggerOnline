using System;
using System.Collections.Generic;
using UnityEngine;

public class TestMeshGenerator : MonoBehaviour
{
	private void Start()
	{
		this.mesh = base.GetComponent<MeshFilter>().mesh;
		this.CreateMesh();
		this.MeshApply();
	}

	private void CreateMesh()
	{
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			Vector3[] array = new Vector3[4];
			BlockFaceStair blockFaceStair = (BlockFaceStair)i;
			if (blockFaceStair == BlockFaceStair.Bottom || blockFaceStair == BlockFaceStair.Forward)
			{
				for (int j = 0; j < 4; j++)
				{
					Vector3 vector = this.offsets[i][j];
					array[j] = vector;
				}
				this.AddBlockSide(array, num);
				num += 4;
			}
			else if (blockFaceStair == BlockFaceStair.Top || blockFaceStair == BlockFaceStair.Back)
			{
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				int num2;
				int num3;
				int num4;
				int num5;
				if (blockFaceStair == BlockFaceStair.Back)
				{
					zero = new Vector3(0.5f, 0f, 0f);
					zero2 = new Vector3(0f, 0.5f, 0f);
					num2 = 0;
					num3 = 1;
					num4 = 2;
					num5 = 3;
				}
				else
				{
					zero = new Vector3(0f, -0.5f, 0f);
					zero2 = new Vector3(-0.5f, 0f, 0f);
					num2 = 2;
					num3 = 3;
					num4 = 0;
					num5 = 1;
				}
				for (int k = 0; k < 2; k++)
				{
					for (int l = 0; l < 4; l++)
					{
						Vector3 vector2 = this.offsets[i][l];
						if (k == 0)
						{
							vector2 += zero;
							if (l == num2 || l == num3)
							{
								vector2 += zero2;
							}
						}
						else if (l == num4 || l == num5)
						{
							vector2 += zero2 * -1f;
						}
						array[l] = vector2;
					}
					this.AddBlockSide(array, num);
					num += 4;
				}
			}
			else
			{
				Vector3 vector3 = new Vector3(-0.5f, 0f, 0f);
				Vector3 b = new Vector3(0f, -0.5f, 0f);
				int num6;
				int num7;
				int num8;
				int num9;
				if (blockFaceStair == BlockFaceStair.Right)
				{
					num6 = 2;
					num7 = 3;
					num8 = 1;
					num9 = 0;
				}
				else
				{
					num6 = 1;
					num7 = 0;
					num8 = 2;
					num9 = 3;
				}
				for (int m = 0; m < 2; m++)
				{
					for (int n = 0; n < 4; n++)
					{
						Vector3 vector4 = this.offsets[i][n];
						if (m == 0)
						{
							if (n == num6 || n == num7)
							{
								vector4 += vector3;
							}
							if (n == num6 || n == num8)
							{
								vector4 += b;
							}
						}
						else if (n == num9 || n == num8)
						{
							vector4 += vector3 * -1f;
						}
						array[n] = vector4;
					}
					this.AddBlockSide(array, num);
					num += 4;
				}
			}
		}
	}

	private void MeshApply()
	{
		this.mesh.vertices = this.m_Vertices.ToArray();
		this.mesh.triangles = this.m_Indices.ToArray();
		this.mesh.uv = this.m_Uvs.ToArray();
		this.mesh.colors = this.m_Colors.ToArray();
		this.mesh.RecalculateNormals();
	}

	private void AddBlockSide(Vector3[] vert, int index)
	{
		foreach (Vector3 item in vert)
		{
			this.m_Vertices.Add(item);
			this.m_Colors.Add(Color.white);
		}
		this.m_Indices.Add(index);
		this.m_Indices.Add(index + 1);
		this.m_Indices.Add(index + 2);
		this.m_Indices.Add(index + 2);
		this.m_Indices.Add(index + 3);
		this.m_Indices.Add(index);
		this.m_Uvs.Add(new Vector2(0f, 0f));
		this.m_Uvs.Add(new Vector2(1f, 0f));
		this.m_Uvs.Add(new Vector2(1f, 1f));
		this.m_Uvs.Add(new Vector2(0f, 1f));
	}

	private Mesh mesh;

	private List<Vector3> m_Vertices = new List<Vector3>();

	private List<int> m_Indices = new List<int>();

	private List<Vector2> m_Uvs = new List<Vector2>();

	private List<Color> m_Colors = new List<Color>();

	private readonly IntVect[][] offsets = new IntVect[][]
	{
		new IntVect[]
		{
			new IntVect(0, 0, 0),
			new IntVect(0, 0, 1),
			new IntVect(0, 1, 1),
			new IntVect(0, 1, 0)
		},
		new IntVect[]
		{
			new IntVect(1, 1, 0),
			new IntVect(1, 1, 1),
			new IntVect(1, 0, 1),
			new IntVect(1, 0, 0)
		},
		new IntVect[]
		{
			new IntVect(1, 0, 0),
			new IntVect(1, 0, 1),
			new IntVect(0, 0, 1),
			new IntVect(0, 0, 0)
		},
		new IntVect[]
		{
			new IntVect(0, 1, 0),
			new IntVect(0, 1, 1),
			new IntVect(1, 1, 1),
			new IntVect(1, 1, 0)
		},
		new IntVect[]
		{
			new IntVect(0, 0, 0),
			new IntVect(0, 1, 0),
			new IntVect(1, 1, 0),
			new IntVect(1, 0, 0)
		},
		new IntVect[]
		{
			new IntVect(1, 0, 1),
			new IntVect(1, 1, 1),
			new IntVect(0, 1, 1),
			new IntVect(0, 0, 1)
		}
	};
}
