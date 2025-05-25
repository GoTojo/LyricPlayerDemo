/// MIDIHandler.cs
/// Interface Class for Receive MidiEvents
/// Copyright (c) 2025 gotojo, All Rights Reserved.

using System;

public class MIDIHandler {
	public enum Event {
		Start,
		Stop,
		Reset,
		End
	};
	public virtual void MIDIIn(int track, byte[] midiEvent, float position, uint currentMsec)
	{
	}
	public virtual void LyricIn(int track, string lyric, float position, uint currentMsec)
	{
	}
	public virtual void TempoIn(float msecPerQuaterNote, uint tempo, uint currentMsec)
	{
	}
	public virtual void BeatIn(int numerator, int denominator, uint currentMsec)
	{
	}
	public virtual void MeasureIn(int measure, int measureInterval, uint currentMsec)
	{
	}
	public virtual void EventIn(Event playerEvent)
	{
	}
}