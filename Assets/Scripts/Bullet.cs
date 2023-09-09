using System;
using UnityEngine;

public class Bullet : bs
{
	private void Start()
	{
		UnityEngine.Object.Destroy(base.gameObject, this.gun.explosionTime);
		if (this.gun.grenade)
		{
			base.transform.Translate(this.gun.GrenadeThrowForce.normalized);
			base.GetComponent<Rigidbody>().velocity = base.transform.rotation * this.gun.GrenadeThrowForce * Mathf.Clamp01(this.throwFactorSpeed);
			base.GetComponent<Rigidbody>().angularVelocity = UnityEngine.Random.insideUnitSphere;
		}
		if (this.gun.bazuka)
		{
			base.GetComponent<AudioSource>().volume = ProfileINI.sound_volume * ProfileINI.sound_scale;
			base.GetComponent<AudioSource>().Play();
		}
	}

	private void OnDestroy()
	{
		if (!this.gun.grenade && !this.gun.bazuka)
		{
			return;
		}
		if (this.gun.playerCC != null)
		{
			bs._WorldGameObjectX.photonView.RPC("Explosion", PhotonTargets.All, new object[]
			{
				base.pos
			});
			foreach (PlayerNetwork playerNetwork in UnityEngine.Object.FindObjectsOfType(typeof(PlayerNetwork)))
			{
				if ((playerNetwork.pos - base.pos).magnitude < this.gun.expDist && (TeamBattle.Instance.IsOpposite(playerNetwork.PlayerName) || playerNetwork.GetComponent<CameraController>() != null))
				{
					playerNetwork.GetComponent<PhotonView>().RPC("OnHit", PhotonTargets.All, new object[]
					{
						(playerNetwork.pos - base.pos).normalized * 1000f * (float)this.gun.ragdollForce,
						base.pos
					});
					bs._WorldGameObjectX.PunchPlayer(playerNetwork.photonView.owner, playerNetwork.pos - base.pos, this.gun.damage, this.gun.arrayId, null, false);
				}
			}
			Vector3 vector = base.transform.position + ((!this.gun.bazuka) ? Vector3.down : Vector3.zero);
			int num = 1;
			int num2 = (int)vector.x;
			int num3 = (int)vector.z;
			int num4 = (int)vector.y;
			if (App.Instance.Settings.destroyable && num2 >= 0 && num3 >= 0 && num4 >= 0 && num2 < WorldData.Instance.WidthInBlocks && num3 < WorldData.Instance.HeightInBlocks && num4 < WorldData.Instance.DepthInBlocks)
			{
				bs._WorldGameObjectX.photonView.RPC("RemoveBlocksAt", PhotonTargets.All, new object[]
				{
					num2,
					num3,
					num4,
					num
				});
			}
		}
	}

	private void Update()
	{
		this.Update2(Time.deltaTime);
	}

	private void OnCollisionEnter(Collision collision)
	{
		base.PlayOneShot(bs._Igor.grenadeHit);
	}

	public void Update2(float deltaTime)
	{
		if (this.gun.grenade)
		{
			return;
		}
		if (this.gun.BulletGravity > 0f)
		{
			base.transform.forward += Vector3.down * this.gun.BulletGravity * deltaTime;
		}
		Vector3 vector = base.transform.forward * this.gun.bulletSpeed * deltaTime;
		UnityEngine.Debug.DrawRay(base.transform.position, vector.normalized);
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, vector, out raycastHit, vector.magnitude + 2f, 1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("Players")) && raycastHit.collider.tag != "Water")
		{
			if (this.gun.playerCC != null && !this.gun.bazuka)
			{
				PlayerNode playerNode = bs._WorldGameObjectX.FindPlayerByAvatar(raycastHit.collider.gameObject);
				if (playerNode != null)
				{
					if (TeamBattle.Instance.IsOpposite(playerNode.NetPlayer.name))
					{
						SecuredValue<float> v = this.gun.damage;
						if ((raycastHit.point - playerNode.Avatar.GetComponent<PlayerNetwork>().Head.position).magnitude < 0.5f)
						{
							v *= this.gun.HeadHit;
						}
						playerNode.Avatar.GetComponent<PhotonView>().RPC("OnHit", PhotonTargets.All, new object[]
						{
							vector.normalized * 1000f * (float)this.gun.ragdollForce,
							raycastHit.point
						});
						bs._WorldGameObjectX.PunchPlayer(playerNode.NetPlayer, vector.normalized, v, this.gun.arrayId, null, false);
					}
				}
				else
				{
					UnityEngine.Object.Instantiate(bs._Igor.concerne, raycastHit.point, Quaternion.LookRotation(raycastHit.normal));
					Vector3 vector2 = raycastHit.point - raycastHit.normal * 0.1f;
					IntVect intVect = new IntVect((int)vector2.x, (int)vector2.z, (int)vector2.y);
				}
			}
			AudioSource.PlayClipAtPoint(bs._Igor.hitSound[UnityEngine.Random.Range(0, bs._Igor.hitSound.Length)], raycastHit.point, ProfileINI.sound_volume * ProfileINI.sound_scale);
			base.transform.position = raycastHit.point;
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		base.transform.position += vector;
	}

	internal Gun gun;

	internal float throwFactorSpeed;
}
