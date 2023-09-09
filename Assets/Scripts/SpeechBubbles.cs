using System;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubbles : MonoBehaviour
{
	public static void AddText(GameObject author, string text)
	{
		if (SpeechBubbles._Instance != null && ProfileINI.showBaloons && (ProfileINI.showSelfBaloons || author != WorldGameObjectX.Instance.MainPlayer))
		{
			SpeechBubbles._Instance.Add(author, text, null);
		}
	}

	public static void AddEmotion(GameObject author, string emotion)
	{
		if (SpeechBubbles._Instance != null)
		{
			SpeechBubbles._Instance.Add(author, null, emotion);
		}
	}

	private void Add(GameObject author, string text, string emotion)
	{
		for (int i = 0; i < this._SpeechBubbles.Count; i++)
		{
			if (this._SpeechBubbles[i].Author == author)
			{
				UnityEngine.Object.Destroy(this._SpeechBubbles[i].GUIObject);
				this._SpeechBubbles.RemoveAt(i);
				break;
			}
		}
		if (author == WorldGameObjectX.Instance.MainPlayer && this._EmotionImage.activeInHierarchy)
		{
			Utils.SetActiveRecursively(this._EmotionImage, false);
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this._GUIObjectPrototype);
		Utils.SetActiveRecursively(gameObject, true);
		UILabel component = gameObject.transform.Find("txt_text").GetComponent<UILabel>();
		UISprite component2 = gameObject.transform.Find("bubble").GetComponent<UISprite>();
		UISprite component3 = gameObject.transform.Find("bubble_cap").GetComponent<UISprite>();
		Transform transform = gameObject.transform.Find("emotion_image");
		if (text != null)
		{
			component.text = text;
			text = component.processedText;
			component.text = text;
			float num = component.transform.localPosition.y;
			num += component.font.CalculatePrintedSize(text, component.supportEncoding, component.symbolStyle).y * component.transform.localScale.y;
			component2.fillAmount = num / component2.transform.localScale.y;
			Vector3 localPosition = component3.transform.localPosition;
			component3.transform.localPosition = new Vector3(localPosition.x, num, localPosition.z);
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		else
		{
			KGUI.SetControlSprite(transform, emotion, 256f);
			UnityEngine.Object.Destroy(component.gameObject);
			UnityEngine.Object.Destroy(component2.gameObject);
			UnityEngine.Object.Destroy(component3.gameObject);
		}
		gameObject.transform.parent = this._GUIObjectPrototype.transform.parent;
		gameObject.transform.localScale = this._GUIObjectPrototype.transform.localScale;
		gameObject.transform.localPosition = this._GUIObjectPrototype.transform.localPosition;
		gameObject.transform.localRotation = this._GUIObjectPrototype.transform.localRotation;
		SpeechBubbles.SpeechBubble speechBubble = new SpeechBubbles.SpeechBubble();
		speechBubble.Author = author;
		speechBubble.Text = text;
		speechBubble.EmotionName = ((text != null) ? null : emotion);
		speechBubble.StartTime = Time.time;
		speechBubble.LifeTime = this.BaseLifeTime + ((text == null) ? 0f : (this.SymbolLifeTime * (float)text.Length));
		speechBubble.GUIObject = gameObject;
		if (text != null)
		{
			speechBubble.GUIObjectTransforms = new Transform[]
			{
				component2.transform,
				component3.transform,
				component.transform
			};
			speechBubble.GUIObjectBasePositions = new Vector3[]
			{
				component2.transform.localPosition,
				component3.transform.localPosition,
				component.transform.localPosition
			};
			speechBubble.GUIObjectBaseScales = new Vector3[]
			{
				component2.transform.localScale,
				component3.transform.localScale,
				component.transform.localScale
			};
		}
		this._SpeechBubbles.Add(speechBubble);
		this.RefreshBubbleView(speechBubble);
		this.RaycastAuthor(speechBubble);
	}

	public static SpeechBubbles.Emotion GetEmotion(string emotionName)
	{
		if (SpeechBubbles._Instance != null)
		{
			foreach (SpeechBubbles.Emotion emotion in SpeechBubbles._Instance.Emotions)
			{
				if (emotion.Picture.name == emotionName)
				{
					return emotion;
				}
			}
		}
		return null;
	}

	public static SpeechBubbles.Emotion[] GetActiveEmotions()
	{
		if (SpeechBubbles._Instance != null)
		{
			List<SpeechBubbles.Emotion> list = new List<SpeechBubbles.Emotion>();
			for (int i = 0; i < SpeechBubbles._Instance.Emotions.Length; i++)
			{
				if (i <= 5 || i >= 18 || ProfileINI.GetPurchaseValue(StorePurchase.EMOTIONS) != 0)
				{
					if (i < 18 || ProfileINI.GetPurchaseValue(StorePurchase.MEM_SMILES) != 0)
					{
						if (!(SpeechBubbles._Instance.Emotions[i].Text == "voiceChat"))
						{
							list.Add(SpeechBubbles._Instance.Emotions[i]);
						}
					}
				}
			}
			return list.ToArray();
		}
		return new SpeechBubbles.Emotion[0];
	}

	private void Awake()
	{
		SpeechBubbles._Instance = this;
		this._GUIObjectPrototype = KGUI.FindNode("hud.speech_bubbles.speech_bubble", false).gameObject;
		this._EmotionImage = KGUI.FindNode("hud.emotion_image", false).gameObject;
		foreach (object obj in KGUI.FindNode("hud.speech_bubbles", false))
		{
			Transform transform = (Transform)obj;
			if (transform.name != "speech_bubble")
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
	}

	private void Update()
	{
		bool flag = false;
		for (int i = this._SpeechBubbles.Count - 1; i >= 0; i--)
		{
			SpeechBubbles.SpeechBubble speechBubble = this._SpeechBubbles[i];
			if (speechBubble.Author == null || speechBubble.Author.GetComponent<Collider>() == null || Time.time - speechBubble.StartTime >= speechBubble.LifeTime)
			{
				UnityEngine.Object.Destroy(speechBubble.GUIObject);
				this._SpeechBubbles.RemoveAt(i);
			}
			else
			{
				if (Time.time - speechBubble.StartTime >= speechBubble.LifeTime - this.FadeTime)
				{
					speechBubble.Alpha = Mathf.MoveTowards(speechBubble.Alpha, 0f, Time.deltaTime * this.FadeTime);
				}
				else if (speechBubble.Alpha < 1f)
				{
					speechBubble.Alpha = Mathf.MoveTowards(speechBubble.Alpha, 1f, Time.deltaTime * this.FadeTime);
				}
				if (!(speechBubble.Author == WorldGameObjectX.Instance.MainPlayer) || CameraController.Instance.IsThirdPerson)
				{
					if (Time.time - speechBubble.RaycastLastTime >= this.RaycastPeriod)
					{
						this.RaycastAuthor(speechBubble);
					}
					if (!speechBubble.RaycastSuccesfull)
					{
						if (speechBubble.GUIObject.activeInHierarchy)
						{
							speechBubble.GUIObject.SetActive(false);
						}
					}
					else
					{
						this.RefreshBubbleView(speechBubble);
					}
				}
				else
				{
					if (speechBubble.EmotionName != null)
					{
						flag = true;
						if (!this._EmotionImage.activeInHierarchy)
						{
							Utils.SetActiveRecursively(this._EmotionImage, true);
							KGUI.SetControlSprite(this._EmotionImage.transform, speechBubble.EmotionName, 128f);
							this._EmotionImage.GetComponentInChildren<UIWidget>().alpha = speechBubble.Alpha;
							speechBubble.CurAlpha = speechBubble.Alpha;
						}
						if (speechBubble.Alpha != speechBubble.CurAlpha)
						{
							speechBubble.CurAlpha = speechBubble.Alpha;
							this._EmotionImage.GetComponentInChildren<UIWidget>().alpha = speechBubble.Alpha;
						}
					}
					if (speechBubble.GUIObject.activeInHierarchy)
					{
						speechBubble.GUIObject.SetActive(false);
					}
				}
			}
		}
		if (!flag && this._EmotionImage.activeInHierarchy)
		{
			Utils.SetActiveRecursively(this._EmotionImage, false);
		}
	}

	private void RefreshBubbleView(SpeechBubbles.SpeechBubble bubble)
	{
		if (CameraController.RaycastCamera == null)
		{
			if (bubble.GUIObject.activeInHierarchy)
			{
				bubble.GUIObject.SetActive(false);
			}
			return;
		}
		Vector3 vector = bubble.Author.transform.position;
		if (bubble.Author.GetComponent<Collider>() != null && bubble.Author.GetComponent<Collider>().enabled)
		{
			vector.y += bubble.Author.GetComponent<Collider>().bounds.size.y + 0.1f;
		}
		else
		{
			vector.y += 1.88f;
		}
		vector = CameraController.RaycastCamera.WorldToScreenPoint(vector);
		if (vector.z <= 0f)
		{
			if (bubble.GUIObject.activeInHierarchy)
			{
				bubble.GUIObject.SetActive(false);
			}
			return;
		}
		if (!bubble.GUIObject.activeInHierarchy)
		{
			bubble.GUIObject.SetActive(true);
		}
		float num = Vector3.Distance(CameraController.RaycastCamera.transform.position, bubble.Author.transform.position);
		float num2 = 1f - Mathf.Clamp01(num / this.RaycastDistance) / 2f;
		if (bubble.EmotionName != null)
		{
			num2 /= 2f;
		}
		num2 = Mathf.Round(num2 * 100f) / 100f;
		vector = KGUI.ScreenToGUIPoint(vector);
		vector.z = -(num / this.RaycastDistance);
		vector = NGUITools.Round(vector);
		if (bubble.GUIObject.transform.localPosition != vector)
		{
			bubble.GUIObject.transform.localPosition = vector;
		}
		if (bubble.Scale != num2)
		{
			bubble.Scale = num2;
			if (bubble.GUIObjectTransforms != null)
			{
				for (int i = 0; i < bubble.GUIObjectTransforms.Length; i++)
				{
					Vector3 vector2 = bubble.GUIObjectBasePositions[i] * num2;
					vector2.x = Mathf.Floor(vector2.x);
					vector2.y = Mathf.Floor(vector2.y);
					bubble.GUIObjectTransforms[i].localPosition = vector2;
					vector2 = bubble.GUIObjectBaseScales[i] * num2;
					if (i != bubble.GUIObjectTransforms.Length - 1)
					{
						vector2.x = Mathf.Floor(vector2.x);
						vector2.y = Mathf.Floor(vector2.y);
					}
					bubble.GUIObjectTransforms[i].localScale = vector2;
				}
			}
			else
			{
				bubble.GUIObject.transform.localScale = new Vector3(num2, num2, 1f);
			}
		}
		if (bubble.Alpha != bubble.CurAlpha)
		{
			bubble.CurAlpha = bubble.Alpha;
			foreach (UIWidget uiwidget in bubble.GUIObject.GetComponentsInChildren<UIWidget>())
			{
				uiwidget.alpha = bubble.Alpha;
			}
		}
	}

	private void RaycastAuthor(SpeechBubbles.SpeechBubble bubble)
	{
		bubble.RaycastSuccesfull = false;
		bubble.RaycastLastTime = Time.time;
		if (CameraController.Instance == null)
		{
			return;
		}
		if (bubble.Author == WorldGameObjectX.Instance.MainPlayer)
		{
			bubble.RaycastSuccesfull = true;
			return;
		}
		Vector3 position = CameraController.RaycastCamera.transform.position;
		PlayerNetwork component = bubble.Author.GetComponent<PlayerNetwork>();
		Vector3 vector;
		if (component != null && component.HeadBone != null && component.HeadBone.transform.childCount > 0)
		{
			vector = component.HeadBone.transform.GetChild(0).transform.position;
		}
		else if (component != null && component.HeadBone != null)
		{
			vector = component.HeadBone.transform.position;
		}
		else
		{
			vector = bubble.Author.GetComponent<Collider>().bounds.center;
		}
		float num = Vector3.Distance(position, vector);
		if (num > this.RaycastDistance)
		{
			return;
		}
		int layerMask = 1 << LayerMask.NameToLayer("Terrain");
		if (Physics.Raycast(position, (vector - position).normalized, num, layerMask))
		{
			return;
		}
		bubble.RaycastSuccesfull = true;
	}

	public float RaycastDistance = 50f;

	public float RaycastPeriod = 1f;

	public float BaseLifeTime = 5f;

	public float SymbolLifeTime = 0.2f;

	public float FadeTime = 1f;

	public SpeechBubbles.Emotion[] Emotions;

	private static SpeechBubbles _Instance;

	private List<SpeechBubbles.SpeechBubble> _SpeechBubbles = new List<SpeechBubbles.SpeechBubble>();

	private GameObject _GUIObjectPrototype;

	private GameObject _EmotionImage;

	[Serializable]
	public class Emotion
	{
		public Texture Picture;

		public string Text;
	}

	private class SpeechBubble
	{
		public GameObject Author;

		public float StartTime;

		public float LifeTime;

		public bool RaycastSuccesfull;

		public float RaycastLastTime;

		public float Alpha = 1f;

		public float CurAlpha = 1f;

		public string Text;

		public string EmotionName;

		public float Scale = 1f;

		public GameObject GUIObject;

		public Transform[] GUIObjectTransforms;

		public Vector3[] GUIObjectBasePositions;

		public Vector3[] GUIObjectBaseScales;
	}
}
