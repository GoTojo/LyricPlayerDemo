///
///	MidiWatcher
/// 
/// Copyright (c) gotojo, All Rights Reserved.
/// 

using System;
using UnityEngine;  

public class MidiWatcher : MIDIHandler
{
	private static MidiWatcher _instance;	// singleton
    public static MidiWatcher Instance
    {
        get {
            if (_instance == null) {
                _instance = new MidiWatcher();
            }
            return _instance;
        }
    }

	public delegate void midiInHandler(int track, byte[] midiEvent, float position, UInt32 currentMsec);
	public delegate void lyricInHandler(int track, string lyric, float position, UInt32 currentMsec);
	public delegate void tempoInHandler(float msecPerQuaterNote, uint tempo, UInt32 currentMsec);
	public delegate void beatInHandler(int numerator, int denominator, UInt32 currentMsec);
	public delegate void measureInHandler(int measure, int measureInterval, UInt32 currentMsec); 
	public event midiInHandler 		onMidiIn;
	public event lyricInHandler 	onLyricIn;
	public event tempoInHandler 	onTempoIn;
	public event beatInHandler 		onBeatIn;
	public event measureInHandler 	onMeasureIn;

	private MidiWatcher()
	{
	}

	public override void MIDIIn(int track, byte[] midiEvent, float position, UInt32 currentMsec)
	{
		onMidiIn?.Invoke(track, midiEvent, position, currentMsec);
	}
	public override void LyricIn(int track, string lyric, float position, UInt32 currentMsec)
	{
		// Debug.Log(lyric);
		onLyricIn?.Invoke(track, lyric, position, currentMsec);
	}
	public override void TempoIn(float msecPerQuaterNote, uint tempo, UInt32 currentMsec)
	{
		onTempoIn?.Invoke(msecPerQuaterNote, tempo, currentMsec);
	}
	public override void BeatIn(int numerator, int denominator, UInt32 currentMsec)
	{
		// Debug.Log($"BeatIn: {numerator}/{denominator}");
		onBeatIn?.Invoke(numerator, denominator, currentMsec);
	}
	public override void MeasureIn(int measure, int measureInterval, UInt32 currentMsec)
	{
		// Debug.Log($"MeasureIn: Measure: {measure}, Interval: {measureInterval}");
		onMeasureIn?.Invoke(measure, measureInterval, currentMsec);
	}
}
