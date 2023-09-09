using System;
using UnityEngine;

public class PlayerFlyCheatDetector : MonoBehaviour
{
	private void Start()
	{
		this._cameraController = base.GetComponent<CameraController>();
		this._characterController = base.GetComponent<CharacterController>();
		this._playerNode = WorldGameObjectX.Instance.FindPlayerByNameX(base.GetComponent<PlayerNetwork>().PlayerName);
		this._suspectedCheatStartTime = Time.time;
	}

	private void Update()
	{
	}

	private void CheatFound()
	{
		UnityEngine.Debug.LogError("CheatFound");
		Chat.SendInfoF("Игрок " + base.GetComponent<PlayerNetwork>().PlayerName + " был кикнут за использование читов. Код: 001", true);
		MainMenu.IsCheatOn = true;
		MainMenu.Instance.ShowHint("Вы были кикнуты за использование читов \n Код: 001", true);
	}

	private const string JUMP_ANIM_NAME = "jump";

	private CameraController _cameraController;

	private CharacterController _characterController;

	private PlayerNode _playerNode;

	private bool _suspectedCheat;

	private float _suspectedCheatStartTime;
}
