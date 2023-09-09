using System;
using UnityEngine;

public class PreviewCubes : MonoBehaviour
{
	private void Awake()
	{
		PreviewCubes.Instance = this;
		this._RailsCountStyle = new GUIStyle();
		this._RailsCountStyle.fontSize = 40;
		this._RailsCountStyle.normal.textColor = Color.white;
		this._RailsCountStyle.alignment = TextAnchor.MiddleCenter;
		this.DrawCube();
	}

	private void Update()
	{
		if (this._CurModel != null)
		{
			this._CurModel.transform.Rotate(Vector3.up * Time.deltaTime * 20f);
		}
	}

	private void OnGUI()
	{
		GUI.depth = 100;
		if (this._CurModel == this.Rail)
		{
			GUI.Label(new Rect(20f, (float)(Screen.height - 115), 100f, 100f), ProfileINI.GetPurchaseValue(StorePurchase.RAIL).ToString(), this._RailsCountStyle);
		}
	}

	public void DrawCube()
	{
		if (this._CurModel == this.Cube)
		{
			return;
		}
		this._CurModel = this.Cube;
		this.Cube.GetComponent<Renderer>().enabled = true;
		this.Rail.GetComponent<Renderer>().enabled = false;
	}

	public void DrawRail()
	{
		if (this._CurModel == this.Rail)
		{
			return;
		}
		this._CurModel = this.Rail;
		this.Rail.GetComponent<Renderer>().enabled = true;
		this.Cube.GetComponent<Renderer>().enabled = false;
	}

	public static PreviewCubes Instance;

	public GameObject Cube;

	public GameObject Rail;

	private GameObject _CurModel;

	private GUIStyle _RailsCountStyle;
}
