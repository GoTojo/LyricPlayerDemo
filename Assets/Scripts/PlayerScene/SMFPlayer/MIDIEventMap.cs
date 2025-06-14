///	
///	MIDIEventMap
/// copyright (c) 2025 gotojo, All Rights Reserved
///
using UnityEngine;
using System;
using System.Collections.Generic;

public class MIDIEventMap : MIDIHandler {
	public struct LyricData {
		public string lyric;
		public float position;
		public uint msec;
		public LyricData(string lyric, float position, uint msec) {
			this.lyric = lyric;
			this.position = position;
			this.msec = msec;
		}
	};
	public class TrackData {
		public string sentence = "";
		public List<LyricData> data;
	}
	public List<List<TrackData>> lyrics = new List<List<TrackData>>();
	public List<SMFPlayer.Beat> beats = new List<SMFPlayer.Beat>();
	public int numOfMeasure = 0;
	public int numOfTrack = 0;
	private int currentMeasure = 0;
	private SMFPlayer player;

	public MIDIEventMap() {
	}
	public void Init(SMFPlayer player) {
		this.player = player;
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
			SMFPlayer.Beat beat = new SMFPlayer.Beat();
			beats.Add(beat);
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
	public void Reset() {
		currentMeasure = 0;
	}

	public override void MIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) {
	}
	public override void LyricIn(int track, string lyric, float position, uint currentMsec) {
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
	public override void TempoIn(float msecPerQuaterNote, uint tempo, uint currentMsec) {
	}
	public override void BeatIn(int numerator, int denominator, uint currentMsec) {
	}
	public override void MeasureIn(int measure, int measureInterval, uint currentMsec) {
		if (lyrics == null) {
			return;
		}
		if (measure < 0) {
			return;
		}
		currentMeasure = measure;
		SMFPlayer.Beat beat = beats[currentMeasure];
		beat.count = player.beat.count;
		beat.unit = player.beat.unit;
		beats[currentMeasure] = beat;
	}

	public int GetNumOfLyrics(int measure, int track) {
		return lyrics[measure][track].data.Count;
	}
	public bool DataExist(int measure, int track, int num = 0) {
		if (measure < lyrics.Count) {
			if (track < lyrics[measure].Count) {
				if (num < lyrics[measure][track].data.Count) {
					return true;
				}
			}
		}
		return false;
	}
	public LyricData GetLyricData(int measure, int track, int num) {
		if (!DataExist(measure, track, num)) {
			LyricData data = new LyricData();
			return data;
		}
		return lyrics[measure][track].data[num];
	}
	public string GetSentence(int measure, int track) {
		return lyrics[measure][track].sentence;
	}
	public string GetLyric(int measure, int track, int num) {
		LyricData lyricData = GetLyricData(measure, track, num);
		return lyricData.lyric;
	}
	public float GetPosition(int measure, int track, int num) {
		LyricData lyricData = GetLyricData(measure, track, num);
		return lyricData.position;
	}
	public uint GetMsec(int measure, int track, int num) {
		LyricData lyricData = GetLyricData(measure, track, num);
		return lyricData.msec;
	}
	public SMFPlayer.Beat GetBeat(int measure) {
		return beats[measure];
	}
}
