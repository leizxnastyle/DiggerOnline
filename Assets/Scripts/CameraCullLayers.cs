using System;
using UnityEngine;

[Serializable]
public class CameraCullLayers : MonoBehaviour
{
	public virtual void Start()
	{
		float[] array = new float[32];
		array[8] = (float)20;
		array[12] = (float)40;
		this.GetComponent<Camera>().layerCullDistances = array;
	}

	public virtual void Main()
	{
	}
}
