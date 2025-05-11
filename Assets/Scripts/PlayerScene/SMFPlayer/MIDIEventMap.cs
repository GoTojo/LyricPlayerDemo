///	
///	MIDIEventMap
/// copyright (c) 2025 gotojo, All Rights Reserved
///
using UnityEngine;
using System;
using System.Collections.Generic;

public class MIDIEventMap : MIDIHandler
{
	public struct LyricData
	{
		public string lyric;
		public float position;
		public UInt32 msec;
		public LyricData(string lyric, float position, UInt32 msec)
		{
			this.lyric = lyric;
			this.position = position;
			this.msec = msec;
		}
	};
	private List<LyricData> [] lyrics = null;
	private int currentMeasure = 0;

	public MIDIEventMap()
	{
	}
	public void Init(SMFPlayer player)
	{
		lyrics = new List<LyricData>[player.numOfMeasure];
		MIDIHandler backupHandler = player.midiHandler;
		player.midiHandler = this;
		bool fLastMute = player.mute;
		player.mute = false;
		player.Reset();
		player.Start();
		uint time = 0;
		while (player.Update(time)) {
			time += 10;
		}
		player.Reset();
		player.mute = fLastMute;
		player.midiHandler = backupHandler;
		Reset();
	}
	public void Reset()
	{
		currentMeasure = 0;
	}

	public override void MIDIIn(int track, byte[] midiEvent, float position, UInt32 currentMsec)
	{
	}
	public override void LyricIn(int track, string lyric, float position, UInt32 currentMsec)
	{
		if (lyrics == null) {
			return;
		}
		if (currentMeasure >= lyrics.Length) {
			return;
		}
		LyricData data = new LyricData(lyric, position, currentMsec);
		// Debug.Log($"EventMap.LyricIn: currentMeasure:{currentMeasure}");
		lyrics[currentMeasure].Add(data);
	}
	public override void TempoIn(float msecPerQuaterNote, uint tempo, UInt32 currentMsec)
	{
	}
	public override void BeatIn(int numerator, int denominator, UInt32 currentMsec)
	{
	}
	public override void MeasureIn(int measure, int measureInterval, UInt32 currentMsec)
	{
		if (lyrics == null) {
			return;
		}
		if (measure < 0) {
			return;
		}
		currentMeasure = measure;
		lyrics[currentMeasure] = new List<LyricData>();
	}

	public int GetNumOfLyrics(int measure)
	{
		return lyrics[measure].Count;
	}
	public LyricData GetLyricData(int measure, int num) {
		if (measure < lyrics.Length) {
			if (num < lyrics[measure].Count) {
				return lyrics[measure][num];
			}
		}
		LyricData data = new LyricData();
		return data;
	}
	public string GetLyric(int measure, int num)
	{
		LyricData lyricData = GetLyricData(measure, num);
		return lyricData.lyric;
	}
	public float GetPosition(int measure, int num)
	{
		LyricData lyricData = GetLyricData(measure, num);
		return lyricData.position;
	}
	public UInt32 GetMsec(int measure, int num)
	{
		LyricData lyricData = GetLyricData(measure, num);
		return lyricData.msec;
	}
}

public class MidiEventMapAccessor
{
	private static MidiEventMapAccessor _instance;	// singleton
	public static MidiEventMapAccessor Instance
	{
		get {
			if (_instance == null) {
				_instance = new MidiEventMapAccessor();
			}
			return _instance;
		}
	}
	private const int numOfEventMap = 2;
	private MIDIEventMap [] eventMap = new MIDIEventMap[numOfEventMap];
	private int currentMap = 0;
	private MidiEventMapAccessor()
	{
		for (var i = 0; i < numOfEventMap; i++) {
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
	public int GetNumOfLyrics(int measure, int map = -1)
	{
		if (map < 0) map = currentMap;
		return eventMap[map].GetNumOfLyrics(measure);
	}
	public string GetLyric(int measure, int num, int map = -1)
	{
		if (map < 0) map = currentMap;
		return eventMap[map].GetLyric(measure, num);
	}
	public float GetPosition(int measure, int num, int map = -1)
	{
		if (map < 0) map = currentMap;
		return eventMap[map].GetPosition(measure, num);
	}
	public UInt32 GetMsec(int measure, int num, int map = -1)
	{
		if (map < 0) map = currentMap;
		return eventMap[map].GetMsec(measure, num);
	}
}