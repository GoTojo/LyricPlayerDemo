///
/// FontResource.cs
/// Font Resources
/// Copyright (c) 2025 gotojo
using UnityEngine;
using TMPro;

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

	FontResource() {
	}
	public void LoadFont() {
		fontJKMaruGothic = Resources.Load<TMP_FontAsset>("Fonts/JK-Maru-Gothic-M SDF");
		fontDelaGothicOne = Resources.Load<TMP_FontAsset>("Fonts/DelaGothicOne-Regular SDF");
		fontHachiMaruPop = Resources.Load<TMP_FontAsset>("Fonts/HachiMaruPop-Regular SDF");
		fontKaiseiTokumin = Resources.Load<TMP_FontAsset>("Fonts/KaiseiTokumin-Regular SDF");
		fontLightNovelPOP = Resources.Load<TMP_FontAsset>("Fonts/LightNovelPOPv2 SDF");
		fontRocknRollOne = Resources.Load<TMP_FontAsset>("Fonts/RocknRollOne-Regular SDF");
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
	public TMP_FontAsset GetFont() {
		TMP_FontAsset font;
		switch (curFontType) {
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