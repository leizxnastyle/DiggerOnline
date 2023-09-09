using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentUpdater : MonoBehaviour
{
	public void Start()
	{
		base.StartCoroutine(this.LoadContentProcess());
	}

	private IEnumerator LoadContentProcess()
	{
		ContentUpdater.NumberContent = 0;
		ContentUpdater.NumberContent = 1;
		this.RequestContent = WWW.LoadFromCacheOrDownload("https://vk.diggerworld.ru/KO/Builds/Packages/" + this.EntitiesTexturesName + ".unity3d", 1);
		yield return this.RequestContent;
		if (this.RequestContent.error != null)
		{
			UnityEngine.Debug.Log("Load asset bundle fail: " + this.RequestContent.error);
		}
		if (this.RequestContent.assetBundle == null)
		{
			UnityEngine.Debug.Log("Loaded file not contains asset bundle!");
		}
		ContentUpdater.EntitiesBundle = this.RequestContent.assetBundle;
		foreach (EntityBase entity in UnityEngine.Object.FindObjectsOfType(typeof(EntityBase)))
		{
			entity.UpdateTextures();
		}
		ContentUpdater.NumberContent = 2;
		this.RequestContent = WWW.LoadFromCacheOrDownload("https://vk.diggerworld.ru/KO/Builds/Packages/" + this.CharacterTexturesName + ".unity3d", 1);
		yield return this.RequestContent;
		if (this.RequestContent.error != null)
		{
			UnityEngine.Debug.Log("Load asset bundle fail: " + this.RequestContent.error);
		}
		if (this.RequestContent.assetBundle == null)
		{
			UnityEngine.Debug.Log("Loaded file not contains asset bundle!");
		}
		ContentUpdater.CharacterBundle = this.RequestContent.assetBundle;
		foreach (SkinManager skinManager in UnityEngine.Object.FindObjectsOfType(typeof(SkinManager)))
		{
			skinManager.UpdateTextures();
		}
		ContentUpdater.NumberContent = 3;
		this.RequestContent = WWW.LoadFromCacheOrDownload("https://vk.diggerworld.ru/KO/Builds/Packages/" + this.WeaponTexturesName + ".unity3d", 1);
		yield return this.RequestContent;
		if (this.RequestContent.error != null)
		{
			UnityEngine.Debug.Log("Load asset bundle fail: " + this.RequestContent.error);
		}
		if (this.RequestContent.assetBundle == null)
		{
			UnityEngine.Debug.Log("Loaded file not contains asset bundle!");
		}
		UnityEngine.Object[] assets = null;
		assets = this.RequestContent.assetBundle.LoadAllAssets();
		ContentUpdater.WeaponTextures.Clear();
		for (int i = 0; i < assets.Length; i++)
		{
			ContentUpdater.WeaponTextures.Add((Texture2D)assets[i]);
		}
		foreach (SkinManager skinManager2 in UnityEngine.Object.FindObjectsOfType(typeof(SkinManager)))
		{
			skinManager2.UpdateTextures();
		}
		ContentUpdater.NumberContent = 4;
		MainMenu.Instance.HideLoadContentText();
		yield break;
	}

	private void Update()
	{
		if (ContentUpdater.NumberContent == -1)
		{
			ContentUpdater.UpdaterProgress = 0;
		}
		if (ContentUpdater.NumberContent == 0)
		{
			ContentUpdater.UpdaterProgress = (int)(this.RequestContent.progress * 100f);
		}
		if (ContentUpdater.NumberContent == 1)
		{
			ContentUpdater.UpdaterProgress = (int)(this.RequestContent.progress * 100f);
		}
		if (ContentUpdater.NumberContent == 2)
		{
			ContentUpdater.UpdaterProgress = (int)(this.RequestContent.progress * 100f);
		}
		if (ContentUpdater.NumberContent == 3)
		{
			ContentUpdater.UpdaterProgress = (int)(this.RequestContent.progress * 100f);
		}
	}

	public static IEnumerator UpdateTextures(Transform node, AssetBundle bundle)
	{
		if (node == null)
		{
			yield break;
		}
		if (bundle == null)
		{
			yield break;
		}
		if (node.GetComponent<Renderer>() != null)
		{
			foreach (Material material in node.GetComponent<Renderer>().materials)
			{
				if (material.mainTexture != null)
				{
					AssetBundleRequest request = bundle.LoadAssetAsync(material.mainTexture.name, typeof(Texture2D));
					yield return request;
					if (request.asset != null)
					{
						material.mainTexture = (request.asset as Texture2D);
					}
				}
			}
		}
		if (node == null)
		{
			yield break;
		}
		foreach (object obj in node)
		{
			Transform child = (Transform)obj;
			if (child != null)
			{
				App.Instance.StartCoroutine(ContentUpdater.UpdateTextures(child, bundle));
			}
		}
		yield break;
	}

	private string WeaponTexturesName = "WeaponTextures";

	private string EntitiesTexturesName = "EntitiesTextures";

	private string CharacterTexturesName = "CharacterTextures";

	public static List<Texture2D> WorldTextures = new List<Texture2D>();

	public static List<Texture2D> WeaponTextures = new List<Texture2D>();

	public static AssetBundle EntitiesBundle = null;

	public static AssetBundle CharacterBundle = null;

	public static int UpdaterProgress = 0;

	private WWW RequestContent;

	public static int NumberContent = -1;
}
