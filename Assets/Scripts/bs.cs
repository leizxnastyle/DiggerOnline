using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Photon;
using UnityEngine;

public class bs : Photon.MonoBehaviour
{
	public static Igor _Igor
	{
		get
		{
			return Igor.Instance;
		}
	}

	public static WorldGameObjectX _WorldGameObjectX
	{
		get
		{
			return WorldGameObjectX.Instance;
		}
	}

	public static MainMenu _MainMenu
	{
		get
		{
			return MainMenu.Instance;
		}
	}

	public static World _World
	{
		get
		{
			return World.Instance;
		}
	}

	public static IEnumerator AddUpdate(Func<bool> y)
	{
		yield return null;
		while (y())
		{
			yield return null;
		}
		yield break;
	}

	public static bool DebugKey(KeyCode key)
	{
		return bs._Igor.debug && UnityEngine.Input.GetKey(KeyCode.LeftShift) && UnityEngine.Input.GetKeyDown(key);
	}

	public static IEnumerator AddMethod(Func<bool> y, Action a)
	{
		yield return null;
		while (!y())
		{
			yield return null;
		}
		a();
		yield break;
	}

	public static IEnumerator AddMethod(float sec, Action a)
	{
		yield return new WaitForSeconds(sec);
		a();
		yield break;
	}

	public static IEnumerator AddMethod(Action a)
	{
		yield return null;
		a();
		yield break;
	}

	public static IEnumerable<Transform> GetTransforms(Transform ts)
	{
		yield return ts;
		foreach (object obj in ts)
		{
			Transform t = (Transform)obj;
			foreach (Transform t2 in bs.GetTransforms(t))
			{
				yield return t2;
			}
		}
		yield break;
	}

	public static T Log<T>(T s)
	{
		bs.stringBuilder.AppendLine(s.ToString());
		return s;
	}

	public Vector3 ZeroY(Vector3 v)
	{
		v.y = 0f;
		return v;
	}

	public Vector3 pos
	{
		get
		{
			return base.transform.position;
		}
	}

	public Quaternion rot
	{
		get
		{
			return base.transform.rotation;
		}
	}

	public static void Log(params object[] s)
	{
		foreach (object arg in s)
		{
			bs.stringBuilder.Append(arg + "\t");
		}
		bs.stringBuilder.AppendLine();
	}

	public void PlayOneShot(AudioClip audioClip)
	{
		if (WorldGameObjectX.Instance.IsWorldGenerated)
		{
			WorldGameObjectX.Instance.MainPlayerEyes.GetComponent<AudioSource>().PlayOneShot(audioClip, ProfileINI.sound_volume * ProfileINI.sound_scale);
		}
	}

	public int GetInt(string s, int def = 0)
	{
		int num;
		if (!this.hackDictInt.TryGetValue(s, out num))
		{
			return def;
		}
		return num - bs.rndhack;
	}

	public void SetInt(string s, int value)
	{
		this.hackDictInt[s] = value + bs.rndhack;
	}

	public float Getfloat(string s, float def = 0f)
	{
		float num;
		if (!this.hackDictfloat.TryGetValue(s, out num))
		{
			return def;
		}
		return num - (float)bs.rndhack;
	}

	public void Setfloat(string s, float value)
	{
		this.hackDictfloat[s] = value + (float)bs.rndhack;
	}

	public static float immportalTime = 3f;

	public static StringBuilder stringBuilder = new StringBuilder();

	public static int rndhack = new System.Random().Next(50, 1000);

	private Dictionary<string, int> hackDictInt = new Dictionary<string, int>();

	private Dictionary<string, float> hackDictfloat = new Dictionary<string, float>();
}
