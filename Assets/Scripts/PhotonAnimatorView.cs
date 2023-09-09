using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[AddComponentMenu("Photon Networking/Photon Animator View")]
[RequireComponent(typeof(PhotonView))]
public class PhotonAnimatorView : MonoBehaviour
{
	private void Awake()
	{
		this.m_PhotonView = base.GetComponent<PhotonView>();
		this.m_StreamQueue = new PhotonStreamQueue(120);
		this.m_Animator = base.GetComponent<Animator>();
	}

	private void Update()
	{
		if (!this.m_PhotonView.isMine && PhotonNetwork.connected)
		{
			this.m_Animator.applyRootMotion = false;
		}
		if (!PhotonNetwork.inRoom || PhotonNetwork.room.playerCount <= 1)
		{
			this.m_StreamQueue.Reset();
			return;
		}
		if (this.m_PhotonView.isMine)
		{
			this.SerializeDataContinuously();
		}
		else
		{
			this.DeserializeDataContinuously();
		}
	}

	public bool DoesLayerSynchronizeTypeExist(int layerIndex)
	{
		return this.m_SynchronizeLayers.FindIndex((PhotonAnimatorView.SynchronizedLayer item) => item.LayerIndex == layerIndex) != -1;
	}

	public bool DoesParameterSynchronizeTypeExist(string name)
	{
		return this.m_SynchronizeParameters.FindIndex((PhotonAnimatorView.SynchronizedParameter item) => item.Name == name) != -1;
	}

	public List<PhotonAnimatorView.SynchronizedLayer> GetSynchronizedLayers()
	{
		return this.m_SynchronizeLayers;
	}

	public List<PhotonAnimatorView.SynchronizedParameter> GetSynchronizedParameters()
	{
		return this.m_SynchronizeParameters;
	}

	public PhotonAnimatorView.SynchronizeType GetLayerSynchronizeType(int layerIndex)
	{
		int num = this.m_SynchronizeLayers.FindIndex((PhotonAnimatorView.SynchronizedLayer item) => item.LayerIndex == layerIndex);
		if (num == -1)
		{
			return PhotonAnimatorView.SynchronizeType.Disabled;
		}
		return this.m_SynchronizeLayers[num].SynchronizeType;
	}

	public PhotonAnimatorView.SynchronizeType GetParameterSynchronizeType(string name)
	{
		int num = this.m_SynchronizeParameters.FindIndex((PhotonAnimatorView.SynchronizedParameter item) => item.Name == name);
		if (num == -1)
		{
			return PhotonAnimatorView.SynchronizeType.Disabled;
		}
		return this.m_SynchronizeParameters[num].SynchronizeType;
	}

	public void SetLayerSynchronized(int layerIndex, PhotonAnimatorView.SynchronizeType synchronizeType)
	{
		if (Application.isPlaying)
		{
			this.m_WasSynchronizeTypeChanged = true;
		}
		int num = this.m_SynchronizeLayers.FindIndex((PhotonAnimatorView.SynchronizedLayer item) => item.LayerIndex == layerIndex);
		if (num == -1)
		{
			this.m_SynchronizeLayers.Add(new PhotonAnimatorView.SynchronizedLayer
			{
				LayerIndex = layerIndex,
				SynchronizeType = synchronizeType
			});
		}
		else
		{
			this.m_SynchronizeLayers[num].SynchronizeType = synchronizeType;
		}
	}

	public void SetParameterSynchronized(string name, PhotonAnimatorView.ParameterType type, PhotonAnimatorView.SynchronizeType synchronizeType)
	{
		if (Application.isPlaying)
		{
			this.m_WasSynchronizeTypeChanged = true;
		}
		int num = this.m_SynchronizeParameters.FindIndex((PhotonAnimatorView.SynchronizedParameter item) => item.Name == name);
		if (num == -1)
		{
			this.m_SynchronizeParameters.Add(new PhotonAnimatorView.SynchronizedParameter
			{
				Name = name,
				Type = type,
				SynchronizeType = synchronizeType
			});
		}
		else
		{
			this.m_SynchronizeParameters[num].SynchronizeType = synchronizeType;
		}
	}

