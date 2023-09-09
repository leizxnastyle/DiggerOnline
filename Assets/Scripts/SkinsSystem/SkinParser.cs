using System;
using System.Collections.Generic;
using System.Xml;
using InventorySystem;
using UnityEngine;

namespace SkinsSystem
{
	public static class SkinParser
	{
		public static void ParseSkins()
		{
			SkinParser.temp_skins = new Dictionary<int, SS_mdl_Skin>();
			SkinParser.skins_xml = new XmlDocument();
			TextAsset textAsset = Resources.Load("DWork/Config/Skins") as TextAsset;
			SkinParser.skins_xml.LoadXml(textAsset.text);
			SkinParser.ParseItems();
		}

		private static void ParseItems()
		{
			XmlNodeList elementsByTagName = SkinParser.skins_xml.GetElementsByTagName("skin");
			for (int i = 0; i < elementsByTagName.Count; i++)
			{
				SkinParser.Parse(elementsByTagName[i]);
			}
		}

		private static void Parse(XmlNode xmlNode)
		{
			SS_mdl_Skin ss_mdl_Skin = new SS_mdl_Skin();
			ss_mdl_Skin.skin_id = int.Parse(xmlNode.Attributes["skin_id"].Value);
			ss_mdl_Skin.skin_name = xmlNode.Attributes["skin_name"].Value;
			XmlNodeList childNodes = xmlNode.ChildNodes;
			if (childNodes.Count > 0)
			{
				for (int i = 0; i < childNodes.Count; i++)
				{
					ss_mdl_Skin.item_id[i] = int.Parse(childNodes[i].Attributes["item_id"].Value);
					if (childNodes[i].Name == "part_head")
					{
						ss_mdl_Skin.head = SkinParser.GetPart(childNodes[i], ss_mdl_Skin.skin_id, ss_mdl_Skin.item_id[i]);
					}
					else if (childNodes[i].Name == "part_body")
					{
						ss_mdl_Skin.body = SkinParser.GetPart(childNodes[i], ss_mdl_Skin.skin_id, ss_mdl_Skin.item_id[i]);
					}
					else if (childNodes[i].Name == "part_legs")
					{
						ss_mdl_Skin.legs = SkinParser.GetPart(childNodes[i], ss_mdl_Skin.skin_id, ss_mdl_Skin.item_id[i]);
					}
				}
			}
			SkinParser.temp_skins.Add(ss_mdl_Skin.skin_id, ss_mdl_Skin);
		}

		private static SS_mdl_Body_Path GetPart(XmlNode xmlNode, int sid, int item_id)
		{
			return new SS_mdl_Body_Path((eIS_Arrmor_SUBT)int.Parse(xmlNode.Attributes["id"].Value), sid, xmlNode.Attributes["db_part_Name"].Value, item_id);
		}

		private static XmlDocument skins_xml;

		public static Dictionary<int, SS_mdl_Skin> temp_skins = new Dictionary<int, SS_mdl_Skin>();
	}
}
