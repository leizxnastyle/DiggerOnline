using System;
using UnityEngine;

[Serializable]
public class TerrainGen : MonoBehaviour
{
	public virtual void Start()
	{
		for (int i = -10; i < 10; i++)
		{
			for (int j = -10; j < 10; j++)
			{
				for (int k = -10; k <= 0; k++)
				{
					UnityEngine.Object.Instantiate(this.prefab, new Vector3((float)j, (float)k, (float)i), Quaternion.identity);
				}
			}
		}
	}

	public virtual void Main()
	{
	}

	public GameObject prefab;
}
