///
/// FontResource.cs
/// Font Resources
/// Copyright (c) 2025 gotojo
using UnityEngine;
using TMPro;
using System;

class FontResource {
	public enum Type {
		JKMaruGothic,
		DelaGothicOne,
		HachiMaruPop,
		KaiseiTokumin,
		LightNovelPOP,
		RocknRollOne
	};
	private static FontResource _instance;  // singleton
	public static FontResource Instance {
		get {
			if (_instance == null) {
				_instance = new FontResource();
			}
			return _instance;
		}
	}
	private TMP_FontAsset fontJKMaruGothic;
	private TMP_FontAsset fontDelaGothicOne;
	private TMP_FontAsset fontHachiMaruPop;
	private TMP_FontAsset fontKaiseiTokumin;
	private TMP_FontAsset fontLightNovelPOP;
	private TMP_FontAsset fontRocknRollOne;
	private Type curFontType = Type.JKMaruGothic;

	private string[] resourceName = new string[] {
		"JK-Maru-Gothic-M SDF",
		"DelaGothicOne-Regular SDF",
		"HachiMaruPop-Regular SDF",
		"KaiseiTokumin-Regular SDF",
		"LightNovelPOPv2 SDF",
		"RocknRollOne-Regular SDF",
	};

	FontResource() {
	}
	public void LoadFont() {
		fontJKMaruGothic = Resources.Load<TMP_FontAsset>($"Fonts/{resourceName[(int)Type.JKMaruGothic]}");
		fontDelaGothicOne = Resources.Load<TMP_FontAsset>($"Fonts/{resourceName[(int)Type.DelaGothicOne]}");
		fontHachiMaruPop = Resources.Load<TMP_FontAsset>($"Fonts/{resourceName[(int)Type.HachiMaruPop]}");
		fontKaiseiTokumin = Resources.Load<TMP_FontAsset>($"Fonts/{resourceName[(int)Type.KaiseiTokumin]}");
		fontLightNovelPOP = Resources.Load<TMP_FontAsset>($"Fonts/{resourceName[(int)Type.LightNovelPOP]}");
		fontRocknRollOne = Resources.Load<TMP_FontAsset>($"Fonts/{resourceName[(int)Type.RocknRollOne]}");
	}
	public void SetCurFont(Type type) {
		curFontType = type;
	}
	public void IncFont() {
		if (curFontType == Type.RocknRollOne) return;
		curFontType = (Type)((int)curFontType + 1);
	}
	public void DecFont() {
		if (curFontType == Type.JKMaruGothic) return;
		curFontType = (Type)((int)curFontType - 1);
	}
	public Type GetFontType(string fontname) {
		int index = Array.IndexOf(resourceName, fontname);
		if (index < 0) index = 0;
		return (Type)index;
	}
	public TMP_FontAsset GetCurFont() {
		return GetFont(curFontType);
	}
	public TMP_FontAsset GetFont(Type fontType) {
		TMP_FontAsset font;
		switch (fontType) {
		default:
		case Type.JKMaruGothic:
			font = fontJKMaruGothic;
			break;
		case Type.DelaGothicOne:
			font = fontDelaGothicOne;
			break;
		case Type.HachiMaruPop:
			font = fontHachiMaruPop;
			break;
		case Type.KaiseiTokumin:
			font = fontKaiseiTokumin;
			break;
		case Type.LightNovelPOP:
			font = fontLightNovelPOP;
			break;
		case Type.RocknRollOne:
			font = fontRocknRollOne;
			break;
		}
		return font;
	}
	public int numOfFontType() {
		return System.Enum.GetNames(typeof(Type)).Length;
	}
}