using System;
using UnityEngine;

public class ErrorUI : MonoBehaviour
{
	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
		if (ErrorUI.Instance == null)
		{
			ErrorUI.Instance = this;
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	private static ErrorUI Instance;
}
