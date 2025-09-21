/// LyricControl.cs
/// Lyricのエディットやイベントの反映などを行う
/// Copyright (c) 2025 gotojo
using System;
using UnityEngine;
using TMPro;
public class LyricControl : MonoBehaviour {
	public SimpleLyricGen words;
	public LyricGenUnder1Line line;
	public LyricGenMultiLine multiL;
	public LyricGenMultiLine multiR;
	public LyricGenMultiLine multiVL;
	public LyricGenMultiLine multiVR;
	public LyricGenMultiLineByWord multiWordL;
	public LyricGenMultiLineByWord multiWordR;
	public LyricGenMultiLineByWord multiWordVL;
	public LyricGenMultiLineByWord multiWordVR;
	public TitleControl title;

	public enum Type {
		Title,
		Line,
		Words,
		MultiL,
		MultiR,
		MultiVL,
		MultiVR,
		MultiWordL,
		MultiWordR,
		MultiWordVL,
		MultiWordVR,
	};
	public enum Command {
		On,
		Off,
		Stop,
		Clear,
		POSX,
		POSY,
		POSW,
		POSH,
		FONT
	};
	public LyricBase GetLyricObj(Type type) {
		LyricBase lyric = null;
		switch (type) {
		case Type.Title:
			lyric = title;
			break;
		case Type.Line:
			lyric = line;
			break;
		case Type.Words:
			lyric = words;
			break;
		case Type.MultiL:
			lyric = multiL;
			break;
		case Type.MultiR:
			lyric = multiR;
			break;
		case Type.MultiVL:
			lyric = multiVL;
			break;
		case Type.MultiVR:
			lyric = multiVR;
			break;
		case Type.MultiWordL:
			lyric = multiWordL;
			break;
		case Type.MultiWordR:
			lyric = multiWordR;
			break;
		case Type.MultiWordVL:
			lyric = multiWordVL;
			break;
		case Type.MultiWordVR:
			lyric = multiWordVR;
			break;
		default:
			break;
		}
		return lyric;
	}
	public void SetPosition(LyricBase lyric, string param, float value) {
		switch (param) {
		case "POSX":
			lyric.SetPosX(value);
			break;
		case "POSY":
			lyric.SetPosY(value);
			break;
		case "POSW":
			lyric.SetPosW(value);
			break;
		case "POSH":
			lyric.SetPosH(value);
			break;
		case "FONT":
			lyric.SetPosH(value);
			break;
		default:
			break;
		}
	}
	public float GetPosition(LyricBase lyric, string param) {
		float position = 0;
		switch (param) {
		case "POSX":
			position = lyric.GetPosX();
			break;
		case "POSY":
			position = lyric.GetPosY();
			break;
		case "POSW":
			position = lyric.GetPosW();
			break;
		case "POSH":
			position = lyric.GetPosH();
			break;
		default:
			break;
		}
		return position;
	}
	public void ApplyControl(string[] args) {
		if (args.Length == 0) return;
		Type type = (Type)Enum.Parse(typeof(Type), args[0]);
		LyricBase lyric = GetLyricObj(type);
		if (!lyric) return;
		if (args.Length < 2) return;
		switch (args[1]) {
		case "On":
			lyric.SetActive(true);
			break;
		case "Stop":
			lyric.SetActive(false);
			break;
		case "Off":
			lyric.SetActive(false);
			lyric.Clear();
			break;
		case "Clear":
			lyric.Clear();
			break;
		case "FONT":
			if (args.Length < 3) return;
			FontResource.Type fontType = (FontResource.Type)Enum.Parse(typeof(FontResource.Type), args[2]);
			lyric.SetFont(FontResource.Instance.GetFont(fontType));
			break;
		case "POSX":
		case "POSY":
		case "POSW":
		case "POSH":
			if (args.Length < 3) return;
			SetPosition(lyric, args[1], float.Parse(args[2]));
			break;
		default:
			break;
		}
	}
	public static string [] GetOptions(string command, int num) {
		string[] args = command.Split("_");
		string[] options = null;
		if (num == 0) {
			options = Enum.GetNames(typeof(Command));
		} else if (num == 1) {
			if (args.Length < 2) return options;
			switch (args[1]) {
			case "FONT":
				options = Enum.GetNames(typeof(FontResource.Type));
				break;
			case "POSX":
			case "POSY":
			case "POSW":
			case "POSH":
				options = new string[] {"VARIABLE"};
				break;
			default:
				break;
			}
		}
		return options;
	}
};