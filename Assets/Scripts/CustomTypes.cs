using System;
using System.IO;
using ExitGames.Client.Photon;
using UnityEngine;

internal static class CustomTypes
{
	internal static void Register()
	{
		PhotonPeer.RegisterType(typeof(Vector2), 87, new SerializeStreamMethod(CustomTypes.SerializeVector2), new DeserializeStreamMethod(CustomTypes.DeserializeVector2));
		PhotonPeer.RegisterType(typeof(Vector3), 86, new SerializeStreamMethod(CustomTypes.SerializeVector3), new DeserializeStreamMethod(CustomTypes.DeserializeVector3));
		PhotonPeer.RegisterType(typeof(Quaternion), 81, new SerializeStreamMethod(CustomTypes.SerializeQuaternion), new DeserializeStreamMethod(CustomTypes.DeserializeQuaternion));
		PhotonPeer.RegisterType(typeof(PhotonPlayer), 80, new SerializeStreamMethod(CustomTypes.SerializePhotonPlayer), new DeserializeStreamMethod(CustomTypes.DeserializePhotonPlayer));
	}

	private static short SerializeVector3(MemoryStream outStream, object customobject)
	{
		Vector3 vector = (Vector3)customobject;
		int num = 0;
		byte[] obj = CustomTypes.memVector3;
		lock (obj)
		{
			byte[] array = CustomTypes.memVector3;
			Protocol.Serialize(vector.x, array, ref num);
			Protocol.Serialize(vector.y, array, ref num);
			Protocol.Serialize(vector.z, array, ref num);
			outStream.Write(array, 0, 12);
		}
		return 12;
	}

	private static object DeserializeVector3(MemoryStream inStream, short length)
	{
		Vector3 vector = default(Vector3);
		byte[] obj = CustomTypes.memVector3;
		lock (obj)
		{
			inStream.Read(CustomTypes.memVector3, 0, 12);
			int num = 0;
			Protocol.Deserialize(out vector.x, CustomTypes.memVector3, ref num);
			Protocol.Deserialize(out vector.y, CustomTypes.memVector3, ref num);
			Protocol.Deserialize(out vector.z, CustomTypes.memVector3, ref num);
		}
		return vector;
	}

	private static short SerializeVector2(MemoryStream outStream, object customobject)
	{
		Vector2 vector = (Vector2)customobject;
		byte[] obj = CustomTypes.memVector2;
		lock (obj)
		{
			byte[] array = CustomTypes.memVector2;
			int num = 0;
			Protocol.Serialize(vector.x, array, ref num);
			Protocol.Serialize(vector.y, array, ref num);
			outStream.Write(array, 0, 8);
		}
		return 8;
	}

	private static object DeserializeVector2(MemoryStream inStream, short length)
	{
		Vector2 vector = default(Vector2);
		byte[] obj = CustomTypes.memVector2;
		lock (obj)
		{
			inStream.Read(CustomTypes.memVector2, 0, 8);
			int num = 0;
			Protocol.Deserialize(out vector.x, CustomTypes.memVector2, ref num);
			Protocol.Deserialize(out vector.y, CustomTypes.memVector2, ref num);
		}
		return vector;
	}

	private static short SerializeQuaternion(MemoryStream outStream, object customobject)
	{
		Quaternion quaternion = (Quaternion)customobject;
		byte[] obj = CustomTypes.memQuarternion;
		lock (obj)
		{
			byte[] array = CustomTypes.memQuarternion;
			int num = 0;
			Protocol.Serialize(quaternion.w, array, ref num);
			Protocol.Serialize(quaternion.x, array, ref num);
			Protocol.Serialize(quaternion.y, array, ref num);
			Protocol.Serialize(quaternion.z, array, ref num);
			outStream.Write(array, 0, 16);
		}
		return 16;
	}

	private static object DeserializeQuaternion(MemoryStream inStream, short length)
	{
		Quaternion quaternion = default(Quaternion);
		byte[] obj = CustomTypes.memQuarternion;
		lock (obj)
		{
			inStream.Read(CustomTypes.memQuarternion, 0, 16);
			int num = 0;
			Protocol.Deserialize(out quaternion.w, CustomTypes.memQuarternion, ref num);
			Protocol.Deserialize(out quaternion.x, CustomTypes.memQuarternion, ref num);
			Protocol.Deserialize(out quaternion.y, CustomTypes.memQuarternion, ref num);
			Protocol.Deserialize(out quaternion.z, CustomTypes.memQuarternion, ref num);
		}
		return quaternion;
	}

	private static short SerializePhotonPlayer(MemoryStream outStream, object customobject)
	{
		int id = ((PhotonPlayer)customobject).ID;
		byte[] obj = CustomTypes.memPlayer;
		short result;
		lock (obj)
		{
			byte[] array = CustomTypes.memPlayer;
			int num = 0;
			Protocol.Serialize(id, array, ref num);
			outStream.Write(array, 0, 4);
			result = 4;
		}
		return result;
	}

	private static object DeserializePhotonPlayer(MemoryStream inStream, short length)
	{
		byte[] obj = CustomTypes.memPlayer;
		int key;
		lock (obj)
		{
			inStream.Read(CustomTypes.memPlayer, 0, (int)length);
			int num = 0;
			Protocol.Deserialize(out key, CustomTypes.memPlayer, ref num);
		}
		if (PhotonNetwork.networkingPeer.mActors.ContainsKey(key))
		{
			return PhotonNetwork.networkingPeer.mActors[key];
		}
		return null;
	}

	public static readonly byte[] memVector3 = new byte[12];

	public static readonly byte[] memVector2 = new byte[8];

	public static readonly byte[] memQuarternion = new byte[16];

	public static readonly byte[] memPlayer = new byte[4];
}
