using System;
using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ShowInfoOfPlayer : Photon.MonoBehaviour
{
	private void Start()
	{
		if (this.font == null)
		{
			this.font = (Font)Resources.FindObjectsOfTypeAll(typeof(Font))[0];
			UnityEngine.Debug.LogWarning("No font defined. Found font: " + this.font);
		}
		if (this.tm == null)
		{
			this.textGo = new GameObject("3d text");
			this.textGo.transform.parent = base.gameObject.transform;
			this.textGo.transform.localPosition = Vector3.zero;
			MeshRenderer meshRenderer = this.textGo.AddComponent<MeshRenderer>();
			meshRenderer.material = this.font.material;
			this.tm = this.textGo.AddComponent<TextMesh>();
			this.tm.font = this.font;
			this.tm.anchor = TextAnchor.MiddleCenter;
			if (this.CharacterSize > 0f)
			{
				this.tm.characterSize = this.CharacterSize;
			}
		}
	}

	private void Update()
	{
		bool flag = !this.DisableOnOwnObjects || base.photonView.isMine;
		if (this.textGo != null)
		{
			this.textGo.SetActive(flag);
		}
		if (!flag)
		{
			return;
		}
		PhotonPlayer owner = base.photonView.owner;
		if (owner != null)
		{
			this.tm.text = ((!string.IsNullOrEmpty(owner.name)) ? owner.name : ("player" + owner.ID));
		}
		else if (base.photonView.isSceneView)
		{
			this.tm.text = "scn";
		}
		else
		{
			this.tm.text = "n/a";
		}
	}

	private GameObject textGo;

	private TextMesh tm;

	public float CharacterSize;

	public Font font;

	public bool DisableOnOwnObjects;
}
