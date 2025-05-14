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
	public class TrackData
	{
		public string sentence;
		public List<LyricData> data;
	}
	public List<List<TrackData>> lyrics = new List<List<TrackData>>();
	public int numOfMeasure = 0;
	public int numOfTrack = 0;
	private int currentMeasure = 0;

	public MIDIEventMap()
	{
	}
	public void Init(SMFPlayer player)
	{
		numOfMeasure = player.numOfMeasure;
		numOfTrack = player.numOfTrack;
		for (int meas = 0; meas < numOfMeasure; meas++) {
			var tracks = new List<TrackData>();
			for (int track = 0; track < numOfTrack; track++) {
				TrackData trackData = new TrackData();
				trackData.data = new List<LyricData>();
				tracks.Add(trackData); 
			}
			lyrics.Add(tracks);
		}
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
		if (currentMeasure >= lyrics.Count) {
			return;
		}
		LyricData data = new LyricData(lyric, position, currentMsec);
		lyrics[currentMeasure][track].data.Add(data);
		lyrics[currentMeasure][track].sentence += lyric;
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
	}

	public int GetNumOfLyrics(int measure, int track)
	{
		return lyrics[measure][track].data.Count;
	}
	public bool DataExist(int measure, int track, int num = 0)
	{
		if (measure < lyrics.Count) {
			if (track < lyrics[measure].Count) {
				if (num < lyrics[measure][track].data.Count) {
					return true;
				}
			}
		}
		return false;
	}
	public LyricData GetLyricData(int measure, int track, int num)
	{
		if (!DataExist(measure, track, num)) {
			LyricData data = new LyricData();
			return data;
		}
		return lyrics[measure][track].data[num];
	}
	public string GetSentence(int measure, int track)
	{
		return lyrics[measure][track].sentence;
	}
	public string GetLyric(int measure, int track, int num)
	{
		LyricData lyricData = GetLyricData(measure, track, num);
		return lyricData.lyric;
	}
	public float GetPosition(int measure, int track, int num)
	{
		LyricData lyricData = GetLyricData(measure, track, num);
		return lyricData.position;
	}
	public UInt32 GetMsec(int measure, int track, int num)
	{
		LyricData lyricData = GetLyricData(measure, track, num);
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