using System;
using UnityEngine;

[Serializable]
public class TouchStick : TouchableControl
{
	public bool dynamicVisible
	{
		get
		{
			return this.animAlpha.cur > 0.01f;
		}
	}

	public bool Pressed()
	{
		return this.pressedCur;
	}

	public bool JustPressed()
	{
		return this.pressedCur && !this.pressedPrev;
	}

	public bool JustReleased()
	{
		return !this.pressedCur && this.pressedPrev;
	}

	public float GetTilt()
	{
		return this.tilt;
	}

	public float GetAngle()
	{
		return this.angle;
	}

	public Vector2 GetVec()
	{
		return this.posRaw;
	}

	public Vector2 GetNormalizedVec()
	{
		return this.dirVec;
	}

	public Vector2 GetVecEx(TouchStick.StickPosMode vis)
	{
		float dirCodeAngle = this.angle;
		float num = this.tilt;
		switch (vis)
		{
		case TouchStick.StickPosMode.FULL_ANALOG:
			return this.posRaw;
		case TouchStick.StickPosMode.ANALOG_8WAY:
			dirCodeAngle = TouchStick.GetDirCodeAngle(this.dir8way);
			num = ((this.dir8way == TouchStick.StickDir.NEUTRAL) ? 0f : num);
			break;
		case TouchStick.StickPosMode.ANALOG_4WAY:
			dirCodeAngle = TouchStick.GetDirCodeAngle(this.dir4way);
			num = ((this.dir4way == TouchStick.StickDir.NEUTRAL) ? 0f : num);
			break;
		case TouchStick.StickPosMode.DIGITAL_8WAY:
			dirCodeAngle = TouchStick.GetDirCodeAngle(this.dir8way);
			num = (float)((this.dir8way == TouchStick.StickDir.NEUTRAL) ? 0 : 1);
			break;
		case TouchStick.StickPosMode.DIGITAL_4WAY:
			dirCodeAngle = TouchStick.GetDirCodeAngle(this.dir4way);
			num = (float)((this.dir4way == TouchStick.StickDir.NEUTRAL) ? 0 : 1);
			break;
		}
		return TouchStick.RotateVec2(new Vector2(0f, 1f), dirCodeAngle) * num;
	}

	public Vector3 GetVec3d(bool normalized, float orientByAngle)
	{
		Vector2 pos = (!normalized) ? this.posRaw : this.dirVec;
		if (orientByAngle != 0f)
		{
			pos = TouchStick.RotateVec2(pos, orientByAngle);
		}
		return new Vector3(pos.x, 0f, pos.y);
	}

	public Vector3 GetVec3d(TouchStick.Vec3DMode vecMode, bool normalized, float orientByAngle)
	{
		Vector2 pos = (!normalized) ? this.posRaw : this.dirVec;
		if (orientByAngle != 0f)
		{
			pos = TouchStick.RotateVec2(pos, orientByAngle);
		}
		if (vecMode == TouchStick.Vec3DMode.XZ)
		{
			return new Vector3(pos.x, 0f, pos.y);
		}
		if (vecMode != TouchStick.Vec3DMode.XY)
		{
			return Vector3.zero;
		}
		return new Vector3(pos.x, pos.y, 0f);
	}

	public Vector3 GetVec3d(TouchStick.Vec3DMode vecMode, bool normalized)
	{
		return this.GetVec3d(vecMode, normalized, 0f);
	}

	public TouchStick.StickDir GetDigitalDir(bool eightWayMode)
	{
		return (!eightWayMode) ? this.dir4way : this.dir8way;
	}

	public TouchStick.StickDir GetDigitalDir()
	{
		return this.dir8way;
	}

	public TouchStick.StickDir GetFourWayDir()
	{
		return this.dir4way;
	}

	public TouchStick.StickDir GetPrevDigitalDir(bool eightWayMode = true)
	{
		return (!eightWayMode) ? this.dir4wayPrev : this.dir8wayPrev;
	}

	public TouchStick.StickDir GetPrevDigitalDir()
	{
		return this.dir8wayPrev;
	}

	public TouchStick.StickDir GetPrevFourWayDir()
	{
		return this.dir4wayPrev;
	}

	public bool DigitalJustChanged(bool eightWayMode)
	{
		return (!eightWayMode) ? (this.dir4way != this.dir4wayPrev) : (this.dir8way != this.dir8wayPrev);
	}

	public bool DigitalJustChanged()
	{
		return this.dir8way != this.dir8wayPrev;
	}

	public bool FourWayJustChanged()
	{
		return this.dir4way != this.dir4wayPrev;
	}

