using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class GameObjectSpawner : MonoBehaviour
{
	public GameObjectSpawner()
	{
		this.maxButtons = 10;
		this.spawnOnAwake = true;
		this._active = true;
		this.counter = -1;
		this.matCounter = -1;
	}

	public virtual void Start()
	{
		System.Array.Sort<GameObject>(this.particles, new Comparison<GameObject>(this._0024Start_0024closure_00248));
		System.Array.Sort<Material>(this.materials, new Comparison<Material>(this._0024Start_0024closure_00249));
		this.pages = (int)Mathf.Ceil((float)((Extensions.get_length(this.particles) - 1) / this.maxButtons));
		if (this.spawnOnAwake)
		{
			this.counter = 0;
			this.ReplaceGO(this.particles[this.counter]);
			this.Info(this.particles[this.counter], this.counter);
		}
		if (this.autoChangeDelay > (float)0)
		{
			this.InvokeRepeating("NextModel", this.autoChangeDelay, this.autoChangeDelay);
		}
	}

	public virtual void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
		{
			if (this._active)
			{
				this._active = false;
				if (this.image)
				{
					this.image.enabled = false;
				}
			}
			else
			{
				this._active = true;
				if (this.image)
				{
					this.image.enabled = true;
				}
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
		{
			this.NextModel();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
		{
			this.counter--;
			if (this.counter < 0)
			{
				this.counter = this.particles.Length - 1;
			}
			this.ReplaceGO(this.particles[this.counter]);
			this.Info(this.particles[this.counter], this.counter + 1);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) && this.materials.Length > 0)
		{
			this.matCounter++;
			if (this.matCounter > this.materials.Length - 1)
			{
				this.matCounter = 0;
			}
			this.material = this.materials[this.matCounter];
			if (this.currentGO)
			{
				this.currentGO.GetComponent<Renderer>().sharedMaterial = this.material;
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) && this.materials.Length > 0)
		{
			this.matCounter--;
			if (this.matCounter < 0)
			{
				this.matCounter = this.materials.Length - 1;
			}
			this.material = this.materials[this.matCounter];
			if (this.currentGO)
			{
				this.currentGO.GetComponent<Renderer>().sharedMaterial = this.material;
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.B))
		{
			this.colorCounter++;
			if (this.colorCounter > this.cameraColors.Length - 1)
			{
				this.colorCounter = 0;
			}
		}
		Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, this.cameraColors[this.colorCounter], Time.deltaTime * (float)3);
	}

	public virtual void NextModel()
	{
		this.counter++;
		if (this.counter > this.particles.Length - 1)
		{
			this.counter = 0;
		}
		this.ReplaceGO(this.particles[this.counter]);
		this.Info(this.particles[this.counter], this.counter + 1);
	}

	public virtual void OnGUI()
	{
		if (this.showInfo)
		{
			GUI.Label(new Rect((float)Screen.width * 0.5f - (float)250, (float)20, (float)500, (float)500), this.currentGOInfo, this.bigStyle);
		}
		if (this._active)
		{
			if (Extensions.get_length(this.particles) > this.maxButtons)
			{
				if (GUI.Button(new Rect((float)20, (float)((this.maxButtons + 1) * 18), (float)75, (float)18), "Prev"))
				{
					if (this.page > 0)
					{
						this.page--;
					}
					else
					{
						this.page = this.pages;
					}
				}
				if (GUI.Button(new Rect((float)95, (float)((this.maxButtons + 1) * 18), (float)75, (float)18), "Next"))
				{
					if (this.page < this.pages)
					{
						this.page++;
					}
					else
					{
						this.page = 0;
					}
				}
				GUI.Label(new Rect((float)60, (float)((this.maxButtons + 2) * 18), (float)150, (float)22), "Page" + (this.page + 1) + " / " + (this.pages + 1));
			}
			this.showInfo = GUI.Toggle(new Rect((float)185, (float)20, (float)75, (float)25), this.showInfo, "Info");
			int num = Extensions.get_length(this.particles) - this.page * this.maxButtons;
			if (num > this.maxButtons)
			{
				num = this.maxButtons;
			}
			for (int i = 0; i < num; i++)
			{
				string text = this.particles[i + this.page * this.maxButtons].transform.name;
				if (this.removeTextFromButton != string.Empty)
				{
					text = text.Replace(this.removeTextFromButton, string.Empty);
				}
				if (GUI.Button(new Rect((float)20, (float)(i * 18 + 18), (float)150, (float)18), text))
				{
					if (this.currentGO)
					{
						UnityEngine.Object.Destroy(this.currentGO);
					}
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.particles[i + this.page * this.maxButtons]);
					this.currentGO = gameObject;
					this.counter = i + this.page * this.maxButtons;
					if (this.material)
					{
						gameObject.GetComponent<Renderer>().sharedMaterial = this.material;
					}
					this.Info(gameObject, i + this.page * this.maxButtons + 1);
				}
			}
			for (int j = 0; j < this.materials.Length; j++)
			{
				string text2 = this.materials[j].name;
				if (this.removeTextFromMaterialButton != string.Empty)
				{
					text2 = text2.Replace(this.removeTextFromMaterialButton, string.Empty);
				}
				if (GUI.Button(new Rect((float)20, (float)((this.maxButtons + j + 4) * 18), (float)150, (float)18), text2))
				{
					this.material = this.materials[j];
					if (this.currentGO)
					{
						this.currentGO.GetComponent<Renderer>().sharedMaterial = this.material;
					}
				}
			}
		}
		if (this.image)
		{
			int num2 = Screen.width - this.image.texture.width;
			Rect pixelInset = this.image.pixelInset;
			float num3 = pixelInset.x = (float)num2;
			Rect rect = this.image.pixelInset = pixelInset;
		}
	}

	public virtual void Info(GameObject go, int i)
	{
		if ((ParticleSystem)go.GetComponent(typeof(ParticleSystem)))
		{
			this.PlayPS((ParticleSystem)go.GetComponent(typeof(ParticleSystem)), i);
			this.InfoPS((ParticleSystem)go.GetComponent(typeof(ParticleSystem)), i);
		}
		else
		{
			this.InfoGO(go, i);
		}
	}

	public virtual void ReplaceGO(GameObject _go)
	{
		if (this.currentGO)
		{
			UnityEngine.Object.Destroy(this.currentGO);
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(_go);
		this.currentGO = gameObject;
		if (this.material)
		{
			gameObject.GetComponent<Renderer>().sharedMaterial = this.material;
		}
	}

	public virtual void PlayPS(ParticleSystem _ps, int _nr)
	{
		Time.timeScale = (float)1;
		_ps.Play();
	}

	public virtual void InfoGO(GameObject _ps, int _nr)
	{
		this.currentGOInfo = string.Empty + string.Empty + _nr + "/" + Extensions.get_length(this.particles) + "\n" + _ps.gameObject.name + "\n" + ((MeshFilter)_ps.GetComponent(typeof(MeshFilter))).sharedMesh.triangles.Length / 3 + " Tris";
		this.currentGOInfo = this.currentGOInfo.Replace("_", " ");
	}

	public virtual void Instructions()
	{
		this.currentGOInfo = this.currentGOInfo + "\n\nUse mouse wheel to zoom \n" + "Click and hold to rotate\n" + "Press Space to show or hide menu\n" + "Press Up and Down arrows to cycle materials\n" + "Press B to cycle background colors";
		this.currentGOInfo = this.currentGOInfo.Replace("(Clone)", string.Empty);
	}

	public virtual void InfoPS(ParticleSystem _ps, int _nr)
	{
		this.currentGOInfo = "System" + ": " + _nr + "/" + Extensions.get_length(this.particles) + "\n" + _ps.gameObject.name + "\n\n";
		this.currentGOInfo = this.currentGOInfo.Replace("_", " ");
		this.currentGOInfo = this.currentGOInfo.Replace("(Clone)", string.Empty);
	}

	public virtual void Main()
	{
	}

	internal int _0024Start_0024closure_00248(GameObject g1, GameObject g2)
	{
		return string.Compare(g1.name, g2.name);
	}

	internal int _0024Start_0024closure_00249(Material g1, Material g2)
	{
		return string.Compare(g1.name, g2.name);
	}

	public GameObject[] particles;

	public Material[] materials;

	public Color[] cameraColors;

	public int maxButtons;

	public bool spawnOnAwake;

	public bool showInfo;

	public string removeTextFromButton;

	public string removeTextFromMaterialButton;

	public float autoChangeDelay;

	public GUITexture image;

	private int page;

	private int pages;

	private string currentGOInfo;

	private GameObject currentGO;

	private Color currentColor;

	private bool isPS;

	private Material material;

	private bool _active;

	private int counter;

	private int matCounter;

	private int colorCounter;

	public GUIStyle bigStyle;
}
