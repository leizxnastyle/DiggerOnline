using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("NGUI/Examples/Load Level On Click")]
public class LoadLevelOnClick : MonoBehaviour
{
	private void OnClick()
	{
		if (!string.IsNullOrEmpty(this.levelName))
		{
			SceneManager.LoadScene(this.levelName);
		}
	}

	public string levelName;
}