	private static bool KeyCodeInDir(KeyCode keyCode, TouchStick.StickDir dir)
	{
		if (dir == TouchStick.StickDir.NEUTRAL)
		{
			return false;
		}
		switch (keyCode)
		{
		case KeyCode.A:
			break;
		default:
			switch (keyCode)
			{
			case KeyCode.UpArrow:
				break;
			case KeyCode.DownArrow:
				goto IL_6A;
			case KeyCode.RightArrow:
				goto IL_96;
			case KeyCode.LeftArrow:
				goto IL_80;
			default:
				if (keyCode == KeyCode.S)
				{
					goto IL_6A;
				}
				if (keyCode != KeyCode.W)
				{
					return false;
				}
				break;
			}
			return dir == TouchStick.StickDir.U || dir == TouchStick.StickDir.UL || dir == TouchStick.StickDir.UR;
			IL_6A:
			return dir == TouchStick.StickDir.D || dir == TouchStick.StickDir.DL || dir == TouchStick.StickDir.DR;
		case KeyCode.D:
			goto IL_96;
		}
		IL_80:
		return dir == TouchStick.StickDir.L || dir == TouchStick.StickDir.DL || dir == TouchStick.StickDir.UL;
		IL_96:
		return dir == TouchStick.StickDir.R || dir == TouchStick.StickDir.DR || dir == TouchStick.StickDir.UR;
	}

	public float GetAxis(string name)
	{
		bool flag = false;
		return this.GetAxisEx(name, out flag);
	}

	public float GetAxisEx(string name, out bool supported)
	{
		if (!this.enableGetAxis)
		{
			supported = false;
			return 0f;
		}
		if (name == this.axisHorzName)
		{
			supported = true;
			return (!this.axisHorzFlip) ? this.posRaw.x : (-this.posRaw.x);
		}
		if (name == this.axisVertName)
		{
			supported = true;
			return (!this.axisVertFlip) ? this.posRaw.y : (-this.posRaw.y);
		}
		supported = false;
		return 0f;
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
		return (buttonSupported = (this.enableGetButton && buttonName == this.getButtonName)) && this.Pressed();
	}

	public bool GetButtonDownEx(string buttonName, out bool buttonSupported)
	{
		return (buttonSupported = (this.enableGetButton && buttonName == this.getButtonName)) && this.JustPressed();
	}

	public bool GetButtonUpEx(string buttonName, out bool buttonSupported)
	{
		return (buttonSupported = (this.enableGetButton && buttonName == this.getButtonName)) && this.JustReleased();
	}

	public bool GetKey(KeyCode key)
	{
		return this.enableGetKey && key != KeyCode.None && (((key == this.getKeyCodePress || key == this.getKeyCodePressAlt) && this.Pressed()) || (this.dir8way != TouchStick.StickDir.NEUTRAL && this.CheckKeyCode(key, this.dir8way)));
	}

	public bool GetKeyDown(KeyCode key)
	{
		return this.enableGetKey && key != KeyCode.None && (((key == this.getKeyCodePress || key == this.getKeyCodePressAlt) && this.JustPressed()) || (this.dir8way != this.dir8wayPrev && this.CheckKeyCode(key, this.dir8way) && !this.CheckKeyCode(key, this.dir8wayPrev)));
	}

	public bool GetKeyUp(KeyCode key)
	{
		return this.enableGetKey && key != KeyCode.None && (((key == this.getKeyCodePress || key == this.getKeyCodePressAlt) && this.JustReleased()) || (this.dir8way != this.dir8wayPrev && !this.CheckKeyCode(key, this.dir8way) && this.CheckKeyCode(key, this.dir8wayPrev)));
	}

	public bool GetKeyEx(KeyCode key, out bool keySupported)
	{
		keySupported = this.IsKeySupported(key);
		return this.GetKey(key);
	}

	public bool GetKeyDownEx(KeyCode key, out bool keySupported)
	{
		keySupported = this.IsKeySupported(key);
		return this.GetKeyDown(key);
	}

	public bool GetKeyUpEx(KeyCode key, out bool keySupported)
	{
		keySupported = this.IsKeySupported(key);
		return this.GetKeyUp(key);
	}

	public bool IsKeySupported(KeyCode key)
	{
		return !this.enableGetKey || key == this.getKeyCodePress || key == this.getKeyCodePressAlt || key == this.getKeyCodeUp || key == this.getKeyCodeUpAlt || key == this.getKeyCodeDown || key == this.getKeyCodeDownAlt || key == this.getKeyCodeLeft || key == this.getKeyCodeLeftAlt || key == this.getKeyCodeRight || key == this.getKeyCodeRightAlt;
	}

