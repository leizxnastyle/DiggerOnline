using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class EntityBase : Photon.MonoBehaviour
{
	public int BlockX
	{
		get
		{
			return (int)base.transform.position.x;
		}
	}

	public int BlockY
	{
		get
		{
			return (int)base.transform.position.z;
		}
	}

	public int BlockZ
	{
		get
		{
			return (int)base.transform.position.y;
		}
	}

	public bool IsPreview
	{
		get
		{
			return base.photonView.instantiationData == null;
		}
	}

	protected virtual void Awake()
	{
		EntityBase.LayerEntity = LayerMask.NameToLayer("Entity");
		EntityBase.LayerSmallDecor = LayerMask.NameToLayer("SmallDecor");
		EntityBase.LayerStorePreview = LayerMask.NameToLayer("StorePreview");
		this.UpdateTextures();
		foreach (object obj in base.transform)
		{
			Transform node = (Transform)obj;
			this.FindColliders(node);
		}
	}

	protected virtual void Creation(object[] data)
	{
	}

	protected virtual void PreviewCreation(object[] data)
	{
	}

	private void Start()
	{
		if (!this.IsPreview)
		{
			if (WorldGameObjectX.Instance != null && WorldGameObjectX.Instance.IsWorldGenerated)
			{
				this.InitializeInternal();
			}
			else
			{
				EntityBase.Entities.Add(this);
			}
		}
		if (App.Instance.Settings.gameType == GameINI.GameType.HUNGER_GAMES && base.name.Contains("hg_arenaSpawnPoint"))
		{
			base.transform.FindChild("arena_flag").gameObject.SetActive(false);
		}
	}

	private void InitializeInternal()
	{
		this.FixWorldBounds();
		TimeOfDay.Affect(base.gameObject);
		this.Type = (EntityType)((int)base.photonView.instantiationData[0]);
		this.Name = WorldGameObjectX.Instance.EnityParametrs[(int)this.Type].name;
		this.Creator = (string)base.photonView.instantiationData[1];
		if (WorldGameObjectX.Instance.EnityParametrs[(int)this.Type].small)
		{
			Utils.SetLayerRecursively(base.gameObject, EntityBase.LayerSmallDecor);
		}
		else
		{
			Utils.SetLayerRecursively(base.gameObject, EntityBase.LayerEntity);
		}
		object[] array = null;
		if (base.photonView.instantiationData.Length > 3)
		{
			array = new object[base.photonView.instantiationData.Length - 3];
			Array.Copy(base.photonView.instantiationData, 3, array, 0, array.Length);
		}
		EntityBase.Entities.Add(this);
		if (EntityBase._EntityGroup == null)
		{
			EntityBase._EntityGroup = new GameObject("Entity").transform;
		}
		base.transform.parent = EntityBase._EntityGroup;
		this.Creation(array);
	}

	public void InitializePreview(EntityType type, object[] data)
	{
		this.Type = type;
		this.Name = WorldGameObjectX.Instance.EnityParametrs[(int)this.Type].name;
		Utils.SetLayerRecursively(base.gameObject, EntityBase.LayerStorePreview);
		this.PreviewCreation(data);
	}

	public static void OnWorldGenerated()
	{
		EntityBase[] array = EntityBase.Entities.ToArray();
		EntityBase.Entities.Clear();
		foreach (EntityBase entityBase in array)
		{
			entityBase.InitializeInternal();
		}
	}

	protected virtual void Updating()
	{
	}

	protected virtual void PreviewUpdating()
	{
	}

	private void Update()
	{
		if (!WorldGameObjectX.Instance.IsWorldGenerated)
		{
			return;
		}
		if (!this.IsPreview)
		{
			this.Updating();
		}
		else
		{
			this.PreviewUpdating();
		}
		if (this._ShakeTime != 0f)
		{
			base.transform.position -= this._ShakeOffset;
			if (Time.time >= this._ShakeTime)
			{
				this._ShakeTime = 0f;
				this._ShakeOffset = Vector3.zero;
			}
			else
			{
				this._ShakeOffset = new Vector3(UnityEngine.Random.Range(-0.05f, 0.05f), UnityEngine.Random.Range(-0.05f, 0.05f), UnityEngine.Random.Range(-0.01f, 0.01f));
				base.transform.position += this._ShakeOffset;
			}
		}
	}

	protected virtual void Destruction()
	{
	}

	protected virtual void PreviewDestruction()
	{
	}

	public virtual void SelfDelete()
	{
		WorldGameObjectX.Instance.photonView.RPC("DeleteEntityNetworkAll", PhotonTargets.All, new object[]
		{
			base.photonView.viewID
		});
		WorldGameObjectX.Instance.photonView.RPC("DeleteEntityNetworkMasterClient", PhotonTargets.MasterClient, new object[]
		{
			base.photonView.viewID
		});
	}

	private void OnDestroy()
	{
		if (this.IsPreview)
		{
			this.PreviewDestruction();
		}
	}

	public virtual void OnLeftMouseHit(string playerName)
	{
		if (this.Life == 0)
		{
			return;
		}
		if (this.Creator == playerName || Level.Instance.IsAdmin(null))
		{
			this.Life -= 1;
			if (this.Life > 0)
			{
				this._ShakeTime = Time.time + 0.5f;
			}
			else
			{
				this.SelfDelete();
			}
		}
		else
		{
			Chat.SendInfoF(ProfileINI.nickname + Localize.GetText("CANT_DESTROY_ITEMS", null), false);
		}
	}

	public virtual void OnButtonE(string playerName)
	{
		if (TeamBattle.Instance != null)
		{
			return;
		}
		if (this.Creator == playerName || Level.Instance.IsAdmin(null) || Level.Instance.IsModerator(null))
		{
			WorldGameObjectX.Instance.TakeEnity(base.gameObject);
		}
		else
		{
			Chat.SendInfoF(ProfileINI.nickname + Localize.GetText("CANT_TAKE_AND_DESTROY_ITEMS", null), false);
		}
	}

	public virtual void OnButtonF(string playerName)
	{
	}

	protected void FixWorldBounds()
	{
		Vector3 position = base.transform.position;
		if (position.x < 0f)
		{
			base.transform.position = new Vector3(0f, base.transform.position.y, base.transform.position.z);
		}
		else if (position.x >= (float)WorldData.Instance.WidthInBlocks)
		{
			base.transform.position = new Vector3((float)(WorldData.Instance.WidthInBlocks - 1), base.transform.position.y, base.transform.position.z);
		}
		if (position.z < 0f)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, 0f);
		}
		else if (position.z >= (float)WorldData.Instance.HeightInBlocks)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, (float)(WorldData.Instance.HeightInBlocks - 1));
		}
		if (position.y < 0f)
		{
			base.transform.position = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
		}
		else if (position.y >= (float)WorldData.Instance.DepthInBlocks)
		{
			base.transform.position = new Vector3(base.transform.position.x, (float)(WorldData.Instance.DepthInBlocks - 1), base.transform.position.z);
		}
	}

	public static EntityBase FindEntity(GameObject go)
	{
		EntityBase entityBase = go.GetComponent<EntityBase>();
		if (entityBase == null)
		{
			EntityBaseChild component = go.GetComponent<EntityBaseChild>();
			if (component != null)
			{
				entityBase = component.Parent;
			}
		}
		return entityBase;
	}

	public virtual object[] GetData()
	{
		return null;
	}

	public void UpdateTextures()
	{
		base.StartCoroutine(ContentUpdater.UpdateTextures(base.transform, ContentUpdater.EntitiesBundle));
	}

	private void FindColliders(Transform node)
	{
		if (node.GetComponent<Collider>() != null)
		{
			if (node.GetComponent<EntityBaseChild>() == null)
			{
				node.gameObject.AddComponent<EntityBaseChild>();
			}
			node.GetComponent<EntityBaseChild>().Parent = this;
		}
		foreach (object obj in node)
		{
			Transform node2 = (Transform)obj;
			this.FindColliders(node2);
		}
	}

	public virtual void ObjectIsMoved()
	{
	}

	public static List<EntityBase> Entities = new List<EntityBase>();

	public static int LayerEntity;

	public static int LayerSmallDecor;

	public static int LayerStorePreview;

	private static Transform _EntityGroup = null;

	[NonSerialized]
	public string Name;

	[NonSerialized]
	public EntityType Type;

	[NonSerialized]
	public string Creator;

	[NonSerialized]
	public byte Life = 4;

	protected float _ShakeTime;

	protected Vector3 _ShakeOffset = Vector3.zero;
}
