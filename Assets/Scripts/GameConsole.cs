using System;
using System.Text;
using UnityEngine;

public class GameConsole : MonoBehaviour
{
	private void Awake()
	{
		GameConsole.Instance = this;
		this.strb.AppendLine("Version: " + SettingsManager.Version);
		this.strb.AppendLine(string.Empty);
		this.strb.AppendLine("System info:");
		this.strb.AppendLine("CPU: " + SystemInfo.processorType);
		this.strb.AppendLine("GPU: " + SystemInfo.graphicsDeviceName);
		this.strb.AppendLine("DirectX: " + SystemInfo.graphicsDeviceVersion);
		this.strb.AppendLine("OC: " + SystemInfo.operatingSystem);
		this.strb.AppendLine("RAM: " + SystemInfo.systemMemorySize);
		this.strb.AppendLine(string.Empty);
		this.strb.AppendLine("Console:");
		Application.logMessageReceived += this.HandleLog;
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown("`") && UnityEngine.Input.GetKey("left ctrl"))
		{
			GameConsole.show = !GameConsole.show;
		}
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Exception || type == LogType.Error)
		{
			this.error_count++;
		}
		if (this.show_output || this.show_stack)
		{
			this.strb.Append("\n");
			if (this.show_output)
			{
				this.strb.AppendLine(logString);
			}
			if (this.show_stack || type == LogType.Exception || type == LogType.Error)
			{
				this.strb.AppendLine(stackTrace);
			}
		}
	}

	public void OnGUI()
	{
		if (GameConsole.show)
		{
			this.scroll_pos = GUI.BeginScrollView(this.pos_rect, this.scroll_pos, this.view_rect);
			GUI.TextArea(new Rect(0f, 0f, this.view_rect.width - 50f, this.view_rect.height), this.strb.ToString());
			GUI.EndScrollView();
		}
	}

	public static GameConsole Instance;

	public bool show_output = true;

	public bool show_stack;

	private static bool show;

	private int error_count;

	private StringBuilder strb = new StringBuilder();

	private Rect pos_rect = new Rect(50f, 75f, 400f, 400f);

	private Rect view_rect = new Rect(0f, 0f, 450f, 30000f);

	private Vector2 scroll_pos;
}
