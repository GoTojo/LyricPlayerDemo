/// SentenceList.cs
/// 小節毎のLyricをひとまとめにしたsentenceのリスト
/// Copyright (c) 2025 gotojo

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class SentenceList : MonoBehaviour
{
	public string [,] sentence;
	private List<Track> [] tracks;
	private MidiEventMapAccessor eventMap;
	
	// LyricList, MidiEventMapが初期化した後に呼ぶ
	public void Init()
	{
		LyricList[] maps = GetComponents<LyricList>();
		eventMap = GetComponent<MidiEventMapAccessor>();
		const int numOfMap = MidiEventMapAccessor.numOfEventMap;
		tracks = new List<Track>[numOfMap];
		int numOfTrack = 0;
		for (var i = 0; i < numOfMap; i++) {
			tracks[i] = maps[i].tracks;
			if (numOfTrack < tracks.Length) {
				numOfTrack = tracks.Length;
			}
		}
		sentence = new string[numOfMap, numOfTrack];
	}
}