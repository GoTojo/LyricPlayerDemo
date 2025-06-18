using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongInfo
{
    private static string [] songbasenamesAscii = new string [] {
		@"tabeyo",
		@"madakana",
		@"遥かな旅路",
		@"3分間のトキメキ",
		@"約束の場所へ"
	};
    private static string [] songtitle = new string [] {
		@"らーめんたべよう",
		@"らーめんまだかな",
		@"遥かな旅路",
		@"3分間のトキメキ",
		@"約束の場所へ"
	};
	public static int[] numOfMeasure{ get; } = new int[] {
		72, // @"らーめんたべよう",
		72, // @"らーめんまだかな",
		-1, // @"遥かな旅路",
		-1, // @"3分間のトキメキ",
		-1, // @"約束の場所へ"
	};

	public static int NumOfSongs() {
		return songtitle.Length;
	}
	public static bool CheckSongNum(int num) {
		if (num < 0) return false;
		if (num >= NumOfSongs()) return false;
		return true;		
	}
	public static string GetTitle(int num)
	{
		string nosong = "no song";
		if (!CheckSongNum(num)) return nosong;
		return songtitle[num];
	}
	public static string GetBaseNameAscii(int num)
	{
		string nosong = "no song";
		if (!CheckSongNum(num)) return nosong;
		return songbasenamesAscii[num];
	}
	public static string GetBaseName(int num)
	{
		string nosong = "no song";
		if (!CheckSongNum(num)) return nosong;
		return songtitle[num];
	}
	public static string GetAudioClipName(int num)
	{
		if (!CheckSongNum(num)) return "";
		return $"Audio/{GetBaseNameAscii(num)}";
	}
	public static string GetSMFPath(int num, bool isKanji)
	{
		if (!CheckSongNum(num)) return "";
		string option = isKanji ? "(漢字)" : "";
		return $"{Application.streamingAssetsPath}/{GetBaseName(num)}{option}.mid";
	}
	public static string GetInfoPath(int num, bool isKanji)
	{
		if (!CheckSongNum(num)) return "";
		string option = isKanji ? "(漢字)" : "";
		return $"{Application.streamingAssetsPath}/{GetBaseName(num)}{option}.json";		
	}
}
