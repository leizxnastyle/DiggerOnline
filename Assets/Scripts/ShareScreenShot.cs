using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using com.playGenesis.VkUnityPlugin;
using UnityEngine;

public class ShareScreenShot : MonoBehaviour
{
	private void Start()
	{
		this.vkapi = VkApi.VkApiInstance;
		if (!this.vkapi.IsUserLoggedIn)
		{
			this.vkapi.Login();
		}
	}

	public void TakeScreenShot()
	{
		string text = "screnshot.jpg";
		this._filePath = Path.Combine(Application.persistentDataPath, text);
		UnityEngine.ScreenCapture.CaptureScreenshot(text);
		base.StartCoroutine(this.LoadScreenShot());
	}

	private IEnumerator LoadScreenShot()
	{
		yield return new WaitForSeconds(3f);
		while (!this.vkapi.IsUserLoggedIn)
		{
			yield return null;
		}
		WWW www = new WWW("file:///" + this._filePath);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			Texture2D tex = www.texture;
			this.jpegScreenShotBytes = tex.EncodeToJPG();
			List<ShareImage> imgs = new List<ShareImage>();
			ShareImage screenshot = new ShareImage
			{
				data = this.jpegScreenShotBytes,
				imageName = "screenshot.jpg",
				imagetype = ImageType.Jpeg
			};
			imgs.Add(screenshot);
			VKShare vkShare = new VKShare(new Action<VKRequest>(this.OnShareFinished), "Hello From VK Api", imgs, "http://u3d.as/8HK", 0L);
			vkShare.Share();
		}
		yield break;
	}

	private void OnShareFinished(VKRequest resp)
	{
		if (resp.error != null)
		{
			return;
		}
		UnityEngine.Debug.Log("Succesfully finished sharing");
	}

	public void Back()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("StarterScene");
	}

	private VkApi vkapi;

	private string _filePath;

	private byte[] jpegScreenShotBytes;
}
