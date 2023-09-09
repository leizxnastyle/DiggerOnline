using System;
using System.Collections;
using UnityEngine;

public class KickReasonChecker : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(1f);
		yield break;
	}
}
