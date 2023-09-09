using System;
using System.Collections.Generic;
using com.playGenesis.VkUnityPlugin;
using UnityEngine;
using UnityEngine.UI;

public class FriendManager : MonoBehaviour
{
	public VKUser friend
	{
		get
		{
			return this._friend;
		}
		set
		{
			if (value == null)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				this._friend = value;
				base.gameObject.SetActive(true);
				this.t.text = this._friend.first_name + " " + this._friend.last_name;
				this.GetImageFromCacheOrDownload(value.id);
			}
		}
	}

	private void GetImageFromCacheOrDownload(long id)
	{
		VKFriedImage vkfriedImage = FriendManager.fImages.Find((VKFriedImage i) => i.VKUserId == id);
		if (vkfriedImage != null && vkfriedImage.Img != null)
		{
			this.setUpImage(vkfriedImage.Img);
		}
		else if (!string.IsNullOrEmpty(this.friend.photo_200))
		{
			this.DownloadFriendImage(this.friend.photo_200, this.friend.id);
		}
		else
		{
			this.i.sprite = this.noPhoto;
			FriendManager.fImages.Add(new VKFriedImage
			{
				VKUserId = id,
				Img = null
			});
		}
	}

	private void DownloadFriendImage(string url, long id)
	{
		Action<DownloadRequest> onFinished = delegate(DownloadRequest d)
		{
			long num = (long)d.CustomData[0];
			if (d.DownloadResult.error == null && this.friend.id == num)
			{
				this.setUpImage(d.DownloadResult.texture);
				UnityEngine.Object.Destroy(d.DownloadResult.texture);
				FriendManager.fImages.Add(new VKFriedImage
				{
					VKUserId = num,
					Img = this.i.sprite.texture
				});
			}
		};
		DownloadRequest d2 = new DownloadRequest
		{
			url = url,
			onFinished = onFinished,
			CustomData = new object[]
			{
				id
			}
		};
		VkApi.Downloader.download(d2);
	}

	public void setUpImage(byte[] photo)
	{
		Texture2D texture2D = new Texture2D(200, 200);
		texture2D.LoadImage(photo);
		this.i.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, 200f, 200f), new Vector2(0.5f, 0.5f));
		UnityEngine.Object.Destroy(texture2D);
	}

	public void setUpImage(Texture2D photo)
	{
		if (this.i.sprite != this.noPhoto)
		{
			UnityEngine.Object.DestroyObject(this.i.sprite);
		}
		this.i.sprite = Sprite.Create(photo, new Rect(0f, 0f, 200f, 200f), new Vector2(0.5f, 0.5f));
	}

	public virtual void Invite()
	{
		if (this.friend != null)
		{
			VKRequest httprequest = new VKRequest
			{
				url = "apps.sendRequest?user_id=" + this.friend.id + "&text=hello_from_vk_plugin2&type=invite&name=sayhello",
				CallBackFunction = new Action<VKRequest>(this.OnAppSendRequest)
			};
			VkApi.VkApiInstance.Call(httprequest);
		}
	}

	public virtual void OnAppSendRequest(VKRequest r)
	{
		if (r.error != null)
		{
			GlobalErrorHandler.Instance.Notification.Notify(r);
			return;
		}
		UnityEngine.Debug.Log(r.response);
	}

	public Text t;

	public Image i;

	public Sprite noPhoto;

	public static List<VKFriedImage> fImages = new List<VKFriedImage>();

	private VKUser _friend;
}