	private void SerializeDataContinuously()
	{
		if (this.m_Animator == null)
		{
			return;
		}
		for (int i = 0; i < this.m_SynchronizeLayers.Count; i++)
		{
			if (this.m_SynchronizeLayers[i].SynchronizeType == PhotonAnimatorView.SynchronizeType.Continuous)
			{
				this.m_StreamQueue.SendNext(this.m_Animator.GetLayerWeight(this.m_SynchronizeLayers[i].LayerIndex));
			}
		}
		for (int j = 0; j < this.m_SynchronizeParameters.Count; j++)
		{
			PhotonAnimatorView.SynchronizedParameter synchronizedParameter = this.m_SynchronizeParameters[j];
			if (synchronizedParameter.SynchronizeType == PhotonAnimatorView.SynchronizeType.Continuous)
			{
				PhotonAnimatorView.ParameterType type = synchronizedParameter.Type;
				switch (type)
				{
				case PhotonAnimatorView.ParameterType.Float:
					this.m_StreamQueue.SendNext(this.m_Animator.GetFloat(synchronizedParameter.Name));
					break;
				default:
					if (type != PhotonAnimatorView.ParameterType.Trigger)
					{
					}
					break;
				case PhotonAnimatorView.ParameterType.Int:
					this.m_StreamQueue.SendNext(this.m_Animator.GetInteger(synchronizedParameter.Name));
					break;
				case PhotonAnimatorView.ParameterType.Bool:
					this.m_StreamQueue.SendNext(this.m_Animator.GetBool(synchronizedParameter.Name));
					break;
				}
			}
		}
	}

	private void DeserializeDataContinuously()
	{
		if (!this.m_StreamQueue.HasQueuedObjects())
		{
			return;
		}
		for (int i = 0; i < this.m_SynchronizeLayers.Count; i++)
		{
			if (this.m_SynchronizeLayers[i].SynchronizeType == PhotonAnimatorView.SynchronizeType.Continuous)
			{
				this.m_Animator.SetLayerWeight(this.m_SynchronizeLayers[i].LayerIndex, (float)this.m_StreamQueue.ReceiveNext());
			}
		}
		for (int j = 0; j < this.m_SynchronizeParameters.Count; j++)
		{
			PhotonAnimatorView.SynchronizedParameter synchronizedParameter = this.m_SynchronizeParameters[j];
			if (synchronizedParameter.SynchronizeType == PhotonAnimatorView.SynchronizeType.Continuous)
			{
				PhotonAnimatorView.ParameterType type = synchronizedParameter.Type;
				switch (type)
				{
				case PhotonAnimatorView.ParameterType.Float:
					this.m_Animator.SetFloat(synchronizedParameter.Name, (float)this.m_StreamQueue.ReceiveNext());
					break;
				default:
					if (type != PhotonAnimatorView.ParameterType.Trigger)
					{
					}
					break;
				case PhotonAnimatorView.ParameterType.Int:
					this.m_Animator.SetInteger(synchronizedParameter.Name, (int)this.m_StreamQueue.ReceiveNext());
					break;
				case PhotonAnimatorView.ParameterType.Bool:
					this.m_Animator.SetBool(synchronizedParameter.Name, (bool)this.m_StreamQueue.ReceiveNext());
					break;
				}
			}
		}
	}

	private void SerializeDataDiscretly(PhotonStream stream)
	{
		for (int i = 0; i < this.m_SynchronizeLayers.Count; i++)
		{
			if (this.m_SynchronizeLayers[i].SynchronizeType == PhotonAnimatorView.SynchronizeType.Discrete)
			{
				stream.SendNext(this.m_Animator.GetLayerWeight(this.m_SynchronizeLayers[i].LayerIndex));
			}
		}
		for (int j = 0; j < this.m_SynchronizeParameters.Count; j++)
		{
			PhotonAnimatorView.SynchronizedParameter synchronizedParameter = this.m_SynchronizeParameters[j];
			if (synchronizedParameter.SynchronizeType == PhotonAnimatorView.SynchronizeType.Discrete)
			{
				PhotonAnimatorView.ParameterType type = synchronizedParameter.Type;
				switch (type)
				{
				case PhotonAnimatorView.ParameterType.Float:
					stream.SendNext(this.m_Animator.GetFloat(synchronizedParameter.Name));
					break;
				default:
					if (type != PhotonAnimatorView.ParameterType.Trigger)
					{
					}
					break;
				case PhotonAnimatorView.ParameterType.Int:
					stream.SendNext(this.m_Animator.GetInteger(synchronizedParameter.Name));
					break;
				case PhotonAnimatorView.ParameterType.Bool:
					stream.SendNext(this.m_Animator.GetBool(synchronizedParameter.Name));
					break;
				}
			}
		}
	}