	private bool CheckKeyCode(KeyCode key, TouchStick.StickDir dir)
	{
		if (dir == TouchStick.StickDir.NEUTRAL)
		{
			return false;
		}
		if (key == this.getKeyCodeUp || key == this.getKeyCodeUpAlt)
		{
			return dir == TouchStick.StickDir.U || dir == TouchStick.StickDir.UL || dir == TouchStick.StickDir.UR;
		}
		if (key == this.getKeyCodeDown || key == this.getKeyCodeDownAlt)
		{
			return dir == TouchStick.StickDir.D || dir == TouchStick.StickDir.DL || dir == TouchStick.StickDir.DR;
		}
		if (key == this.getKeyCodeLeft || key == this.getKeyCodeLeftAlt)
		{
			return dir == TouchStick.StickDir.L || dir == TouchStick.StickDir.DL || dir == TouchStick.StickDir.UL;
		}
		return (key == this.getKeyCodeRight || key == this.getKeyCodeRightAlt) && (dir == TouchStick.StickDir.R || dir == TouchStick.StickDir.DR || dir == TouchStick.StickDir.UR);
	}

	public void SetDynamicMode(bool dynamicMode)
	{
		if (this.dynamicMode != dynamicMode)
		{
			this.dynamicMode = dynamicMode;
			this.joy.SetLayoutDirtyFlag();
		}
	}

	public override void Enable(bool skipAnimation)
	{
		this.enabled = true;
		this.AnimateParams((!this.overrideScale) ? this.joy.releasedStickHatScale : this.releasedHatScale, (!this.overrideScale) ? this.joy.releasedStickBaseScale : this.releasedBaseScale, (!this.overrideColors) ? this.joy.defaultReleasedStickHatColor : this.releasedHatColor, (!this.overrideColors) ? this.joy.defaultReleasedStickBaseColor : this.releasedBaseColor, (float)((!this.dynamicMode) ? 1 : 0), (!skipAnimation) ? ((!this.overrideAnimDuration) ? this.joy.enableAnimDuration : this.enableAnimDuration) : 0f);
	}

	public override void Disable(bool skipAnimation)
	{
		this.enabled = false;
		this.ReleaseTouches();
		this.AnimateParams((!this.overrideScale) ? this.joy.disabledStickHatScale : this.disabledHatScale, (!this.overrideScale) ? this.joy.disabledStickBaseScale : this.disabledBaseScale, (!this.overrideColors) ? this.joy.defaultDisabledStickHatColor : this.disabledHatColor, (!this.overrideColors) ? this.joy.defaultDisabledStickBaseColor : this.disabledBaseColor, (float)((!this.dynamicMode) ? 1 : 0), (!skipAnimation) ? ((!this.overrideAnimDuration) ? this.joy.disableAnimDuration : this.disableAnimDuration) : 0f);
	}

	public override void Show(bool skipAnim)
	{
		this.visible = true;
		this.AnimateParams((!this.overrideScale) ? ((!this.enabled) ? this.joy.disabledStickHatScale : this.joy.releasedStickHatScale) : ((!this.enabled) ? this.disabledHatScale : this.releasedHatScale), (!this.overrideScale) ? ((!this.enabled) ? this.joy.disabledStickBaseScale : this.joy.releasedStickBaseScale) : ((!this.enabled) ? this.disabledBaseScale : this.releasedBaseScale), (!this.overrideColors) ? ((!this.enabled) ? this.joy.defaultDisabledStickHatColor : this.joy.defaultReleasedStickHatColor) : ((!this.enabled) ? this.disabledHatColor : this.releasedHatColor), (!this.overrideColors) ? ((!this.enabled) ? this.joy.defaultDisabledStickBaseColor : this.joy.defaultReleasedStickBaseColor) : ((!this.enabled) ? this.disabledBaseColor : this.releasedBaseColor), (float)((!this.dynamicMode) ? 1 : ((!this.Pressed()) ? 0 : 1)), (!skipAnim) ? ((!this.overrideAnimDuration) ? this.joy.showAnimDuration : this.showAnimDuration) : 0f);
	}

	public override void Hide(bool skipAnim)
	{
		this.visible = false;
		this.ReleaseTouches();
		Color end = this.animHatColor.end;
		Color end2 = this.animBaseColor.end;
		this.AnimateParams(this.animHatScale.end, this.animBaseScale.end, end, end2, 0f, (!skipAnim) ? ((!this.overrideAnimDuration) ? this.joy.hideAnimDuration : this.hideAnimDuration) : 0f);
	}

	public void SetRect(Rect r)
	{
		Vector2 center = r.center;
		float num = Mathf.Min(r.width, r.height) / 2f;
		if (!this.dynamicMode && (this.posPx != center || this.radPx != num))
		{
			this.posPx = center;
			this.radPx = num;
			this.OnReset();
		}
	}

	public override void ResetRect()
	{
		if (!this.dynamicMode)
		{
			this.posPx = this.layoutPosPx;
			this.radPx = this.layoutRadPx;
		}
	}

	public Rect GetRect(bool getAutoRect)
	{
		return TouchController.GetCenRect((!getAutoRect) ? this.posPx : this.layoutPosPx, ((!getAutoRect) ? this.radPx : this.layoutRadPx) * 2f);
	}

