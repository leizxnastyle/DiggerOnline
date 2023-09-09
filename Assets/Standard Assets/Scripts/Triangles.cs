using System;
using UnityEngine;

[Serializable]
public class Triangles : MonoBehaviour
{
	public static bool HasMeshes()
	{
		bool result;
		if (Triangles.meshes == null)
		{
			result = false;
		}
		else
		{
			int i = 0;
			Mesh[] array = Triangles.meshes;
			int length = array.Length;
			while (i < length)
			{
				if (null == array[i])
				{
					return false;
				}
				i++;
			}
			result = true;
		}
		return result;
	}

	public static void Cleanup()
	{
		if (Triangles.meshes != null)
		{
			int i = 0;
			Mesh[] array = Triangles.meshes;
			int length = array.Length;
			while (i < length)
			{
				if (null != array[i])
				{
					UnityEngine.Object.DestroyImmediate(array[i]);
					array[i] = null;
				}
				i++;
			}
			Triangles.meshes = null;
		}
	}

	public static Mesh[] GetMeshes(int totalWidth, int totalHeight)
	{
		Mesh[] result;
		if (Triangles.HasMeshes() && Triangles.currentTris == totalWidth * totalHeight)
		{
			result = Triangles.meshes;
		}
		else
		{
			int num = 21666;
			int num2 = totalWidth * totalHeight;
			Triangles.currentTris = num2;
			int num3 = Mathf.CeilToInt(1f * (float)num2 / (1f * (float)num));
			Triangles.meshes = new Mesh[num3];
			int num4 = 0;
			for (int i = 0; i < num2; i += num)
			{
				int triCount = Mathf.FloorToInt((float)Mathf.Clamp(num2 - i, 0, num));
				Triangles.meshes[num4] = Triangles.GetMesh(triCount, i, totalWidth, totalHeight);
				num4++;
			}
			result = Triangles.meshes;
		}
		return result;
	}

	public static Mesh GetMesh(int triCount, int triOffset, int totalWidth, int totalHeight)
	{
		Mesh mesh = new Mesh();
		mesh.hideFlags = HideFlags.DontSave;
		Vector3[] array = new Vector3[triCount * 3];
		Vector2[] array2 = new Vector2[triCount * 3];
		Vector2[] array3 = new Vector2[triCount * 3];
		int[] array4 = new int[triCount * 3];
		for (int i = 0; i < triCount; i++)
		{
			int num = i * 3;
			int num2 = triOffset + i;
			float num3 = Mathf.Floor((float)(num2 % totalWidth)) / (float)totalWidth;
			float num4 = Mathf.Floor((float)(num2 / totalWidth)) / (float)totalHeight;
			Vector3 vector = new Vector3(num3 * (float)2 - (float)1, num4 * (float)2 - (float)1, 1f);
			array[num + 0] = vector;
			array[num + 1] = vector;
			array[num + 2] = vector;
			array2[num + 0] = new Vector2((float)0, (float)0);
			array2[num + 1] = new Vector2(1f, (float)0);
			array2[num + 2] = new Vector2((float)0, 1f);
			array3[num + 0] = new Vector2(num3, num4);
			array3[num + 1] = new Vector2(num3, num4);
			array3[num + 2] = new Vector2(num3, num4);
			array4[num + 0] = num + 0;
			array4[num + 1] = num + 1;
			array4[num + 2] = num + 2;
		}
		mesh.vertices = array;
		mesh.triangles = array4;
		mesh.uv = array2;
		mesh.uv2 = array3;
		return mesh;
	}

	public virtual void Main()
	{
	}

	[NonSerialized]
	public static Mesh[] meshes;

	[NonSerialized]
	public static int currentTris;
}
