/// SentenceList.cs
/// 小節毎のLyricをひとまとめにしたsentenceのリスト
/// Copyright (c) 2025 gotojo

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SentenceList : MonoBehaviour {
	private MidiEventMapAccessor eventMap;
	private List<Track>[] tracks;

	// LyricList, MidiEventMapが初期化した後に呼ぶ
	public void Init() {
		GameObject mainObj = GameObject.Find("MainGameObject");
		LyricList[] maps = mainObj.GetComponents<LyricList>();
		eventMap = mainObj.GetComponent<MidiEventMapAccessor>();
		const int numOfMap = MidiEventMapAccessor.numOfEventMap;
		tracks = new List<Track>[numOfMap];
		for (var i = 0; i < numOfMap; i++) {
			tracks[i] = maps[i].tracks;
		}
	}
	public int GetNumOfTrack() {
		return tracks.Length;
	}
	public bool IsActive(int track, int map = -1) {
		if (map < 0) map = eventMap.currentMap;
		if (map > tracks.Length) return false;
		List<Track> trackList = tracks[map];
		if (track > trackList.Count) return false;
		Track trackData = trackList[track];
		return trackData.active;
	}
	public LyricData GetSentence(int track, int measure, int map = -1) {
		if (map < 0) map = eventMap.currentMap;
		LyricData emptyData = new LyricData(0, "", 1);
		if (map > tracks.Length) return emptyData;
		List<Track> trackList = tracks[map];
		if (track > trackList.Count) return emptyData;
		Track trackData = trackList[track];
		if (measure > trackData.lyrics.Count) return emptyData;
		return trackData.lyrics[measure];
	}
	public bool IsExist(int measure, int map = -1) {
		if (map < 0) map = eventMap.currentMap;
		bool fIsExist = false;
		for (int track = 0; track < tracks[map].Count; track++) {
			string sentence = tracks[map][track].lyrics[measure].sentence;
			if (!String.IsNullOrEmpty(sentence)) {
				fIsExist = true;
				break;
			}
		}
		return fIsExist;
	}
}