	public Rect GetRect()
	{
		return this.GetRect(false);
	}

	public Vector2 GetScreenPos()
	{
		return this.posPx;
	}

	public float GetScreenRad()
	{
		return this.radPx;
	}

	public Rect GetHatDisplayRect(bool applyScale)
	{
		return TouchController.GetCenRect(this.posPx + TouchStick.InternalToScreenPos(this.displayPos) * this.radPx * this.hatMoveScale, 2f * this.radPx * ((!applyScale) ? 1f : this.animHatScale.cur));
	}

	public Rect GetHatDisplayRect()
	{
		return this.GetHatDisplayRect(true);
	}

	public Rect GetBaseDisplayRect(bool applyScale = true)
	{
		return TouchController.GetCenRect(this.posPx, this.radPx * 2f * ((!applyScale) ? 1f : this.animBaseScale.cur));
	}

	public Rect GetBaseDisplayRect()
	{
		return this.GetBaseDisplayRect(true);
	}

	public Color GetHatColor()
	{
		return this.animHatColor.cur;
	}

	public Color GetBaseColor()
	{
		return this.animBaseColor.cur;
	}

	public int GetGUIDepth()
	{
		return this.joy.guiDepth + this.guiDepth + ((!this.Pressed()) ? 0 : this.joy.guiPressedOfs);
	}

	public Texture2D GetBaseDisplayTex()
	{
		return (!this.enabled || !this.Pressed()) ? this.releasedBaseImg : this.pressedBaseImg;
	}

	public Texture2D GetHatDisplayTex()
	{
		return (!this.enabled || !this.Pressed()) ? this.releasedHatImg : this.pressedHatImg;
	}

	public static bool IsDiagonalAxis(TouchStick.StickDir dir)
	{
		return (dir - TouchStick.StickDir.U & 1) == 1;
	}

	public static float GetDirCodeAngle(TouchStick.StickDir d)
	{
		if (d < TouchStick.StickDir.U || d > TouchStick.StickDir.UL)
		{
			return 0f;
		}
		return (float)(d - TouchStick.StickDir.U) * 45f;
	}

	public static TouchStick.StickDir GetDirCodeFromAngle(float ang, bool as8way)
	{
		ang += ((!as8way) ? 45f : 22.5f);
		ang = TouchStick.NormalizeAnglePositive(ang);
		if (as8way)
		{
			if (ang < 45f)
			{
				return TouchStick.StickDir.U;
			}
			if (ang < 90f)
			{
				return TouchStick.StickDir.UR;
			}
			if (ang < 135f)
			{
				return TouchStick.StickDir.R;
			}
			if (ang < 180f)
			{
				return TouchStick.StickDir.DR;
			}
			if (ang < 225f)
			{
				return TouchStick.StickDir.D;
			}
			if (ang < 270f)
			{
				return TouchStick.StickDir.DL;
			}
			if (ang < 315f)
			{
				return TouchStick.StickDir.L;
			}
			return TouchStick.StickDir.UL;
		}
		else
		{
			if (ang < 90f)
			{
				return TouchStick.StickDir.U;
			}
			if (ang < 180f)
			{
				return TouchStick.StickDir.R;
			}
			if (ang < 270f)
			{
				return TouchStick.StickDir.D;
			}
			return TouchStick.StickDir.L;
		}
	}

	private void AnimateParams(float hatScale, float baseScale, Color hatColor, Color baseColor, float alpha, float duration)
	{
		if (duration <= 0f)
		{
			this.animTimer.Reset(0f);
			this.animTimer.Disable();
			this.animHatColor.Reset(hatColor);
			this.animHatScale.Reset(hatScale);
			this.animBaseColor.Reset(baseColor);
			this.animBaseScale.Reset(baseScale);
			this.animAlpha.Reset(alpha);
			this.displayPosStart = (this.displayPos = ((!this.Pressed()) ? Vector2.zero : this.GetVecEx(this.stickVis)));
		}
		else
		{
			this.animTimer.Start(duration);
			this.animHatScale.MoveTo(hatScale);
			this.animHatColor.MoveTo(hatColor);
			this.animBaseScale.MoveTo(baseScale);
			this.animBaseColor.MoveTo(baseColor);
			this.animAlpha.MoveTo(alpha);
		}
	}

	public override void Init(TouchController joy)
	{
		base.Init(joy);
		this.OnReset();
		if (this.initiallyDisabled)
		{
			this.Disable(true);
		}
		if (this.initiallyHidden)
		{
			this.Hide(true);
		}
	}

