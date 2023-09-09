using System;
using System.Collections.Generic;
using System.Reflection;
using Photon;
using UnityEngine;

[AddComponentMenu("Photon Networking/Photon View &v")]
public class PhotonView : Photon.MonoBehaviour
{
	public int prefix
	{
		get
		{
			if (this.prefixBackup == -1 && PhotonNetwork.networkingPeer != null)
			{
				this.prefixBackup = (int)PhotonNetwork.networkingPeer.currentLevelPrefix;
			}
			return this.prefixBackup;
		}
		set
		{
			this.prefixBackup = value;
		}
	}

	public object[] instantiationData
	{
		get
		{
			if (!this.didAwake)
			{
				this.instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(this.instantiationId);
			}
			return this.instantiationDataField;
		}
		set
		{
			this.instantiationDataField = value;
		}
	}

	public int viewID
	{
		get
		{
			return this.viewIdField;
		}
		set
		{
			bool flag = this.didAwake && this.viewIdField == 0;
			this.ownerId = value / PhotonNetwork.MAX_VIEW_IDS;
			this.viewIdField = value;
			if (flag)
			{
				PhotonNetwork.networkingPeer.RegisterPhotonView(this);
			}
		}
	}

	public bool isSceneView
	{
		get
		{
			return this.CreatorActorNr == 0;
		}
	}

	public PhotonPlayer owner
	{
		get
		{
			return PhotonPlayer.Find(this.ownerId);
		}
	}

	public int OwnerActorNr
	{
		get
		{
			return this.ownerId;
		}
	}

	public bool isOwnerActive
	{
		get
		{
			return this.ownerId != 0 && PhotonNetwork.networkingPeer.mActors.ContainsKey(this.ownerId);
		}
	}

	public int CreatorActorNr
	{
		get
		{
			return this.viewIdField / PhotonNetwork.MAX_VIEW_IDS;
		}
	}

	public bool isMine
	{
		get
		{
			return this.isForcedSending != -1 && (this.isForcedSending == 1 || this.ownerId == PhotonNetwork.player.ID || (!this.isOwnerActive && PhotonNetwork.isMasterClient));
		}
	}

	protected internal void Awake()
	{
		if (this.viewID != 0)
		{
			PhotonNetwork.networkingPeer.RegisterPhotonView(this);
			this.instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(this.instantiationId);
		}
		this.didAwake = true;
	}

	public void RequestOwnership()
	{
		PhotonNetwork.networkingPeer.RequestOwnership(this.viewID, this.ownerId);
	}

	public void TransferOwnership(PhotonPlayer newOwner)
	{
		this.TransferOwnership(newOwner.ID);
	}

	public void TransferOwnership(int newOwnerId)
	{
		PhotonNetwork.networkingPeer.TransferOwnership(this.viewID, newOwnerId);
		this.ownerId = newOwnerId;
	}

	protected internal void OnDestroy()
	{
		if (!this.removedFromLocalViewList)
		{
			bool flag = PhotonNetwork.networkingPeer.LocalCleanPhotonView(this);
			bool flag2 = false;
			if (flag && !flag2 && this.instantiationId > 0 && !PhotonHandler.AppQuits && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				UnityEngine.Debug.Log("PUN-instantiated '" + base.gameObject.name + "' got destroyed by engine. This is OK when loading levels. Otherwise use: PhotonNetwork.Destroy().");
			}
		}
	}

