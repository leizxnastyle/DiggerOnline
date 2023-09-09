using System;
using System.Collections.Generic;
using com.playGenesis.VkUnityPlugin;
using com.playGenesis.VkUnityPlugin.MiniJSON;
using UnityEngine;
using UnityEngine.UI;

public class CaptchaDialog : QueueWorker<VKRequest>
{
	private void Start()
	{
		this.dnl = UnityEngine.Object.FindObjectOfType<Downloader>();
		this.captchaImage = base.transform.GetChild(0).GetComponent<Image>();
		this.captchaText = base.GetComponentInChildren<InputField>();
		this.captchaText.DeactivateInputField();
	}

	private void ParseCaptchaIdAndUrl(string response)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(response) as Dictionary<string, object>;
		Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["error"];
		string id = (string)dictionary2["captcha_sid"];
		string url = (string)dictionary2["captcha_img"];
		this.captchaData.url = url;
		this.captchaData.id = id;
	}

	protected override void StartProcessing()
	{
		this.ParseCaptchaIdAndUrl(this._current.Element.response);
		if (this.dnl == null)
		{
			this.dnl = UnityEngine.Object.FindObjectOfType<Downloader>();
		}
		DownloadRequest d = new DownloadRequest
		{
			url = this.captchaData.url,
			onFinished = new Action<DownloadRequest>(this.OnGotCaptchaImage)
		};
		this.dnl.download(d);
	}

	private void OnGotCaptchaImage(DownloadRequest d)
	{
		if (d.DownloadResult.error != null)
		{
			return;
		}
		Rect rect = new Rect(0f, 0f, 130f, 50f);
		Texture2D texture = d.DownloadResult.texture;
		this.captchaImage.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
		this.captchaText.ActivateInputField();
		this.captchaText.text = string.Empty;
		base.gameObject.SetActive(true);
	}

	public void OnCaptchaEntered(string s)
	{
		this.captchaData.key = this.captchaText.text;
		if (this._current.Element.url.Contains("captcha_sid"))
		{
			this._current.Element.url = this._current.Element.url.Replace(this.lastaddedCaptchaParams, "&captcha_sid=" + this.captchaData.id + "&captcha_key=" + this.captchaData.key);
			this.lastaddedCaptchaParams = "&captcha_sid=" + this.captchaData.id + "&captcha_key=" + this.captchaData.key;
		}
		else
		{
			this.lastaddedCaptchaParams = "&captcha_sid=" + this.captchaData.id + "&captcha_key=" + this.captchaData.key;
			VKRequest element = this._current.Element;
			string url = element.url;
			element.url = string.Concat(new string[]
			{
				url,
				"&captcha_sid=",
				this.captchaData.id,
				"&captcha_key=",
				this.captchaData.key
			});
		}
		this._current.Element.fullurl = null;
		VkApi.VkApiInstance.Call(this._current.Element);
		UnityEngine.Debug.Log(this._current.Element.url);
		base.gameObject.SetActive(false);
		base.ProccessNext();
	}

	public Image captchaImage;

	public InputField captchaText;

	private Downloader dnl;

	private VKCaptcha captchaData;

	private string lastaddedCaptchaParams;
}
