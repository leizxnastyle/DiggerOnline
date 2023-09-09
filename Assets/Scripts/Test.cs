using System;
using Photon;

public class Test : MonoBehaviour
{
	private void Awake()
	{
		if (Test.Instance == null)
		{
			Test.Instance = this;
		}
	}

	public static Test Instance;
}
