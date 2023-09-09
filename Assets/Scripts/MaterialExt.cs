using System;
using UnityEngine;

public class MaterialExt : MonoBehaviour
{
	public static void AffectColor(GameObject go, float r, float g, float b)
	{
		MaterialExt.AffectNode(go.transform, 0, r, g, b, 0f);
	}

	public static void SetColor(GameObject go, float r, float g, float b)
	{
		MaterialExt.AffectNode(go.transform, 1, r, g, b, 0f);
	}

	public static void SetTransparent(GameObject go, float a)
	{
		MaterialExt.AffectNode(go.transform, 2, 0f, 0f, 0f, a);
	}

	public static void SetShader(GameObject go, Shader shader)
	{
		MaterialExt.SetShaderInternal(go.transform, shader);
	}

	private static void AffectNode(Transform node, int affectType, float r, float g, float b, float a)
	{
		if (node.GetComponent<Renderer>() != null)
		{
			MaterialExt materialExt = MaterialExt.GetMaterialExt(node);
			Material[] materials = node.GetComponent<Renderer>().materials;
			for (int i = 0; i < materialExt._OriginalMaterials.Length; i++)
			{
				if (materialExt._HaveColorProperty[i])
				{
					if (affectType == 0)
					{
						Color color = materialExt._OriginalMaterials[i].color;
						materials[i].color = new Color(Mathf.Clamp01(color.r + r), Mathf.Clamp01(color.g + g), Mathf.Clamp01(color.b + b), materials[i].color.a);
					}
					else if (affectType == 1)
					{
						materials[i].color = new Color(r, g, b, materials[i].color.a);
					}
					else
					{
						Color color2 = materialExt.GetComponent<Renderer>().materials[i].color;
						materials[i].color = new Color(color2.r, color2.g, color2.b, a);
					}
				}
			}
			node.GetComponent<Renderer>().materials = materials;
		}
		foreach (object obj in node)
		{
			Transform node2 = (Transform)obj;
			MaterialExt.AffectNode(node2, affectType, r, g, b, a);
		}
	}

	private static void SetShaderInternal(Transform node, Shader shader)
	{
		if (node.GetComponent<Renderer>() != null)
		{
			MaterialExt materialExt = MaterialExt.GetMaterialExt(node);
			Material[] materials = materialExt.GetComponent<Renderer>().materials;
			for (int i = 0; i < materialExt._OriginalMaterials.Length; i++)
			{
				if (materialExt._OriginalMaterials[i] != null)
				{
					materials[i].shader = ((!(shader != null)) ? materialExt._OriginalMaterials[i].shader : shader);
				}
			}
			node.GetComponent<Renderer>().materials = materials;
		}
		foreach (object obj in node)
		{
			Transform node2 = (Transform)obj;
			MaterialExt.SetShaderInternal(node2, shader);
		}
	}

	private static MaterialExt GetMaterialExt(Transform node)
	{
		MaterialExt materialExt = node.GetComponent<MaterialExt>();
		if (materialExt == null)
		{
			materialExt = node.gameObject.AddComponent<MaterialExt>();
			materialExt._OriginalMaterials = new Material[node.GetComponent<Renderer>().sharedMaterials.Length];
			materialExt._HaveColorProperty = new bool[materialExt._OriginalMaterials.Length];
			for (int i = 0; i < materialExt._OriginalMaterials.Length; i++)
			{
				if (node.GetComponent<Renderer>().sharedMaterials[i] != null)
				{
					materialExt._OriginalMaterials[i] = UnityEngine.Object.Instantiate<Material>(node.GetComponent<Renderer>().sharedMaterials[i]);
					materialExt._HaveColorProperty[i] = materialExt._OriginalMaterials[i].HasProperty("_Color");
				}
			}
		}
		return materialExt;
	}

	private Material[] _OriginalMaterials;

	private bool[] _HaveColorProperty;
}
