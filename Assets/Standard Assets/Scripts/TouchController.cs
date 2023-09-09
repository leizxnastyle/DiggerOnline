using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("ControlFreak/Control Freak Controller")]
[ExecuteInEditMode]
public class TouchController : MonoBehaviour
{
	public void InitController()
	{
		this.contentDirtyFlag = false;
		this.firstPostPollUpdate = true;
		this.curTime = 0f;
		this.deltaTime = 0.0166666675f;
		this.invDeltaTime = 1f / this.deltaTime;
		this.lastRealTime = Time.realtimeSinceStartup;
		this.emuMousePos = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
		if (this.sticks == null)
		{
			this.sticks = new TouchStick[0];
		}
		if (this.touchZones == null)
		{
			this.touchZones = new TouchZone[0];
		}
		if (this.touchables == null)
		{
			this.touchables = new List<TouchableControl>(16);
		}
		this.touchables.Clear();
		if (this.sticks != null)
		{
			foreach (TouchStick touchStick in this.sticks)
			{
				if (touchStick != null)
				{
					this.touchables.Add(touchStick);
				}
			}
		}
		if (this.touchZones != null)
		{
			foreach (TouchZone touchZone in this.touchZones)
			{
				if (touchZone != null)
				{
					this.touchables.Add(touchZone);
				}
			}
		}
		foreach (TouchableControl touchableControl in this.touchables)
		{
			touchableControl.Init(this);
		}
		if (!this.initialized)
		{
			this.StartAlphaAnim(1f, 0f);
		}
		this.Layout();
		this.initialized = true;
	}

	public void PollController()
	{
		if (this.automaticMode)
		{
			return;
		}
		this.PollControllerInternal();
	}

	public void UpdateController()
	{
		if (this.automaticMode)
		{
			return;
		}
		this.UpdateControllerInternal();
	}

	public void DrawControllerGUI()
	{
		if (this.automaticMode && !this.manualGui)
		{
			return;
		}
		this.DrawControllerGUIInternal();
	}

	public void ResetController()
	{
		if (this.touchables != null)
		{
			for (int i = 0; i < this.touchables.Count; i++)
			{
				this.touchables[i].OnReset();
			}
		}
	}

	public void ReleaseTouches()
	{
		foreach (TouchableControl touchableControl in this.touchables)
		{
			touchableControl.ReleaseTouches();
		}
	}

	public void ShowController(float animDuration)
	{
		this.StartAlphaAnim(1f, animDuration);
	}

	public void HideController(float animDuration)
	{
		this.StartAlphaAnim(0f, animDuration);
	}

	public float GetAlpha()
	{
		return this.globalAlpha;
	}

	public void DisableController()
	{
		this.disableAll = true;
		this.ReleaseTouches();
	}

	public void EnableController()
	{
		this.disableAll = false;
	}

	public bool ControllerEnabled()
	{
		return !this.disableAll;
	}

	public bool LayoutChanged()
	{
		return this.customLayoutNeedsRebuild;
	}

	public void LayoutChangeHandled()
	{
		this.customLayoutNeedsRebuild = false;
	}

	public void ResetAllRects()
	{
		foreach (TouchableControl touchableControl in this.touchables)
		{
			touchableControl.ResetRect();
		}
	}

	public float GetDPI()
	{
		return this.GetActualDPI();
	}

	public float GetDPCM()
	{
		return this.GetDPI() / 2.54f;
	}

	public float GetActualDPI()
	{
		if (Screen.dpi != 0f)
		{
			return Screen.dpi;
		}
		return 96f;
	}

	public Rect GetScreenEmuRect(bool viewportRect = false)
	{
		return new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
	}

	public bool GetLeftHandedMode()
	{
		return this.leftHandedMode;
	}

	public void SetLeftHandedMode(bool enableLeftedHandMode)
	{
		if (this.leftHandedMode != enableLeftedHandMode)
		{
			this.leftHandedMode = enableLeftedHandMode;
			this.SetLayoutDirtyFlag();
		}
	}

	public void ResetMaskAreas()
	{
		if (this.maskAreas == null)
		{
			this.maskAreas = new List<Rect>(8);
		}
		else
		{
			this.maskAreas.Clear();
		}
	}

	public void AddMaskArea(Rect r)
	{
		if (this.maskAreas == null)
		{
			this.maskAreas = new List<Rect>(8);
		}
		this.maskAreas.Add(r);
	}

	public int StickCount
	{
		get
		{
			return this.sticks.Length;
		}
	}

	public int GetStickCount()
	{
		return this.sticks.Length;
	}

	public int GetStickId(string name)
	{
		return this.GetTouchableArrayElemId(this.sticks, name);
	}

	public TouchStick GetStick(int id)
	{
		if (id < 0 || this.sticks == null || id >= this.sticks.Length)
		{
			return this.GetBlankStick();
		}
		return this.sticks[id];
	}

	public TouchStick GetStick(string name)
	{
		return this.GetStick(this.GetStickId(name));
	}

	public TouchStick GetStickOrNull(int id)
	{
		if (id < 0 || this.sticks == null || id >= this.sticks.Length)
		{
			return null;
		}
		return this.sticks[id];
	}

	public TouchStick GetStickOrNull(string name)
	{
		return this.GetStickOrNull(this.GetStickId(name));
	}

	private TouchStick GetBlankStick()
	{
		if (this.blankStick != null)
		{
			return this.blankStick;
		}
		this.blankStick = new TouchStick();
		this.blankStick.Init(this);
		this.blankStick.OnReset();
		this.blankStick.name = "BLANK-STICK";
		return this.blankStick;
	}

	public int ZoneCount
	{
		get
		{
			return this.touchZones.Length;
		}
	}

	public int GetZoneCount()
	{
		return this.touchZones.Length;
	}

	public int GetZoneId(string name)
	{
		return this.GetTouchableArrayElemId(this.touchZones, name);
	}

	public TouchZone GetZone(int id)
	{
		if (id < 0 || this.touchZones == null || id >= this.touchZones.Length)
		{
			return this.GetBlankZone();
		}
		return this.touchZones[id];
	}

