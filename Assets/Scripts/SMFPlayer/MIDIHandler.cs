/// MIDIHandler.cs
/// Interface Class for Receive MidiEvents
/// Copyright (c) 2025 gotojo, All Rights Reserved.

using System;

public class MIDIHandler {
	public virtual void MIDIIn(int track, byte[] midiEvent, float position, UInt32 currentMsec)
	{
	}
	public virtual void LyricIn(int track, string lyric, float position, UInt32 currentMsec)
	{
	}
	public virtual void TempoIn(float msecPerQuaterNote, UInt32 tempo, UInt32 currentMsec)
	{
	}
	public virtual void BeatIn(int numerator, int denominator, UInt32 currentMsec)
	{
	}
	public virtual void MeasureIn(int measure, int measureInterval, UInt32 currentMsec)
	{
	}
}