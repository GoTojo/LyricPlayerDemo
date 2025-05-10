using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongInfo
{
    private static string [] songbasenames = new string [] {
		@"らーめんたべよう",
		@"らーめんまだかな",
		@"遥かな旅路",
		@"3分間のトキメキ",
		@"約束の場所へ"
	};

	public static int NumOfSongs()
	{
		return songbasenames.Length;
	}
	public static bool CheckSongNum(int num) {
		if (num < 0) return false;
		if (num >= NumOfSongs()) return false;
		return true;		
	}
	public static string GetBaseName(int num)
	{
		string nosong = "no song";
		if (!CheckSongNum(num)) return nosong;
		if (num >= NumOfSongs()) return nosong;
		return songbasenames[num];
	}
	public static string GetAudioClipName(int num)
	{
		if (!CheckSongNum(num)) return "";
		return $"Audio/{GetBaseName(num)}";
	}
	public static string GetSMFPath(int num, bool isKanji)
	{
		if (!CheckSongNum(num)) return "";
		string option = isKanji ? "(漢字)" : "";
		return $"{Application.streamingAssetsPath}/{GetBaseName(num)}{option}.mid";
	}
}
