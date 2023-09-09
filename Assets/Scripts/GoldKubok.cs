using System;
using UnityEngine;

public class GoldKubok : EntityBase
{
	protected override void Creation(object[] data)
	{
		WorldGameObjectX.Instance.KubokCountInGame++;
		WorldGameObjectX.Instance.UpdateKubokText();
	}

	public override void OnLeftMouseHit(string player_name)
	{
		if (!App.Instance.Settings.isWatch)
		{
			if (Level.Instance.CanBuild)
			{
				if (this.Creator == player_name || Level.Instance.IsAdmin(null))
				{
					this.Life -= 1;
					if (this.Life > 0)
					{
						iTween.ShakePosition(base.gameObject, new Vector3(0.05f, 0.05f, 0.01f), 0.5f);
						SoundManager.Instance.Play(SoundManager.Sound.HitMetal, WorldGameObjectX.Instance.MainPlayer.gameObject.GetComponent<AudioSource>());
					}
					else
					{
						this.SelfDelete();
						SoundManager.Instance.Play(SoundManager.Sound.HitMetal, WorldGameObjectX.Instance.MainPlayer.gameObject.GetComponent<AudioSource>());
					}
				}
				else
				{
					Chat.SendInfoF(ProfileINI.nickname + Localize.GetText("CANT_TAKE_AND_DESTROY_ITEMS", null), false);
				}
			}
			else
			{
				Chat.SendWarning(Localize.GetText("CANT_EDIT_MAP", null), false);
			}
		}
		else
		{
			Chat.SendWarning(Localize.GetText("CANT_EDIT_MAP_IN_WATCH", null), false);
		}
	}

	protected override void Destruction()
	{
		WorldGameObjectX.Instance.KubokCountInGame--;
		WorldGameObjectX.Instance.UpdateKubokText();
		if (!Level.Instance.IsAdmin(null) && !WorldGameObjectX.Instance.FindAllKubok && WorldGameObjectX.Instance.KubokFindInGame >= WorldGameObjectX.Instance.KubokCountInGame)
		{
			WorldGameObjectX.Instance.FindAllKubok = true;
		}
	}

	public override void OnButtonE(string player_name)
	{
		if (Level.Instance.IsAdmin(null) && !App.Instance.Settings.isWatch)
		{
			WorldGameObjectX.Instance.TakeEnity(base.gameObject);
		}
		else
		{
			SoundManager.Instance.Play(SoundManager.Sound.Treasure, WorldGameObjectX.Instance.MainPlayer.gameObject.GetComponent<AudioSource>());
			iTween.FadeTo(base.gameObject, 0f, 0.5f);
			this.partical.GetComponent<ParticleEmitter>().emit = false;
			UnityEngine.Object.Destroy(base.gameObject.GetComponent<BoxCollider>());
			WorldGameObjectX.Instance.KubokFindInGame++;
			WorldGameObjectX.Instance.UpdateKubokText();
			if (!WorldGameObjectX.Instance.FindAllKubok)
			{
				if (WorldGameObjectX.Instance.KubokFindInGame < WorldGameObjectX.Instance.KubokCountInGame)
				{
					Chat.SendInfoF(string.Concat(new string[]
					{
						Localize.GetText("CUP_PLAYER", null),
						ProfileINI.nickname,
						Localize.GetText("CUP_PLAYER_FIND", null),
						" [",
						WorldGameObjectX.Instance.KubokFindInGame.ToString(),
						"]"
					}), true);
				}
				else
				{
					Chat.SendInfoF(Localize.GetText("CUP_PLAYER", null) + ProfileINI.nickname + Localize.GetText("CUP_PLAYER_FIND_ALL", null), true);
					WorldGameObjectX.Instance.FindAllKubok = true;
				}
			}
		}
	}

	public GameObject Cubok;

	public GameObject partical;
}
