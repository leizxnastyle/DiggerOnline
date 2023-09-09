using System;
using UnityEngine;

[Serializable]
public class TouchZone : TouchableControl
{
	private TouchZone.Finger GetFinger(int i)
	{
		return (i != 0) ? ((i != 1) ? null : this.fingerB) : this.fingerA;
	}

	public bool Pressed(int fingerId, bool trueOnMidFramePress, bool falseOnMidFrameRelease)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return finger.Pressed(trueOnMidFramePress, falseOnMidFrameRelease);
	}

	public bool Pressed(int fingerId)
	{
		return this.Pressed(fingerId, false, false);
	}

	public bool UniPressed(bool trueOnMidFramePress, bool falseOnMidFrameRelease)
	{
		return (this.uniCur || (trueOnMidFramePress && this.uniMidFramePressed)) && (!falseOnMidFrameRelease || !this.uniMidFrameReleased);
	}

	public bool UniPressed()
	{
		return this.UniPressed(false, false);
	}

	public bool MultiPressed(bool trueOnMidFramePress, bool falseOnMidFrameRelease)
	{
		return (this.multiCur || (trueOnMidFramePress && this.multiMidFramePressed)) && (!falseOnMidFrameRelease || !this.multiMidFrameReleased);
	}

	public bool MultiPressed()
	{
		return this.MultiPressed(false, false);
	}

	public bool JustPressed(int fingerId, bool trueOnMidFramePress, bool trueOnMidFrameRelease)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return finger.JustPressed(trueOnMidFramePress, trueOnMidFrameRelease);
	}

	public bool JustPressed(int fingerId)
	{
		return this.JustPressed(fingerId, false, false);
	}

	public bool JustUniPressed(bool trueOnMidFramePress, bool trueOnMidFrameRelease)
	{
		return (!this.uniPrev && this.uniCur) || (trueOnMidFramePress && this.uniMidFramePressed) || (trueOnMidFrameRelease && this.uniMidFrameReleased);
	}

	public bool JustUniPressed()
	{
		return this.JustUniPressed(false, false);
	}

	public bool JustMultiPressed(bool trueOnMidFramePress, bool trueOnMidFrameRelease)
	{
		return (!this.multiPrev && this.multiCur) || (trueOnMidFramePress && this.multiMidFramePressed) || (trueOnMidFrameRelease && this.multiMidFrameReleased);
	}

	public bool JustMultiPressed()
	{
		return this.JustMultiPressed(false, false);
	}

	public bool JustReleased(int fingerId, bool trueOnMidFramePress, bool trueOnMidFrameRelease)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return finger.JustPressed(trueOnMidFramePress, trueOnMidFrameRelease);
	}

	public bool JustReleased(int fingerId)
	{
		return this.JustReleased(fingerId, false, false);
	}

	public bool JustUniReleased(bool trueOnMidFramePress, bool trueOnMidFrameRelease)
	{
		return (this.uniPrev && !this.uniCur) || (trueOnMidFramePress && this.uniMidFramePressed) || (trueOnMidFrameRelease && this.uniMidFrameReleased);
	}

	public bool JustUniReleased()
	{
		return this.JustUniReleased(false, false);
	}

	public bool JustMultiReleased(bool trueOnMidFramePress, bool trueOnMidFrameRelease)
	{
		return (this.multiPrev && !this.multiCur) || (trueOnMidFramePress && this.multiMidFramePressed) || (trueOnMidFrameRelease && this.multiMidFrameReleased);
	}

	public bool JustMultiReleased()
	{
		return this.JustMultiReleased(false, false);
	}

	public bool JustMidFramePressed(int fingerId)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return finger.midFramePressed;
	}

	public bool JustMidFrameReleased(int fingerId)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return finger.midFrameReleased;
	}

	public bool JustMidFrameUniPressed()
	{
		return this.uniMidFramePressed;
	}

	public bool JustMidFrameUniReleased()
	{
		return this.uniMidFrameReleased;
	}

	public bool JustMidFrameMultiPressed()
	{
		return this.multiMidFramePressed;
	}

	public bool JustMidFrameMultiReleased()
	{
		return this.multiMidFrameReleased;
	}

	public float GetTouchDuration(int fingerId)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return (!finger.curState) ? 0f : (this.joy.curTime - finger.startTime);
	}

	public float GetUniTouchDuration()
	{
		return (!this.uniCur) ? 0f : (this.joy.curTime - this.uniStartTime);
	}

	public float GetMultiTouchDuration()
	{
		return (!this.multiCur) ? 0f : (this.joy.curTime - this.multiStartTime);
	}

	public Vector2 GetPos(int fingerId, TouchCoordSys cs)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return this.TransformPos(finger.posCur, cs, false);
	}

	public Vector2 GetPos(int fingerId)
	{
		return this.GetPos(fingerId, TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetUniPos(TouchCoordSys cs)
	{
		return this.TransformPos(this.uniPosCur, cs, false);
	}

	public Vector2 GetUniPos()
	{
		return this.GetUniPos(TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetMultiPos(TouchCoordSys cs)
	{
		return this.TransformPos(this.multiPosCur, cs, false);
	}

	public Vector2 GetMultiPos()
	{
		return this.GetMultiPos(TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetStartPos(int fingerId, TouchCoordSys cs)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return this.TransformPos(finger.startPos, cs, false);
	}

	public Vector2 GetStartPos(int fingerId)
	{
		return this.GetStartPos(fingerId, TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetUniStartPos(TouchCoordSys cs)
	{
		return this.TransformPos(this.uniPosStart, cs, false);
	}

	public Vector2 GetUniStartPos()
	{
		return this.GetUniStartPos(TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetMultiStartPos(TouchCoordSys cs)
	{
		return this.TransformPos(this.multiPosStart, cs, false);
	}

	public Vector2 GetMultiStartPos()
	{
		return this.GetMultiStartPos(TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetDragVec(int fingerId, TouchCoordSys cs, bool raw)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		if (!raw && !finger.moved)
		{
			return Vector2.zero;
		}
		return this.TransformPos(finger.posCur - finger.startPos, cs, true);
	}

	public Vector2 GetDragVec(int fingerId)
	{
		return this.GetDragVec(fingerId, TouchCoordSys.SCREEN_PX, false);
	}

	public Vector2 GetDragVec(int fingerId, TouchCoordSys cs)
	{
		return this.GetDragVec(fingerId, cs, false);
	}

	public Vector2 GetDragVec(int fingerId, bool raw)
	{
		return this.GetDragVec(fingerId, TouchCoordSys.SCREEN_PX, raw);
	}

	public Vector2 GetDragVecRaw(int fingerId)
	{
		return this.GetDragVec(fingerId, TouchCoordSys.SCREEN_PX, true);
	}

	public Vector2 GetDragVecRaw(int fingerId, TouchCoordSys cs)
	{
		return this.GetDragVec(fingerId, cs, true);
	}

	public Vector2 GetUniDragVec(TouchCoordSys cs, bool raw)
	{
		if (!raw && !this.uniMoved)
		{
			return Vector2.zero;
		}
		return this.TransformPos(this.uniTotalDragCur, cs, true);
	}

	public Vector2 GetUniDragVec()
	{
		return this.GetUniDragVec(TouchCoordSys.SCREEN_PX, false);
	}

	public Vector2 GetUniDragVec(TouchCoordSys cs)
	{
		return this.GetUniDragVec(cs, false);
	}

	public Vector2 GetUniDragVec(bool raw)
	{
		return this.GetUniDragVec(TouchCoordSys.SCREEN_PX, raw);
	}

	public Vector2 GetUniDragVecRaw()
	{
		return this.GetUniDragVec(TouchCoordSys.SCREEN_PX, true);
	}

	public Vector2 GetUniDragVecRaw(TouchCoordSys cs)
	{
		return this.GetUniDragVec(cs, true);
	}

	public Vector2 GetMultiDragVec(TouchCoordSys cs, bool raw)
	{
		if (!raw && !this.uniMoved)
		{
			return Vector2.zero;
		}
		return this.TransformPos(this.multiPosCur - this.multiPosStart, cs, true);
	}

	public Vector2 GetMultiDragVec()
	{
		return this.GetMultiDragVec(TouchCoordSys.SCREEN_PX, false);
	}

	public Vector2 GetMultiDragVec(TouchCoordSys cs)
	{
		return this.GetMultiDragVec(cs, false);
	}

	public Vector2 GetMultiDragVec(bool raw)
	{
		return this.GetMultiDragVec(TouchCoordSys.SCREEN_PX, raw);
	}

	public Vector2 GetMultiDragVecRaw()
	{
		return this.GetMultiDragVec(TouchCoordSys.SCREEN_PX, true);
	}

	public Vector2 GetMultiDragVecRaw(TouchCoordSys cs)
	{
		return this.GetMultiDragVec(cs, true);
	}

	public Vector2 GetDragDelta(int fingerId, TouchCoordSys cs, bool raw)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		if (!raw && !finger.moved)
		{
			return Vector2.zero;
		}
		return this.TransformPos((raw || !finger.justMoved) ? (finger.posCur - finger.posPrev) : (finger.posCur - finger.startPos), cs, true);
	}

	public Vector2 GetDragDelta(int fingerId)
	{
		return this.GetDragDelta(fingerId, TouchCoordSys.SCREEN_PX, false);
	}

	public Vector2 GetDragDelta(int fingerId, TouchCoordSys cs)
	{
		return this.GetDragDelta(fingerId, cs, false);
	}

	public Vector2 GetDragDelta(int fingerId, bool raw)
	{
		return this.GetDragDelta(fingerId, TouchCoordSys.SCREEN_PX, raw);
	}

	public Vector2 GetDragDeltaRaw(int fingerId)
	{
		return this.GetDragDelta(fingerId, TouchCoordSys.SCREEN_PX, true);
	}

	public Vector2 GetDragDeltaRaw(int fingerId, TouchCoordSys cs)
	{
		return this.GetDragDelta(fingerId, cs, true);
	}

	public Vector2 GetUniDragDelta(TouchCoordSys cs, bool raw)
	{
		if (!raw && !this.uniMoved)
		{
			return Vector2.zero;
		}
		return this.TransformPos((raw || !this.uniJustMoved) ? (this.uniTotalDragCur - this.uniTotalDragPrev) : this.uniTotalDragCur, cs, true);
	}

	public Vector2 GetUniDragDelta()
	{
		return this.GetUniDragDelta(TouchCoordSys.SCREEN_PX, false);
	}

	public Vector2 GetUniDragDelta(TouchCoordSys cs)
	{
		return this.GetUniDragDelta(cs, false);
	}

	public Vector2 GetUniDragDelta(bool raw)
	{
		return this.GetUniDragDelta(TouchCoordSys.SCREEN_PX, raw);
	}

	public Vector2 GetUniDragDeltaRaw()
	{
		return this.GetUniDragDelta(TouchCoordSys.SCREEN_PX, true);
	}

	public Vector2 GetUniDragDeltaRaw(TouchCoordSys cs)
	{
		return this.GetUniDragDelta(cs, true);
	}

	public Vector2 GetMultiDragDelta(TouchCoordSys cs, bool raw)
	{
		if (!raw && !this.multiMoved)
		{
			return Vector2.zero;
		}
		return this.TransformPos((raw || !this.multiJustMoved) ? (this.multiPosCur - this.multiPosPrev) : (this.multiPosCur - this.multiPosStart), cs, true);
	}

	public Vector2 GetMultiDragDelta()
	{
		return this.GetMultiDragDelta(TouchCoordSys.SCREEN_PX, false);
	}

	public Vector2 GetMultiDragDelta(TouchCoordSys cs)
	{
		return this.GetMultiDragDelta(cs, false);
	}

	public Vector2 GetMultiDragDelta(bool raw)
	{
		return this.GetMultiDragDelta(TouchCoordSys.SCREEN_PX, raw);
	}

	public Vector2 GetMultiDragDeltaRaw()
	{
		return this.GetMultiDragDelta(TouchCoordSys.SCREEN_PX, true);
	}

	public Vector2 GetMultiDragDeltaRaw(TouchCoordSys cs)
	{
		return this.GetMultiDragDelta(cs, true);
	}

	public bool Dragged(int fingerId)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return finger.curState && finger.moved;
	}

	public bool UniDragged()
	{
		return this.uniCur && this.uniMoved;
	}

	public bool MultiDragged()
	{
		return this.multiCur && this.multiMoved;
	}

	public bool JustDragged(int fingerId)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return finger.curState && finger.justMoved;
	}

	public bool JustUniDragged()
	{
		return this.uniCur && this.uniJustMoved;
	}

	public bool JustMultiDragged()
	{
		return this.multiCur && this.multiJustMoved;
	}

	public Vector2 GetDragVel(int fingerId, TouchCoordSys cs)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return this.TransformPos(finger.dragVel, cs, true);
	}

	public Vector2 GetDragVel(int fingerId)
	{
		return this.GetDragVel(fingerId, TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetUniDragVel(TouchCoordSys cs)
	{
		return this.TransformPos(this.uniDragVel, cs, true);
	}

	public Vector2 GetUniDragVel()
	{
		return this.GetUniDragVel(TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetMultiDragVel(TouchCoordSys cs)
	{
		return this.TransformPos(this.multiDragVel, cs, true);
	}

	public Vector2 GetMultiDragVel()
	{
		return this.GetMultiDragVel(TouchCoordSys.SCREEN_PX);
	}

	public bool Twisted()
	{
		return this.twistMoved;
	}

	public bool JustTwisted()
	{
		return this.twistJustMoved;
	}

	public float GetTwistVel()
	{
		return this.twistVel;
	}

	public float GetTotalTwist(bool raw)
	{
		if (!raw && !this.twistMoved)
		{
			return 0f;
		}
		return this.twistCur;
	}

	public float GetTotalTwist()
	{
		return this.GetTotalTwist(false);
	}

	public float GetTotalTwistRaw()
	{
		return this.GetTotalTwist(true);
	}

	public float GetTwistDelta(bool raw)
	{
		if (!raw && this.twistJustMoved)
		{
			return this.twistCur;
		}
		return Mathf.DeltaAngle(this.twistPrev, this.twistCur);
	}

	public float GetTwistDelta()
	{
		return this.GetTwistDelta(false);
	}

	public float GetTwistDeltaRaw()
	{
		return this.GetTwistDelta(true);
	}

	public bool Pinched()
	{
		return this.pinchMoved;
	}

	public bool JustPinched()
	{
		return this.pinchJustMoved;
	}

	public float GetPinchDist(TouchCoordSys cs, bool raw)
	{
		if (!this.multiCur || (!raw && !this.pinchMoved))
		{
			return 0f;
		}
		return this.TransformPos(this.pinchCurDist, cs);
	}

	public float GetPinchDist()
	{
		return this.GetPinchDist(TouchCoordSys.SCREEN_PX, false);
	}

	public float GetPinchDist(TouchCoordSys cs)
	{
		return this.GetPinchDist(cs, false);
	}

	public float GetPinchDist(bool raw)
	{
		return this.GetPinchDist(TouchCoordSys.SCREEN_PX, raw);
	}

	public float GetPinchDistRaw()
	{
		return this.GetPinchDist(TouchCoordSys.SCREEN_PX, true);
	}

	public float GetPinchDistRaw(TouchCoordSys cs)
	{
		return this.GetPinchDist(cs, true);
	}

	public float GetPinchDistDelta(TouchCoordSys cs, bool raw)
	{
		if (!this.multiCur || (!raw && !this.pinchMoved))
		{
			return 0f;
		}
		return this.TransformPos(this.pinchCurDist - ((raw || !this.pinchJustMoved) ? this.pinchPrevDist : this.pinchDistStart), cs);
	}

	public float GetPinchDistDelta()
	{
		return this.GetPinchDistDelta(TouchCoordSys.SCREEN_PX, false);
	}

	public float GetPinchDistDelta(TouchCoordSys cs)
	{
		return this.GetPinchDistDelta(cs, false);
	}

	public float GetPinchDistDelta(bool raw)
	{
		return this.GetPinchDistDelta(TouchCoordSys.SCREEN_PX, raw);
	}

	public float GetPinchDistDeltaRaw()
	{
		return this.GetPinchDistDelta(TouchCoordSys.SCREEN_PX, true);
	}

	public float GetPinchDistDeltaRaw(TouchCoordSys cs)
	{
		return this.GetPinchDistDelta(cs, true);
	}

	public float GetPinchScale(bool raw)
	{
		if (!this.multiCur || (!raw && !this.pinchMoved))
		{
			return 1f;
		}
		return this.pinchCurDist / this.pinchDistStart;
	}

	public float GetPinchScale()
	{
		return this.GetPinchScale(false);
	}

	public float GetPinchScaleRaw()
	{
		return this.GetPinchScale(true);
	}

	public float GetPinchRelativeScale(bool raw)
	{
		if (!this.multiCur || (!raw && !this.pinchMoved))
		{
			return 1f;
		}
		return this.pinchCurDist / ((raw || !this.pinchJustMoved) ? this.pinchPrevDist : this.pinchDistStart);
	}

	public float GetPinchRelativeScale()
	{
		return this.GetPinchRelativeScale(false);
	}

	public float GetPinchRelativeScaleRaw()
	{
		return this.GetPinchRelativeScale(true);
	}

	public float GetPinchDistVel()
	{
		return this.pinchDistVel;
	}

	public bool JustTapped()
	{
		return this.fingerA.JustTapped(false);
	}

	public bool JustMultiTapped()
	{
		return this.justMultiTapped;
	}

	public bool JustSingleTapped()
	{
		return this.fingerA.JustTapped(true);
	}

	public bool JustMultiSingleTapped()
	{
		return this.justMultiDelayTapped;
	}

	public bool JustDoubleTapped()
	{
		return this.fingerA.JustDoubleTapped();
	}

	public bool JustMultiDoubleTapped()
	{
		return this.justMultiDoubleTapped;
	}

	public Vector2 GetTapPos(TouchCoordSys cs)
	{
		return this.fingerA.GetTapPos(cs);
	}

	public Vector2 GetTapPos()
	{
		return this.GetTapPos(TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetMultiTapPos(TouchCoordSys cs)
	{
		return this.TransformPos(this.lastMultiTapPos, cs, false);
	}

	public Vector2 GetMultiTapPos()
	{
		return this.GetMultiTapPos(TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetReleasedStartPos(int fingerId, TouchCoordSys cs)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return this.TransformPos(finger.endedPosStart, cs, false);
	}

	public Vector2 GetReleasedStartPos(int fingerId)
	{
		return this.GetReleasedStartPos(fingerId, TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetReleasedUniStartPos(TouchCoordSys cs)
	{
		return this.TransformPos(this.endedUniPosStart, cs, false);
	}

	public Vector2 GetReleasedUniStartPos()
	{
		return this.GetReleasedUniStartPos(TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetReleasedMultiStartPos(TouchCoordSys cs)
	{
		return this.TransformPos(this.endedMultiPosStart, cs, false);
	}

	public Vector2 GetReleasedMultiStartPos()
	{
		return this.GetReleasedMultiStartPos(TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetReleasedEndPos(int fingerId, TouchCoordSys cs)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return this.TransformPos(finger.endedPosEnd, cs, false);
	}

	public Vector2 GetReleasedEndPos(int fingerId)
	{
		return this.GetReleasedEndPos(fingerId, TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetReleasedUniEndPos(TouchCoordSys cs)
	{
		return this.TransformPos(this.endedUniPosEnd, cs, false);
	}

	public Vector2 GetReleasedUniEndPos()
	{
		return this.GetReleasedUniEndPos(TouchCoordSys.SCREEN_PX);
	}

	public Vector2 GetReleasedMultiEndPos(TouchCoordSys cs)
	{
		return this.TransformPos(this.endedMultiPosEnd, cs, false);
	}

	public Vector2 GetReleasedMultiEndPos()
	{
		return this.GetReleasedMultiEndPos(TouchCoordSys.SCREEN_PX);
	}

	public bool ReleasedDragged(int fingerId)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return finger.endedMoved;
	}

	public bool ReleasedUniDragged()
	{
		return this.endedUniMoved;
	}

	public bool ReleasedMultiDragged()
	{
		return this.endedMultiMoved;
	}

	public bool ReleasedMoved(int fingerId)
	{
		return this.ReleasedDragged(fingerId);
	}

	public bool ReleasedUniMoved()
	{
		return this.ReleasedUniDragged();
	}

	public bool ReleasedMultiMoved()
	{
		return this.ReleasedMultiDragged();
	}

	public Vector2 GetReleasedDragVec(int fingerId, TouchCoordSys cs, bool raw)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		if (!raw && !finger.endedMoved)
		{
			return Vector2.zero;
		}
		return this.TransformPos(finger.endedPosEnd - finger.endedPosStart, cs, true);
	}

	public Vector2 GetReleasedDragVec(int fingerId)
	{
		return this.GetReleasedDragVec(fingerId, TouchCoordSys.SCREEN_PX, false);
	}

	public Vector2 GetReleasedDragVec(int fingerId, TouchCoordSys cs)
	{
		return this.GetReleasedDragVec(fingerId, cs, false);
	}

	public Vector2 GetReleasedDragVec(int fingerId, bool raw)
	{
		return this.GetReleasedDragVec(fingerId, TouchCoordSys.SCREEN_PX, raw);
	}

	public Vector2 GetReleasedDragVecRaw(int fingerId)
	{
		return this.GetReleasedDragVec(fingerId, TouchCoordSys.SCREEN_PX, true);
	}

	public Vector2 GetReleasedDragVecRaw(int fingerId, TouchCoordSys cs)
	{
		return this.GetReleasedDragVec(fingerId, cs, true);
	}

	public Vector2 GetReleasedUniDragVec(TouchCoordSys cs, bool raw)
	{
		if (!raw && !this.endedUniMoved)
		{
			return Vector2.zero;
		}
		return this.TransformPos(this.endedUniTotalDrag, cs, true);
	}

	public Vector2 GetReleasedUniDragVec()
	{
		return this.GetReleasedUniDragVec(TouchCoordSys.SCREEN_PX, false);
	}

	public Vector2 GetReleasedUniDragVec(TouchCoordSys cs)
	{
		return this.GetReleasedUniDragVec(cs, false);
	}

	public Vector2 GetReleasedUniDragVec(bool raw)
	{
		return this.GetReleasedUniDragVec(TouchCoordSys.SCREEN_PX, raw);
	}

	public Vector2 GetReleasedUniDragVecRaw()
	{
		return this.GetReleasedUniDragVec(TouchCoordSys.SCREEN_PX, true);
	}

	public Vector2 GetReleasedUniDragVecRaw(TouchCoordSys cs)
	{
		return this.GetReleasedUniDragVec(cs, true);
	}

	public Vector2 GetReleasedMultiDragVec(TouchCoordSys cs, bool raw)
	{
		if (!raw && !this.endedMultiMoved)
		{
			return Vector2.zero;
		}
		return this.TransformPos(this.endedMultiPosEnd - this.endedMultiPosStart, cs, true);
	}

	public Vector2 GetReleasedMultiDragVec()
	{
		return this.GetReleasedMultiDragVec(TouchCoordSys.SCREEN_PX, false);
	}

	public Vector2 GetReleasedMultiDragVec(TouchCoordSys cs)
	{
		return this.GetReleasedMultiDragVec(cs, false);
	}

	public Vector2 GetReleasedMultiDragVec(bool raw)
	{
		return this.GetReleasedMultiDragVec(TouchCoordSys.SCREEN_PX, raw);
	}

	public Vector2 GetReleasedMultiDragVecRaw()
	{
		return this.GetReleasedMultiDragVec(TouchCoordSys.SCREEN_PX, true);
	}

	public Vector2 GetReleasedMultiDragVecRaw(TouchCoordSys cs)
	{
		return this.GetReleasedMultiDragVec(cs, true);
	}

	public float GetReleasedDuration(int fingerId)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		return finger.endedEndTime - finger.endedStartTime;
	}

	public float GetReleasedUniDuration()
	{
		return this.endedUniEndTime - this.endedUniStartTime;
	}

	public float GetReleasedMultiDuration()
	{
		return this.endedMultiEndTime - this.endedMultiStartTime;
	}

	public Vector2 GetReleasedDragVel(int fingerId, TouchCoordSys cs, bool raw)
	{
		TouchZone.Finger finger = (fingerId != 1) ? this.fingerA : this.fingerB;
		if (!raw && !finger.endedMoved)
		{
			return Vector2.zero;
		}
		return this.TransformPos(finger.endedDragVel, cs, true);
	}

	public Vector2 GetReleasedDragVel(int fingerId)
	{
		return this.GetReleasedDragVel(fingerId, TouchCoordSys.SCREEN_PX, false);
	}

	public Vector2 GetReleasedDragVel(int fingerId, TouchCoordSys cs)
	{
		return this.GetReleasedDragVel(fingerId, cs, false);
	}

	public Vector2 GetReleasedDragVel(int fingerId, bool raw)
	{
		return this.GetReleasedDragVel(fingerId, TouchCoordSys.SCREEN_PX, raw);
	}

	public Vector2 GetReleasedDragVelRaw(int fingerId)
	{
		return this.GetReleasedDragVel(fingerId, TouchCoordSys.SCREEN_PX, true);
	}

	public Vector2 GetReleasedDragVelRaw(int fingerId, TouchCoordSys cs)
	{
		return this.GetReleasedDragVel(fingerId, cs, true);
	}

	public Vector2 GetReleasedUniDragVel(TouchCoordSys cs, bool raw)
	{
		if (!raw && !this.endedUniMoved)
		{
			return Vector2.zero;
		}
		return this.TransformPos(this.endedUniDragVel, cs, true);
	}

	public Vector2 GetReleasedUniDragVel()
	{
		return this.GetReleasedUniDragVel(TouchCoordSys.SCREEN_PX, false);
	}

	public Vector2 GetReleasedUniDragVel(TouchCoordSys cs)
	{
		return this.GetReleasedUniDragVel(cs, false);
	}

	public Vector2 GetReleasedUniDragVel(bool raw)
	{
		return this.GetReleasedUniDragVel(TouchCoordSys.SCREEN_PX, raw);
	}

	public Vector2 GetReleasedUniDragVelRaw()
	{
		return this.GetReleasedUniDragVel(TouchCoordSys.SCREEN_PX, true);
	}

	public Vector2 GetReleasedUniDragVelRaw(TouchCoordSys cs)
	{
		return this.GetReleasedUniDragVel(cs, true);
	}

	public Vector2 GetReleasedMultiDragVel(TouchCoordSys cs, bool raw)
	{
		if (!raw && !this.endedMultiMoved)
		{
			return Vector2.zero;
		}
		return this.TransformPos(this.endedMultiDragVel, cs, true);
	}

	public Vector2 GetReleasedMultiDragVel()
	{
		return this.GetReleasedMultiDragVel(TouchCoordSys.SCREEN_PX, false);
	}

	public Vector2 GetReleasedMultiDragVel(TouchCoordSys cs)
	{
		return this.GetReleasedMultiDragVel(cs, false);
	}

	public Vector2 GetReleasedMultiDragVel(bool raw)
	{
		return this.GetReleasedMultiDragVel(TouchCoordSys.SCREEN_PX, raw);
	}

	public Vector2 GetReleasedMultiDragVelRaw()
	{
		return this.GetReleasedMultiDragVel(TouchCoordSys.SCREEN_PX, true);
	}

	public Vector2 GetReleasedMultiDragVelRaw(TouchCoordSys cs)
	{
		return this.GetReleasedMultiDragVel(cs, true);
	}

	public bool ReleasedTwisted()
	{
		return this.endedTwistMoved;
	}

	public bool ReleasedTwistMoved()
	{
		return this.ReleasedTwisted();
	}

	public float GetReleasedTwistAngle(bool raw)
	{
		if (!raw && !this.endedTwistMoved)
		{
			return 0f;
		}
		return this.endedTwistAngle;
	}

	public float GetReleasedTwistAngle()
	{
		return this.GetReleasedTwistAngle(false);
	}

	public float GetReleasedTwistAngleRaw()
	{
		return this.GetReleasedTwistAngle(true);
	}

	public float GetReleasedTwistVel(bool raw)
	{
		if (!raw && !this.endedTwistMoved)
		{
			return 0f;
		}
		return this.endedTwistVel;
	}

	public float GetReleasedTwistVel()
	{
		return this.GetReleasedTwistVel(false);
	}

	public float GetReleasedTwistVelRaw()
	{
		return this.GetReleasedTwistVel(true);
	}

	public bool ReleasedPinched()
	{
		return this.endedPinchMoved;
	}

	public bool ReleasedPinchMoved()
	{
		return this.ReleasedPinched();
	}

	public float GetReleasedPinchScale(bool raw)
	{
		if (!raw && !this.endedPinchMoved)
		{
			return 1f;
		}
		return this.endedPinchDistEnd / this.endedPinchDistStart;
	}

	public float GetReleasedPinchScale()
	{
		return this.GetReleasedPinchScale(false);
	}

	public float GetReleasedPinchScaleRaw()
	{
		return this.GetReleasedPinchScale(true);
	}

	public float GetReleasedPinchStartDist(TouchCoordSys cs)
	{
		return this.TransformPos(this.endedPinchDistStart, cs);
	}

	public float GetReleasedPinchStartDist()
	{
		return this.GetReleasedPinchStartDist(TouchCoordSys.SCREEN_PX);
	}

	public float GetReleasedPinchEndDist(TouchCoordSys cs)
	{
		return this.TransformPos(this.endedPinchDistEnd, cs);
	}

	public float GetReleasedPinchEndDist()
	{
		return this.GetReleasedPinchEndDist(TouchCoordSys.SCREEN_PX);
	}

	public float GetReleasedPinchDistVel(TouchCoordSys cs, bool raw)
	{
		if (!raw && !this.endedPinchMoved)
		{
			return 0f;
		}
		return this.TransformPos(this.endedPinchDistVel, cs);
	}

	public float GetReleasedPinchDistVel()
	{
		return this.GetReleasedPinchDistVel(TouchCoordSys.SCREEN_PX, false);
	}

	public float GetReleasedPinchDistVel(TouchCoordSys cs)
	{
		return this.GetReleasedPinchDistVel(cs, false);
	}

	public float GetReleasedPinchDistVel(bool raw)
	{
		return this.GetReleasedPinchDistVel(TouchCoordSys.SCREEN_PX, raw);
	}

	public float GetReleasedPinchDistVelRaw()
	{
		return this.GetReleasedPinchDistVel(TouchCoordSys.SCREEN_PX, true);
	}

	public float GetReleasedPinchDistVelRaw(TouchCoordSys cs)
	{
		return this.GetReleasedPinchDistVel(cs, true);
	}

	public void TotalTakeover()
	{
		this.joy.EndTouch(this.fingerA.touchId, this);
		this.joy.EndTouch(this.fingerB.touchId, this);
	}

	public override void Enable(bool skipAnimation)
	{
		this.enabled = true;
		this.AnimateParams((!this.overrideScale) ? this.joy.releasedZoneScale : this.releasedScale, TouchController.ScaleAlpha((!this.overrideColors) ? this.joy.defaultReleasedZoneColor : this.releasedColor, (float)((!this.visible) ? 0 : 1)), (!skipAnimation) ? ((!this.overrideAnimDuration) ? this.joy.enableAnimDuration : this.enableAnimDuration) : 0f);
	}

	public override void Disable(bool skipAnim)
	{
		this.enabled = false;
		this.ReleaseTouches();
		this.AnimateParams((!this.overrideScale) ? this.joy.disabledZoneScale : this.disabledScale, TouchController.ScaleAlpha((!this.overrideColors) ? this.joy.defaultDisabledZoneColor : this.disabledColor, (float)((!this.visible) ? 0 : 1)), (!skipAnim) ? ((!this.overrideAnimDuration) ? this.joy.disableAnimDuration : this.disableAnimDuration) : 0f);
	}

	public override void Show(bool skipAnim)
	{
		this.visible = true;
		this.AnimateParams((!this.overrideScale) ? ((!this.enabled) ? this.joy.disabledZoneScale : this.joy.releasedZoneScale) : ((!this.enabled) ? this.disabledScale : this.releasedScale), (!this.overrideColors) ? ((!this.enabled) ? this.joy.defaultDisabledZoneColor : this.joy.defaultReleasedZoneColor) : ((!this.enabled) ? this.disabledColor : this.releasedColor), (!skipAnim) ? ((!this.overrideAnimDuration) ? this.joy.showAnimDuration : this.showAnimDuration) : 0f);
	}

	public override void Hide(bool skipAnim)
	{
		this.visible = false;
		this.ReleaseTouches();
		Color end = this.animColor.end;
		end.a = 0f;
		this.AnimateParams(this.animScale.end, end, (!skipAnim) ? ((!this.overrideAnimDuration) ? this.joy.hideAnimDuration : this.hideAnimDuration) : 0f);
	}

	public void SetRect(Rect r)
	{
		if (this.screenRectPx != r)
		{
			this.screenRectPx = r;
			this.posPx = r.center;
			if (this.shape == TouchController.ControlShape.CIRCLE)
			{
				this.sizePx.x = (this.sizePx.y = Mathf.Min(r.width, r.height));
			}
			else
			{
				this.sizePx.x = r.width;
				this.sizePx.y = r.height;
			}
			this.OnReset();
		}
	}

	public override void ResetRect()
	{
		this.SetRect(this.layoutRectPx);
	}

	public Rect GetRect(bool getAutoRect)
	{
		return (!getAutoRect) ? this.screenRectPx : this.layoutRectPx;
	}

	public Rect GetRect()
	{
		return this.GetRect(false);
	}

	public Rect GetDisplayRect(bool applyScale)
	{
		Rect cenRect = this.screenRectPx;
		if (this.shape == TouchController.ControlShape.CIRCLE || this.shape == TouchController.ControlShape.RECT)
		{
			cenRect = TouchController.GetCenRect(this.posPx, this.sizePx * ((!applyScale) ? 1f : this.animScale.cur));
		}
		return cenRect;
	}

	public Rect GetDisplayRect()
	{
		return this.GetDisplayRect(true);
	}

	public Color GetColor()
	{
		return this.animColor.cur;
	}

	public int GetGUIDepth()
	{
		return this.joy.guiDepth + this.guiDepth + ((!this.UniPressed()) ? 0 : this.joy.guiPressedOfs);
	}

	public Texture2D GetDisplayTex()
	{
		return (!this.enabled || !this.UniPressed()) ? this.releasedImg : this.pressedImg;
	}

	public bool GetKey(KeyCode key)
	{
		bool flag = false;
		return this.GetKeyEx(key, out flag);
	}

	public bool GetKeyDown(KeyCode key)
	{
		bool flag = false;
		return this.GetKeyDownEx(key, out flag);
	}

	public bool GetKeyUp(KeyCode key)
	{
		bool flag = false;
		return this.GetKeyUpEx(key, out flag);
	}

	public bool GetKeyEx(KeyCode key, out bool keySupported)
	{
		keySupported = false;
		if (!this.enableGetKey || key == KeyCode.None)
		{
			return false;
		}
		if (key == this.getKeyCode || key == this.getKeyCodeAlt)
		{
			keySupported = true;
			if (this.UniPressed())
			{
				return true;
			}
		}
		if (key == this.getKeyCodeMulti || key == this.getKeyCodeMultiAlt)
		{
			keySupported = true;
			if (this.MultiPressed())
			{
				return true;
			}
		}
		return false;
	}

	public bool GetKeyDownEx(KeyCode key, out bool keySupported)
	{
		keySupported = false;
		if (!this.enableGetKey || key == KeyCode.None)
		{
			return false;
		}
		if (key == this.getKeyCode || key == this.getKeyCodeAlt)
		{
			keySupported = true;
			if (this.JustUniPressed())
			{
				return true;
			}
		}
		if (key == this.getKeyCodeMulti || key == this.getKeyCodeMultiAlt)
		{
			keySupported = true;
			if (this.JustMultiPressed())
			{
				return true;
			}
		}
		return false;
	}

	public bool GetKeyUpEx(KeyCode key, out bool keySupported)
	{
		keySupported = false;
		if (!this.enableGetKey || key == KeyCode.None)
		{
			return false;
		}
		if (key == this.getKeyCode || key == this.getKeyCodeAlt)
		{
			keySupported = true;
			if (this.JustUniReleased())
			{
				return true;
			}
		}
		if (key == this.getKeyCodeMulti || key == this.getKeyCodeMultiAlt)
		{
			keySupported = true;
			if (this.JustMultiReleased())
			{
				return true;
			}
		}
		return false;
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
		if (!this.enableGetButton)
		{
			return false;
		}
		if (buttonName == this.getButtonName)
		{
			buttonSupported = true;
			if (this.UniPressed())
			{
				return true;
			}
		}
		if (buttonName == this.getButtonMultiName)
		{
			buttonSupported = true;
			if (this.MultiPressed())
			{
				return true;
			}
		}
		return false;
	}

	public bool GetButtonDownEx(string buttonName, out bool buttonSupported)
	{
		buttonSupported = false;
		if (!this.enableGetButton)
		{
			return false;
		}
		if (buttonName == this.getButtonName)
		{
			buttonSupported = true;
			if (this.JustUniPressed())
			{
				return true;
			}
		}
		if (buttonName == this.getButtonMultiName)
		{
			buttonSupported = true;
			if (this.JustMultiPressed())
			{
				return true;
			}
		}
		return false;
	}

	public bool GetButtonUpEx(string buttonName, out bool buttonSupported)
	{
		buttonSupported = false;
		if (!this.enableGetButton)
		{
			return false;
		}
		if (buttonName == this.getButtonName)
		{
			buttonSupported = true;
			if (this.JustUniReleased())
			{
				return true;
			}
		}
		if (buttonName == this.getButtonMultiName)
		{
			buttonSupported = true;
			if (this.JustMultiReleased())
			{
				return true;
			}
		}
		return false;
	}

	public float GetAxis(string axisName)
	{
		bool flag = false;
		return this.GetAxisEx(axisName, out flag);
	}

	public float GetAxisEx(string axisName, out bool supported)
	{
		if (this.enableGetAxis)
		{
			if (this.axisHorzName == axisName)
			{
				supported = true;
				return this.GetUniDragDelta(true).x * this.axisValScale * ((!this.axisHorzFlip) ? 1f : -1f);
			}
			if (this.axisVertName == axisName)
			{
				supported = true;
				return this.GetUniDragDelta(true).y * -this.axisValScale * ((!this.axisVertFlip) ? 1f : -1f);
			}
		}
		supported = false;
		return 0f;
	}

	public static int GetBoxPortion(int horzSections, int vertSections, Vector2 normalizedPos)
	{
		int num = 0;
		int num2 = 0;
		if (horzSections == 2)
		{
			num = ((normalizedPos.x >= 0.5f) ? 4 : 1);
		}
		else if (horzSections >= 3)
		{
			num = ((normalizedPos.x >= 0.333f) ? ((normalizedPos.x <= 0.666f) ? 2 : 4) : 1);
		}
		if (vertSections == 2)
		{
			num2 = ((normalizedPos.y >= 0.5f) ? 32 : 8);
		}
		else if (vertSections >= 3)
		{
			num2 = ((normalizedPos.y >= 0.333f) ? ((normalizedPos.y <= 0.666f) ? 16 : 32) : 8);
		}
		return num | num2;
	}

	public override void Init(TouchController joy)
	{
		base.Init(joy);
		this.joy = joy;
		this.fingerA = new TouchZone.Finger(this);
		this.fingerB = new TouchZone.Finger(this);
		this.AnimateParams((!this.overrideScale) ? this.joy.releasedZoneScale : this.releasedScale, (!this.overrideColors) ? this.joy.defaultReleasedZoneColor : this.releasedColor, 0f);
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

	public int GetFingerNum()
	{
		return ((!this.fingerA.curState) ? 0 : 1) + ((!this.fingerB.curState) ? 0 : 1);
	}

	private void AnimateParams(float scale, Color color, float duration)
	{
		if (duration <= 0f)
		{
			this.animTimer.Reset(0f);
			this.animTimer.Disable();
			this.animColor.Reset(color);
			this.animScale.Reset(scale);
		}
		else
		{
			this.animTimer.Start(duration);
			this.animScale.MoveTo(scale);
			this.animColor.MoveTo(color);
		}
	}

	public override void OnReset()
	{
		this.fingerA.Reset();
		this.fingerB.Reset();
		this.multiCur = (this.multiPrev = (this.justMultiTapped = (this.justMultiDelayTapped = (this.justMultiDoubleTapped = (this.nextTapCanBeMultiDoubleTap = false)))));
		this.twistMoved = (this.twistJustMoved = (this.pinchMoved = (this.pinchJustMoved = (this.uniMoved = (this.uniJustMoved = (this.uniCur = (this.uniPrev = false)))))));
		this.multiStartTime = (this.lastMultiTapTime = (this.uniStartTime = -100f));
		this.multiPosCur = (this.multiPosPrev = (this.multiPosStart = (this.lastMultiTapPos = Vector2.zero)));
		this.multiDragVel = Vector2.zero;
		this.uniDragVel = Vector2.zero;
		this.twistVel = 0f;
		this.pinchDistVel = 0f;
		this.twistStartAbs = (this.twistCurAbs = (this.twistCur = (this.twistCurRaw = (this.twistPrevAbs = (this.twistPrev = 0f)))));
		this.twistVel = 0f;
		this.pinchDistStart = (this.pinchCurDist = (this.pinchPrevDist = (this.pinchDistVel = 0f)));
		this.uniPosCur = (this.uniPosStart = (this.uniTotalDragCur = (this.uniTotalDragPrev = Vector2.zero)));
		this.touchCanceled = false;
		this.AnimateParams((!this.overrideScale) ? this.joy.releasedZoneScale : this.releasedScale, (!this.overrideColors) ? this.joy.defaultReleasedZoneColor : this.releasedColor, 0f);
		if (!this.enabled)
		{
			this.Disable(true);
		}
		if (!this.visible)
		{
			this.Hide(true);
		}
	}

	public override void OnPrePoll()
	{
		this.fingerA.OnPrePoll();
		this.fingerB.OnPrePoll();
	}

	public override void OnPostPoll()
	{
		if (this.fingerA.touchId >= 0 && !this.fingerA.touchVerified)
		{
			this.OnTouchEnd(this.fingerA.touchId, false);
		}
		if (this.fingerB.touchId >= 0 && !this.fingerB.touchVerified)
		{
			this.OnTouchEnd(this.fingerB.touchId, false);
		}
	}

	public override void ReleaseTouches()
	{
		if (this.fingerA.touchId >= 0)
		{
			this.OnTouchEnd(this.fingerA.touchId, true);
		}
		if (this.fingerB.touchId >= 0)
		{
			this.OnTouchEnd(this.fingerB.touchId, true);
		}
	}

	private void OnMultiStart(Vector2 startPos, Vector2 curPos)
	{
		this.multiCur = true;
		this.multiStartTime = this.joy.curTime;
		this.multiPosStart = startPos;
		this.multiPosCur = curPos;
		this.multiPosPrev = curPos;
		this.multiMoved = false;
		this.multiLastMoveTime = 0f;
		this.multiDragVel = Vector2.zero;
		this.multiExtremeCurVec = Vector2.zero;
		this.multiExtremeCurDist = 0f;
		this.pinchCurDist = (this.pinchPrevDist = (this.pinchDistStart = this.GetFingerDist()));
		this.pinchMoved = false;
		this.pinchJustMoved = false;
		this.pinchLastMoveTime = 0f;
		this.pinchDistVel = 0f;
		this.pinchExtremeCurDist = 0f;
		this.twistCurAbs = (this.twistPrevAbs = (this.twistStartAbs = this.GetFingerAbsAngle(0f)));
		this.twistCur = (this.twistPrev = 0f);
		this.twistMoved = false;
		this.twistJustMoved = false;
		this.twistLastMoveTime = 0f;
		this.twistVel = 0f;
		this.twistExtremeCur = 0f;
	}

	private void OnMultiEnd(Vector2 endPos)
	{
		this.multiPosCur = endPos;
		this.UpdateMultiTouchState(true);
		this.multiCur = false;
		this.endedMultiStartTime = this.multiStartTime;
		this.endedMultiEndTime = this.joy.curTime;
		this.endedMultiPosEnd = endPos;
		this.endedMultiPosStart = this.multiPosStart;
		this.endedMultiDragVel = this.multiDragVel;
		this.endedTwistAngle = this.twistCur;
		this.endedTwistVel = this.twistVel;
		this.endedPinchDistStart = this.pinchDistStart;
		this.endedPinchDistEnd = this.pinchCurDist;
		this.endedPinchDistVel = this.pinchDistVel;
		this.endedMultiMoved = this.multiMoved;
		this.endedTwistMoved = this.twistMoved;
		this.endedPinchMoved = this.pinchMoved;
	}

	private void OnUniStart(Vector2 startPos, Vector2 curPos)
	{
		this.uniCur = true;
		this.uniStartTime = this.joy.curTime;
		this.uniPosStart = startPos;
		this.uniPosCur = curPos;
		this.uniMoved = false;
		this.uniJustMoved = false;
		this.uniExtremeDragCurVec = Vector2.zero;
		this.uniExtremeDragCurDist = 0f;
		this.uniDragVel = Vector2.zero;
		this.uniTotalDragPrev = Vector2.zero;
		this.uniTotalDragCur = Vector2.zero;
	}

	private void OnUniEnd(Vector2 endPos, Vector2 endDeltaAccum)
	{
		this.uniTotalDragCur += endDeltaAccum;
		this.uniPosCur = endPos;
		this.UpdateUniTouchState(true);
		this.uniCur = false;
		this.endedUniPosEnd = endPos;
		this.endedUniStartTime = this.uniStartTime;
		this.endedUniEndTime = this.joy.curTime;
		this.endedUniDragVel = this.uniDragVel;
		this.endedUniPosStart = this.uniPosStart;
		this.endedUniTotalDrag = this.uniTotalDragCur;
		this.endedUniMoved = this.uniMoved;
	}

	private void OnPinchStart()
	{
		if (!this.pinchMoved)
		{
			this.pinchMoved = true;
			this.pinchJustMoved = true;
			if (this.startDragWhenPinching)
			{
				this.OnMultiDragStart();
			}
			if (this.startTwistWhenPinching)
			{
				this.OnTwistStart();
			}
		}
	}

	private void OnTwistStart()
	{
		if (!this.twistMoved)
		{
			this.twistMoved = true;
			this.twistJustMoved = true;
			this.twistStartRaw = this.twistCurRaw;
			this.twistCur = 0f;
			if (this.startDragWhenTwisting)
			{
				this.OnMultiDragStart();
			}
			if (this.startPinchWhenTwisting)
			{
				this.OnPinchStart();
			}
		}
	}

	private void OnMultiDragStart()
	{
		if (!this.multiMoved)
		{
			this.multiMoved = true;
			this.multiJustMoved = true;
			if (this.startTwistWhenDragging)
			{
				this.OnTwistStart();
			}
			if (this.startPinchWhenDragging)
			{
				this.OnPinchStart();
			}
		}
	}

	private void UpdateUniTouchState(bool lastUpdateMode = false)
	{
		if (lastUpdateMode)
		{
			return;
		}
		this.uniExtremeDragCurVec.x = Mathf.Max(Mathf.Abs(this.uniTotalDragCur.x), this.uniExtremeDragCurVec.x);
		this.uniExtremeDragCurVec.y = Mathf.Max(Mathf.Abs(this.uniTotalDragCur.y), this.uniExtremeDragCurVec.y);
		this.uniExtremeDragCurDist = Mathf.Max(this.uniTotalDragCur.magnitude, this.uniExtremeDragCurDist);
		this.uniJustMoved = false;
		if (!this.uniMoved && this.uniExtremeDragCurDist > this.joy.touchTapMaxDistPx)
		{
			this.uniMoved = true;
			this.uniJustMoved = true;
		}
		if (this.uniCur)
		{
			if (TouchZone.PxPosEquals(this.uniTotalDragCur, this.uniTotalDragPrev))
			{
				if (this.joy.curTime - this.uniLastMoveTime > this.joy.velPreserveTime)
				{
					this.uniDragVel = Vector2.zero;
				}
			}
			else
			{
				this.uniLastMoveTime = this.joy.curTime;
				this.uniDragVel = (this.uniTotalDragCur - this.uniTotalDragPrev) * this.joy.invDeltaTime;
			}
		}
	}

	private void UpdateMultiTouchState(bool lastUpdateMode = false)
	{
		if (lastUpdateMode)
		{
			return;
		}
		this.multiJustMoved = false;
		this.pinchJustMoved = false;
		this.twistJustMoved = false;
		if (this.multiCur)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			Vector2 vector = this.multiPosCur - this.multiPosStart;
			this.multiExtremeCurVec.x = Mathf.Max(Mathf.Abs(vector.x), this.multiExtremeCurVec.x);
			this.multiExtremeCurVec.y = Mathf.Max(Mathf.Abs(vector.y), this.multiExtremeCurVec.y);
			this.multiExtremeCurDist = Mathf.Max(vector.magnitude, this.multiExtremeCurDist);
			if (!this.multiMoved && this.multiExtremeCurDist > this.joy.touchTapMaxDistPx)
			{
				flag = true;
			}
			this.pinchJustMoved = false;
			this.pinchCurDist = this.GetFingerDist();
			this.pinchExtremeCurDist = Mathf.Max(Mathf.Abs(this.pinchCurDist - this.pinchDistStart), this.pinchExtremeCurDist);
			if (!this.pinchMoved && this.pinchExtremeCurDist > this.joy.pinchMinDistPx)
			{
				flag2 = true;
			}
			this.twistJustMoved = false;
			this.twistCurAbs = this.GetFingerAbsAngle(this.twistPrevAbs);
			this.twistCurRaw = Mathf.DeltaAngle(this.twistCurAbs, this.twistStartAbs);
			bool flag4 = this.pinchCurDist > this.joy.twistSafeFingerDistPx;
			if (!this.twistMoved && flag4 && Mathf.Abs(this.twistCurRaw) * 0.0174532924f * 2f * this.pinchCurDist > this.joy.pinchMinDistPx)
			{
				flag3 = true;
			}
			if (this.twistMoved && (flag4 || !this.freezeTwistWhenTooClose))
			{
				this.twistCur = this.twistCurRaw - this.twistStartRaw;
				this.twistExtremeCur = Mathf.Max(Mathf.Abs(this.twistCur), this.twistExtremeCur);
			}
			int num = 0;
			switch (this.gestureDetectionOrder)
			{
			case TouchZone.GestureDetectionOrder.TWIST_PINCH_DRAG:
				num = 136;
				break;
			case TouchZone.GestureDetectionOrder.TWIST_DRAG_PINCH:
				num = 80;
				break;
			case TouchZone.GestureDetectionOrder.PINCH_TWIST_DRAG:
				num = 129;
				break;
			case TouchZone.GestureDetectionOrder.PINCH_DRAG_TWIST:
				num = 17;
				break;
			case TouchZone.GestureDetectionOrder.DRAG_TWIST_PINCH:
				num = 66;
				break;
			case TouchZone.GestureDetectionOrder.DRAG_PINCH_TWIST:
				num = 10;
				break;
			}
			for (int i = 0; i < 3; i++)
			{
				switch (num >> i * 3 & 7)
				{
				case 0:
					if (this.twistMoved || flag3)
					{
						if (this.noDragAfterTwist)
						{
							flag = false;
						}
						if (this.noPinchAfterTwist)
						{
							flag2 = false;
						}
					}
					break;
				case 1:
					if (this.pinchMoved || flag2)
					{
						if (this.noDragAfterPinch)
						{
							flag = false;
						}
						if (this.noTwistAfterPinch)
						{
							flag3 = false;
						}
					}
					break;
				case 2:
					if (this.multiMoved || flag)
					{
						if (this.noTwistAfterDrag)
						{
							flag3 = false;
						}
						if (this.noPinchAfterDrag)
						{
							flag2 = false;
						}
					}
					break;
				}
			}
			if (flag)
			{
				this.OnMultiDragStart();
			}
			if (flag2)
			{
				this.OnPinchStart();
			}
			if (flag3)
			{
				this.OnTwistStart();
			}
		}
		if (this.multiCur)
		{
			if (TouchZone.PxPosEquals(this.multiPosCur, this.multiPosPrev))
			{
				if (this.joy.curTime - this.multiLastMoveTime > this.joy.velPreserveTime)
				{
					this.multiDragVel = Vector2.zero;
				}
			}
			else
			{
				this.multiLastMoveTime = this.joy.curTime;
				this.multiDragVel = (this.multiPosCur - this.multiPosPrev) * this.joy.invDeltaTime;
			}
			if (TouchZone.PxDistEquals(this.pinchCurDist, this.pinchPrevDist))
			{
				if (this.joy.curTime - this.pinchLastMoveTime > this.joy.velPreserveTime)
				{
					this.pinchDistVel = 0f;
				}
			}
			else
			{
				this.pinchLastMoveTime = this.joy.curTime;
				this.pinchDistVel = (this.pinchCurDist - this.pinchPrevDist) * this.joy.invDeltaTime;
			}
			if (TouchZone.TwistAngleEquals(this.twistCur, this.twistPrev))
			{
				if (this.joy.curTime - this.twistLastMoveTime > this.joy.velPreserveTime)
				{
					this.twistVel = 0f;
				}
			}
			else
			{
				this.twistLastMoveTime = this.joy.curTime;
				this.twistVel = (this.twistCur - this.twistPrev) * this.joy.invDeltaTime;
			}
		}
	}

	public override void OnUpdate(bool firstUpdate)
	{
		this.fingerA.PreUpdate(firstUpdate);
		this.fingerB.PreUpdate(firstUpdate);
		this.uniPrev = this.uniCur;
		this.uniTotalDragPrev = this.uniTotalDragCur;
		this.uniMidFramePressed = false;
		this.uniMidFrameReleased = false;
		if (this.uniCur && this.pollUniReleasedInitial)
		{
			this.OnUniEnd(this.pollUniPosEnd, this.pollUniDeltaAccumAtEnd);
		}
		if (this.pollUniTouched)
		{
			this.OnUniStart(this.pollUniPosStart, this.pollUniPosCur);
		}
		if ((this.fingerA.touchId >= 0 || this.fingerB.touchId >= 0) != this.uniCur)
		{
			if (this.uniCur)
			{
				this.OnUniEnd(this.pollUniPosEnd, this.pollUniDeltaAccumAtEnd);
			}
			else
			{
				this.OnUniStart(this.pollUniPosStart, this.pollUniPosCur);
			}
		}
		this.uniMidFramePressed = (!this.pollUniInitialState && this.pollUniTouched && !this.uniCur);
		this.uniMidFrameReleased = (this.pollUniInitialState && this.pollUniReleasedInitial && this.uniCur);
		if (this.uniCur)
		{
			this.uniPosCur = this.pollUniPosCur;
		}
		this.uniTotalDragCur += this.pollUniDeltaAccum;
		this.UpdateUniTouchState(false);
		this.pollUniReleasedInitial = false;
		this.pollUniReleased = false;
		this.pollUniTouched = false;
		this.pollUniInitialState = this.uniCur;
		this.pollUniPosCur = (this.pollUniPosPrev = (this.pollUniPosStart = (this.pollUniPosEnd = this.uniPosCur)));
		this.pollUniWaitForDblStart = false;
		this.pollUniWaitForDblEnd = false;
		this.pollUniDeltaAccum = (this.pollUniDblEndPos = (this.pollUniDeltaAccumAtEnd = Vector2.zero));
		this.multiPrev = this.multiCur;
		this.multiPosPrev = this.multiPosCur;
		this.pinchPrevDist = this.pinchCurDist;
		this.twistPrevAbs = this.twistCurAbs;
		this.twistPrev = this.twistCur;
		this.multiMidFramePressed = false;
		this.multiMidFrameReleased = false;
		if (this.multiCur && this.pollMultiReleasedInitial)
		{
			this.OnMultiEnd(this.pollMultiPosEnd);
		}
		if (this.pollMultiTouched)
		{
			this.OnMultiStart(this.pollMultiPosStart, this.pollMultiPosCur);
		}
		if ((this.fingerA.touchId >= 0 && this.fingerB.touchId >= 0) != this.multiCur)
		{
			if (this.multiCur)
			{
				this.OnMultiEnd(this.pollMultiPosEnd);
			}
			else
			{
				this.OnMultiStart(this.pollMultiPosStart, this.pollMultiPosCur);
			}
		}
		this.multiMidFramePressed = (!this.pollMultiInitialState && this.pollMultiTouched && !this.multiCur);
		this.multiMidFrameReleased = (this.pollMultiInitialState && this.pollMultiReleasedInitial && this.multiCur);
		if (this.multiCur)
		{
			this.multiPosCur = this.pollMultiPosCur;
		}
		this.UpdateMultiTouchState(false);
		this.pollMultiReleasedInitial = false;
		this.pollMultiReleased = false;
		this.pollMultiTouched = false;
		this.pollMultiInitialState = this.multiCur;
		this.pollMultiPosCur = (this.pollMultiPosStart = (this.pollMultiPosEnd = this.multiPosCur));
		this.justMultiDoubleTapped = false;
		this.justMultiTapped = false;
		this.justMultiDelayTapped = false;
		if (this.JustMultiReleased(true, true))
		{
			if (!this.endedMultiMoved && this.endedMultiEndTime - this.endedMultiStartTime <= this.joy.touchTapMaxTime)
			{
				bool flag = this.nextTapCanBeMultiDoubleTap && this.endedMultiStartTime - this.lastMultiTapTime <= this.joy.doubleTapMaxGapTime;
				this.waitForMultiDelyedTap = !flag;
				this.justMultiDoubleTapped = flag;
				this.justMultiTapped = true;
				this.lastMultiTapPos = this.endedMultiPosStart;
				this.lastMultiTapTime = this.joy.curTime;
				this.nextTapCanBeMultiDoubleTap = !flag;
				this.fingerA.CancelTap();
				this.fingerB.CancelTap();
			}
			else
			{
				this.waitForMultiDelyedTap = false;
				this.nextTapCanBeMultiDoubleTap = false;
			}
		}
		else if (this.JustMultiPressed(true, true))
		{
			this.waitForMultiDelyedTap = false;
		}
		else if (this.waitForMultiDelyedTap && this.joy.curTime - this.lastMultiTapTime > this.joy.doubleTapMaxGapTime)
		{
			this.justMultiDelayTapped = true;
			this.waitForMultiDelyedTap = false;
			this.nextTapCanBeMultiDoubleTap = true;
		}
		if (this.emulateMouse)
		{
			this.joy.SetInternalMousePos((!this.mousePosFromFirstFinger) ? this.GetUniPos(TouchCoordSys.SCREEN_PX) : this.GetPos(0, TouchCoordSys.SCREEN_PX), true);
		}
		if (this.uniCur != this.uniPrev && this.enabled)
		{
			if (this.uniCur)
			{
				this.AnimateParams((!this.overrideScale) ? this.joy.pressedZoneScale : this.pressedScale, (!this.overrideColors) ? this.joy.defaultPressedZoneColor : this.pressedColor, (!this.overrideAnimDuration) ? this.joy.pressAnimDuration : this.pressAnimDuration);
			}
			else
			{
				this.AnimateParams((!this.overrideScale) ? this.joy.releasedZoneScale : this.releasedScale, (!this.overrideColors) ? this.joy.defaultReleasedZoneColor : this.releasedColor, (!this.touchCanceled) ? ((!this.overrideAnimDuration) ? this.joy.releaseAnimDuration : this.releaseAnimDuration) : this.joy.cancelAnimDuration);
			}
		}
		if (this.animTimer.Enabled)
		{
			this.animTimer.Update(this.joy.deltaTime);
			float lerpt = TouchController.SlowDownEase(this.animTimer.Nt);
			this.animColor.Update(lerpt);
			this.animScale.Update(lerpt);
			if (this.animTimer.Completed)
			{
				this.animTimer.Disable();
			}
		}
	}

	public override void OnPostUpdate(bool firstUpdate)
	{
		this.fingerA.PostUpdate(firstUpdate);
		this.fingerB.PostUpdate(firstUpdate);
	}

	public override void OnLayoutAddContent()
	{
		if (this.shape == TouchController.ControlShape.SCREEN_REGION)
		{
			return;
		}
		TouchController.LayoutBox layoutBox = this.joy.layoutBoxes[this.layoutBoxId];
		TouchController.ControlShape controlShape = this.shape;
		if (controlShape != TouchController.ControlShape.CIRCLE)
		{
			if (controlShape == TouchController.ControlShape.RECT)
			{
				layoutBox.AddContent(this.posCm, this.sizeCm);
			}
		}
		else
		{
			layoutBox.AddContent(this.posCm, this.sizeCm.x);
		}
	}

	public override void OnLayout()
	{
		switch (this.shape)
		{
		case TouchController.ControlShape.CIRCLE:
		case TouchController.ControlShape.RECT:
		{
			TouchController.LayoutBox layoutBox = this.joy.layoutBoxes[this.layoutBoxId];
			this.layoutPosPx = layoutBox.GetScreenPos(this.posCm);
			this.layoutSizePx = layoutBox.GetScreenSize(this.sizeCm);
			this.layoutRectPx = new Rect(this.layoutPosPx.x - 0.5f * this.layoutSizePx.x, this.layoutPosPx.y - 0.5f * this.layoutSizePx.y, this.layoutSizePx.x, this.layoutSizePx.y);
			break;
		}
		case TouchController.ControlShape.SCREEN_REGION:
			this.layoutRectPx = this.joy.NormalizedRectToPx(this.regionRect, true);
			this.layoutPosPx = this.layoutRectPx.center;
			this.layoutSizePx.x = this.layoutRectPx.width;
			this.layoutSizePx.y = this.layoutRectPx.height;
			this.screenRectPx = this.layoutRectPx;
			break;
		}
		this.posPx = this.layoutPosPx;
		this.sizePx = this.layoutSizePx;
		this.screenRectPx = this.layoutRectPx;
		this.OnReset();
	}

	public override void DrawGUI()
	{
		if (this.disableGui)
		{
			return;
		}
		bool flag = this.UniPressed(true, false);
		Texture2D texture2D = (!flag) ? this.releasedImg : this.pressedImg;
		if (texture2D != null)
		{
			GUI.depth = this.joy.guiDepth + this.guiDepth + ((!flag) ? 0 : this.joy.guiPressedOfs);
			Rect displayRect = this.GetDisplayRect(true);
			GUI.color = TouchController.ScaleAlpha(this.animColor.cur, this.joy.GetAlpha());
			GUI.DrawTexture(displayRect, texture2D);
		}
	}

	public override void TakeoverTouches(TouchableControl controlToUntouch)
	{
		if (controlToUntouch != null)
		{
			if (this.fingerA.touchId >= 0)
			{
				controlToUntouch.OnTouchEnd(this.fingerA.touchId, true);
			}
			if (this.fingerB.touchId >= 0)
			{
				controlToUntouch.OnTouchEnd(this.fingerB.touchId, true);
			}
		}
	}

	public bool MultiTouchPossible()
	{
		return this.enableSecondFinger && this.fingerA.touchId >= 0 && this.fingerB.touchId < 0 && (!this.strictTwoFingerStart || this.joy.curTime - this.fingerA.startTime < this.joy.strictMultiFingerMaxTime);
	}

	public override TouchController.HitTestResult HitTest(Vector2 touchPos, int touchId)
	{
		if (!this.enabled || !this.visible || (this.fingerA.touchId >= 0 && (!this.enableSecondFinger || this.fingerB.touchId >= 0 || (this.strictTwoFingerStart && !this.fingerA.pollTouched && this.joy.curTime - this.fingerA.startTime > this.joy.strictMultiFingerMaxTime))) || touchId == this.fingerA.touchId || touchId == this.fingerB.touchId)
		{
			return new TouchController.HitTestResult(false);
		}
		TouchController.HitTestResult result;
		switch (this.shape)
		{
		case TouchController.ControlShape.CIRCLE:
			result = this.joy.HitTestCircle(this.posPx, 0.5f * this.sizePx.x, touchPos, true);
			break;
		case TouchController.ControlShape.RECT:
			result = this.joy.HitTestBox(this.posPx, this.sizePx, touchPos, true);
			break;
		case TouchController.ControlShape.SCREEN_REGION:
			result = this.joy.HitTestRect(this.screenRectPx, touchPos, true);
			break;
		default:
			result = new TouchController.HitTestResult(false);
			break;
		}
		result.prio = this.prio;
		result.distScale = this.hitDistScale;
		return result;
	}

	public override TouchController.EventResult OnTouchStart(int touchId, Vector2 pos)
	{
		TouchZone.Finger finger = (this.fingerA.touchId >= 0) ? ((this.fingerB.touchId >= 0) ? null : this.fingerB) : this.fingerA;
		if (finger == null)
		{
			return TouchController.EventResult.NOT_HANDLED;
		}
		this.touchCanceled = false;
		TouchZone.Finger finger2 = (finger != this.fingerA) ? this.fingerA : this.fingerB;
		finger.touchId = touchId;
		finger.touchVerified = true;
		finger.touchPos = pos;
		finger.pollTouched = true;
		finger.pollPosStart = pos;
		finger.pollPosCur = pos;
		if (finger2.touchId < 0)
		{
			this.pollUniTouched = true;
			this.pollUniPosStart = pos;
			this.pollUniPosCur = pos;
			this.pollUniWaitForDblStart = true;
			this.pollUniWaitForDblEnd = false;
			this.pollUniDeltaAccum = Vector2.zero;
		}
		else
		{
			finger2.CancelTap();
			this.pollMultiTouched = true;
			this.pollMultiPosStart = (this.pollMultiPosCur = (this.fingerA.pollPosCur + this.fingerB.pollPosCur) / 2f);
			this.pollUniPosCur = this.pollMultiPosCur;
			if (this.pollUniWaitForDblStart)
			{
				this.pollUniPosStart = this.pollUniPosCur;
				this.pollUniWaitForDblStart = false;
				this.pollUniWaitForDblEnd = true;
			}
		}
		this.pollUniPosPrev = this.pollUniPosCur;
		return (!this.nonExclusiveTouches) ? TouchController.EventResult.HANDLED : TouchController.EventResult.SHARED;
	}

	public override TouchController.EventResult OnTouchEnd(int touchId, bool canceled = false)
	{
		TouchZone.Finger finger = (this.fingerA.touchId != touchId) ? ((this.fingerB.touchId != touchId) ? null : this.fingerB) : this.fingerA;
		if (finger == null)
		{
			return TouchController.EventResult.NOT_HANDLED;
		}
		TouchZone.Finger finger2 = (finger != this.fingerA) ? this.fingerA : this.fingerB;
		finger.touchId = -1;
		finger.touchVerified = true;
		if (!finger.pollReleased)
		{
			finger.pollReleased = true;
			finger.pollPosEnd = finger.pollPosCur;
			if (finger.pollInitialState)
			{
				finger.pollReleasedInitial = true;
			}
		}
		finger.pollTouched = false;
		if (finger2.touchId >= 0)
		{
			this.pollUniPosCur = finger2.pollPosCur;
			this.pollUniWaitForDblEnd = true;
			this.pollUniDblEndPos = (this.fingerA.pollPosCur + this.fingerB.pollPosCur) / 2f;
		}
		else
		{
			if (!this.pollUniReleased)
			{
				this.pollUniReleased = true;
				if (this.pollUniWaitForDblEnd)
				{
					this.pollUniPosEnd = this.pollUniDblEndPos;
					this.pollUniWaitForDblEnd = false;
				}
				else
				{
					this.pollUniPosEnd = this.pollUniPosCur;
				}
				this.pollUniDeltaAccumAtEnd = this.pollUniDeltaAccum;
				this.pollUniDeltaAccum = Vector2.zero;
				if (this.pollUniInitialState)
				{
					this.pollUniReleasedInitial = true;
				}
			}
			this.pollUniTouched = false;
		}
		this.pollUniPosPrev = this.pollUniPosCur;
		if (finger2.touchId >= 0)
		{
			if (!this.pollMultiReleased)
			{
				this.pollMultiReleased = true;
				this.pollMultiPosEnd = this.pollMultiPosCur;
				if (this.pollMultiInitialState)
				{
					this.pollMultiReleasedInitial = true;
				}
			}
			this.pollMultiTouched = false;
		}
		return (!this.nonExclusiveTouches) ? TouchController.EventResult.HANDLED : TouchController.EventResult.SHARED;
	}

	public override TouchController.EventResult OnTouchMove(int touchId, Vector2 pos)
	{
		TouchZone.Finger finger = (this.fingerA.touchId != touchId) ? ((this.fingerB.touchId != touchId) ? null : this.fingerB) : this.fingerA;
		if (finger == null)
		{
			return TouchController.EventResult.NOT_HANDLED;
		}
		TouchZone.Finger finger2 = (finger != this.fingerA) ? this.fingerA : this.fingerB;
		finger.touchVerified = true;
		finger.pollPosCur = pos;
		if (finger2.touchId >= 0)
		{
			this.pollMultiPosCur = (this.fingerA.pollPosCur + this.fingerB.pollPosCur) / 2f;
			this.pollUniPosCur = this.pollMultiPosCur;
		}
		else
		{
			this.pollUniPosCur = pos;
		}
		if (this.pollUniPosCur != this.pollUniPosPrev)
		{
			this.pollUniWaitForDblEnd = false;
			this.pollUniWaitForDblStart = false;
			this.pollUniDeltaAccum += this.pollUniPosCur - this.pollUniPosPrev;
			this.pollUniPosPrev = this.pollUniPosCur;
		}
		return (!this.nonExclusiveTouches) ? TouchController.EventResult.HANDLED : TouchController.EventResult.SHARED;
	}

	private Vector2 GetCenterPos()
	{
		return (this.fingerA.posCur + this.fingerB.posCur) * 0.5f;
	}

	private float GetFingerDist()
	{
		return Mathf.Max(2f, Vector2.Distance(this.fingerA.posCur, this.fingerB.posCur));
	}

	private float GetFingerAbsAngle(float lastAngle = 0f)
	{
		Vector2 vector = this.fingerB.posCur - this.fingerA.posCur;
		if (vector.sqrMagnitude < 1E-05f)
		{
			return lastAngle;
		}
		vector.Normalize();
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	private Vector2 TransformPos(Vector2 screenPosPx, TouchCoordSys posType, bool deltaMode)
	{
		Vector2 vector = screenPosPx;
		if (!deltaMode && (posType == TouchCoordSys.LOCAL_CM || posType == TouchCoordSys.LOCAL_INCH || posType == TouchCoordSys.LOCAL_NORMALIZED || posType == TouchCoordSys.LOCAL_PX))
		{
			vector.x -= this.screenRectPx.xMin;
			vector.y -= this.screenRectPx.yMin;
		}
		switch (posType)
		{
		case TouchCoordSys.SCREEN_PX:
		case TouchCoordSys.LOCAL_PX:
			return vector;
		case TouchCoordSys.SCREEN_NORMALIZED:
			vector.x /= this.joy.GetScreenWidth();
			vector.y /= this.joy.GetScreenHeight();
			return vector;
		case TouchCoordSys.SCREEN_CM:
		case TouchCoordSys.LOCAL_CM:
			return vector / this.joy.GetDPCM();
		case TouchCoordSys.SCREEN_INCH:
		case TouchCoordSys.LOCAL_INCH:
			return vector / this.joy.GetDPI();
		case TouchCoordSys.LOCAL_NORMALIZED:
			vector.x /= this.screenRectPx.width;
			vector.y /= this.screenRectPx.height;
			return vector;
		default:
			return vector;
		}
	}

	private float TransformPos(float screenPosPx, TouchCoordSys posType)
	{
		switch (posType)
		{
		case TouchCoordSys.SCREEN_PX:
		case TouchCoordSys.LOCAL_PX:
			return screenPosPx;
		case TouchCoordSys.SCREEN_NORMALIZED:
			return screenPosPx / Mathf.Max(this.joy.GetScreenWidth(), this.joy.GetScreenHeight());
		case TouchCoordSys.SCREEN_CM:
		case TouchCoordSys.LOCAL_CM:
			return screenPosPx / this.joy.GetDPCM();
		case TouchCoordSys.SCREEN_INCH:
		case TouchCoordSys.LOCAL_INCH:
			return screenPosPx / this.joy.GetDPI();
		case TouchCoordSys.LOCAL_NORMALIZED:
			return screenPosPx / this.screenRectPx.width;
		default:
			return screenPosPx;
		}
	}

	private float TransformPosX(float screenPosPx, TouchCoordSys posType)
	{
		switch (posType)
		{
		case TouchCoordSys.SCREEN_PX:
		case TouchCoordSys.LOCAL_PX:
			return screenPosPx;
		case TouchCoordSys.SCREEN_NORMALIZED:
			return screenPosPx / this.joy.GetScreenWidth();
		case TouchCoordSys.SCREEN_CM:
		case TouchCoordSys.LOCAL_CM:
			return screenPosPx / this.joy.GetDPCM();
		case TouchCoordSys.SCREEN_INCH:
		case TouchCoordSys.LOCAL_INCH:
			return screenPosPx / this.joy.GetDPI();
		case TouchCoordSys.LOCAL_NORMALIZED:
			return screenPosPx / this.screenRectPx.width;
		default:
			return screenPosPx;
		}
	}

	private float TransformPosY(float screenPosPx, TouchCoordSys posType)
	{
		switch (posType)
		{
		case TouchCoordSys.SCREEN_PX:
		case TouchCoordSys.LOCAL_PX:
			return screenPosPx;
		case TouchCoordSys.SCREEN_NORMALIZED:
			return screenPosPx / this.joy.GetScreenHeight();
		case TouchCoordSys.SCREEN_CM:
		case TouchCoordSys.LOCAL_CM:
			return screenPosPx / this.joy.GetDPCM();
		case TouchCoordSys.SCREEN_INCH:
		case TouchCoordSys.LOCAL_INCH:
			return screenPosPx / this.joy.GetDPI();
		case TouchCoordSys.LOCAL_NORMALIZED:
			return screenPosPx / this.screenRectPx.height;
		default:
			return screenPosPx;
		}
	}

	public static bool PxPosEquals(Vector2 p0, Vector2 p1)
	{
		return (p0 - p1).sqrMagnitude < 0.1f;
	}

	public static bool PxDistEquals(float d0, float d1)
	{
		return Mathf.Abs(d0 - d1) < 0.1f;
	}

	public static bool TwistAngleEquals(float a0, float a1)
	{
		return Mathf.Abs(Mathf.DeltaAngle(a0, a1)) < 0.5f;
	}

	private const float MIN_PINCH_DIST_PX = 2f;

	private const float PIXEL_POS_EPSILON_SQR = 0.1f;

	private const float PIXEL_DIST_EPSILON = 0.1f;

	private const float TWIST_ANGLE_EPSILON = 0.5f;

	public const int BOX_LEFT = 1;

	public const int BOX_CEN = 2;

	public const int BOX_RIGHT = 4;

	public const int BOX_TOP = 8;

	public const int BOX_MID = 16;

	public const int BOX_BOTTOM = 32;

	public const int BOX_TOP_LEFT = 9;

	public const int BOX_TOP_CEN = 10;

	public const int BOX_TOP_RIGHT = 12;

	public const int BOX_MID_LEFT = 17;

	public const int BOX_MID_CEN = 18;

	public const int BOX_MID_RIGHT = 20;

	public const int BOX_BOTTOM_LEFT = 33;

	public const int BOX_BOTTOM_CEN = 34;

	public const int BOX_BOTTOM_RIGHT = 36;

	public const int BOX_H_MASK = 7;

	public const int BOX_V_MASK = 56;

	public TouchController.ControlShape shape;

	public Vector2 posCm;

	public Vector2 sizeCm;

	public Rect regionRect;

	private Vector2 posPx;

	private Vector2 sizePx;

	private Vector2 layoutPosPx;

	private Vector2 layoutSizePx;

	private Rect screenRectPx;

	private Rect layoutRectPx;

	public bool enableSecondFinger;

	public bool nonExclusiveTouches;

	public bool strictTwoFingerStart;

	public bool freezeTwistWhenTooClose;

	public bool noPinchAfterDrag;

	public bool noPinchAfterTwist;

	public bool noTwistAfterDrag;

	public bool noTwistAfterPinch;

	public bool noDragAfterPinch;

	public bool noDragAfterTwist;

	public bool startPinchWhenTwisting;

	public bool startPinchWhenDragging;

	public bool startDragWhenPinching;

	public bool startDragWhenTwisting;

	public bool startTwistWhenDragging;

	public bool startTwistWhenPinching;

	public TouchZone.GestureDetectionOrder gestureDetectionOrder;

	public KeyCode debugKey;

	public Texture2D releasedImg;

	public Texture2D pressedImg;

	public bool overrideScale;

	public float releasedScale;

	public float pressedScale;

	public float disabledScale;

	public bool overrideColors;

	public Color releasedColor;

	public Color pressedColor;

	public Color disabledColor;

	public bool overrideAnimDuration;

	public float pressAnimDuration;

	public float releaseAnimDuration;

	public float disableAnimDuration;

	public float enableAnimDuration;

	public float showAnimDuration;

	public float hideAnimDuration;

	private AnimTimer animTimer;

	private TouchController.AnimFloat animScale;

	private TouchController.AnimColor animColor;

	private TouchZone.Finger fingerA;

	private TouchZone.Finger fingerB;

	private bool multiCur;

	private bool multiPrev;

	private bool multiMoved;

	private bool multiJustMoved;

	private bool multiMidFrameReleased;

	private bool multiMidFramePressed;

	private float multiStartTime;

	private Vector2 multiPosCur;

	private Vector2 multiPosPrev;

	private Vector2 multiPosStart;

	private float multiExtremeCurDist;

	private Vector2 multiExtremeCurVec;

	private float multiLastMoveTime;

	private Vector2 multiDragVel;

	private bool justMultiTapped;

	private bool justMultiDoubleTapped;

	private bool justMultiDelayTapped;

	private bool waitForMultiDelyedTap;

	private float lastMultiTapTime;

	private bool nextTapCanBeMultiDoubleTap;

	private Vector2 lastMultiTapPos;

	[NonSerialized]
	private float twistStartAbs;

	[NonSerialized]
	private float twistCurAbs;

	[NonSerialized]
	private float twistPrevAbs;

	[NonSerialized]
	private float twistCur;

	[NonSerialized]
	private float twistPrev;

	[NonSerialized]
	private float twistCurRaw;

	[NonSerialized]
	private float twistStartRaw;

	[NonSerialized]
	private float twistExtremeCur;

	[NonSerialized]
	private float twistLastMoveTime;

	[NonSerialized]
	private float twistVel;

	[NonSerialized]
	private float pinchDistStart;

	[NonSerialized]
	private float pinchCurDist;

	[NonSerialized]
	private float pinchPrevDist;

	[NonSerialized]
	private float pinchExtremeCurDist;

	[NonSerialized]
	private float pinchLastMoveTime;

	[NonSerialized]
	private float pinchDistVel;

	private bool endedMultiMoved;

	private bool endedTwistMoved;

	private bool endedPinchMoved;

	private float endedMultiStartTime;

	private float endedMultiEndTime;

	private float endedPinchDistStart;

	private float endedPinchDistEnd;

	private float endedPinchDistVel;

	private float endedTwistAngle;

	private float endedTwistVel;

	private Vector2 endedMultiPosStart;

	private Vector2 endedMultiPosEnd;

	private Vector2 endedMultiDragVel;

	private bool pollMultiInitialState;

	private bool pollMultiReleasedInitial;

	private bool pollMultiTouched;

	private bool pollMultiReleased;

	private Vector2 pollMultiPosEnd;

	private Vector2 pollMultiPosStart;

	private Vector2 pollMultiPosCur;

	private bool twistMoved;

	private bool twistJustMoved;

	private bool pinchMoved;

	private bool pinchJustMoved;

	private bool uniMoved;

	private bool uniJustMoved;

	private bool uniCur;

	private bool uniPrev;

	private float uniStartTime;

	private Vector2 uniPosCur;

	private Vector2 uniPosStart;

	private Vector2 uniTotalDragCur;

	private Vector2 uniTotalDragPrev;

	private float uniExtremeDragCurDist;

	private Vector2 uniExtremeDragCurVec;

	private float uniLastMoveTime;

	private Vector2 uniDragVel;

	private float endedUniStartTime;

	private float endedUniEndTime;

	private Vector2 endedUniPosStart;

	private Vector2 endedUniPosEnd;

	private Vector2 endedUniTotalDrag;

	private Vector2 endedUniDragVel;

	private bool endedUniMoved;

	private bool uniMidFrameReleased;

	private bool uniMidFramePressed;

	private bool pollUniInitialState;

	private bool pollUniReleasedInitial;

	private bool pollUniTouched;

	private bool pollUniReleased;

	private bool pollUniWaitForDblStart;

	private bool pollUniWaitForDblEnd;

	private Vector2 pollUniPosEnd;

	private Vector2 pollUniPosStart;

	private Vector2 pollUniPosCur;

	private Vector2 pollUniPosPrev;

	private Vector2 pollUniDeltaAccum;

	private Vector2 pollUniDblEndPos;

	private Vector2 pollUniDeltaAccumAtEnd;

	private bool touchCanceled;

	public bool enableGetKey;

	public KeyCode getKeyCode;

	public KeyCode getKeyCodeAlt;

	public KeyCode getKeyCodeMulti;

	public KeyCode getKeyCodeMultiAlt;

	public bool enableGetButton;

	public string getButtonName;

	public string getButtonMultiName;

	public bool emulateMouse;

	public bool mousePosFromFirstFinger;

	public bool enableGetAxis;

	public string axisHorzName;

	public string axisVertName;

	public bool axisHorzFlip;

	public bool axisVertFlip;

	public float axisValScale;

	public bool codeUniJustPressed;

	public bool codeUniPressed;

	public bool codeUniJustReleased;

	public bool codeUniJustDragged;

	public bool codeUniDragged;

	public bool codeUniReleasedDrag;

	public bool codeMultiJustPressed;

	public bool codeMultiPressed;

	public bool codeMultiJustReleased;

	public bool codeMultiJustDragged;

	public bool codeMultiDragged;

	public bool codeMultiReleasedDrag;

	public bool codeJustTwisted;

	public bool codeTwisted;

	public bool codeReleasedTwist;

	public bool codeJustPinched;

	public bool codePinched;

	public bool codeReleasedPinch;

	public bool codeSimpleTap;

	public bool codeSingleTap;

	public bool codeDoubleTap;

	public bool codeSimpleMultiTap;

	public bool codeMultiSingleTap;

	public bool codeMultiDoubleTap;

	public bool codeCustomGUI;

	public bool codeCustomLayout;

	public enum GestureDetectionOrder
	{
		TWIST_PINCH_DRAG,
		TWIST_DRAG_PINCH,
		PINCH_TWIST_DRAG,
		PINCH_DRAG_TWIST,
		DRAG_TWIST_PINCH,
		DRAG_PINCH_TWIST
	}

	private class Finger
	{
		public Finger(TouchZone tzone)
		{
			this.zone = tzone;
			this.Reset();
		}

		public bool JustPressed(bool includeMidFramePress, bool includeMidFrameRelease)
		{
			return (this.curState && !this.prevState) || (includeMidFramePress && this.midFramePressed) || (includeMidFrameRelease && this.midFrameReleased);
		}

		public bool JustReleased(bool includeMidFramePress, bool includeMidFrameRelease)
		{
			return (!this.curState && this.prevState) || (includeMidFramePress && this.midFramePressed) || (includeMidFrameRelease && this.midFrameReleased);
		}

		public bool Pressed(bool includeMidFramePress, bool returnFalseOnMidFrameRelease)
		{
			return (this.curState || (includeMidFramePress && this.midFramePressed)) && (!returnFalseOnMidFrameRelease || !this.midFrameReleased);
		}

		public bool JustTapped(bool onlyOnce = false)
		{
			return (!onlyOnce) ? this.justTapped : this.justDelayTapped;
		}

		public bool JustDoubleTapped()
		{
			return this.justDoubleTapped;
		}

		public Vector2 GetTapPos(TouchCoordSys cs)
		{
			return this.zone.TransformPos(this.lastTapPos, cs, false);
		}

		public void OnTouchStart(Vector2 startPos, Vector2 curPos)
		{
			this.startTime = this.zone.joy.curTime;
			this.startPos = startPos;
			this.posPrev = startPos;
			this.posCur = curPos;
			this.curState = true;
			this.tapCanceled = false;
			this.moved = false;
			this.justMoved = false;
			this.lastMoveTime = 0f;
			this.dragVel = Vector2.zero;
			this.dragVel = Vector2.zero;
			this.extremeDragCurVec = (this.extremeDragPrevVec = Vector2.zero);
			this.extremeDragCurDist = (this.extremeDragPrevDist = 0f);
		}

		public void OnTouchEnd(Vector2 endPos)
		{
			this.posCur = endPos;
			this.UpdateState(true);
			this.endedMoved = this.moved;
			this.endedStartTime = this.startTime;
			this.endedEndTime = this.zone.joy.curTime;
			this.endedPosStart = this.startPos;
			this.endedPosEnd = endPos;
			this.endedDragVel = this.dragVel;
			this.endedExtremeDragVec = this.extremeDragCurVec;
			this.endedExtremeDragDist = this.extremeDragCurDist;
			this.endedWasTapCanceled = this.tapCanceled;
			this.curState = false;
		}

		public void Reset()
		{
			this.touchId = -1;
			this.curState = false;
			this.prevState = false;
			this.moved = false;
			this.justMoved = false;
			this.touchVerified = true;
			this.dragVel = Vector2.zero;
			this.pollInitialState = false;
			this.pollReleasedInitial = false;
			this.pollTouched = false;
			this.pollReleased = false;
			this.tapCanceled = false;
			this.lastTapPos = Vector2.zero;
			this.lastTapTime = -100f;
			this.nextTapCanBeDoubleTap = false;
		}

		public void OnPrePoll()
		{
			this.touchVerified = false;
		}

		private void UpdateState(bool lastUpdateMode = false)
		{
			this.justMoved = false;
			Vector2 vector = this.posCur - this.startPos;
			this.extremeDragCurVec.x = Mathf.Max(Mathf.Abs(vector.x), this.extremeDragCurVec.x);
			this.extremeDragCurVec.y = Mathf.Max(Mathf.Abs(vector.y), this.extremeDragCurVec.y);
			this.extremeDragCurDist = Mathf.Max(vector.magnitude, this.extremeDragCurDist);
			if (!this.moved && this.extremeDragCurDist > this.zone.joy.touchTapMaxDistPx)
			{
				this.moved = true;
				this.justMoved = true;
			}
			if (lastUpdateMode)
			{
				return;
			}
			if (this.curState)
			{
				if (TouchZone.PxPosEquals(this.posCur, this.posPrev))
				{
					if (this.zone.joy.curTime - this.lastMoveTime > this.zone.joy.velPreserveTime)
					{
						this.dragVel = Vector2.zero;
					}
				}
				else
				{
					this.lastMoveTime = this.zone.joy.curTime;
					this.dragVel = (this.posCur - this.posPrev) * this.zone.joy.invDeltaTime;
				}
			}
			else
			{
				this.dragVel = Vector2.zero;
			}
		}

		public void PreUpdate(bool firstUpdate)
		{
			this.prevState = this.curState;
			this.posPrev = this.posCur;
			this.extremeDragPrevDist = this.extremeDragCurDist;
			this.extremeDragPrevVec = this.extremeDragCurVec;
			this.midFramePressed = false;
			this.midFrameReleased = false;
			if (this.curState && this.pollReleasedInitial)
			{
				this.OnTouchEnd(this.pollPosEnd);
			}
			if (this.pollTouched && (!this.pollInitialState || this.touchId >= 0))
			{
				this.OnTouchStart(this.pollPosStart, this.pollPosCur);
			}
			if (this.touchId >= 0 != this.curState)
			{
				if (this.curState)
				{
					this.OnTouchEnd(this.pollPosEnd);
				}
				else
				{
					this.OnTouchStart(this.pollPosStart, this.pollPosCur);
				}
			}
			if (this.touchId >= 0)
			{
				this.posCur = this.pollPosCur;
			}
			this.midFramePressed = (!this.pollInitialState && this.pollTouched && !this.curState);
			this.midFrameReleased = (this.pollInitialState && this.pollReleasedInitial && this.curState);
			this.UpdateState(false);
			this.justDelayTapped = false;
			this.justTapped = false;
			this.justDoubleTapped = false;
			if (this.JustReleased(true, true))
			{
				if (!this.endedMoved && !this.endedWasTapCanceled && this.zone.joy.curTime - this.endedStartTime <= this.zone.joy.touchTapMaxTime)
				{
					bool flag = this.nextTapCanBeDoubleTap && this.endedStartTime - this.lastTapTime <= this.zone.joy.doubleTapMaxGapTime;
					this.waitForDelyedTap = !flag;
					this.justDoubleTapped = flag;
					this.justTapped = true;
					this.lastTapPos = this.endedPosStart;
					this.lastTapTime = this.zone.joy.curTime;
					this.nextTapCanBeDoubleTap = !flag;
				}
				else
				{
					this.waitForDelyedTap = false;
					this.nextTapCanBeDoubleTap = false;
				}
			}
			else if (this.JustPressed(false, false))
			{
				this.waitForDelyedTap = false;
			}
			else if (this.waitForDelyedTap && this.zone.joy.curTime - this.lastTapTime > this.zone.joy.doubleTapMaxGapTime)
			{
				this.justDelayTapped = true;
				this.waitForDelyedTap = false;
				this.nextTapCanBeDoubleTap = false;
			}
			this.pollInitialState = this.curState;
			this.pollReleasedInitial = false;
			this.pollReleased = false;
			this.pollTouched = false;
			this.pollPosCur = this.posCur;
			this.pollPosStart = this.posCur;
			this.pollPosEnd = this.posCur;
		}

		public void PostUpdate(bool firstUpdate)
		{
		}

		public void CancelTap()
		{
			this.waitForDelyedTap = false;
			this.tapCanceled = true;
			this.nextTapCanBeDoubleTap = false;
		}

		private TouchZone zone;

		public int touchId;

		public bool touchVerified;

		public Vector2 touchPos;

		public Vector2 startPos;

		public Vector2 posPrev;

		public Vector2 posCur;

		public float startTime;

		public bool moved;

		public bool justMoved;

		public bool prevState;

		public bool curState;

		public Vector2 extremeDragCurVec;

		public Vector2 extremeDragPrevVec;

		public float extremeDragCurDist;

		public float extremeDragPrevDist;

		public float lastMoveTime;

		public Vector2 dragVel;

		public bool endedMoved;

		public bool endedWasTapCanceled;

		public float endedStartTime;

		public float endedEndTime;

		public Vector2 endedDragVel;

		public Vector2 endedPosStart;

		public Vector2 endedPosEnd;

		public Vector2 endedExtremeDragVec;

		public float endedExtremeDragDist;

		private bool justTapped;

		private bool justDoubleTapped;

		private bool justDelayTapped;

		private bool waitForDelyedTap;

		private float lastTapTime;

		private bool nextTapCanBeDoubleTap;

		private Vector2 lastTapPos;

		private bool tapCanceled;

		public bool midFrameReleased;

		public bool midFramePressed;

		public bool pollInitialState;

		public bool pollReleasedInitial;

		public bool pollTouched;

		public bool pollReleased;

		public Vector2 pollPosEnd;

		public Vector2 pollPosStart;

		public Vector2 pollPosCur;
	}
}
