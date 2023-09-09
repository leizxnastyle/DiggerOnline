using System;
using UnityEngine;

public class OnJoinedInstantiate : MonoBehaviour
{
	public void OnJoinedRoom()
	{
		if (this.PrefabsToInstantiate != null)
		{
			foreach (GameObject gameObject in this.PrefabsToInstantiate)
			{
				UnityEngine.Debug.Log("Instantiating: " + gameObject.name);
				Vector3 a = Vector3.up;
				if (this.SpawnPosition != null)
				{
					a = this.SpawnPosition.position;
				}
				Vector3 a2 = UnityEngine.Random.insideUnitSphere;
				a2.y = 0f;
				a2 = a2.normalized;
				Vector3 position = a + this.PositionOffset * a2;
				PhotonNetwork.Instantiate(gameObject.name, position, Quaternion.identity, 0);
			}
		}
	}

	public Transform SpawnPosition;

	public float PositionOffset = 2f;

	public GameObject[] PrefabsToInstantiate;
}