	public void SerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		this.SerializeComponent(this.observed, stream, info);
		if (this.ObservedComponents != null && this.ObservedComponents.Count > 0)
		{
			for (int i = 0; i < this.ObservedComponents.Count; i++)
			{
				this.SerializeComponent(this.ObservedComponents[i], stream, info);
			}
		}
	}

	public void DeserializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		this.DeserializeComponent(this.observed, stream, info);
		if (this.ObservedComponents != null && this.ObservedComponents.Count > 0)
		{
			for (int i = 0; i < this.ObservedComponents.Count; i++)
			{
				this.DeserializeComponent(this.ObservedComponents[i], stream, info);
			}
		}
	}

	protected internal void DeserializeComponent(Component component, PhotonStream stream, PhotonMessageInfo info)
	{
		if (component == null)
		{
			return;
		}
		if (component is UnityEngine.MonoBehaviour)
		{
			this.ExecuteComponentOnSerialize(component, stream, info);
		}
		else if (component is Transform)
		{
			Transform transform = (Transform)component;
			switch (this.onSerializeTransformOption)
			{
			case OnSerializeTransform.OnlyPosition:
				transform.localPosition = (Vector3)stream.ReceiveNext();
				break;
			case OnSerializeTransform.OnlyRotation:
				transform.localRotation = (Quaternion)stream.ReceiveNext();
				break;
			case OnSerializeTransform.OnlyScale:
				transform.localScale = (Vector3)stream.ReceiveNext();
				break;
			case OnSerializeTransform.PositionAndRotation:
				transform.localPosition = (Vector3)stream.ReceiveNext();
				transform.localRotation = (Quaternion)stream.ReceiveNext();
				break;
			case OnSerializeTransform.All:
				transform.localPosition = (Vector3)stream.ReceiveNext();
				transform.localRotation = (Quaternion)stream.ReceiveNext();
				transform.localScale = (Vector3)stream.ReceiveNext();
				break;
			}
		}
		else if (component is Rigidbody)
		{
			Rigidbody rigidbody = (Rigidbody)component;
			switch (this.onSerializeRigidBodyOption)
			{
			case OnSerializeRigidBody.OnlyVelocity:
				rigidbody.velocity = (Vector3)stream.ReceiveNext();
				break;
			case OnSerializeRigidBody.OnlyAngularVelocity:
				rigidbody.angularVelocity = (Vector3)stream.ReceiveNext();
				break;
			case OnSerializeRigidBody.All:
				rigidbody.velocity = (Vector3)stream.ReceiveNext();
				rigidbody.angularVelocity = (Vector3)stream.ReceiveNext();
				break;
			}
		}
		else if (component is Rigidbody2D)
		{
			Rigidbody2D rigidbody2D = (Rigidbody2D)component;
			switch (this.onSerializeRigidBodyOption)
			{
			case OnSerializeRigidBody.OnlyVelocity:
				rigidbody2D.velocity = (Vector2)stream.ReceiveNext();
				break;
			case OnSerializeRigidBody.OnlyAngularVelocity:
				rigidbody2D.angularVelocity = (float)stream.ReceiveNext();
				break;
			case OnSerializeRigidBody.All:
				rigidbody2D.velocity = (Vector2)stream.ReceiveNext();
				rigidbody2D.angularVelocity = (float)stream.ReceiveNext();
				break;
			}
		}
		else
		{
			UnityEngine.Debug.LogError("Type of observed is unknown when receiving.");
		}
	}

	protected internal void SerializeComponent(Component component, PhotonStream stream, PhotonMessageInfo info)
	{
		if (component == null)
		{
			return;
		}
		if (component is UnityEngine.MonoBehaviour)
		{
			this.ExecuteComponentOnSerialize(component, stream, info);
		}
		else if (component is Transform)
		{
			Transform transform = (Transform)component;
			switch (this.onSerializeTransformOption)
			{
			case OnSerializeTransform.OnlyPosition:
				stream.SendNext(transform.localPosition);
				break;
			case OnSerializeTransform.OnlyRotation:
				stream.SendNext(transform.localRotation);
				break;
			case OnSerializeTransform.OnlyScale:
				stream.SendNext(transform.localScale);
				break;
			case OnSerializeTransform.PositionAndRotation:
				stream.SendNext(transform.localPosition);
				stream.SendNext(transform.localRotation);
				break;
			case OnSerializeTransform.All:
				stream.SendNext(transform.localPosition);
				stream.SendNext(transform.localRotation);
				stream.SendNext(transform.localScale);
				break;
			}
		}
		else if (component is Rigidbody)
		{
			Rigidbody rigidbody = (Rigidbody)component;
			switch (this.onSerializeRigidBodyOption)
			{
			case OnSerializeRigidBody.OnlyVelocity:
				stream.SendNext(rigidbody.velocity);
				break;
			case OnSerializeRigidBody.OnlyAngularVelocity:
				stream.SendNext(rigidbody.angularVelocity);
				break;
			case OnSerializeRigidBody.All:
				stream.SendNext(rigidbody.velocity);
				stream.SendNext(rigidbody.angularVelocity);
				break;
			}
		}
		else if (component is Rigidbody2D)
		{
			Rigidbody2D rigidbody2D = (Rigidbody2D)component;
			switch (this.onSerializeRigidBodyOption)
			{
			case OnSerializeRigidBody.OnlyVelocity:
				stream.SendNext(rigidbody2D.velocity);
				break;
			case OnSerializeRigidBody.OnlyAngularVelocity:
				stream.SendNext(rigidbody2D.angularVelocity);
				break;
			case OnSerializeRigidBody.All:
				stream.SendNext(rigidbody2D.velocity);
				stream.SendNext(rigidbody2D.angularVelocity);
				break;
			}
		}
		else
		{
			UnityEngine.Debug.LogError("Observed type is not serializable: " + component.GetType());
		}
	}

	protected internal void ExecuteComponentOnSerialize(Component component, PhotonStream stream, PhotonMessageInfo info)
	{
		if (component != null)
		{
			if (!this.m_OnSerializeMethodInfos.ContainsKey(component))
			{
				MethodInfo value = null;
				if (!NetworkingPeer.GetMethod(component as UnityEngine.MonoBehaviour, PhotonNetworkingMessage.OnPhotonSerializeView.ToString(), out value))
				{
					UnityEngine.Debug.LogError("The observed monobehaviour (" + component.name + ") of this PhotonView does not implement OnPhotonSerializeView()!");
					value = null;
				}
				this.m_OnSerializeMethodInfos.Add(component, value);
			}
			if (this.m_OnSerializeMethodInfos[component] != null)
			{
				this.m_OnSerializeMethodInfos[component].Invoke(component, new object[]
				{
					stream,
					info
				});
			}
		}
	}

	public void RefreshRpcMonoBehaviourCache()
	{
		this.RpcMonoBehaviours = base.GetComponents<UnityEngine.MonoBehaviour>();
	}

	public void RPC(string methodName, PhotonTargets target, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, target, false, parameters);
	}

	public void RpcSecure(string methodName, PhotonTargets target, bool encrypt, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, target, encrypt, parameters);
	}

	public void RPC(string methodName, PhotonPlayer targetPlayer, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, false, parameters);
	}

	public void RpcSecure(string methodName, PhotonPlayer targetPlayer, bool encrypt, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, encrypt, parameters);
	}

	public static PhotonView Get(Component component)
	{
		return component.GetComponent<PhotonView>();
	}

	public static PhotonView Get(GameObject gameObj)
	{
		return gameObj.GetComponent<PhotonView>();
	}

	public static PhotonView Find(int viewID)
	{
		return PhotonNetwork.networkingPeer.GetPhotonView(viewID);
	}

	public override string ToString()
	{
		return string.Format("View ({3}){0} on {1} {2}", new object[]
		{
			this.viewID,
			(!(base.gameObject != null)) ? "GO==null" : base.gameObject.name,
			(!this.isSceneView) ? string.Empty : "(scene)",
			this.prefix
		});
	}

	public int ownerId;

	public int group;

	protected internal bool mixedModeIsReliable;

	public int prefixBackup = -1;

	internal object[] instantiationDataField;

	protected internal object[] lastOnSerializeDataSent;

	protected internal object[] lastOnSerializeDataReceived;

	public Component observed;

	public ViewSynchronization synchronization;

	public OnSerializeTransform onSerializeTransformOption = OnSerializeTransform.PositionAndRotation;

	public OnSerializeRigidBody onSerializeRigidBodyOption = OnSerializeRigidBody.All;

	public OwnershipOption ownershipTransfer;

	public List<Component> ObservedComponents;

	private Dictionary<Component, MethodInfo> m_OnSerializeMethodInfos = new Dictionary<Component, MethodInfo>();

	[SerializeField]
	private int viewIdField;

	public int instantiationId;

	protected internal bool didAwake;

	[SerializeField]
	protected internal bool isRuntimeInstantiated;

	protected internal bool removedFromLocalViewList;

	internal UnityEngine.MonoBehaviour[] RpcMonoBehaviours;

	private MethodInfo OnSerializeMethodInfo;

	private bool failedToFindOnSerialize;

	public int isForcedSending;
}
