using System;

public class TypedLobby
{
	public TypedLobby()
	{
		this.Name = string.Empty;
		this.Type = LobbyType.Default;
	}

	public TypedLobby(string name, LobbyType type)
	{
		this.Name = name;
		this.Type = type;
	}

	public bool IsDefault
	{
		get
		{
			return this.Type == LobbyType.Default && string.IsNullOrEmpty(this.Name);
		}
	}

	public override string ToString()
	{
		return string.Format("lobby '{0}'[{1}]", this.Name, this.Type);
	}

	public string Name;

	public LobbyType Type;

	public static readonly TypedLobby Default = new TypedLobby();
}