	public override void OnReset()
	{
		this.pressedCur = false;
		this.pressedPrev = false;
		this.touchId = -1;
		this.dir4way = TouchStick.StickDir.NEUTRAL;
		this.dir8way = TouchStick.StickDir.NEUTRAL;
		this.dir4wayLastNonNeutral = TouchStick.StickDir.NEUTRAL;
		this.dir8wayLastNonNeutral = TouchStick.StickDir.NEUTRAL;
		this.dir4wayPrev = TouchStick.StickDir.NEUTRAL;
		this.dir8wayPrev = TouchStick.StickDir.NEUTRAL;
		this.touchCanceled = false;
		this.SetInternalPos(Vector2.zero);
		this.tilt = 0f;
		this.dirVec = Vector2.zero;
		this.posRaw = Vector2.zero;
		this.displayPos = Vector2.zero;
		this.displayPosStart = Vector2.zero;
		this.AnimateParams((!this.overrideScale) ? this.joy.releasedStickHatScale : this.releasedHatScale, (!this.overrideScale) ? this.joy.releasedStickBaseScale : this.releasedBaseScale, (!this.overrideColors) ? this.joy.defaultReleasedStickHatColor : this.releasedHatColor, (!this.overrideColors) ? this.joy.defaultReleasedStickBaseColor : this.releasedBaseColor, (float)((!this.dynamicMode) ? 1 : 0), 0f);
		if (!this.enabled)
		{
			this.Disable(true);
		}
		if (!this.visible)
		{
			this.Hide(true);
		}
	}

	public static Vector2 InternalToScreenPos(Vector2 internalStickPos)
	{
		internalStickPos.y = -internalStickPos.y;
		return internalStickPos;
	}

	private void SetPollPos(Vector2 pos, bool screenPos)
	{
		if (screenPos)
		{
			pos = (pos - this.posPx) / this.radPx;
			pos.y = -pos.y;
		}
		this.pollPos = pos;
	}

	private void SetInternalPos(Vector2 pos)
	{
		this.pollPos = pos;
		if (this.disableX)
		{
			pos.x = 0f;
		}
		if (this.disableY)
		{
			pos.y = 0f;
		}
		float num = Mathf.Clamp01(pos.magnitude);
		Vector2 normalized = this.dirVec;
		float num2 = this.safeAngle;
		if (num > 0.01f)
		{
			normalized = pos.normalized;
			num2 = Mathf.Atan2(normalized.x, normalized.y) * 57.29578f;
		}
		if (num > ((this.dir8way != TouchStick.StickDir.NEUTRAL) ? this.joy.stickDigitalLeaveThresh : this.joy.stickDigitalEnterThresh))
		{
			if (this.dir8wayLastNonNeutral == TouchStick.StickDir.NEUTRAL)
			{
				this.dir4way = TouchStick.GetDirCodeFromAngle(num2, false);
				this.dir8way = TouchStick.GetDirCodeFromAngle(num2, true);
			}
			else if (num > this.joy.stickDigitalEnterThresh)
			{
				float dirCodeAngle = TouchStick.GetDirCodeAngle(this.dir8wayLastNonNeutral);
				if (Mathf.Abs(Mathf.DeltaAngle(dirCodeAngle, num2)) > 22.5f + this.joy.stickMagnetAngleMargin)
				{
					this.dir8way = TouchStick.GetDirCodeFromAngle(num2, true);
				}
				else
				{
					this.dir8way = this.dir8wayLastNonNeutral;
				}
				float dirCodeAngle2 = TouchStick.GetDirCodeAngle(this.dir4wayLastNonNeutral);
				if (Mathf.Abs(Mathf.DeltaAngle(dirCodeAngle2, num2)) > 45f + this.joy.stickMagnetAngleMargin)
				{
					this.dir4way = TouchStick.GetDirCodeFromAngle(num2, false);
				}
				else
				{
					this.dir4way = this.dir4wayLastNonNeutral;
				}
			}
		}
		else
		{
			this.dir4way = TouchStick.StickDir.NEUTRAL;
			this.dir8way = TouchStick.StickDir.NEUTRAL;
		}
		if (this.dir4way != TouchStick.StickDir.NEUTRAL)
		{
			this.dir4wayLastNonNeutral = this.dir4way;
		}
		if (this.dir8way != TouchStick.StickDir.NEUTRAL)
		{
			this.dir8wayLastNonNeutral = this.dir8way;
		}
		this.tilt = num;
		this.angle = num2;
		this.safeAngle = num2;
		this.posRaw = normalized * num;
		this.dirVec = normalized;
	}

	public override void OnPrePoll()
	{
		this.touchVerified = false;
	}

	public override void OnPostPoll()
	{
		if (!this.touchVerified && this.touchId >= 0)
		{
			this.OnTouchEnd(this.touchId, false);
		}
	}

	public override void ReleaseTouches()
	{
		if (this.touchId >= 0)
		{
			this.OnTouchEnd(this.touchId, true);
		}
	}

