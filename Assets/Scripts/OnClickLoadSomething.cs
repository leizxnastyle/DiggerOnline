using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickLoadSomething : MonoBehaviour
{
	public void OnClick()
	{
		OnClickLoadSomething.ResourceTypeOption resourceTypeToLoad = this.ResourceTypeToLoad;
		if (resourceTypeToLoad != OnClickLoadSomething.ResourceTypeOption.Scene)
		{
			if (resourceTypeToLoad == OnClickLoadSomething.ResourceTypeOption.Web)
			{
				SceneManager.LoadScene(this.ResourceToLoad);
			}
		}
		else
		{
			SceneManager.LoadScene(this.ResourceToLoad);
		}
	}

	public OnClickLoadSomething.ResourceTypeOption ResourceTypeToLoad;

	public string ResourceToLoad;

	public enum ResourceTypeOption : byte
	{
		Scene,
		Web
	}
}
