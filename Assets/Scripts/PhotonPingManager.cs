using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonPingManager
{
	public Region BestRegion
	{
		get
		{
			Region result = null;
			int num = int.MaxValue;
			foreach (Region region in PhotonNetwork.networkingPeer.AvailableRegions)
			{
				UnityEngine.Debug.Log("BestRegion checks region: " + region);
				if (region.Ping != 0 && region.Ping < num)
				{
					num = region.Ping;
					result = region;
				}
			}
			return result;
		}
	}

	public bool Done
	{
		get
		{
			return this.PingsRunning == 0;
		}
	}

	public IEnumerator PingSocket(Region region)
	{
		region.Ping = PhotonPingManager.Attempts * PhotonPingManager.MaxMilliseconsPerPing;
		this.PingsRunning++;
		PhotonPing ping;
		if (PhotonHandler.PingImplementation == typeof(PingNativeDynamic))
		{
			UnityEngine.Debug.Log("Using constructor for new PingNativeDynamic()");
			ping = new PingNativeDynamic();
		}
		else if (PhotonHandler.PingImplementation == typeof(PingMono))
		{
			ping = new PingMono();
		}
		else
		{
			ping = (PhotonPing)Activator.CreateInstance(PhotonHandler.PingImplementation);
		}
		float rttSum = 0f;
		int replyCount = 0;
		string cleanIpOfRegion = region.HostAndPort;
		int indexOfColon = cleanIpOfRegion.LastIndexOf(':');
		if (indexOfColon > 1)
		{
			cleanIpOfRegion = cleanIpOfRegion.Substring(0, indexOfColon);
		}
		cleanIpOfRegion = PhotonPingManager.ResolveHost(cleanIpOfRegion);
		for (int i = 0; i < PhotonPingManager.Attempts; i++)
		{
			bool overtime = false;
			Stopwatch sw = new Stopwatch();
			sw.Start();
			try
			{
				ping.StartPing(cleanIpOfRegion);
			}
			catch (Exception ex)
			{
				Exception e = ex;
				UnityEngine.Debug.Log("catched: " + e);
				this.PingsRunning--;
				break;
			}
			while (!ping.Done())
			{
				if (sw.ElapsedMilliseconds >= (long)PhotonPingManager.MaxMilliseconsPerPing)
				{
					overtime = true;
					break;
				}
				yield return 0;
			}
			int rtt = (int)sw.ElapsedMilliseconds;
			if (!PhotonPingManager.IgnoreInitialAttempt || i != 0)
			{
				if (ping.Successful && !overtime)
				{
					rttSum += (float)rtt;
					replyCount++;
					region.Ping = (int)(rttSum / (float)replyCount);
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
		this.PingsRunning--;
		yield return null;
		yield break;
	}

	public static string ResolveHost(string hostName)
	{
		try
		{
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
			if (hostAddresses.Length == 1)
			{
				return hostAddresses[0].ToString();
			}
			foreach (IPAddress ipaddress in hostAddresses)
			{
				if (ipaddress != null)
				{
					string text = ipaddress.ToString();
					if (text.IndexOf('.') >= 0)
					{
						return text;
					}
				}
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log("Exception caught! " + ex.Source + " Message: " + ex.Message);
		}
		return string.Empty;
	}

	public bool UseNative;

	public static int Attempts = 5;

	public static bool IgnoreInitialAttempt = true;

	public static int MaxMilliseconsPerPing = 800;

	private int PingsRunning;
}