	public override void TakeoverTouches(TouchableControl controlToUntouch)
	{
		if (controlToUntouch != null && this.touchId >= 0)
		{
			controlToUntouch.OnTouchEnd(this.touchId, true);
		}
	}

	public override void OnUpdate(bool firstUpdate)
	{
		this.dir8wayPrev = this.dir8way;
		this.dir4wayPrev = this.dir4way;
		this.pressedPrev = this.pressedCur;
		this.pressedCur = (this.touchId >= 0);
		this.SetInternalPos(this.pollPos);
		if (this.pressedCur)
		{
			this.displayPos = (this.displayPosStart = this.GetVecEx(this.stickVis));
		}
		else if (!this.smoothReturn)
		{
			this.displayPos = (this.displayPosStart = Vector2.zero);
		}
		if (this.pressedCur != this.pressedPrev && this.enabled)
		{
			if (this.pressedCur)
			{
				this.dynamicFadeOutAnimPending = false;
				this.AnimateParams((!this.overrideScale) ? this.joy.pressedStickHatScale : this.pressedHatScale, (!this.overrideScale) ? this.joy.pressedStickBaseScale : this.pressedBaseScale, (!this.overrideColors) ? this.joy.defaultPressedStickHatColor : this.pressedHatColor, (!this.overrideColors) ? this.joy.defaultPressedStickBaseColor : this.pressedBaseColor, 1f, (!this.overrideAnimDuration) ? this.joy.pressAnimDuration : this.pressAnimDuration);
			}
			else
			{
				this.dynamicFadeOutAnimPending = (this.dynamicMode && !this.touchCanceled);
				this.AnimateParams((!this.overrideScale) ? this.joy.releasedStickHatScale : this.releasedHatScale, (!this.overrideScale) ? this.joy.releasedStickBaseScale : this.releasedBaseScale, (!this.overrideColors) ? this.joy.defaultReleasedStickHatColor : this.releasedHatColor, (!this.overrideColors) ? this.joy.defaultReleasedStickBaseColor : this.releasedBaseColor, (!this.dynamicMode) ? 1f : ((!this.touchCanceled) ? this.animAlpha.cur : 0f), (!this.touchCanceled) ? ((!this.overrideAnimDuration) ? this.joy.releaseAnimDuration : this.releaseAnimDuration) : this.joy.cancelAnimDuration);
			}
		}
		if (this.animTimer.Enabled)
		{
			this.animTimer.Update(this.joy.deltaTime);
			float num = TouchController.SlowDownEase(this.animTimer.Nt);
			this.animAlpha.Update(num);
			this.animHatColor.Update(num);
			this.animHatScale.Update(num);
			this.animBaseColor.Update(num);
			this.animBaseScale.Update(num);
			if (this.smoothReturn && !this.Pressed())
			{
				this.displayPos = Vector2.Lerp(this.displayPosStart, Vector2.zero, num);
			}
			if (this.animTimer.Completed)
			{
				this.displayPosStart = this.displayPos;
				if (this.dynamicMode && this.dynamicFadeOutAnimPending)
				{
					this.dynamicFadeOutAnimPending = false;
					this.AnimateParams((!this.overrideScale) ? this.joy.releasedStickHatScale : this.releasedHatScale, (!this.overrideScale) ? this.joy.releasedStickBaseScale : this.releasedBaseScale, (!this.overrideColors) ? this.joy.defaultReleasedStickHatColor : this.releasedHatColor, (!this.overrideColors) ? this.joy.defaultReleasedStickBaseColor : this.releasedBaseColor, 0f, this.dynamicFadeOutDuration);
				}
				else
				{
					this.animTimer.Disable();
				}
			}
		}
	}

	public override TouchController.HitTestResult HitTest(Vector2 pos, int touchId)
	{
		if (this.touchId >= 0 || !this.enabled || !this.visible)
		{
			return new TouchController.HitTestResult(false);
		}
		TouchController.HitTestResult result;
		if (this.dynamicMode)
		{
			if (!this.dynamicAlwaysReset && this.dynamicVisible)
			{
				TouchController.HitTestResult hitTestResult;
				result = (hitTestResult = this.joy.HitTestCircle(this.posPx, this.radPx, pos, true));
				if (hitTestResult.hit)
				{
					result.prio = this.prio;
					this.dynamicResetPos = false;
					return result;
				}
			}
			this.dynamicResetPos = true;
			result = this.joy.HitTestRect(this.dynamicRegionPx, pos, true);
			result.prio = this.dynamicRegionPrio;
			result.distScale = this.hitDistScale;
			return result;
		}
		result = this.joy.HitTestCircle(this.posPx, this.radPx, pos, true);
		result.prio = this.prio;
		result.distScale = this.hitDistScale;
		return result;
	}

