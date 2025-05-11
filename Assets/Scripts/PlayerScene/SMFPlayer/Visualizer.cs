using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using MidiJack;
using System;
using System.Collections.Generic;

public class Visualizer : MonoBehaviour
{
	public Rect area = new Rect(-2, 5, 4, 10);
	public SMFPlayer smfPlayer;
	public SMFPlayer kanjiPlayer;
	private LyricMode lyricMode = LyricMode.Original;

	public enum LyricMode
	{
		Original,
		Kanji
	}

	void Awake()
	{
		lyricMode = (LyricMode)PlayerPrefs.GetInt("LyricMode");
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMidiIn += MIDIIn;
		midiWatcher.onLyricIn += LyricIn;
		midiWatcher.onTempoIn += TempoIn;
		midiWatcher.onBeatIn += BeatIn;
		midiWatcher.onMeasureIn += MeasureIn;
	}
	void OnDestroy()
	{
		MidiMaster.noteOnDelegate -= NoteOn;
		MidiMaster.noteOffDelegate -= NoteOff;
		MidiMaster.knobDelegate -= knobChanged;
		PlayerPrefs.SetInt("LyricMode", (int)lyricMode);
		// Debug.Log("Destract");
	}
	void Start()
	{
	}
	// Update is called once per frame
	void Update()
	{
	}

	public void SetSMFPlayer(SMFPlayer player, SMFPlayer _kanjiPlayer)
	{
		smfPlayer = player;
		kanjiPlayer = _kanjiPlayer;
		MidiEventMapAccessor.Instance.Init(smfPlayer, kanjiPlayer);
		smfPlayer.midiHandler = MidiWatcher.Instance;
		kanjiPlayer.midiHandler = MidiWatcher.Instance;
		SetLyricMode(lyricMode);
	}

	public void SetLyricMode(LyricMode mode)
	{
		lyricMode = mode;
		if (lyricMode == LyricMode.Kanji) {
			smfPlayer.mute = true;
			kanjiPlayer.mute = false;
			MidiEventMapAccessor.Instance.SetCurrentMap(1);
		} else {
			smfPlayer.mute = false;
			kanjiPlayer.mute = true;
			MidiEventMapAccessor.Instance.SetCurrentMap(0);
		}
	}

	public void MIDIIn(int track, byte[] midiEvent, float position, UInt32 currentMsec)
	{
		byte status = (byte)(midiEvent[0] & 0xF0);
		int ch = (status & 0xF0) >> 4;
		switch (status) {
			case 0x90:
				if (midiEvent[2] == 0) {
					Debug.Log($"NoteOff, position: {position}");
				} else {
					Debug.Log($"NoteOn, position: {position}");
				}
				break;
			case 0x80:
				Debug.Log($"NoteOff, position: {position}");
				break;
			default:
				// Debug.Log($"MIDIData Status = {status}");
				break;
		}
	}

	public void LyricIn(int track, string lyric, float position, UInt32 currentMsec)
	{
		Debug.Log($"lyric: {lyric}, position: {position}");
	}
	public void TempoIn(float msecPerQuaterNote, uint tempo, UInt32 currentMsec)
	{
	}

	public void BeatIn(int numerator, int denominator, UInt32 currentMsec)
	{
		// LyricItem.OnBeat += () => { };
	}

	public void MeasureIn(int measure, int measureInterval, UInt32 currentMsec)
	{
	}

	private void NoteOn(MidiChannel channel, int note, float velocity)
	{
		// Debug.Log("NoteOn: " + channel + ", " + note + ", " + velocity);
	}

	private void NoteOff(MidiChannel channel, int note)
	{
		// Debug.Log("NoteOff: " + channel + ", " + note);
	}

	private void knobChanged(MidiChannel ch, int ccNum, float value)
	{
		// Debug.Log($"knobChanged: ch{ch}, cc{ccNum}: {value}");
	}
}