	public TouchZone GetZone(string name)
	{
		return this.GetZone(this.GetZoneId(name));
	}

	public TouchZone GetZoneOrNull(int id)
	{
		if (id < 0 || this.touchZones == null || id >= this.touchZones.Length)
		{
			return null;
		}
		return this.touchZones[id];
	}

	public TouchZone GetZoneOrNull(string name)
	{
		return this.GetZoneOrNull(this.GetZoneId(name));
	}

	private TouchZone GetBlankZone()
	{
		if (this.blankZone != null)
		{
			return this.blankZone;
		}
		this.blankZone = new TouchZone();
		this.blankZone.Init(this);
		this.blankZone.OnReset();
		this.blankZone.name = "NULL";
		return this.blankZone;
	}

	public int ControlCount
	{
		get
		{
			return (this.touchables != null) ? this.touchables.Count : 0;
		}
	}

	public int GetControlCount()
	{
		return (this.touchables != null) ? this.touchables.Count : 0;
	}

	public TouchableControl GetControl(int id)
	{
		if (id < 0 || this.touchables == null || id >= this.touchables.Count)
		{
			return null;
		}
		return this.touchables[id];
	}

	public float GetAxisEx(string name, out bool axisSupported)
	{
		axisSupported = false;
		float num = 0f;
		for (int i = 0; i < this.sticks.Length; i++)
		{
			bool flag = false;
			float axisEx = this.sticks[i].GetAxisEx(name, out flag);
			if (flag)
			{
				axisSupported = true;
				num += axisEx;
			}
		}
		for (int j = 0; j < this.touchZones.Length; j++)
		{
			bool flag2 = false;
			float axisEx2 = this.touchZones[j].GetAxisEx(name, out flag2);
			if (flag2)
			{
				axisSupported = true;
				num += axisEx2;
			}
		}
		return num;
	}

	public float GetAxis(string name)
	{
		bool flag = false;
		return this.GetAxisEx(name, out flag);
	}

	public float GetAxisRaw(string name)
	{
		return this.GetAxis(name);
	}

	public bool GetButton(string buttonName)
	{
		bool flag = false;
		return this.GetButtonEx(buttonName, out flag);
	}

	public bool GetButtonDown(string buttonName)
	{
		bool flag = false;
		return this.GetButtonDownEx(buttonName, out flag);
	}

	public bool GetButtonUp(string buttonName)
	{
		bool flag = false;
		return this.GetButtonUpEx(buttonName, out flag);
	}

	public bool GetButtonEx(string buttonName, out bool buttonSupported)
	{
		buttonSupported = false;
		for (int i = 0; i < this.sticks.Length; i++)
		{
			bool flag = false;
			bool buttonEx = this.sticks[i].GetButtonEx(buttonName, out flag);
			if (flag)
			{
				buttonSupported = true;
			}
			if (buttonEx)
			{
				return true;
			}
		}
		for (int j = 0; j < this.touchZones.Length; j++)
		{
			bool flag2 = false;
			bool buttonEx2 = this.touchZones[j].GetButtonEx(buttonName, out flag2);
			if (flag2)
			{
				buttonSupported = true;
			}
			if (buttonEx2)
			{
				return true;
			}
		}
		return false;
	}

	public bool GetButtonDownEx(string buttonName, out bool buttonSupported)
	{
		buttonSupported = false;
		for (int i = 0; i < this.sticks.Length; i++)
		{
			bool flag = false;
			bool buttonDownEx = this.sticks[i].GetButtonDownEx(buttonName, out flag);
			if (flag)
			{
				buttonSupported = true;
			}
			if (buttonDownEx)
			{
				return true;
			}
		}
		for (int j = 0; j < this.touchZones.Length; j++)
		{
			bool flag2 = false;
			bool buttonDownEx2 = this.touchZones[j].GetButtonDownEx(buttonName, out flag2);
			if (flag2)
			{
				buttonSupported = true;
			}
			if (buttonDownEx2)
			{
				return true;
			}
		}
		return false;
	}

	public bool GetButtonUpEx(string buttonName, out bool buttonSupported)
	{
		buttonSupported = false;
		for (int i = 0; i < this.sticks.Length; i++)
		{
			bool flag = false;
			bool buttonUpEx = this.sticks[i].GetButtonUpEx(buttonName, out flag);
			if (flag)
			{
				buttonSupported = true;
			}
			if (buttonUpEx)
			{
				return true;
			}
		}
		for (int j = 0; j < this.touchZones.Length; j++)
		{
			bool flag2 = false;
			bool buttonUpEx2 = this.touchZones[j].GetButtonUpEx(buttonName, out flag2);
			if (flag2)
			{
				buttonSupported = true;
			}
			if (buttonUpEx2)
			{
				return true;
			}
		}
		return false;
	}

	public bool GetKey(KeyCode keyCode)
	{
		bool flag = false;
		return this.GetKeyEx(keyCode, out flag);
	}

	public bool GetKeyDown(KeyCode keyCode)
	{
		bool flag = false;
		return this.GetKeyDownEx(keyCode, out flag);
	}

	public bool GetKeyUp(KeyCode keyCode)
	{
		bool flag = false;
		return this.GetKeyUpEx(keyCode, out flag);
	}

	public bool GetKeyEx(KeyCode keyCode, out bool keySupported)
	{
		keySupported = false;
		for (int i = 0; i < this.sticks.Length; i++)
		{
			bool flag = false;
			bool keyEx = this.sticks[i].GetKeyEx(keyCode, out flag);
			if (flag)
			{
				keySupported = true;
			}
			if (keyEx)
			{
				return true;
			}
		}
		for (int j = 0; j < this.touchZones.Length; j++)
		{
			bool flag2 = false;
			bool keyEx2 = this.touchZones[j].GetKeyEx(keyCode, out flag2);
			if (flag2)
			{
				keySupported = true;
			}
			if (keyEx2)
			{
				return true;
			}
		}
		return false;
	}

