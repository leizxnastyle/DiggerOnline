using System;
using System.Collections;
using UnityEngine;

public class fw1Sound : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(this.sdeleyTime);
		NGUITools.PlaySound(this.startAC);
		yield return new WaitForSeconds(this.startTime);
		NGUITools.PlaySound(this.boomAC);
		yield return new WaitForSeconds(this.boomTime);
		NGUITools.PlaySound(this.endAC);
		yield return new WaitForSeconds(0.2f);
		NGUITools.PlaySound(this.endAC2);
		yield break;
	}

	public float sdeleyTime;

	public AudioClip startAC;

	public float startTime;

	public AudioClip boomAC;

	public float boomTime;

	public AudioClip endAC;

	public AudioClip endAC2;
}
