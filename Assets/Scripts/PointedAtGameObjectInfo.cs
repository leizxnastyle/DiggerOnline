using System;
using UnityEngine;

[RequireComponent(typeof(InputToEvent))]
public class PointedAtGameObjectInfo : MonoBehaviour
{
	private void OnGUI()
	{
		if (InputToEvent.goPointedAt != null)
		{
			PhotonView photonView = InputToEvent.goPointedAt.GetPhotonView();
			if (photonView != null)
			{
				GUI.Label(new Rect(UnityEngine.Input.mousePosition.x + 5f, (float)Screen.height - UnityEngine.Input.mousePosition.y - 15f, 300f, 30f), string.Format("ViewID {0} InstID {1} Lvl {2} {3}", new object[]
				{
					photonView.viewID,
					photonView.instantiationId,
					photonView.prefix,
					(!photonView.isSceneView) ? ((!photonView.isMine) ? ("owner: " + photonView.ownerId) : "mine") : "scene"
				}));
			}
		}
	}
}
