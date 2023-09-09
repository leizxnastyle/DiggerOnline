using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	public static bool isTutorialEnded
	{
		get
		{
			return PlayerPrefs.GetInt("isTutorialEnded") > 0;
		}
		set
		{
			PlayerPrefs.SetInt("isTutorialEnded", (!value) ? 0 : 1);
		}
	}

	public static bool isTutorialMenuEnded
	{
		get
		{
			return PlayerPrefs.GetInt("isTutorialMenuEnded") > 0;
		}
		set
		{
			PlayerPrefs.SetInt("isTutorialMenuEnded", (!value) ? 0 : 1);
		}
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void WorldGameObject_isMapLoaded()
	{
	}

	private void TutorialHint_OnClose(bool isRunNext)
	{
	}

	public void ToNext()
	{
	}

	public void ToNextMenu()
	{
	}

	public void ToNextMenuUp()
	{
	}

	private void Start()
	{
		TutorialManager.IsStarted = false;
		if (TutorialManager.Inst == null)
		{
			TutorialManager.Inst = this;
		}
	}

	public void StartTutorial()
	{
	}

	private void SetTutorialStage(int stage)
	{
	}

	public void SetTutorialBlocked(bool isShow)
	{
	}

	public void SetTutorialMenuBlocked(bool isShow)
	{
	}

	private void OnLevelWasLoaded(int level)
	{
	}

	private List<Vector3> triggetPosition = new List<Vector3>
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(28f, 51f, 7f),
		new Vector3(39f, 53f, 7f),
		new Vector3(42f, 54f, 18f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(42f, 61f, 24f),
		new Vector3(0f, 0f, 0f),
		new Vector3(42f, 64f, 26f),
		new Vector3(42f, 64f, 33f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(42f, 68f, 38f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f)
	};

	private List<Vector3> triggetRotation = new List<Vector3>
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 0f)
	};

	public static TutorialManager Inst;

	public TutorialHint tutorialHint;

	public GameObject tutorialTriger;

	public GameObject leaderShadow;

	public GameObject cubeShadow;

	public GameObject stairShadow;

	public GameObject tutorialBlocked;

	public GameObject tutorialMenuBlocked;

	public List<GameObject> instObject = new List<GameObject>();

	private TuttorialTrigger tutorialTriggerInst;

	public static int curentTutorialStep;

	private string tutorialMap = "http://95.211.210.142/standart_maps/tutorial_map_v2.map";

	public static bool IsStarted;

	public static bool hintIsShow;

	public static string ItemAded = string.Empty;

	public static bool isTutorialClosed;
}
