using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KGUI : MonoBehaviour
{
	public static event Action OnLocaleChenged;

	private void Awake()
	{
		if (KGUI._Instance != null)
		{
			return;
		}
		KGUI._Instance = this;
		Utils.SetActiveRecursively(KGUI.FindNode("Anchor", false), true);
		List<Transform> list = new List<Transform>
		{
			KGUI.FindNode("MapsList.clip_servers.servers_grid.slot_prototype.txt_title", false),
			KGUI.FindNode("MapsList.clip_servers.servers_grid.slot_prototype.txt_players", false),
			KGUI.FindNode("hud.battle.anchor_bottom_right.Bullet.Text", false),
			KGUI.FindNode("bonus.txt_count", false),
			KGUI.FindNode("Shop.txt_item_name", false),
			KGUI.FindNode("hud.chat.label", false),
			KGUI.FindNode("hud.chat.txt_text", false),
			KGUI.FindNode("hud.kills_list.txt_killer", false),
			KGUI.FindNode("hud.kills_list.txt_victim", false),
			KGUI.FindNode("hud.speech_bubble.txt_text", false),
			KGUI.FindNode("hud.txt_cheating", false),
			KGUI.FindNode("main_menu_top_maps.txt_author", false),
			KGUI.FindNode("tab_menu.txt_author", false),
			KGUI.FindNode("team_battle.page1.txt_team", false),
			KGUI.FindNode("team_battle.page2.txt_team", false),
			KGUI.FindNode("plate.inp_line1.label", false),
			KGUI.FindNode("plate.inp_line2.label", false),
			KGUI.FindNode("plate.inp_line3.label", false),
			KGUI.FindNode("plate.inp_line4.label", false),
			KGUI.FindNode("book.inp_page_left.label", false),
			KGUI.FindNode("book.inp_page_right.label", false),
			KGUI.FindNode("MyMaps.page1.button_buy_slot.txt_count", false),
			KGUI.FindNode("main_menu.bottom_menu.txt_nickname", false),
			KGUI.FindNode("input_text2.txt_title", false),
			KGUI.FindNode("input_text2.error", false),
			KGUI.FindNode("input_text2.input.inp_input.label", false),
			KGUI.FindNode("input_text2.button_ok.txt_title", false),
			KGUI.FindNode("common.tooltip.text", false),
			KGUI.FindNode("main_menu.bottom_menu.version", false),
			KGUI.FindNode("change_name.txt_name_busy", false),
			KGUI.FindNode("bonus.txt_count2", false),
			KGUI.FindNode("Shop.page1_shop.items.clip_items.grid_items_categories.slot_prototype.bookmark.txt_title", false)
		};
		foreach (UILabel uilabel in UnityEngine.Object.FindObjectsOfType(typeof(UILabel)))
		{
			bool flag = uilabel.text.StartsWith("#");
			if (flag)
			{
				uilabel.text = uilabel.text.Substring(1);
			}
			KGUI._LabelsInitText.Add(uilabel, uilabel.text);
			if (!list.Contains(uilabel.transform) && !flag)
			{
				uilabel.color = new Color(0.3529412f, 0.3529412f, 0.274509817f);
				uilabel.effectStyle = UILabel.Effect.Outline;
				uilabel.effectDistance = new Vector2(2f, 2f);
				uilabel.effectColor = new Color(0.807843149f, 0.807843149f, 0.5647059f);
			}
		}
		foreach (UIInput uiinput in UnityEngine.Object.FindObjectsOfType(typeof(UIInput)))
		{
			if (uiinput.name[0] != '@')
			{
				uiinput.label.text = string.Empty;
				uiinput.text = string.Empty;
			}
		}
		KGUI.SetLocale((Localize.LocaleType)PlayerPrefs.GetInt("locale", 0));
		NGUITools.MakePixelPerfect(KGUI._Instance.transform);
		if (!Application.isEditor)
		{
			foreach (object obj in KGUI.FindNode("Anchor", false))
			{
				Transform transform = (Transform)obj;
				Vector3 localPosition = transform.transform.localPosition;
				localPosition.x -= 0.5f;
				localPosition.y += 0.5f;
				transform.localPosition = localPosition;
			}
		}
		KGUI.SetNodes("Anchor", false, true);
		KGUI.SetNodes("common", true, false);
		ProfileINI.LoadFromPrefs();
		base.gameObject.AddComponent<AudioListener>();
	}

	private IEnumerator OnLevelWasLoaded()
	{
		if (this != KGUI._Instance)
		{
			yield break;
		}
		for (;;)
		{
			string SceneName = SceneManager.GetActiveScene().name;
			if (SceneName == "Menu")
			{
				break;
			}
			AudioListener[] audioListeners = UnityEngine.Object.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];
			if (audioListeners.Length > 1)
			{
				goto Block_3;
			}
			yield return 0;
		}
		base.GetComponent<AudioListener>().enabled = true;
		yield break;
		Block_3:
		base.GetComponent<AudioListener>().enabled = false;
		yield break;
		yield break;
	}

	public static void EnableAudioListener(bool enable)
	{
		KGUI._Instance.GetComponent<AudioListener>().enabled = enable;
	}

	public static void SetLocale(Localize.LocaleType locale)
	{
		Localize.Locale = locale;
		PlayerPrefs.SetInt("locale", (int)locale);
		foreach (KeyValuePair<UILabel, string> keyValuePair in KGUI._LabelsInitText)
		{
			keyValuePair.Key.text = Localize.GetText(keyValuePair.Value, keyValuePair.Key.text);
		}
		UnityEngine.Debug.Log("SetLocale: " + locale);
		if (KGUI.OnLocaleChenged != null)
		{
			KGUI.OnLocaleChenged();
		}
		ProfileINI.Language = string.Empty + locale;
		Bar.Instance.SetLocalization();
	}

	public static void EnableInput(bool enable)
	{
		KGUI.SetModal(new List<string>
		{
			string.Empty
		}, !enable);
	}

	public static void SetModal(List<string> controlsName, bool enable)
	{
		if (enable)
		{
			if (KGUI._ModalControls.Contains(controlsName))
			{
				KGUI._ModalControls.Remove(controlsName);
			}
			KGUI._ModalControls.Add(controlsName);
		}
		else
		{
			KGUI._ModalControls.Remove(controlsName);
		}
		KGUI.EnableCollider(KGUI._Instance.transform, null, true);
		if (KGUI._ModalControls.Count > 0)
		{
			KGUI.EnableCollider(KGUI._Instance.transform, KGUI._ModalControls[KGUI._ModalControls.Count - 1], false);
		}
	}

	private static void EnableCollider(Transform node, List<string> skipNodes, bool enable)
	{
		if (skipNodes != null && skipNodes.Contains(node.name))
		{
			return;
		}
		if (node.GetComponent<Collider>() != null)
		{
			node.GetComponent<Collider>().enabled = enable;
		}
		foreach (object obj in node)
		{
			Transform node2 = (Transform)obj;
			KGUI.EnableCollider(node2, skipNodes, enable);
		}
	}

	public static void SetControlText(string nodeName, string text)
	{
		Transform transform = KGUI.FindNode(nodeName, false);
		UIInput component = transform.GetComponent<UIInput>();
		if (component != null)
		{
			component.text = text;
		}
		else
		{
			transform.GetComponent<UILabel>().text = text;
		}
	}

	public static void SetControlSprite(string nodeName, string sprite_name, int size = 0)
	{
		Transform transform = KGUI.FindNode(nodeName, false);
		UISprite component = transform.GetComponent<UISprite>();
		if (component != null)
		{
			component.spriteName = sprite_name;
		}
		else
		{
			transform.GetComponent<UISprite>().spriteName = sprite_name;
		}
		if (size != 0)
		{
			component.transform.localScale = new Vector3((float)size, (float)size, 1f);
		}
	}

	public static string GetControlText(string nodeName)
	{
		Transform transform = KGUI.FindNode(nodeName, false);
		UIInput component = transform.GetComponent<UIInput>();
		if (component != null)
		{
			return component.text;
		}
		return transform.GetComponentInChildren<UILabel>().text;
	}

	public static void SetControlCheckbox(string nodeName, bool isChecked)
	{
		UICheckbox component = KGUI.FindNode(nodeName, false).GetComponent<UICheckbox>();
		string functionName = component.functionName;
		component.functionName = null;
		component.isChecked = isChecked;
		component.functionName = functionName;
	}

	public static bool GetControlCheckbox(string nodeName)
	{
		return KGUI.FindNode(nodeName, false).GetComponent<UICheckbox>().isChecked;
	}

	public static void HideWeaponIcon()
	{
		Transform transform = KGUI.FindNode("hud.battle.grid_weapons", false);
		if (transform != null && transform.Find("slot_prototype_hg") != null && transform.Find("0") != null)
		{
			for (int i = 0; i < 10; i++)
			{
				Transform transform2 = transform.Find(i.ToString());
				if (transform2 != null)
				{
					UnityEngine.Object.Destroy(transform2.gameObject);
				}
			}
		}
	}

	public static void ResizeGrid(string nodeName, int size, Action<GameObject, int> fillContent, string scrollName = null)
	{
		KGUI._ScrollGrid = null;
		Transform transform = KGUI.FindNode(nodeName, false);
		if (!transform.gameObject.activeInHierarchy)
		{
			return;
		}
		Utils.SetActiveRecursively(transform, true);
		bool flag = false;
		Transform transform2;
		if (!GameType.IsHungerGamesMode)
		{
			transform2 = transform.Find("slot_prototype");
			if (transform.Find("slot_prototype_hg") != null)
			{
				flag = true;
				Utils.SetActiveRecursively(transform.Find("slot_prototype_hg"), false);
			}
		}
		else
		{
			transform2 = transform.Find("slot_prototype_hg");
			if (transform2 == null)
			{
				transform2 = transform.Find("slot_prototype");
			}
			else
			{
				flag = true;
				if (transform.Find("slot_prototype") != null)
				{
					Utils.SetActiveRecursively(transform.Find("slot_prototype"), false);
				}
			}
		}
		bool flag2 = false;
		int num;
		if (!flag)
		{
			num = transform.childCount - 1;
		}
		else
		{
			num = transform.childCount - 2;
		}
		int num2 = Mathf.Max(size, num);
		for (int i = 0; i < num2; i++)
		{
			if (i < size)
			{
				GameObject gameObject;
				if (i >= num)
				{
					gameObject = UnityEngine.Object.Instantiate<GameObject>(transform2.gameObject);
					UnityEngine.Object.Destroy(gameObject.GetComponent<UIPanel>());
					gameObject.name = i.ToString();
					gameObject.transform.parent = transform;
					gameObject.transform.localScale = transform2.transform.localScale;
					gameObject.transform.localPosition = transform2.transform.localPosition;
					gameObject.transform.localRotation = transform2.transform.localRotation;
					flag2 = true;
				}
				else
				{
					gameObject = transform.Find(i.ToString()).gameObject;
				}
				fillContent(gameObject, i);
			}
			else
			{
				Utils.SetActiveRecursively(transform.Find(i.ToString()), false);
			}
		}
		Utils.SetActiveRecursively(transform2, false);
		if (flag2)
		{
			transform.GetComponent<UITable>().repositionNow = true;
		}
		if (transform.parent.GetComponent<UIDraggablePanel>() != null)
		{
			KGUI._Instance.StartCoroutine(KGUI.ResetScrollBarProcess(transform.parent.GetComponent<UIDraggablePanel>(), scrollName));
		}
	}

	public static void ResetScrollBar(string draggablePanelName, string scrollName = null)
	{
		UIDraggablePanel component = KGUI.FindNode(draggablePanelName, false).GetComponent<UIDraggablePanel>();
		KGUI._Instance.StartCoroutine(KGUI.ResetScrollBarProcess(component, scrollName));
	}

	private static IEnumerator ResetScrollBarProcess(UIDraggablePanel panel, string scrollName = null)
	{
		yield return 0;
		yield return 0;
		float scrollValue = 1f;
		if (panel.verticalScrollBar != null && scrollName == null)
		{
			scrollValue = panel.verticalScrollBar.sliderValue;
		}
		panel.RestrictWithinBounds(true);
		panel.ResetPosition();
		if (panel.verticalScrollBar != null)
		{
			bool showSlider = panel.showScrollBars == UIDraggablePanel.ShowCondition.Always || panel.shouldMoveVertically;
			if (showSlider != panel.verticalScrollBar.transform.parent.gameObject.activeInHierarchy)
			{
				Utils.SetActiveRecursively(panel.verticalScrollBar.transform.parent, showSlider);
			}
			KGUI._ScrollGrid = (string.IsNullOrEmpty(scrollName) ? null : scrollName);
			KGUI._ScrollGridNode = panel.transform;
			if (KGUI._ScrollGrid != null && KGUI._ScrollGridNode.gameObject.activeInHierarchy)
			{
				scrollValue = PlayerPrefs.GetFloat("Scroll_" + KGUI._ScrollGrid, 1f);
			}
			panel.verticalScrollBarValue = -1f;
			panel.verticalScrollBar.sliderValue = scrollValue;
		}
		yield break;
	}

	public static void EnableNodes(string nodeName, bool enable, bool onlyChild = false)
	{
		Transform transform = KGUI.FindNode(nodeName, false);
		if (!onlyChild)
		{
			if (transform.gameObject.activeInHierarchy != enable)
			{
				Utils.SetActiveRecursively(transform, enable);
				KGUI.UpdateControls(transform);
			}
		}
		else
		{
			foreach (object obj in transform)
			{
				Transform transform2 = (Transform)obj;
				if (transform2.gameObject.activeInHierarchy != enable)
				{
					Utils.SetActiveRecursively(transform2, enable);
					KGUI.UpdateControls(transform2);
				}
			}
		}
	}

	public static void SetNodes(string nodeName, bool enable, bool onlyChild = false)
	{
		Transform transform = KGUI.FindNode(nodeName, false);
		if (!onlyChild)
		{
			if (transform.gameObject.activeInHierarchy != enable)
			{
				Utils.SetActiveRecursively(transform, enable);
				KGUI.UpdateControls(transform);
			}
		}
		else
		{
			foreach (object obj in transform)
			{
				Transform transform2 = (Transform)obj;
				if (transform2.gameObject.activeInHierarchy != enable)
				{
					Utils.SetActiveRecursively(transform2, enable);
					KGUI.UpdateControls(transform2);
				}
			}
		}
	}

	private static void UpdateControls(Transform node)
	{
		if (!node.gameObject.activeInHierarchy)
		{
			return;
		}
		foreach (UIWidget uiwidget in node.gameObject.GetComponents<UIWidget>())
		{
			uiwidget.Update();
		}
		foreach (object obj in node)
		{
			Transform node2 = (Transform)obj;
			KGUI.UpdateControls(node2);
		}
	}

	public static Transform FindNode(string nodeName, bool mayBeNull = false)
	{
		if (nodeName == "Anchor")
		{
			return KGUI._Instance.transform;
		}
		Transform transform = KGUI._Instance.transform;
		for (;;)
		{
			int num = nodeName.IndexOf('.');
			if (num == -1)
			{
				break;
			}
			transform = KGUI.FindNodeEx(transform, nodeName.Substring(0, num));
			nodeName = nodeName.Substring(num + 1);
		}
		transform = KGUI.FindNodeEx(transform, nodeName);
		if (transform == null && !mayBeNull)
		{
			UnityEngine.Debug.LogError("Node " + nodeName + " not found!");
		}
		return transform;
	}

	private static Transform FindNodeEx(Transform node, string nodeName)
	{
		if (node == null)
		{
			return null;
		}
		foreach (object obj in node)
		{
			Transform transform = (Transform)obj;
			if (transform.name == nodeName)
			{
				return transform;
			}
		}
		foreach (object obj2 in node)
		{
			Transform node2 = (Transform)obj2;
			Transform transform2 = KGUI.FindNodeEx(node2, nodeName);
			if (transform2 != null)
			{
				return transform2;
			}
		}
		return null;
	}

	public static void SetButtonCallback(string buttonName, Action func)
	{
		Transform transform = KGUI.FindNode(buttonName, false);
		KGUI._ControlsCallbacks.Add(KGUI.NodeUniqueName(transform), func);
		UIButtonMessage uibuttonMessage = transform.gameObject.AddComponent<UIButtonMessage>();
		uibuttonMessage.target = KGUI._Instance.gameObject;
		uibuttonMessage.functionName = "OnButtonClick";
		uibuttonMessage.trigger = UIButtonMessage.Trigger.OnClick;
		uibuttonMessage.includeChildren = false;
	}

	public static void SetButtonHoverOnCallback(string buttonName, Action func)
	{
		Transform transform = KGUI.FindNode(buttonName, false);
		if (!KGUI._ControlsCallbacks.ContainsKey(KGUI.NodeUniqueName(transform)))
		{
			KGUI._ControlsCallbacks.Add(KGUI.NodeUniqueName(transform), func);
		}
		else
		{
			KGUI._ControlsCallbacks.Remove(KGUI.NodeUniqueName(transform));
			KGUI._ControlsCallbacks.Add(KGUI.NodeUniqueName(transform), func);
		}
		UIButtonMessage[] components = transform.gameObject.GetComponents<UIButtonMessage>();
		foreach (UIButtonMessage obj in components)
		{
			UnityEngine.Object.Destroy(obj);
		}
		UIButtonMessage uibuttonMessage = transform.gameObject.AddComponent<UIButtonMessage>();
		uibuttonMessage.target = KGUI._Instance.gameObject;
		uibuttonMessage.functionName = "OnButtonClick";
		uibuttonMessage.trigger = UIButtonMessage.Trigger.OnMouseOver;
		uibuttonMessage.includeChildren = false;
		UIButtonMessage uibuttonMessage2 = transform.gameObject.AddComponent<UIButtonMessage>();
		uibuttonMessage2.target = KGUI._Instance.gameObject;
		uibuttonMessage2.functionName = "OnButtonClick";
		uibuttonMessage2.trigger = UIButtonMessage.Trigger.OnMouseOut;
		uibuttonMessage2.includeChildren = false;
	}

	public static void RemoveButtonHoverOnCallback(string buttonName)
	{
		Transform transform = KGUI.FindNode(buttonName, false);
		if (KGUI._ControlsCallbacks.ContainsKey(KGUI.NodeUniqueName(transform)))
		{
			KGUI._ControlsCallbacks.Remove(KGUI.NodeUniqueName(transform));
		}
		UIButtonMessage[] components = transform.gameObject.GetComponents<UIButtonMessage>();
		foreach (UIButtonMessage obj in components)
		{
			UnityEngine.Object.Destroy(obj);
		}
	}

	public static void SetButtonCallback(string[] buttonNames, Action func)
	{
		foreach (string buttonName in buttonNames)
		{
			KGUI.SetButtonCallback(buttonName, func);
		}
	}

	private void OnButtonClick(GameObject sender)
	{
		UIImageButton component = sender.GetComponent<UIImageButton>();
		if (component != null && component.isPressed)
		{
			return;
		}
		SoundManager.Instance.Play(SoundManager.Sound.MenuClick, base.GetComponent<AudioSource>());
		KGUI.CallControlCallback(sender, false);
	}

	public static void SetSliderCallback(string sliderName, Action func)
	{
		Transform transform = KGUI.FindNode(sliderName, false);
		KGUI._ControlsCallbacks.Add(KGUI.NodeUniqueName(transform), func);
		UISlider component = transform.GetComponent<UISlider>();
		component.eventReceiver = KGUI._Instance.gameObject;
		component.functionName = "OnSliderChange";
	}

	private void OnSliderChange(float value)
	{
		KGUI.CallControlCallback(UISlider.current.gameObject, false);
	}

	public static void SetCheckboxCallback(string checkboxName, Action func)
	{
		Transform transform = KGUI.FindNode(checkboxName, false);
		KGUI._ControlsCallbacks.Add(KGUI.NodeUniqueName(transform), func);
		UICheckbox component = transform.GetComponent<UICheckbox>();
		component.eventReceiver = KGUI._Instance.gameObject;
		component.functionName = "OnCheckboxActivate";
	}

	private void OnCheckboxActivate(bool value)
	{
		KGUI.CallControlCallback(UICheckbox.current.gameObject, false);
	}

	public static void SetInputCallback(string inputName, Action func)
	{
		Transform transform = KGUI.FindNode(inputName, false);
		KGUI._ControlsCallbacks.Add(KGUI.NodeUniqueName(transform), func);
		UIForwardEvents uiforwardEvents = transform.gameObject.AddComponent<UIForwardEvents>();
		uiforwardEvents.target = KGUI._Instance.gameObject;
		uiforwardEvents.onInputChanged = true;
	}

	private void OnInputChanged(GameObject from)
	{
		KGUI.CallControlCallback(from, false);
	}

	public static void SetTooltipCallback(string tooltipName, Func<string> func)
	{
		Transform transform = KGUI.FindNode(tooltipName, false);
		KGUI._ControlsCallbacksStr.Add(KGUI.NodeUniqueName(transform), func);
		UIForwardEvents uiforwardEvents = transform.gameObject.AddComponent<UIForwardEvents>();
		uiforwardEvents.target = KGUI._Instance.gameObject;
		uiforwardEvents.onTooltip = true;
	}

	private void OnTooltipShow(GameObject from)
	{
		string tooltipText = KGUI.CallControlCallback(from, true);
		UITooltip.ShowText(tooltipText);
	}

	private void OnTooltipHide(GameObject from)
	{
		UITooltip.ShowText(null);
	}

	private static string NodeUniqueName(Transform node)
	{
		Transform transform = node;
		while (transform.parent.name != "Anchor")
		{
			transform = transform.parent;
		}
		string text = transform.name + node.name;
		int num;
		if (node.parent.name == "slot_prototype" || int.TryParse(node.parent.name, out num))
		{
			text += node.parent.parent.name;
		}
		else
		{
			text += node.parent.name;
		}
		return text;
	}

	private static string CallControlCallback(GameObject sender, bool returnStr = false)
	{
		if (KGUI._Instance.transform.root != sender.transform.root)
		{
			return null;
		}
		KGUI.CallbackSender = sender;
		if (!int.TryParse(KGUI.CallbackSender.transform.parent.name, out KGUI.SlotIndex) && !int.TryParse(KGUI.CallbackSender.transform.parent.parent.name, out KGUI.SlotIndex))
		{
			KGUI.SlotIndex = -1;
		}
		if (!int.TryParse(KGUI.CallbackSender.transform.parent.parent.parent.name, out KGUI.SlotIndex2))
		{
			KGUI.SlotIndex2 = -1;
		}
		if (!returnStr)
		{
			KGUI._ControlsCallbacks[KGUI.NodeUniqueName(sender.transform)]();
			return null;
		}
		return KGUI._ControlsCallbacksStr[KGUI.NodeUniqueName(sender.transform)]();
	}

	public static void SetControlSprite(Transform imageNode, string spriteName, float size)
	{
		UISprite component = imageNode.GetChild(0).GetComponent<UISprite>();
		component.spriteName = spriteName;
		component.MakePixelPerfect();
		if (size != 0f)
		{
			Vector3 localScale = component.transform.localScale;
			float num = Mathf.Clamp01(Mathf.Min(size / localScale.x, size / localScale.y));
			imageNode.localScale = new Vector3(num, num, 1f);
		}
	}

	public static void SavePanelScroll(UISlider slider)
	{
		if (KGUI._ScrollGrid != null && KGUI._ScrollGridNode.gameObject.activeInHierarchy)
		{
			PlayerPrefs.SetFloat("Scroll_" + KGUI._ScrollGrid, slider.sliderValue);
		}
	}

	public static Vector3 ScreenToGUIPoint(Vector3 pos)
	{
		float num = 1f / UIRoot.GetPixelSizeAdjustment(KGUI._Instance.gameObject);
		return new Vector3((pos.x - (float)(Screen.width / 2)) / num, (pos.y - (float)(Screen.height / 2)) / num, 1f);
	}

	public static GameObject CallbackSender;

	public static int SlotIndex;

	public static int SlotIndex2;

	private static KGUI _Instance;

	private static Dictionary<string, Action> _ControlsCallbacks = new Dictionary<string, Action>();

	private static Dictionary<string, Func<string>> _ControlsCallbacksStr = new Dictionary<string, Func<string>>();

	private static List<List<string>> _ModalControls = new List<List<string>>();

	private static string _ScrollGrid = null;

	private static Transform _ScrollGridNode = null;

	private static Dictionary<UILabel, string> _LabelsInitText = new Dictionary<UILabel, string>();
}
