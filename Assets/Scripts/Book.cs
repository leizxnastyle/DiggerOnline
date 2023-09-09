using System;
using System.Collections.Generic;

public class Book : EntityBase
{
	private bool _EditorMode
	{
		get
		{
			return this.Creator == ProfileINI.nickname || Level.Instance.IsAdmin(ProfileINI.nickname);
		}
	}

	public override void OnButtonF(string playerName)
	{
		base.OnButtonF(playerName);
		Book.Current = this;
		MainMenu.Instance.SwitchMenu(Menu.Book, null, null);
	}

	protected override void Creation(object[] data)
	{
		if (data != null)
		{
			this._Texts.Clear();
			for (int i = 0; i < data.Length; i++)
			{
				this._Texts.Add((string)data[i]);
			}
		}
		if (!PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("GetTexts", PhotonTargets.MasterClient, new object[0]);
		}
	}

	public override object[] GetData()
	{
		List<object> list = new List<object>();
		foreach (string item in this._Texts)
		{
			list.Add(item);
		}
		return list.ToArray();
	}

	public void RefreshControls()
	{
		for (int i = 0; i < 2; i++)
		{
			int num = this._CurPage + i;
			string text = "-" + (num + 1).ToString("d2") + "-";
			if (i == 0)
			{
				KGUI.SetControlText("book.txt_page_num_left", text);
			}
			else
			{
				KGUI.SetControlText("book.txt_page_num_right", text);
			}
			string text2 = (num >= this._Texts.Count) ? string.Empty : this._Texts[num];
			if (i == 0)
			{
				KGUI.SetControlText("book.inp_page_left", text2);
			}
			else
			{
				KGUI.SetControlText("book.inp_page_right", text2);
			}
		}
		if (GameType.IsArcadeMode || GameType.IsHungerGamesMode)
		{
			KGUI.FindNode("book.inp_page_left", false).GetComponent<UIInput>().enabled = false;
			KGUI.FindNode("book.inp_page_right", false).GetComponent<UIInput>().enabled = false;
		}
		KGUI.SetNodes("book.ibtn_next", (!this._EditorMode && this._CurPage < this._Texts.Count - 2) || (this._EditorMode && this._CurPage <= this._Texts.Count), false);
		KGUI.SetNodes("book.ibtn_prev", this._CurPage >= 2, false);
	}

	public void NextButton()
	{
		this.SaveCurTexts();
		this._CurPage += 2;
		this.RefreshControls();
	}

	public void PrevButton()
	{
		this.SaveCurTexts();
		this._CurPage -= 2;
		this.RefreshControls();
	}

	public void CloseButton()
	{
		this.SaveCurTexts();
		Book.Current = null;
		MainMenu.Instance.HideMenu();
	}

	public void SaveCurTexts()
	{
		if (!this._EditorMode)
		{
			return;
		}
		base.photonView.RPC("UpdateText", PhotonTargets.All, new object[]
		{
			this._CurPage,
			KGUI.GetControlText("book.inp_page_left")
		});
		base.photonView.RPC("UpdateText", PhotonTargets.All, new object[]
		{
			this._CurPage + 1,
			KGUI.GetControlText("book.inp_page_right")
		});
	}

	[PunRPC]
	private void UpdateText(int index, string text)
	{
		while (index >= this._Texts.Count)
		{
			this._Texts.Add(string.Empty);
		}
		this._Texts[index] = text;
		while (index >= 0 && index == this._Texts.Count - 1 && this._Texts[index] == string.Empty)
		{
			this._Texts.RemoveAt(this._Texts.Count - 1);
			index--;
		}
	}

	[PunRPC]
	private void GetTexts(PhotonMessageInfo info)
	{
		for (int i = this._Texts.Count - 1; i >= 0; i--)
		{
			base.photonView.RPC("UpdateText", info.sender, new object[]
			{
				i,
				this._Texts[i]
			});
		}
	}

	public static Book Current;

	private List<string> _Texts = new List<string>();

	private int _CurPage;
}
