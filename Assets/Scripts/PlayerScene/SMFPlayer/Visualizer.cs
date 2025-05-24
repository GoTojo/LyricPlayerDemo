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
	public Parameter parameter;
	public Rect area = new Rect(-2, 5, 4, 10);
	public SMFPlayer smfPlayer;
	public SMFPlayer kanjiPlayer;
	private LyricMode lyricMode = LyricMode.Original;
	MidiEventMapAccessor eventMap;

	public GameObject unityChanBlack;
	public GameObject unityChanColor;
	public GameObject zeknova;
	public GameObject snow;
	public GameObject confetti;

	public Parameter.ParticleType	particleType;
	public Parameter.UnityChanType	unityChanType;

	public enum LyricMode
	{
		Original,
		Kanji
	}

	void Awake()
	{
		lyricMode = (LyricMode)PlayerPrefs.GetInt("LyricMode");
		particleType = (Parameter.ParticleType)PlayerPrefs.GetInt("Parameter.ParticleType");
		unityChanType = (Parameter.UnityChanType)PlayerPrefs.GetInt("Parameter.UnityChanType");

		MidiMaster.noteOnDelegate += NoteOn;
		MidiMaster.noteOffDelegate += NoteOff;
		MidiMaster.knobDelegate += knobChanged;
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
		// Debug.Log("Destract");
	}
	public void BackupParams()
	{
		PlayerPrefs.SetInt("LyricMode", (int)lyricMode);
		PlayerPrefs.SetInt("ParticleType", (int)particleType);
		PlayerPrefs.SetInt("UnityChanType", (int)unityChanType);
		// 次回起動時ユニティちゃんがスクリプトで色が変えるためactiveにしておく
		unityChanBlack.SetActive(true);
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
		eventMap = GetComponent<MidiEventMapAccessor>();
		eventMap.Init(smfPlayer, kanjiPlayer);
		SetLyricMode(lyricMode);
		ChangeParticle(particleType);
		ChangeUnityChan(unityChanType);
	}

	public void SetLyricMode(LyricMode mode)
	{
		lyricMode = mode;
		if (lyricMode == LyricMode.Kanji) {
			smfPlayer.mute = true;
			kanjiPlayer.mute = false;
			eventMap.SetCurrentMap(1);
			// Debug.Log($"LyricMode: kanji");
		} else {
			smfPlayer.mute = false;
			kanjiPlayer.mute = true;
			eventMap.SetCurrentMap(0);
			// Debug.Log($"LyricMode: original");
		}
	}

	public void MIDIIn(int track, byte[] midiEvent, float position, uint currentMsec)
	{
		byte status = (byte)(midiEvent[0] & 0xF0);
		int ch = (status & 0xF0) >> 4;
		switch (status) {
			case 0x90:
				if (midiEvent[2] == 0) {
					// Debug.Log($"NoteOff, position: {position}");
				} else {
					// Debug.Log($"NoteOn, position: {position}");
				}
				break;
			case 0x80:
				// Debug.Log($"NoteOff, position: {position}");
				break;
			default:
				// Debug.Log($"MIDIData Status = {status}");
				break;
		}
	}

	public void LyricIn(int track, string lyric, float position, uint currentMsec)
	{
		// Debug.Log($"lyric: {lyric}, position: {position}");
	}
	public void TempoIn(float msecPerQuaterNote, uint tempo, uint currentMsec)
	{
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec)
	{
		// LyricItem.OnBeat += () => { };
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec)
	{
	}

	private void ChangeParticle(Parameter.ParticleType type) {
		snow.SetActive(false);
		confetti.SetActive(false);
		zeknova.SetActive(false);
		if (type != particleType) {
			switch (type) {
			case Parameter.ParticleType.Snow:
				snow.SetActive(true);
				break;
			case Parameter.ParticleType.Confetti:
				confetti.SetActive(true);
				break;
			case Parameter.ParticleType.Sakura:
			// not yet
				break;
			case Parameter.ParticleType.Zeknova:
				zeknova.SetActive(true);
				break;
			default:
				break;
			}
			particleType = type;
		} else {
			particleType = Parameter.ParticleType.Off;
		}
	}

	private void ChangeUnityChan(Parameter.UnityChanType type)
	{
		unityChanBlack.SetActive(false);
		unityChanColor.SetActive(false);
		if (type != unityChanType) {
			switch (type) {
			case Parameter.UnityChanType.Black:
				unityChanBlack.SetActive(true);
				break;
			case Parameter.UnityChanType.Color:
				unityChanColor.SetActive(true);
				break;
			default:
				break;
			}
			unityChanType = type;
		} else {
			unityChanType = Parameter.UnityChanType.Off;
		}
	}

	private void NoteOn(MidiChannel channel, int note, float velocity)
	{
		if (note == Parameter.NoteLyricTypeDown) {
		} else if (note == Parameter.NoteLyricTypeUp) {
		} else if (note == Parameter.NoteLyricFontDown) {
		} else if (note == Parameter.NoteLyricFontUp) {
		} else if (note == Parameter.NoteLyricModeOriginal) {
			SetLyricMode(LyricMode.Original);
		} else if (note == Parameter.NoteLyricModeKanji) {
			SetLyricMode(LyricMode.Kanji);
		} else if (note == Parameter.NoteParticleSnow) {
			ChangeParticle(Parameter.ParticleType.Snow);
		} else if (note == Parameter.NoteParticleConfetti) {
			ChangeParticle(Parameter.ParticleType.Confetti);
		} else if (note == Parameter.NoteParticleKiraKira) {
			ChangeParticle(Parameter.ParticleType.Zeknova);
		} else if (note == Parameter.NoteUnityChanBlack) {
			ChangeUnityChan(Parameter.UnityChanType.Black);
		} else if (note == Parameter.NoteUnityChanColor) {
			ChangeUnityChan(Parameter.UnityChanType.Color);
		}
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
