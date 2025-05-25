///
/// FontResource.cs
/// Font Resources
/// Copyright (c) 2025 gotojo
using UnityEngine;
using TMPro;

class FontResource
{
	public enum Type {
		JKMaruGothic,
	};
	private static FontResource _instance;  // singleton
	public static FontResource Instance
	{
		get {
			if (_instance == null) {
				_instance = new FontResource();
			}
			return _instance;
		}
	}
	private TMP_FontAsset fontJKMaruGothic;
	FontResource()
	{
		fontJKMaruGothic = Resources.Load<TMP_FontAsset>("Fonts/JK-Maru-Gothic-M SDF");;
	}
	public TMP_FontAsset GetFont(Type fontType)
	{
		return fontJKMaruGothic;
	}
}