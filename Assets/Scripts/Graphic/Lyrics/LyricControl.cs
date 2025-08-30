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
		Clear
	};
	public void SetPosition(Vector3 position) {
		
	}
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
		default:
			break;
		}
	}
	public static string [] GetOptions(string command, int num) {
		string [] options = null;
		if (num == 0) {
			options = Enum.GetNames(typeof(Command));
		}
		return options;
	}
};