	private void DeserializeDataDiscretly(PhotonStream stream)
	{
		for (int i = 0; i < this.m_SynchronizeLayers.Count; i++)
		{
			if (this.m_SynchronizeLayers[i].SynchronizeType == PhotonAnimatorView.SynchronizeType.Discrete)
			{
				this.m_Animator.SetLayerWeight(this.m_SynchronizeLayers[i].LayerIndex, (float)stream.ReceiveNext());
			}
		}
		for (int j = 0; j < this.m_SynchronizeParameters.Count; j++)
		{
			PhotonAnimatorView.SynchronizedParameter synchronizedParameter = this.m_SynchronizeParameters[j];
			if (synchronizedParameter.SynchronizeType == PhotonAnimatorView.SynchronizeType.Discrete)
			{
				PhotonAnimatorView.ParameterType type = synchronizedParameter.Type;
				switch (type)
				{
				case PhotonAnimatorView.ParameterType.Float:
					if (!(stream.PeekNext() is float))
					{
						return;
					}
					this.m_Animator.SetFloat(synchronizedParameter.Name, (float)stream.ReceiveNext());
					break;
				default:
					if (type != PhotonAnimatorView.ParameterType.Trigger)
					{
					}
					break;
				case PhotonAnimatorView.ParameterType.Int:
					if (!(stream.PeekNext() is int))
					{
						return;
					}
					this.m_Animator.SetInteger(synchronizedParameter.Name, (int)stream.ReceiveNext());
					break;
				case PhotonAnimatorView.ParameterType.Bool:
					if (!(stream.PeekNext() is bool))
					{
						return;
					}
					this.m_Animator.SetBool(synchronizedParameter.Name, (bool)stream.ReceiveNext());
					break;
				}
			}
		}
	}

	private void SerializeSynchronizationTypeState(PhotonStream stream)
	{
		byte[] array = new byte[this.m_SynchronizeLayers.Count + this.m_SynchronizeParameters.Count];
		for (int i = 0; i < this.m_SynchronizeLayers.Count; i++)
		{
			array[i] = (byte)this.m_SynchronizeLayers[i].SynchronizeType;
		}
		for (int j = 0; j < this.m_SynchronizeParameters.Count; j++)
		{
			array[this.m_SynchronizeLayers.Count + j] = (byte)this.m_SynchronizeParameters[j].SynchronizeType;
		}
		stream.SendNext(array);
	}

	private void DeserializeSynchronizationTypeState(PhotonStream stream)
	{
		byte[] array = (byte[])stream.ReceiveNext();
		for (int i = 0; i < this.m_SynchronizeLayers.Count; i++)
		{
			this.m_SynchronizeLayers[i].SynchronizeType = (PhotonAnimatorView.SynchronizeType)array[i];
		}
		for (int j = 0; j < this.m_SynchronizeParameters.Count; j++)
		{
			this.m_SynchronizeParameters[j].SynchronizeType = (PhotonAnimatorView.SynchronizeType)array[this.m_SynchronizeLayers.Count + j];
		}
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.m_Animator == null)
		{
			return;
		}
		if (stream.isWriting)
		{
			if (this.m_WasSynchronizeTypeChanged)
			{
				this.m_StreamQueue.Reset();
				this.SerializeSynchronizationTypeState(stream);
				this.m_WasSynchronizeTypeChanged = false;
			}
			this.m_StreamQueue.Serialize(stream);
			this.SerializeDataDiscretly(stream);
		}
		else
		{
			if (stream.PeekNext() is byte[])
			{
				this.DeserializeSynchronizationTypeState(stream);
			}
			this.m_StreamQueue.Deserialize(stream);
			this.DeserializeDataDiscretly(stream);
		}
	}

	private Animator m_Animator;

	private PhotonStreamQueue m_StreamQueue;

	[HideInInspector]
	[SerializeField]
	private bool ShowLayerWeightsInspector = true;

	[HideInInspector]
	[SerializeField]
	private bool ShowParameterInspector = true;

	[HideInInspector]
	[SerializeField]
	private List<PhotonAnimatorView.SynchronizedParameter> m_SynchronizeParameters = new List<PhotonAnimatorView.SynchronizedParameter>();

	[HideInInspector]
	[SerializeField]
	private List<PhotonAnimatorView.SynchronizedLayer> m_SynchronizeLayers = new List<PhotonAnimatorView.SynchronizedLayer>();

	private Vector3 m_ReceiverPosition;

	private float m_LastDeserializeTime;

	private bool m_WasSynchronizeTypeChanged = true;

	private PhotonView m_PhotonView;

	public enum ParameterType
	{
		Float = 1,
		Int = 3,
		Bool,
		Trigger = 9
	}

	public enum SynchronizeType
	{
		Disabled,
		Discrete,
		Continuous
	}

	[Serializable]
	public class SynchronizedParameter
	{
		public PhotonAnimatorView.ParameterType Type;

		public PhotonAnimatorView.SynchronizeType SynchronizeType;

		public string Name;
	}

	[Serializable]
	public class SynchronizedLayer
	{
		public PhotonAnimatorView.SynchronizeType SynchronizeType;

		public int LayerIndex;
	}
}
