///
/// MidiEventMapAccessor
/// Copyright (c) 2025 gotojo
///
using UnityEngine;
using System;

public class MidiEventMapAccessor : MonoBehaviour
{
	public const int numOfEventMap = 2;
	private MIDIEventMap[] eventMap = new MIDIEventMap[numOfEventMap];
	public int currentMap = 0;
	public MidiEventMapAccessor()
	{
		for (var i = 0; i < numOfEventMap; i++)
		{
			eventMap[i] = new MIDIEventMap();
		}
	}
	public void Init(SMFPlayer player, SMFPlayer subplayer)
	{
		eventMap[0].Init(player);
		eventMap[1].Init(subplayer);
	}
	public void SetCurrentMap(int num)
	{
		currentMap = num;
	}
	public bool IsDataExist(int measure, int track, int map = -1)
	{
		if (map < 0) map = currentMap;
		return eventMap[map].DataExist(measure, track);
	}
	public string GetSentence(int measure, int track, int map = -1)
	{
		if (map < 0) map = currentMap;
		return eventMap[map].GetSentence(measure, track);
	}
	public int GetNumOfLyrics(int measure, int track, int map = -1)
	{
		if (map < 0) map = currentMap;
		return eventMap[map].GetNumOfLyrics(measure, track);
	}
	public string GetLyric(int measure, int track, int num, int map = -1)
	{
		if (map < 0) map = currentMap;
		return eventMap[map].GetLyric(measure, track, num);
	}
	public float GetPosition(int measure, int track, int num, int map = -1)
	{
		if (map < 0) map = currentMap;
		return eventMap[map].GetPosition(measure, track, num);
	}
	public UInt32 GetMsec(int measure, int track, int num, int map = -1)
	{
		if (map < 0) map = currentMap;
		return eventMap[map].GetMsec(measure, track, num);
	}
	public int GetNumOfMeasure(int map = -1)
	{
		if (map < 0) map = currentMap;
		return eventMap[map].numOfMeasure;
	}
	public int GetNumOfTrack(int map = -1)
	{
		if (map < 0) map = currentMap;
		return eventMap[map].numOfTrack;
	}
}