	public override TouchController.EventResult OnTouchStart(int touchId, Vector2 touchPos)
	{
		if (this.dynamicMode && this.dynamicResetPos)
		{
			float num = Mathf.Min(this.joy.GetScreenHeight(), this.joy.GetScreenWidth());
			if (this.dynamicClamp)
			{
				float num2 = this.radPx + this.dynamicMarginCm * this.joy.GetDPCM();
				float num3 = (num - this.radPx * 2f) / 2f;
				num2 = ((num3 > 0f) ? Mathf.Clamp(num2, 0f, num3) : 0f);
				this.posPx.x = Mathf.Clamp(touchPos.x, this.joy.GetScreenX(0f) + num2, this.joy.GetScreenX(1f) - num2);
				this.posPx.y = Mathf.Clamp(touchPos.y, this.joy.GetScreenY(0f) + num2, this.joy.GetScreenY(1f) - num2);
			}
			else
			{
				this.posPx = touchPos;
			}
		}
		this.touchCanceled = false;
		this.touchId = touchId;
		this.touchVerified = true;
		this.SetPollPos(touchPos, true);
		return TouchController.EventResult.HANDLED;
	}

	public override TouchController.EventResult OnTouchEnd(int touchId, bool cancelMode = false)
	{
		if (this.touchId != touchId)
		{
			return TouchController.EventResult.NOT_HANDLED;
		}
		if (this.dynamicMode)
		{
		}
		this.touchId = -1;
		this.touchVerified = true;
		this.touchCanceled = cancelMode;
		this.SetPollPos(Vector2.zero, false);
		return TouchController.EventResult.HANDLED;
	}

	public override TouchController.EventResult OnTouchMove(int touchId, Vector2 touchPos)
	{
		if (this.touchId != touchId)
		{
			return TouchController.EventResult.NOT_HANDLED;
		}
		this.touchVerified = true;
		this.SetPollPos(touchPos, true);
		return TouchController.EventResult.HANDLED;
	}

	public override void OnLayoutAddContent()
	{
		if (this.dynamicMode)
		{
			return;
		}
		this.joy.layoutBoxes[this.layoutBoxId].AddContent(this.posCm, this.sizeCm);
	}

	public override void OnLayout()
	{
		this.dynamicRegionPx = this.joy.NormalizedRectToPx(this.dynamicRegion, true);
		if (this.dynamicMode)
		{
			this.radPx = this.CalculateDynamicRad();
		}
		else
		{
			this.layoutPosPx = this.joy.layoutBoxes[this.layoutBoxId].GetScreenPos(this.posCm);
			this.layoutRadPx = this.joy.layoutBoxes[this.layoutBoxId].GetScreenSize(this.sizeCm / 2f);
			this.posPx = this.layoutPosPx;
			this.radPx = this.layoutRadPx;
		}
		this.OnReset();
	}

	public override void DrawGUI()
	{
		if (this.disableGui || this.joy.GetAlpha() * this.animAlpha.cur < 0.001f)
		{
			return;
		}
		GUI.color = Color.white;
		bool flag = this.Pressed();
		Color cur = this.animHatColor.cur;
		Color cur2 = this.animBaseColor.cur;
		Texture2D texture2D = (!flag) ? this.releasedHatImg : this.pressedHatImg;
		Texture2D texture2D2 = (!flag) ? this.releasedBaseImg : this.pressedBaseImg;
		GUI.depth = this.joy.guiDepth + this.guiDepth + ((!this.Pressed()) ? 0 : this.joy.guiPressedOfs);
		if (texture2D2 != null)
		{
			GUI.color = TouchController.ScaleAlpha(cur2, this.joy.GetAlpha() * this.animAlpha.cur);
			GUI.DrawTexture(this.GetBaseDisplayRect(true), texture2D2);
		}
		if (texture2D != null)
		{
			GUI.color = TouchController.ScaleAlpha(cur, this.joy.GetAlpha() * this.animAlpha.cur);
			GUI.DrawTexture(this.GetHatDisplayRect(true), texture2D);
		}
	}

	private float CalculateDynamicRad()
	{
		float num = Mathf.Min(this.joy.GetScreenHeight(), this.joy.GetScreenWidth());
		return Mathf.Max(4f, 0.5f * Mathf.Min(this.sizeCm * this.joy.GetDPCM(), num * Mathf.Clamp(this.dynamicMaxRelativeSize, 0.01f, 1f)));
	}

	private static Vector2 RotateVec2(Vector2 pos, float ang)
	{
		float num = Mathf.Sin(-ang * 0.0174532924f);
		float num2 = Mathf.Cos(-ang * 0.0174532924f);
		return new Vector2(pos.x * num2 - pos.y * num, pos.x * num + pos.y * num2);
	}