	public bool GetKeyDownEx(KeyCode keyCode, out bool keySupported)
	{
		keySupported = false;
		for (int i = 0; i < this.sticks.Length; i++)
		{
			bool flag = false;
			bool keyDownEx = this.sticks[i].GetKeyDownEx(keyCode, out flag);
			if (flag)
			{
				keySupported = true;
			}
			if (keyDownEx)
			{
				return true;
			}
		}
		for (int j = 0; j < this.touchZones.Length; j++)
		{
			bool flag2 = false;
			bool keyDownEx2 = this.touchZones[j].GetKeyDownEx(keyCode, out flag2);
			if (flag2)
			{
				keySupported = true;
			}
			if (keyDownEx2)
			{
				return true;
			}
		}
		return false;
	}

	public bool GetKeyUpEx(KeyCode keyCode, out bool keySupported)
	{
		keySupported = false;
		for (int i = 0; i < this.sticks.Length; i++)
		{
			bool flag = false;
			bool keyUpEx = this.sticks[i].GetKeyUpEx(keyCode, out flag);
			if (flag)
			{
				keySupported = true;
			}
			if (keyUpEx)
			{
				return true;
			}
		}
		for (int j = 0; j < this.touchZones.Length; j++)
		{
			bool flag2 = false;
			bool keyUpEx2 = this.touchZones[j].GetKeyUpEx(keyCode, out flag2);
			if (flag2)
			{
				keySupported = true;
			}
			if (keyUpEx2)
			{
				return true;
			}
		}
		return false;
	}

	public bool GetMouseButton(int i)
	{
		return this.GetKey((i != 0) ? ((i != 1) ? ((i != 2) ? KeyCode.None : KeyCode.Mouse2) : KeyCode.Mouse1) : KeyCode.Mouse0);
	}

	public bool GetMouseButtonDown(int i)
	{
		return this.GetKeyDown((i != 0) ? ((i != 1) ? ((i != 2) ? KeyCode.None : KeyCode.Mouse2) : KeyCode.Mouse1) : KeyCode.Mouse0);
	}

	public bool GetMouseButtonUp(int i)
	{
		return this.GetKeyUp((i != 0) ? ((i != 1) ? ((i != 2) ? KeyCode.None : KeyCode.Mouse2) : KeyCode.Mouse1) : KeyCode.Mouse0);
	}

	public Vector2 GetMousePos()
	{
		return this.emuMousePos;
	}

	public static Collider PickCollider(Vector2 screenPos, Camera cam, LayerMask layerMask)
	{
		Ray ray = cam.ScreenPointToRay(new Vector3(screenPos.x, (float)Screen.height - screenPos.y, 0f));
		float radius = 0.1f;
		RaycastHit raycastHit;
		if (!Physics.SphereCast(ray, radius, out raycastHit, float.PositiveInfinity, layerMask))
		{
			return null;
		}
		return raycastHit.collider;
	}

	private void InitIfNeeded()
	{
		if (!this.initialized || this.contentDirtyFlag)
		{
			this.InitController();
		}
	}

	private void OnEnable()
	{
		this.InitIfNeeded();
		this.ResetLayoutBoxes();
	}

	public static bool IsSupported()
	{
		return (SystemInfo.deviceType == DeviceType.Handheld && Input.multiTouchEnabled) || Application.platform == RuntimePlatform.IPhonePlayer;
	}

	private void Awake()
	{
		this.InitIfNeeded();
		if (this.disableWhenNoTouchScreen && !TouchController.IsSupported())
		{
			base.enabled = false;
		}
	}

	private void OnDestroy()
	{
		if (CFInput.ctrl == this)
		{
			CFInput.ctrl = null;
		}
	}

	private void Start()
	{
		if (this.automaticMode && !this.initialized)
		{
			this.InitController();
		}
		if (this.autoActivate)
		{
			CFInput.ctrl = this;
		}
	}

	private void Update()
	{
		this.InitIfNeeded();
		if (this.automaticMode)
		{
			this.PollControllerInternal();
			this.UpdateControllerInternal();
		}
	}

	private void OnGUI()
	{
		if (this.automaticMode && !this.manualGui)
		{
			this.DrawControllerGUIInternal();
		}
	}

	private void OnApplicationPause(bool pause)
	{
		this.releaseTouchesFlag = true;
	}

	public void SetLayoutDirtyFlag()
	{
		this.layoutDirtyFlag = true;
	}

	public void SetContentDirtyFlag()
	{
		this.contentDirtyFlag = true;
	}

	private void LayoutIfDirty()
	{
		if (this.layoutDirtyFlag || Screen.width != this.screenWidth || Screen.height != this.screenHeight)
		{
			this.Layout();
		}
	}

	private int GetTouchableArrayElemId(TouchableControl[] carr, string name)
	{
		if (carr == null)
		{
			return -1;
		}
		for (int i = 0; i < carr.Length; i++)
		{
			if (name.Equals(carr[i].name, StringComparison.OrdinalIgnoreCase))
			{
				return i;
			}
		}
		return -1;
	}

	private void Layout()
	{
		this.customLayoutNeedsRebuild = true;
		this.releaseTouchesFlag = true;
		this.layoutDirtyFlag = false;
		this.screenWidth = Screen.width;
		this.screenHeight = Screen.height;
		this.fingerBufferRadPx = Mathf.Max(1f, 0.5f * (this.fingerBufferCm * this.GetDPCM()));
		this.touchTapMaxDistPx = Mathf.Clamp(this.touchTapMaxDistCm * this.GetDPCM(), 1f, Mathf.Min(this.GetScreenWidth(), this.GetScreenHeight()) * 0.3f);
		this.pinchMinDistPx = Mathf.Clamp(this.pinchMinDistCm * this.GetDPCM(), 1f, Mathf.Min(this.GetScreenWidth(), this.GetScreenHeight()) * 0.3f);
		this.twistSafeFingerDistPx = Mathf.Clamp(this.twistSafeFingerDistCm * this.GetDPCM(), 1f, Mathf.Min(this.GetScreenWidth(), this.GetScreenHeight()) * 0.3f);
		this.ResetLayoutBoxes();
		foreach (TouchableControl touchableControl in this.touchables)
		{
			touchableControl.OnLayoutAddContent();
		}
		foreach (TouchController.LayoutBox layoutBox in this.layoutBoxes)
		{
			layoutBox.ContentFinalize();
		}
		if (this.touchables != null)
		{
			foreach (TouchableControl touchableControl2 in this.touchables)
			{
				touchableControl2.OnLayout();
			}
		}
	}

