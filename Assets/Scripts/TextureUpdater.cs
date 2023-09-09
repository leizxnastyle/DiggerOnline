using System;
using UnityEngine;

public class TextureUpdater : MonoBehaviour
{
	private void Start()
	{
		if (this.update_int == 0)
		{
			base.StartCoroutine(ContentUpdater.UpdateTextures(base.transform, ContentUpdater.CharacterBundle));
		}
		else if (this.update_int == 1)
		{
			base.StartCoroutine(ContentUpdater.UpdateTextures(base.transform, ContentUpdater.EntitiesBundle));
		}
	}

	public int update_int;
}
