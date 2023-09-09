using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Language Selection")]
[RequireComponent(typeof(UIPopupList))]
public class LanguageSelection : MonoBehaviour
{
	private void Start()
	{
		this.mList = base.GetComponent<UIPopupList>();
		this.UpdateList();
		this.mList.eventReceiver = base.gameObject;
		this.mList.functionName = "OnLanguageSelection";
	}

	private void UpdateList()
	{
		if (Localization.instance != null && Localization.instance.languages != null && Localization.instance.languages.Length > 0)
		{
			this.mList.items.Clear();
			int i = 0;
			int num = Localization.instance.languages.Length;
			while (i < num)
			{
				TextAsset textAsset = Localization.instance.languages[i];
				if (textAsset != null)
				{
					this.mList.items.Add(textAsset.name);
				}
				i++;
			}
			this.mList.selection = Localization.instance.currentLanguage;
		}
	}

	private void OnLanguageSelection(string language)
	{
		if (Localization.instance != null)
		{
			Localization.instance.currentLanguage = language;
		}
	}

	private UIPopupList mList;
}