	private void ResetLayoutBoxes()
	{
		if (this.layoutBoxes == null || this.layoutBoxes.Length != 16 || this.layoutBoxes[0] == null)
		{
			this.layoutBoxes = new TouchController.LayoutBox[16];
			for (int i = 0; i < this.layoutBoxes.Length; i++)
			{
				switch (i)
				{
				case 0:
					this.layoutBoxes[i] = new TouchController.LayoutBox("Full-Screen", 0f, 0f, 1f, 1f, TouchController.LayoutAnchor.TOP_LEFT);
					break;
				case 1:
					this.layoutBoxes[i] = new TouchController.LayoutBox("Right-Half", 0.5f, 0f, 0.5f, 1f, TouchController.LayoutAnchor.MID_RIGHT);
					break;
				case 2:
					this.layoutBoxes[i] = new TouchController.LayoutBox("Left-Half", 0f, 0f, 0.5f, 1f, TouchController.LayoutAnchor.MID_LEFT);
					break;
				case 3:
					this.layoutBoxes[i] = new TouchController.LayoutBox("Top-Half", 0f, 0f, 1f, 0.5f, TouchController.LayoutAnchor.TOP_CENTER);
					break;
				case 4:
					this.layoutBoxes[i] = new TouchController.LayoutBox("Bottom-Half", 0f, 0.5f, 1f, 0.5f, TouchController.LayoutAnchor.BOTTOM_CENTER);
					break;
				case 5:
					this.layoutBoxes[i] = new TouchController.LayoutBox("Bottom-Right-Qrtr", 0.5f, 0.5f, 0.5f, 0.5f, TouchController.LayoutAnchor.BOTTOM_RIGHT);
					break;
				case 6:
					this.layoutBoxes[i] = new TouchController.LayoutBox("Bottom-Left-Qrtr", 0f, 0.5f, 0.5f, 0.5f, TouchController.LayoutAnchor.BOTTOM_LEFT);
					break;
				case 7:
					this.layoutBoxes[i] = new TouchController.LayoutBox("Top-Right-Qrtr", 0.5f, 0f, 0.5f, 0.5f, TouchController.LayoutAnchor.TOP_RIGHT);
					break;
				case 8:
					this.layoutBoxes[i] = new TouchController.LayoutBox("Top-Left-Qrtr", 0f, 0f, 0.5f, 0.5f, TouchController.LayoutAnchor.TOP_LEFT);
					break;
				default:
					this.layoutBoxes[i] = new TouchController.LayoutBox("User" + i.ToString("00"), 0f, 0f, 1f, 1f, TouchController.LayoutAnchor.TOP_LEFT);
					break;
				}
			}
		}
		foreach (TouchController.LayoutBox layoutBox in this.layoutBoxes)
		{
			layoutBox.SetController(this);
			layoutBox.ResetContent();
		}
	}

	private void PollControllerInternal()
	{
		this.firstPostPollUpdate = true;
		this.LayoutIfDirty();
		foreach (TouchableControl touchableControl in this.touchables)
		{
			touchableControl.OnPrePoll();
		}
		if (this.releaseTouchesFlag)
		{
			foreach (TouchableControl touchableControl2 in this.touchables)
			{
				touchableControl2.ReleaseTouches();
			}
			this.releaseTouchesFlag = false;
		}
		int touchCount = UnityEngine.Input.touchCount;
		int i = 0;
		while (i < touchCount)
		{
			Vector2 zero = Vector2.zero;
			Touch touch = UnityEngine.Input.GetTouch(i);
			TouchPhase phase = touch.phase;
			int fingerId = touch.fingerId;
			zero = new Vector2(touch.position.x, (float)Screen.height - touch.position.y);
			if (phase != TouchPhase.Began || this.maskAreas == null)
			{
				goto IL_162;
			}
			bool flag = false;
			for (int j = 0; j < this.maskAreas.Count; j++)
			{
				if (this.maskAreas[j].Contains(zero))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				goto IL_162;
			}
			IL_314:
			i++;
			continue;
			IL_162:
			switch (phase)
			{
			case TouchPhase.Began:
				if (this.disableAll)
				{
					goto IL_314;
				}
				for (int k = 0; k < 8; k++)
				{
					TouchableControl touchableControl3 = null;
					TouchController.HitTestResult r = new TouchController.HitTestResult(false);
					for (int l = 0; l < this.touchables.Count; l++)
					{
						if (k <= 0 || this.touchables[l].acceptSharedTouches)
						{
							if (touchableControl3 == null || r.prio <= this.touchables[l].prio)
							{
								TouchController.HitTestResult hitTestResult = this.touchables[l].HitTest(zero, fingerId);
								if (hitTestResult.hit)
								{
									if (touchableControl3 == null || hitTestResult.IsCloserThan(r))
									{
										touchableControl3 = this.touchables[l];
										r = hitTestResult;
									}
								}
							}
						}
					}
					if (touchableControl3 != null)
					{
						if (touchableControl3.OnTouchStart(fingerId, zero) != TouchController.EventResult.SHARED)
						{
							break;
						}
					}
				}
				goto IL_314;
			case TouchPhase.Moved:
			case TouchPhase.Stationary:
				for (int m = 0; m < this.touchables.Count; m++)
				{
					this.touchables[m].OnTouchMove(fingerId, zero);
				}
				goto IL_314;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				for (int n = 0; n < this.touchables.Count; n++)
				{
					this.touchables[n].OnTouchEnd(fingerId, false);
				}
				goto IL_314;
			default:
				goto IL_314;
			}
		}
		foreach (TouchableControl touchableControl4 in this.touchables)
		{
			touchableControl4.OnPostPoll();
		}
	}

