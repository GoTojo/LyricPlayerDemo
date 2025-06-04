///
///	MidiWatcher
/// 
/// Copyright (c) gotojo, All Rights Reserved.
/// 

using System;
using System.Data;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;  

public class MidiWatcherBase : MIDIHandler
{
	public delegate void midiInHandler(int track, byte[] midiEvent, float position, uint currentMsec);
	public delegate void lyricInHandler(int track, string lyric, float position, uint currentMsec);
	public delegate void tempoInHandler(float msecPerQuaterNote, uint tempo, uint currentMsec);
	public delegate void beatInHandler(int numerator, int denominator, uint currentMsec);
	public delegate void measureInHandler(int measure, int measureInterval, uint currentMsec);
	public delegate void eventInHandler(MIDIHandler.Event playerEvent);
	public event midiInHandler 		onMidiIn;
	public event lyricInHandler 	onLyricIn;
	public event tempoInHandler 	onTempoIn;
	public event beatInHandler 		onBeatIn;
	public event measureInHandler 	onMeasureIn;
	public event eventInHandler 	onEventIn;
	protected int map;

	public void Clear()
	{
		onMidiIn = null;
		onLyricIn = null;
		onTempoIn = null;
		onBeatIn = null;
		onMeasureIn = null;
		onEventIn = null;
	}
	public int GetMap()
	{
		return map;
	}

	public override void MIDIIn(int track, byte[] midiEvent, float position, uint currentMsec)
	{
		onMidiIn?.Invoke(track, midiEvent, position, currentMsec);
	}
	public override void LyricIn(int track, string lyric, float position, uint currentMsec)
	{
		// Debug.Log(lyric);
		onLyricIn?.Invoke(track, lyric, position, currentMsec);
	}
	public override void TempoIn(float msecPerQuaterNote, uint tempo, uint currentMsec)
	{
		onTempoIn?.Invoke(msecPerQuaterNote, tempo, currentMsec);
	}
	public override void BeatIn(int numerator, int denominator, uint currentMsec)
	{
		// Debug.Log($"BeatIn: {numerator}/{denominator}");
		onBeatIn?.Invoke(numerator, denominator, currentMsec);
	}
	public override void MeasureIn(int measure, int measureInterval, uint currentMsec)
	{
		// Debug.Log($"MeasureIn: Measure: {measure}, Interval: {measureInterval}");
		onMeasureIn?.Invoke(measure, measureInterval, currentMsec);
	}
	public override void EventIn(MIDIHandler.Event playerEvent)
	{
		// Debug.Log($"EventIn: Event: {PlayerEvent}");
		onEventIn?.Invoke(playerEvent);
	}
}
public class MidiWatcher : MidiWatcherBase
{
	private static MidiWatcher _instance;  // singleton
	public static MidiWatcher Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new MidiWatcher();
			}
			return _instance;
		}
	}

	private MidiWatcher()
	{
		map = 1;
	}
}
public class SubMidiWatcher : MidiWatcherBase
{
	private static SubMidiWatcher _instance;  // singleton
	public static SubMidiWatcher Instance
	{
		get {
			if (_instance == null) {
				_instance = new SubMidiWatcher();
			}
			return _instance;
		}
	}

	private SubMidiWatcher()
	{
		map = 0;
	}
}
