using System;
using UnityEngine;

public class SecuredValue<T>
{
	public SecuredValue()
	{
		this._Value = default(T);
		this._Hash = this.GetHash();
	}

	public SecuredValue(T value)
	{
		this._Value = value;
		this._Hash = this.GetHash();
	}

	public T Value
	{
		get
		{
			if (!this.IsSecured())
			{
				UnityEngine.Debug.Log("Secure value failed, disconnecting.");
				PhotonNetwork.Disconnect();
				return default(T);
			}
			return this._Value;
		}
	}

	public bool IsSecured()
	{
		return this._Hash == this.GetHash();
	}

	private string GetHash()
	{
		string text = this._Value.ToString();
		return ("507" + text.Substring(0, text.Length / 2) + "430" + text.Substring(text.Length / 2)).GetHashCode().ToString();
	}

	public override string ToString()
	{
		T value = this.Value;
		return value.ToString();
	}

	public static implicit operator T(SecuredValue<T> v)
	{
		return v.Value;
	}

	public static implicit operator SecuredValue<T>(T v)
	{
		return new SecuredValue<T>(v);
	}

	private T _Value;

	private string _Hash;
}
