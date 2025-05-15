/// LyricList
/// Copyright (c) gotojo, All Rights Reserved.
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;

[Serializable]
public struct LyricData {
	public uint msec;
	public string sentence;
	public LyricData(uint msec, string sentence) {
		this.msec = msec;
		this.sentence = sentence;
	}
}

[Serializable]
public class Track {
	public List<LyricData> lyrics = new List<LyricData>();
}

[Serializable]
public class TrackListWrapper
{
	public List<Track> tracks = new List<Track>();
}

public class LyricList : MonoBehaviour
{
	public List<Track> tracks = new List<Track>();
	public int map = 0;

	public void Init()
	{
		string path = SongInfo.GetInfoPath(PlayerPrefs.GetInt("Song"), map != 0);

		if (File.Exists(path)) {
			Load(path);
		} else {
			GenerateTracks();
			Save(path);
		}
	}
	void Start()
	{
	}
	private void GenerateTracks()
	{
		MidiEventMapAccessor eventMap = GetComponent<MidiEventMapAccessor>();
		int numOfMeasure = eventMap.GetNumOfMeasure(map);
		int numOfTrack = eventMap.GetNumOfTrack(map);

		// Debug.Log($"numOfMeasure: {numOfMeasure}");
		// Debug.Log($"numOfTrack: {numOfTrack}");
		for (var track = 0; track < numOfTrack; track++) {
			var trackData = new Track();
			for (var meas = 0; meas < numOfMeasure; meas++) {
				uint msec = (uint)eventMap.GetMsec(meas, track, 0, map);
				string sentence = eventMap.GetSentence(meas, track, map);
				trackData.lyrics.Add(new LyricData(msec, sentence));
				// Debug.Log($"meas:{meas} {msec}:{sentence}");
			}
			tracks.Add(trackData);
		}
	}
	private void Save(string path)
	{
		var wrapper = new TrackListWrapper { tracks = tracks };
		string json = JsonUtility.ToJson(wrapper, true);
		File.WriteAllText(path, json);
	}
	private void Load(string path)
	{
		string json = File.ReadAllText(path);
		var wrapper = JsonUtility.FromJson<TrackListWrapper>(json);
		tracks = wrapper.tracks;
	}
}