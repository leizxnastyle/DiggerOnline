using System;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PhotonNetworkScript : MonoBehaviour
{
	private void Start()
	{
		UnityEngine.Debug.Log("START ");
	}

	private void Update()
	{
		UnityEngine.Debug.Log("UPDATE ");
	}
}
