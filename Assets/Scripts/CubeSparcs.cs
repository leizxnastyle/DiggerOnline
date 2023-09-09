using System;
using System.Collections;
using UnityEngine;

public class CubeSparcs : MonoBehaviour
{
	private void Start()
	{
		for (int i = 0; i < this.parts.Length; i++)
		{
			float num = UnityEngine.Random.Range(-0.4f, 0.4f);
			float num2 = UnityEngine.Random.Range(-0.4f, 0.4f);
			float num3 = UnityEngine.Random.Range(-0.4f, 0.4f);
			MeshCollider meshCollider = this.parts[i].AddComponent<MeshCollider>();
			meshCollider.convex = true;
			meshCollider.material = this.material;
			Rigidbody rigidbody = this.parts[i].AddComponent<Rigidbody>();
			rigidbody.AddForce(new Vector3(num * 300f, num2 * 300f, num3 * 300f));
			this.parts[i].layer = LayerMask.NameToLayer("ParticleCubesDestroy");
		}
		this.AffectColor();
		base.StartCoroutine(this.FadeProcess());
	}

	public void SetTexture(Texture2D texture)
	{
		for (int i = 0; i < this.parts.Length; i++)
		{
			this.parts[i].GetComponent<Renderer>().material.mainTexture = texture;
		}
	}

	private IEnumerator FadeProcess()
	{
		yield return new WaitForSeconds(1f);
		float alpha = 1f;
		float startTime = Time.time;
		while (alpha > 0f)
		{
			alpha = Mathf.Clamp01(1f - (Time.time - startTime));
			MaterialExt.SetTransparent(base.gameObject, alpha);
			this.AffectColor();
			yield return new WaitForSeconds(0.01f);
		}
		yield return new WaitForSeconds(1f);
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	private void AffectColor()
	{
		int x = (int)base.transform.position.x;
		int y = (int)base.transform.position.z;
		int z = (int)base.transform.position.y;
		if (!WorldData.Instance.IsValidBlock(x, y, z))
		{
			return;
		}
		byte blockLight = World.Instance.Lighting.GetBlockLight(x, y, z);
		float a = (float)(blockLight >> 4) * (1f - TimeOfDay.NormalizedTime);
		float b = (float)(blockLight & 4);
		float num = Mathf.Max(a, b) / (float)World.Instance.Lighting.LightingSteps;
		float num2 = (1f - num) * -0.4f;
		MaterialExt.AffectColor(base.gameObject, num2, num2, num2);
	}

	public GameObject[] parts;

	public PhysicMaterial material;
}
