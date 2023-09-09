using System;
using System.Collections.Generic;
using com.playGenesis.VkUnityPlugin.MiniJSON;
using UnityEngine;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKShare
	{
		public VKShare(Action<VKRequest> CallbackFunction, string text = "", List<ShareImage> images = null, string link = "", long group_id = 0L)
		{
			this.vkapi = VkApi.VkApiInstance;
			this._text = text;
			this._images = images;
			this._link = link;
			this._group_id = group_id;
			if (CallbackFunction == null)
			{
				CallbackFunction = delegate(VKRequest r)
				{
					UnityEngine.Debug.LogError("The Callback Function is not optional in VkShare");
				};
			}
			this._callbackFunction = CallbackFunction;
		}

		public void Share()
		{
			if (this._images == null)
			{
				this.PostToWall(VKShare.RepeatRequest);
				return;
			}
			this._imagesToUpload = this._images.Count;
			string str = (this._group_id != 0L) ? ("group_id=" + this._group_id.ToString()) : string.Empty;
			VKRequest httprequest = new VKRequest
			{
				url = "photos.getWallUploadServer?" + str,
				data = new object[]
				{
					VKShare.RepeatRequest
				},
				CallBackFunction = new Action<VKRequest>(this.GotWallUploadServer)
			};
			this.vkapi.Call(httprequest);
		}

		private void GotWallUploadServer(VKRequest arg1)
		{
			if (arg1.error != null)
			{
				this._callbackFunction(arg1);
				return;
			}
			Dictionary<string, object> dictionary = Json.Deserialize(arg1.response) as Dictionary<string, object>;
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["response"];
			string fullurl = (string)dictionary2["upload_url"];
			foreach (ShareImage shareImage in this._images)
			{
				FileForUpload fileForUpload = new FileForUpload
				{
					data = shareImage.data,
					filename = shareImage.imageName,
					mimeType = (string)shareImage.imagetype
				};
				VKRequest requestString = new VKRequest
				{
					fullurl = fullurl,
					CallBackFunction = new Action<VKRequest>(this.PhotoHasBeenUploaded),
					data = new object[]
					{
						VKShare.RepeatRequest,
						fileForUpload
					}
				};
				this.vkapi.UploadToserverCall(requestString, fileForUpload);
			}
		}

		private void PhotoHasBeenUploaded(VKRequest arg1)
		{
			if (arg1.error != null)
			{
				this._callbackFunction(arg1);
				return;
			}
			Dictionary<string, object> dictionary = Json.Deserialize(arg1.response) as Dictionary<string, object>;
			long num = (long)dictionary["server"];
			string text = (string)dictionary["photo"];
			string text2 = (string)dictionary["hash"];
			string text3 = (this._group_id != 0L) ? ("&group_id=" + this._group_id.ToString()) : string.Empty;
			VKRequest httprequest = new VKRequest
			{
				url = string.Concat(new object[]
				{
					"photos.saveWallPhoto?photo=",
					text,
					"&server=",
					num,
					"&hash=",
					text2,
					text3
				}),
				CallBackFunction = new Action<VKRequest>(this.OnPhotoSaved),
				data = new object[]
				{
					VKShare.RepeatRequest
				}
			};
			this.vkapi.Call(httprequest);
		}

		private void OnPhotoSaved(VKRequest arg1)
		{
			if (arg1.error != null)
			{
				this._callbackFunction(arg1);
				return;
			}
			Dictionary<string, object> dictionary = Json.Deserialize(arg1.response) as Dictionary<string, object>;
			List<object> list = (List<object>)dictionary["response"];
			VKPhoto vkphoto = VKPhoto.Deserialize(list[0]);
			this._photoIds.Add(vkphoto.id);
			this._imagesToUpload--;
			if (this._imagesToUpload == 0)
			{
				this.PostToWall(VKShare.RepeatRequest);
			}
		}

		private string GenerateAttachementsForWall()
		{
			string text = string.Empty;
			string text2 = (this._group_id != 0L) ? ("-" + this._group_id) : VkApi.CurrentToken.user_id;
			foreach (long num in this._photoIds)
			{
				text = string.Concat(new object[]
				{
					text,
					"photo",
					text2,
					"_",
					num,
					","
				});
			}
			return text.Substring(0, text.Length - 1);
		}

		private void PostToWall(int attemptsLeft)
		{
			string text = "wall.post?";
			if (!string.IsNullOrEmpty(this._text))
			{
				text = text + "message=" + this._text;
			}
			if (this._images != null)
			{
				text = text + "&attachments=" + this.GenerateAttachementsForWall();
			}
			if (this._link != null)
			{
				text = text + "," + this._link;
			}
			VKRequest httprequest = new VKRequest
			{
				url = text,
				CallBackFunction = new Action<VKRequest>(this.WhenPosted),
				data = new object[]
				{
					attemptsLeft
				}
			};
			this.vkapi.Call(httprequest);
		}

		private void WhenPosted(VKRequest arg1)
		{
			this._callbackFunction(arg1);
			UnityEngine.Debug.Log("Finished Sharing");
		}

		private VkApi vkapi;

		private string _text;

		private List<ShareImage> _images;

		private string _link;

		private int _imagesToUpload;

		private List<long> _photoIds = new List<long>();

		private long _group_id;

		public static int RepeatRequest = 5;

		private Action<VKRequest> _callbackFunction;
	}
}