	private static float NormalizeAnglePositive(float a)
	{
		if (a >= 360f)
		{
			return Mathf.Repeat(a, 360f);
		}
		if (a >= 0f)
		{
			return a;
		}
		if (a <= -360f)
		{
			a = Mathf.Repeat(a, 360f);
		}
		return 360f + a;
	}

	private const TouchStick.StickDir StickDirFirst = TouchStick.StickDir.U;

	private const TouchStick.StickDir StickDirLast = TouchStick.StickDir.UL;

	private float safeAngle;

	public TouchStick.StickPosMode stickVis;

	public bool smoothReturn = true;

	public Vector2 posCm = new Vector2(2f, 5f);

	public float sizeCm = 2f;

	private Vector2 layoutPosPx;

	private float layoutRadPx;

	private Vector2 posPx = new Vector2(100f, 100f);

	private float radPx = 40f;

	public bool overrideAnimDuration;

	public float pressAnimDuration;

	public float releaseAnimDuration;

	public float disableAnimDuration;

	public float enableAnimDuration;

	public float hideAnimDuration;

	public float showAnimDuration;

	private AnimTimer animTimer;

	private TouchController.AnimFloat animHatScale;

	private TouchController.AnimFloat animBaseScale;

	private TouchController.AnimFloat animAlpha;

	private TouchController.AnimColor animHatColor;

	private TouchController.AnimColor animBaseColor;

	private bool dynamicFadeOutAnimPending;

	public bool keyboardEmu;

	public KeyCode keyUp = KeyCode.W;

	public KeyCode keyDown = KeyCode.S;

	public KeyCode keyLeft = KeyCode.A;

	public KeyCode keyRight = KeyCode.D;

	public bool dynamicMode;

	public int dynamicRegionPrio;

	public bool dynamicClamp;

	public float dynamicMaxRelativeSize = 0.2f;

	public float dynamicMarginCm = 0.5f;

	public float dynamicFadeOutDelay;

	public float dynamicFadeOutDuration = 2f;

	public Rect dynamicRegion = new Rect(0f, 0f, 0.5f, 1f);

	private Rect dynamicRegionPx = new Rect(0f, 0f, 1f, 1f);

	public bool dynamicAlwaysReset;

	private bool dynamicResetPos;

	public float hatMoveScale = 0.5f;

	public bool disableX;

	public bool disableY;

	private Vector2 touchStart;

	private int touchId;

	private bool touchVerified;

	private Vector2 pollPos;

	private float angle;

	private Vector2 posRaw;

	private Vector2 dirVec;

	private float tilt;

	private Vector2 displayPosStart;

	private Vector2 displayPos;

	private TouchStick.StickDir dir8way;

	private TouchStick.StickDir dir4way;

	private TouchStick.StickDir dir8wayPrev;

	private TouchStick.StickDir dir4wayPrev;

	private TouchStick.StickDir dir8wayLastNonNeutral;

	private TouchStick.StickDir dir4wayLastNonNeutral;

	private bool pressedCur;

	private bool pressedPrev;

	public Texture2D releasedHatImg;

	public Texture2D releasedBaseImg;

	public Texture2D pressedHatImg;

	public Texture2D pressedBaseImg;

	public bool overrideScale;

	public float releasedHatScale = 1f;

	public float pressedHatScale = 1f;

	public float disabledHatScale = 1f;

	public float releasedBaseScale = 1f;

	public float pressedBaseScale = 1f;

	public float disabledBaseScale = 1f;

	public bool overrideColors;

	public Color releasedHatColor;

	public Color releasedBaseColor;

	public Color pressedHatColor;

	public Color pressedBaseColor;

	public Color disabledHatColor;

	public Color disabledBaseColor;

	private bool touchCanceled;

	public bool enableGetKey;

	public KeyCode getKeyCodePress;

	public KeyCode getKeyCodePressAlt;

	public KeyCode getKeyCodeUp;

	public KeyCode getKeyCodeUpAlt;

	public KeyCode getKeyCodeDown;

	public KeyCode getKeyCodeDownAlt;

	public KeyCode getKeyCodeLeft;

	public KeyCode getKeyCodeLeftAlt;

	public KeyCode getKeyCodeRight;

	public KeyCode getKeyCodeRightAlt;

	public bool enableGetButton;

	public string getButtonName;

	public bool enableGetAxis;

	public string axisHorzName;

	public string axisVertName;

	public bool axisHorzFlip;

	public bool axisVertFlip;

	public bool codeCustomGUI;

	public bool codeCustomLayout;

	public enum StickPosMode
	{
		FULL_ANALOG,
		ANALOG_8WAY,
		ANALOG_4WAY,
		DIGITAL_8WAY,
		DIGITAL_4WAY
	}

	public enum StickDir
	{
		NEUTRAL,
		U,
		UR,
		R,
		DR,
		D,
		DL,
		L,
		UL
	}

	public enum Vec3DMode
	{
		XZ,
		XY
	}
}
