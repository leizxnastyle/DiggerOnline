using System;
using System.Collections;
using UnityEngine;

public class TutorialHint : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && this.canClose)
		{
			this.infoText.text = string.Empty;
			base.gameObject.SetActive(false);
			TutorialManager.hintIsShow = false;
			this.colider.enabled = false;
			if (this.step == 22)
			{
				TutorialManager.Inst.SetTutorialBlocked(true);
			}
			if (this.OnCloseCallbeck != null)
			{
				this.OnCloseCallbeck(this.runNext);
			}
		}
	}

	public void Show(int tutorialStep, bool isNext, Action<bool> cc)
	{
	}

	private IEnumerator WaitTime()
	{
		yield return new WaitForSeconds((float)this.showTime);
		this.canClose = true;
		yield break;
	}

	public UILabel infoText;

	public BoxCollider colider;

	public GameObject shopButton;

	private int showTime = 1;

	private bool canClose = true;

	private bool runNext;

	private int step;

	private Action<bool> OnCloseCallbeck;
}