	private void UpdateControllerInternal()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		this.deltaTime = realtimeSinceStartup - this.lastRealTime;
		if (this.deltaTime <= 0.0001f)
		{
			this.deltaTime = 0.0166666675f;
		}
		this.invDeltaTime = 1f / this.deltaTime;
		this.curTime += this.deltaTime;
		this.lastRealTime = realtimeSinceStartup;
		if (this.globalAlphaTimer.Enabled)
		{
			this.globalAlphaTimer.Update(this.deltaTime);
			this.globalAlpha = Mathf.Lerp(this.globalAlphaStart, this.globalAlphaEnd, this.globalAlphaTimer.Nt);
			if (this.globalAlphaTimer.Completed)
			{
				this.globalAlphaTimer.Disable();
			}
		}
		if (this.touchables != null)
		{
			foreach (TouchableControl touchableControl in this.touchables)
			{
				touchableControl.OnUpdate(this.firstPostPollUpdate);
			}
			foreach (TouchableControl touchableControl2 in this.touchables)
			{
				touchableControl2.OnPostUpdate(this.firstPostPollUpdate);
			}
		}
		this.firstPostPollUpdate = false;
	}

	private void DrawControllerGUIInternal()
	{
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		bool enabled = GUI.enabled;
		int depth = GUI.depth;
		GUI.depth = this.guiDepth;
		GUI.enabled = true;
		if (this.touchables != null)
		{
			for (int i = 0; i < this.touchables.Count; i++)
			{
				this.touchables[i].DrawGUI();
			}
		}
		GUI.depth = depth;
		if (GUI.enabled != enabled)
		{
			GUI.enabled = enabled;
		}
	}

	public void SetInternalMousePos(Vector2 pos, bool inGuiSpace = true)
	{
		if (inGuiSpace)
		{
			pos.y = (float)Screen.height - pos.y;
		}
		this.emuMousePos = pos;
	}

	private void StartAlphaAnim(float targetAlpha, float time)
	{
		if (time <= 0f)
		{
			this.globalAlphaTimer.Reset(0f);
			this.globalAlpha = targetAlpha;
			this.globalAlphaEnd = targetAlpha;
			this.globalAlphaStart = targetAlpha;
		}
		else
		{
			this.globalAlphaStart = this.globalAlpha;
			this.globalAlphaEnd = targetAlpha;
			this.globalAlphaTimer.Start(time);
		}
	}

	public TouchController.HitTestResult HitTestCircle(Vector2 cen, float rad, Vector2 touchPos, bool useFingerBuffer = true)
	{
		TouchController.HitTestResult result = new TouchController.HitTestResult(false);
		result.dist = (touchPos - cen).magnitude;
		if (result.dist > rad + ((!useFingerBuffer) ? 0f : this.fingerBufferRadPx))
		{
			result.hit = false;
			return result;
		}
		result.hit = true;
		result.hitInside = (result.dist <= rad);
		result.distScale = 1f;
		return result;
	}

	public TouchController.HitTestResult HitTestBox(Vector2 cen, Vector2 size, Vector2 touchPos, bool useFingerBuffer = true)
	{
		TouchController.HitTestResult result = new TouchController.HitTestResult(false);
		float num = (!useFingerBuffer) ? 0f : this.fingerBufferRadPx;
		Vector2 vector = new Vector2(Mathf.Abs(touchPos.x - cen.x), Mathf.Abs(touchPos.y - cen.y));
		size *= 0.5f;
		if (vector.x > size.x + num || vector.y > size.y + num)
		{
			result.hit = false;
			return result;
		}
		result.hit = true;
		result.hitInside = (vector.x <= size.x && vector.y <= size.y);
		result.dist = vector.magnitude;
		result.distScale = 1f;
		return result;
	}

	public TouchController.HitTestResult HitTestRect(Rect rect, Vector2 touchPos, bool useFingerBuffer = true)
	{
		TouchController.HitTestResult result = new TouchController.HitTestResult(false);
		float num = (!useFingerBuffer) ? 0f : this.fingerBufferRadPx;
		Vector2 vector = touchPos - rect.center;
		vector.x = Mathf.Abs(vector.x);
		vector.y = Mathf.Abs(vector.y);
		Vector2 vector2 = new Vector2(rect.width * 0.5f, rect.height * 0.5f);
		if (vector.x > vector2.x + num || vector.y > vector2.y + num)
		{
			result.hit = false;
			return result;
		}
		result.hit = true;
		result.hitInside = (vector.x <= vector2.x && vector.y <= vector2.y);
		result.dist = vector.magnitude;
		result.distScale = 1f;
		return result;
	}

	public void EndTouch(int touchId, TouchableControl ctrlToIgnore)
	{
		if (touchId < 0)
		{
			return;
		}
		foreach (TouchableControl touchableControl in this.touchables)
		{
			if (touchableControl != ctrlToIgnore)
			{
				touchableControl.OnTouchEnd(touchId, false);
			}
		}
	}

	public float GetScreenWidth()
	{
		return (float)Screen.width;
	}

	public float GetScreenHeight()
	{
		return (float)Screen.height;
	}

	public float GetScreenX(float xFactor)
	{
		return xFactor * (float)Screen.width;
	}

	public float GetScreenY(float yFactor)
	{
		return yFactor * (float)Screen.height;
	}

	public float CmToPixels(float cmVal)
	{
		return this.GetDPCM() * cmVal;
	}

	public float PixelsToWorld(float pxVal)
	{
		float num = (this.rwUnit != TouchController.RealWorldUnit.CM) ? this.GetDPI() : this.GetDPCM();
		if (num <= 1E-05f)
		{
			return 0f;
		}
		return pxVal / num;
	}

	public Rect NormalizedRectToPx(Rect nrect, bool respectLeftHandMode = true)
	{
		Rect rect = Rect.MinMaxRect(this.GetScreenX(nrect.xMin), this.GetScreenY(nrect.yMin), this.GetScreenX(nrect.xMax), this.GetScreenY(nrect.yMax));
		if (respectLeftHandMode)
		{
			return this.RightHandToScreenRect(rect);
		}
		return rect;
	}

	public float GetPreviewScale()
	{
		return 1f;
	}

	public Vector2 RightHandToScreen(Vector2 pos)
	{
		if (!this.leftHandedMode)
		{
			return pos;
		}
		pos.x = (float)Screen.width - pos.x;
		return pos;
	}

	public Rect RightHandToScreenRect(Rect rect)
	{
		if (!this.leftHandedMode)
		{
			return rect;
		}
		Vector2 vector = this.RightHandToScreen(new Vector2(rect.xMin, rect.yMin));
		Vector2 vector2 = this.RightHandToScreen(new Vector2(rect.xMax, rect.yMax));
		return Rect.MinMaxRect(Mathf.Min(vector.x, vector2.x), Mathf.Min(vector.y, vector2.y), Mathf.Max(vector.x, vector2.x), Mathf.Max(vector.y, vector2.y));
	}

	private static Vector2 AnchorLeftover(Vector2 topLeftOfs, TouchController.LayoutAnchor anchor, float maxMarginX = 0f, float maxMarginY = 0f, bool uniformMargins = false)
	{
		float num = 0f;
		float num2 = 0f;
		if (topLeftOfs.x > 0f)
		{
			num = Mathf.Min(topLeftOfs.x, maxMarginX);
		}
		if (topLeftOfs.y > 0f)
		{
			num2 = Mathf.Min(topLeftOfs.y, maxMarginY);
		}
		if (uniformMargins && maxMarginX > 0.001f && maxMarginY > 0.001f)
		{
			float num3 = Mathf.Min(num / maxMarginX, num2 / maxMarginY);
			num = num3 * maxMarginX;
			num2 = num3 * maxMarginY;
		}
		switch (anchor)
		{
		case TouchController.LayoutAnchor.BOTTOM_LEFT:
			topLeftOfs.y -= num2;
			topLeftOfs.x = 0f + num;
			break;
		case TouchController.LayoutAnchor.BOTTOM_CENTER:
			topLeftOfs.y -= num2;
			topLeftOfs.x *= 0.5f;
			break;
		case TouchController.LayoutAnchor.BOTTOM_RIGHT:
			topLeftOfs.y -= num2;
			topLeftOfs.x -= num;
			break;
		case TouchController.LayoutAnchor.MID_LEFT:
			topLeftOfs.y *= 0.5f;
			topLeftOfs.x = 0f + num;
			break;
		case TouchController.LayoutAnchor.MID_CENTER:
			topLeftOfs.y *= 0.5f;
			topLeftOfs.x *= 0.5f;
			break;
		case TouchController.LayoutAnchor.MID_RIGHT:
			topLeftOfs.y *= 0.5f;
			topLeftOfs.x -= num;
			break;
		case TouchController.LayoutAnchor.TOP_LEFT:
			topLeftOfs.y = 0f + num2;
			topLeftOfs.x = 0f + num;
			break;
		case TouchController.LayoutAnchor.TOP_CENTER:
			topLeftOfs.y = 0f + num2;
			topLeftOfs.x *= 0.5f;
			break;
		case TouchController.LayoutAnchor.TOP_RIGHT:
			topLeftOfs.y = 0f + num2;
			topLeftOfs.x -= num;
			break;
		}
		return topLeftOfs;
	}

	public static float SlowDownEase(float t)
	{
		t = 1f - t;
		return 1f - t * t;
	}

	public static float SpeedUpEase(float t)
	{
		return t * t;
	}

	public static Color ScaleAlpha(Color c, float alphaScale)
	{
		c.a *= alphaScale;
		return c;
	}

	public static Rect GetCenImgRectAtPos(Vector2 pos, Texture2D img, float scale = 1f)
	{
		if (img == null)
		{
			return new Rect(pos.x, pos.y, 1f, 1f);
		}
		pos.x -= (float)img.width * 0.5f * scale;
		pos.y -= (float)img.height * 0.5f * scale;
		return new Rect(pos.x, pos.y, (float)img.width * scale, (float)img.height * scale);
	}

	public static Rect GetCenRect(Vector2 pos, Vector2 size)
	{
		pos.x -= size.x * 0.5f;
		pos.y -= size.y * 0.5f;
		return new Rect(pos.x, pos.y, size.x, size.y);
	}

	public static Rect GetCenRect(Vector2 pos, float size)
	{
		pos.x -= size * 0.5f;
		pos.y -= size * 0.5f;
		return new Rect(pos.x, pos.y, size, size);
	}

	public const int DEFAULT_ZONE_PRIO = 0;

	public const int DEFAULT_STICK_PRIO = 0;

	private const int MAX_EVENT_SHARE_COUNT = 8;

	private const float NON_MOBILE_DIAGONAL_INCHES = 7f;

	private const float DEFAULT_MONITOR_DPI = 96f;

	public const int LayoutBoxCount = 16;

	[NonSerialized]
	private const double EDITOR_SAFETY_UPDATE_INTERVAL = 2.0;

	public bool automaticMode = true;

	public bool manualGui;

	public bool autoActivate = true;

	public bool disableWhenNoTouchScreen;

	public int guiDepth = 10;

	public int guiPressedOfs = 10;

	public float fingerBufferCm = 0.8f;

	private float fingerBufferRadPx = 10f;

	public float stickMagnetAngleMargin = 10f;

	public float stickDigitalEnterThresh = 0.5f;

	public float stickDigitalLeaveThresh = 0.4f;

	public float touchTapMaxTime = 0.13333334f;

	public float doubleTapMaxGapTime = 0.333333343f;

	public float strictMultiFingerMaxTime = 0.2f;

	public float velPreserveTime = 0.1f;

	public float touchTapMaxDistCm = 0.5f;

	public float twistThresh = 5f;

	public float pinchMinDistCm = 0.5f;

	public float twistSafeFingerDistCm = 1f;

	public float curTime;

	public float deltaTime = 0.0166666675f;

	public float invDeltaTime = 60f;

	private float lastRealTime;

	private bool initialized;

	public TouchStick[] sticks;

	public TouchZone[] touchZones;

	public TouchController.LayoutBox[] layoutBoxes;

	[NonSerialized]
	private TouchStick blankStick;

	[NonSerialized]
	private TouchZone blankZone;

	[NonSerialized]
	private List<TouchableControl> touchables;

	[NonSerialized]
	private List<Rect> maskAreas;

	private bool layoutDirtyFlag;

	private bool contentDirtyFlag;

	private bool releaseTouchesFlag;

	public float pressAnimDuration = 0.1f;

	public float releaseAnimDuration = 0.3f;

	public float disableAnimDuration = 0.3f;

	public float enableAnimDuration = 0.3f;

	public float cancelAnimDuration = 0.3f;

	public float showAnimDuration = 0.3f;

	public float hideAnimDuration = 0.3f;

	public float releasedZoneScale = 1f;

	public float pressedZoneScale = 1.1f;

	public float disabledZoneScale = 1f;

	public float releasedStickHatScale = 0.75f;

	public float pressedStickHatScale = 0.9f;

	public float disabledStickHatScale = 0.75f;

	public float releasedStickBaseScale = 1f;

	public float pressedStickBaseScale = 0.9f;

	public float disabledStickBaseScale = 1f;

	public Color defaultPressedZoneColor = new Color(1f, 1f, 1f, 1f);

	public Color defaultReleasedZoneColor = new Color(1f, 1f, 1f, 0.75f);

	public Color defaultDisabledZoneColor = new Color(0.5f, 0.5f, 0.5f, 0.35f);

	public Color defaultPressedStickHatColor = new Color(1f, 1f, 1f, 1f);

	public Color defaultReleasedStickHatColor = new Color(1f, 1f, 1f, 0.75f);

	public Color defaultDisabledStickHatColor = new Color(0.5f, 0.5f, 0.5f, 0.35f);

	public Color defaultPressedStickBaseColor = new Color(1f, 1f, 1f, 1f);

	public Color defaultReleasedStickBaseColor = new Color(1f, 1f, 1f, 0.75f);

	public Color defaultDisabledStickBaseColor = new Color(0.5f, 0.5f, 0.5f, 0.35f);

	private float globalAlpha = 1f;

	private float globalAlphaStart;

	private float globalAlphaEnd;

	private AnimTimer globalAlphaTimer;

	private int screenWidth;

	private int screenHeight;

	private bool disableAll;

	private bool leftHandedMode;

	public KeyCode debugSecondTouchDragModeKey = KeyCode.LeftShift;

	public KeyCode debugSecondTouchPinchModeKey = KeyCode.LeftControl;

	public KeyCode debugSecondTouchTwistModeKey = KeyCode.LeftAlt;

	public bool debugDrawTouches = true;

	public bool debugDrawLayoutBoxes = true;

	public bool debugDrawAreas = true;

	public Texture2D debugTouchSprite;

	public Texture2D debugSecondTouchSprite;

	public Color debugFirstTouchNormalColor = new Color(1f, 1f, 0.6f, 0.3f);

	public Color debugFirstTouchActiveColor = new Color(1f, 1f, 0f, 0.7f);

	public Color debugSecondTouchNormalColor = new Color(1f, 1f, 1f, 0.3f);

	public Color debugSecondTouchActiveColor = new Color(1f, 0f, 0f, 0.6f);

	public Texture2D defaultZoneImg;

	public Texture2D defaultStickHatImg;

	public Texture2D defaultStickBaseImg;

	public Texture2D debugCircleImg;

	public Texture2D debugRectImg;

	public bool screenEmuOn;

	public bool screenEmuPortrait;

	public bool screenEmuShrink = true;

	public Vector2 screenEmuPan = new Vector2(0.5f, 0.5f);

	public int screenEmuHwDpi = 250;

	public TouchController.ScreenEmuMode screenEmuMode = TouchController.ScreenEmuMode.EXPAND;

	public int screenEmuHwHorzRes = 1024;

	public int screenEmuHwVertRes = 640;

	public float monitorDiagonal = 15f;

	public Color screenEmuBorderColor = new Color(0f, 0f, 0f, 0.75f);

	public Color screenEmuBorderBadColor = new Color(0.5f, 0f, 0f, 0.75f);

	public TouchController.RealWorldUnit rwUnit;

	public TouchController.PreviewMode previewMode;

	private bool firstPostPollUpdate;

	private bool customLayoutNeedsRebuild;

	private Vector2 emuMousePos;

	public int version;

	public float twistSafeFingerDistPx = 10f;

	public float pinchMinDistPx = 10f;

	public float touchTapMaxDistPx = 10f;

	[NonSerialized]
	private double editorLastSafetyUpdateTime;

	public enum RealWorldUnit
	{
		CM,
		INCH
	}

	public enum PreviewMode
	{
		RELEASED,
		PRESSED,
		DISABLED
	}

	public enum LayoutAnchor
	{
		BOTTOM_LEFT,
		BOTTOM_CENTER,
		BOTTOM_RIGHT,
		MID_LEFT,
		MID_CENTER,
		MID_RIGHT,
		TOP_LEFT,
		TOP_CENTER,
		TOP_RIGHT
	}

	public enum ControlShape
	{
		CIRCLE,
		RECT,
		SCREEN_REGION
	}

	public enum ScreenEmuMode
	{
		PIXEL_PERFECT,
		PHYSICAL,
		EXPAND
	}

	public enum EventResult
	{
		NOT_HANDLED,
		HANDLED,
		SHARED
	}

	public struct HitTestResult
	{
		public HitTestResult(bool hit)
		{
			this.hit = hit;
			this.dist = 1f;
			this.distScale = 1f;
			this.hitInside = false;
			this.prio = 0;
		}

		public bool IsCloserThan(TouchController.HitTestResult r)
		{
			return this.hit && (this.prio > r.prio || (this.hitInside && !r.hitInside) || ((this.prio != r.prio) ? (this.dist < r.dist) : (this.dist * this.distScale < r.dist * r.distScale)));
		}

		public bool hit;

		public float dist;

		public int prio;

		public bool hitInside;

		public float distScale;
	}

	[Serializable]
	public class LayoutBox
	{
		public LayoutBox(string name, float left, float top, float width, float height, TouchController.LayoutAnchor anchor)
		{
			this.name = name;
			this.normalizedRect = new Rect(left, top, width, height);
			this.anchor = anchor;
			this.uniformMargins = true;
			this.horzMarginMax = 0.5f;
			this.vertMarginMax = 0.5f;
		}

		public void SetController(TouchController joy)
		{
			this.joy = joy;
		}

		public void ResetContent()
		{
			this.contentSize = Vector2.zero;
		}

		private void AddContentMinMax(Vector2 bbmin, Vector2 bbmax)
		{
			if (this.contentSize.x < 0.001f)
			{
				this.contentOfs = bbmin;
				this.contentSize = bbmax - bbmin;
			}
			else
			{
				Vector2 a = this.contentOfs + this.contentSize;
				this.contentOfs.x = Mathf.Min(bbmin.x, this.contentOfs.x);
				this.contentOfs.y = Mathf.Min(bbmin.y, this.contentOfs.y);
				a.x = Mathf.Max(bbmax.x, a.x);
				a.y = Mathf.Max(bbmax.y, a.y);
				this.contentSize = a - this.contentOfs;
			}
		}

		public void AddContent(Vector2 cen, float size)
		{
			size *= 0.5f;
			Vector2 b = new Vector2(size, size);
			this.AddContentMinMax(cen - b, cen + b);
		}

		public void AddContent(Vector2 cen, Vector2 size)
		{
			size *= 0.5f;
			this.AddContentMinMax(cen - size, cen + size);
		}

		public void ContentFinalize()
		{
			float screenX = this.joy.GetScreenX(this.normalizedRect.xMin);
			float screenX2 = this.joy.GetScreenX(this.normalizedRect.xMax);
			float screenY = this.joy.GetScreenY(this.normalizedRect.yMin);
			float screenY2 = this.joy.GetScreenY(this.normalizedRect.yMax);
			Vector2 a = new Vector2(screenX2 - screenX, screenY2 - screenY);
			float num = this.joy.CmToPixels(this.contentSize.x);
			float num2 = this.joy.CmToPixels(this.contentSize.y);
			if (num < 0.01f || num2 < 0.01f)
			{
				this.contentPosScale = Vector2.one;
				this.contentSizeScale = 1f;
			}
			else
			{
				float num3 = Mathf.Clamp01(a.x / num);
				float num4 = Mathf.Clamp01(a.y / num2);
				this.contentSizeScale = Mathf.Clamp01(Mathf.Min(num3, num4));
				if (this.allowNonuniformScale)
				{
					this.contentPosScale = new Vector2(Mathf.Clamp01(num3), Mathf.Clamp01(num4));
				}
				else
				{
					this.contentPosScale = new Vector2(this.contentSizeScale, this.contentSizeScale);
				}
			}
			Vector2 b = new Vector2(this.contentPosScale.x * num, this.contentPosScale.y * num2);
			this.contentPosScale *= this.joy.GetDPCM();
			this.contentSizeScale *= this.joy.GetDPCM();
			Vector2 topLeftOfs = a - b;
			float maxMarginX = this.horzMarginMax * this.joy.GetDPCM();
			float maxMarginY = this.vertMarginMax * this.joy.GetDPCM();
			this.screenDstOfs = new Vector2(screenX, screenY) + TouchController.AnchorLeftover(topLeftOfs, this.anchor, maxMarginX, maxMarginY, this.uniformMargins);
			this.contentScreenBox = new Rect(this.screenDstOfs.x, this.screenDstOfs.y, b.x, b.y);
			this.availableScreenBox = new Rect(screenX, screenY, a.x, a.y);
			if (!this.ignoreLeftHandedMode)
			{
				this.contentScreenBox = this.joy.RightHandToScreenRect(this.contentScreenBox);
				this.availableScreenBox = this.joy.RightHandToScreenRect(this.availableScreenBox);
			}
		}

		public Vector2 GetScreenPos(Vector2 pos)
		{
			pos -= this.contentOfs;
			pos.x *= this.contentPosScale.x;
			pos.y *= this.contentPosScale.y;
			pos += this.screenDstOfs;
			return (!this.ignoreLeftHandedMode) ? this.joy.RightHandToScreen(pos) : pos;
		}

		public float GetScreenSize(float size)
		{
			return size * this.contentSizeScale;
		}

		public Vector2 GetScreenSize(Vector2 size)
		{
			return size * this.contentSizeScale;
		}

		public string name;

		public TouchController.LayoutAnchor anchor;

		public bool allowNonuniformScale;

		public bool ignoreLeftHandedMode;

		public Rect normalizedRect;

		public float horzMarginMax;

		public float vertMarginMax;

		public bool uniformMargins;

		private TouchController joy;

		private Vector2 contentOfs;

		private Vector2 contentSize;

		private Vector2 contentPosScale;

		private float contentSizeScale;

		private Vector2 screenDstOfs;

		private Rect contentScreenBox;

		private Rect availableScreenBox;

		public Color debugColor = new Color(1f, 1f, 1f, 0.2f);

		public bool debugDraw;
	}

	public struct AnimFloat
	{
		public void Reset(float val)
		{
			this.cur = val;
			this.end = val;
			this.start = val;
		}

		public void MoveTo(float val)
		{
			this.start = this.cur;
			this.end = val;
		}

		public void Update(float lerpt)
		{
			this.cur = Mathf.Lerp(this.start, this.end, lerpt);
		}

		public float start;

		public float end;

		public float cur;
	}

	public struct AnimColor
	{
		public void Reset(Color val)
		{
			this.cur = val;
			this.end = val;
			this.start = val;
		}

		public void MoveTo(Color val)
		{
			this.start = this.cur;
			this.end = val;
		}

		public void Update(float lerpt)
		{
			this.cur = Color.Lerp(this.start, this.end, lerpt);
		}

		public Color start;

		public Color end;

		public Color cur;
	}
}
