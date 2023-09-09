using System;
using UnityEngine;

[RequireComponent(typeof(UISprite))]
[AddComponentMenu("NGUI/Examples/UI Cursor")]
public class UICursor : MonoBehaviour
{
	private void Awake()
	{
		UICursor.mInstance = this;
	}

	private void OnDestroy()
	{
		UICursor.mInstance = null;
	}

	private void Start()
	{
		this.mTrans = base.transform;
		this.mSprite = base.GetComponentInChildren<UISprite>();
		this.mAtlas = this.mSprite.atlas;
		this.mSpriteName = this.mSprite.spriteName;
		this.mSprite.depth = 100;
		if (this.uiCamera == null)
		{
			this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
	}

	private void Update()
	{
		if (this.mSprite.atlas != null)
		{
			Vector3 mousePosition = UnityEngine.Input.mousePosition;
			if (this.uiCamera != null)
			{
				mousePosition.x = Mathf.Clamp01(mousePosition.x / (float)Screen.width);
				mousePosition.y = Mathf.Clamp01(mousePosition.y / (float)Screen.height);
				this.mTrans.position = this.uiCamera.ViewportToWorldPoint(mousePosition);
				if (this.uiCamera.orthographic)
				{
					this.mTrans.localPosition = NGUIMath.ApplyHalfPixelOffset(this.mTrans.localPosition, this.mTrans.localScale);
				}
			}
			else
			{
				mousePosition.x -= (float)Screen.width * 0.5f;
				mousePosition.y -= (float)Screen.height * 0.5f;
				this.mTrans.localPosition = NGUIMath.ApplyHalfPixelOffset(mousePosition, this.mTrans.localScale);
			}
		}
	}

	public static void Clear()
	{
		UICursor.Set(UICursor.mInstance.mAtlas, UICursor.mInstance.mSpriteName);
	}

	public static void Set(UIAtlas atlas, string sprite)
	{
		if (UICursor.mInstance != null)
		{
			UICursor.mInstance.mSprite.atlas = atlas;
			UICursor.mInstance.mSprite.spriteName = sprite;
			UICursor.mInstance.mSprite.MakePixelPerfect();
			UICursor.mInstance.Update();
		}
	}

	private static UICursor mInstance;

	public Camera uiCamera;

	private Transform mTrans;

	private UISprite mSprite;

	private UIAtlas mAtlas;

	private string mSpriteName;
}
