using System;
using UnityEngine;

[Serializable]
public class DetonatorTest : MonoBehaviour
{
	public DetonatorTest()
	{
		this._currentExpIdx = -1;
		this.explosionLife = (float)10;
		this.timeScale = 1f;
		this.detailLevel = 1f;
		this._spawnWallTime = -1000;
		this.checkRect = new Rect((float)0, (float)0, (float)260, (float)180);
	}

	public virtual void Start()
	{
		this.SpawnWall();
		if (!this.currentDetonator)
		{
			this.NextExplosion();
		}
		else
		{
			this._currentExpIdx = 0;
		}
	}

	public virtual void OnGUI()
	{
		this._guiRect = new Rect((float)7, (float)(Screen.height - 180), (float)250, (float)200);
		GUILayout.BeginArea((Rect)this._guiRect);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		string name = this.currentDetonator.name;
		if (GUILayout.Button(name + " (Click For Next)", new GUILayoutOption[0]))
		{
			this.NextExplosion();
		}
		if (GUILayout.Button("Rebuild Wall", new GUILayoutOption[0]))
		{
			this.SpawnWall();
		}
		if (GUILayout.Button("Camera Far", new GUILayoutOption[0]))
		{
			Camera.main.transform.position = new Vector3((float)0, (float)0, (float)-7);
			Camera.main.transform.eulerAngles = new Vector3(13.5f, (float)0, (float)0);
		}
		if (GUILayout.Button("Camera Near", new GUILayoutOption[0]))
		{
			Camera.main.transform.position = new Vector3((float)0, -8.664466f, 31.38269f);
			Camera.main.transform.eulerAngles = new Vector3(1.213462f, (float)0, (float)0);
		}
		GUILayout.Label("Time Scale", new GUILayoutOption[0]);
		this.timeScale = GUILayout.HorizontalSlider(this.timeScale, (float)0, 1f, new GUILayoutOption[0]);
		GUILayout.Label("Detail Level (re-explode after change)", new GUILayoutOption[0]);
		this.detailLevel = GUILayout.HorizontalSlider(this.detailLevel, (float)0, 1f, new GUILayoutOption[0]);
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	public virtual void NextExplosion()
	{
		if (this._currentExpIdx >= this.detonatorPrefabs.Length - 1)
		{
			this._currentExpIdx = 0;
		}
		else
		{
			this._currentExpIdx++;
		}
		this.currentDetonator = this.detonatorPrefabs[this._currentExpIdx];
	}

	public virtual void SpawnWall()
	{
		if (this._currentWall)
		{
			UnityEngine.Object.Destroy(this._currentWall);
		}
		this._currentWall = (GameObject)UnityEngine.Object.Instantiate(this.wall, new Vector3((float)-7, (float)-12, (float)48), Quaternion.identity);
		this._spawnWallTime = (int)Time.time;
	}

	public virtual void Update()
	{
		this._guiRect = new Rect((float)7, (float)(Screen.height - 150), (float)250, (float)200);
		if (Time.time + (float)this._spawnWallTime > 0.5f)
		{
			if (!this.checkRect.Contains(UnityEngine.Input.mousePosition) && Input.GetMouseButtonDown(0))
			{
				this.SpawnExplosion();
			}
			Time.timeScale = this.timeScale;
		}
	}

	public virtual void SpawnExplosion()
	{
		Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
		RaycastHit raycastHit = default(RaycastHit);
		if (Physics.Raycast(ray, out raycastHit, (float)1000))
		{
		}
	}

	public virtual void Main()
	{
	}

	public GameObject currentDetonator;

	private int _currentExpIdx;

	private bool buttonClicked;

	public GameObject[] detonatorPrefabs;

	public float explosionLife;

	public float timeScale;

	public float detailLevel;

	public GameObject wall;

	private GameObject _currentWall;

	private int _spawnWallTime;

	private object _guiRect;

	private bool toggleBool;

	private Rect checkRect;
}
