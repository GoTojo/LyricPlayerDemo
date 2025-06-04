using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using MidiJack;
using System;
using System.Collections.Generic;

public class Visualizer : MonoBehaviour {
	public Parameter parameter;
	public Rect area = new Rect(-2, 5, 4, 10);
	public SMFPlayer smfPlayer;
	public SMFPlayer kanjiPlayer;
	private LyricMode lyricMode = LyricMode.Kanji;
	MidiEventMapAccessor eventMap;

	public GameObject unityChanBlack;
	public GameObject unityChanColor;
	public GameObject zeknova;
	public GameObject snow;
	public GameObject confetti;
	public GameObject ramen;
	public BackGroundController backGroundController;

	public Parameter.ParticleType particleType;
	public Parameter.UnityChanType unityChanType;

	private int particleMeasCount = 0;
	private SentenceList sentenceList;

	public enum LyricMode {
		Original,
		Kanji
	}

	void Awake() {
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
	void OnDestroy() {
		MidiMaster.noteOnDelegate -= NoteOn;
		MidiMaster.noteOffDelegate -= NoteOff;
		MidiMaster.knobDelegate -= knobChanged;
		// Debug.Log("Destract");
	}
	public void BackupParams() {
		PlayerPrefs.SetInt("LyricMode", (int)lyricMode);
		PlayerPrefs.SetInt("ParticleType", (int)particleType);
		PlayerPrefs.SetInt("UnityChanType", (int)unityChanType);
		// 次回起動時ユニティちゃんがスクリプトで色を変えるためactiveにしておく
		unityChanBlack.SetActive(true);
	}
	void Start() {
		GameObject mainObj = GameObject.Find("MainGameObject");
		sentenceList = mainObj.GetComponent<SentenceList>();
	}
	// Update is called once per frame
	void Update() {
	}

	public void SetSMFPlayer(SMFPlayer player, SMFPlayer _kanjiPlayer) {
		smfPlayer = player;
		kanjiPlayer = _kanjiPlayer;
		eventMap = GetComponent<MidiEventMapAccessor>();
		eventMap.Init(smfPlayer, kanjiPlayer);
		eventMap.SetCurrentMap(1);
		kanjiPlayer.mute = false;
		ChangeParticle(particleType);
		SetUnityChan(unityChanType);
	}

	public void ToggleLyricMode() {
		if (lyricMode == LyricMode.Kanji) {
			lyricMode = LyricMode.Original;
			// Debug.Log($"LyricMode: kanji");
		} else {
			lyricMode = LyricMode.Kanji;
			// Debug.Log($"LyricMode: original");
		}
		PlayerPrefs.SetInt("LyricMode", (int)lyricMode);
	}

	public void MIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) {
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

	public void LyricIn(int track, string lyric, float position, uint currentMsec) {
		// Debug.Log($"lyric: {lyric}, position: {position}");
	}
	public void TempoIn(float msecPerQuaterNote, uint tempo, uint currentMsec) {
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec) {
		// LyricItem.OnBeat += () => { };
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec) {
		if (particleMeasCount > 0) {
			particleMeasCount--;
			if (particleMeasCount == 0) {
				ChangeParticle(Parameter.ParticleType.Off);
			}
		}
		for (var track = 0; track < sentenceList.GetNumOfTrack(); track++) {
			if (sentenceList.IsActive(track, 1)) {
				LyricData lyricData = sentenceList.GetSentence(track, measure, 1);
				foreach (string effect in lyricData.effect) {
					ApplyEffect(effect);
				}
			}
		}
	}

	private void ChangeParticle(Parameter.ParticleType type) {
		snow.SetActive(false);
		confetti.SetActive(false);
		zeknova.SetActive(false);
		ramen.SetActive(false);
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
			case Parameter.ParticleType.Ramen:
				ramen.SetActive(true);
				break;
			default:
				break;
			}
			particleType = type;
			particleMeasCount = 4;
		} else {
			particleType = Parameter.ParticleType.Off;
		}
	}
	private void SetUnityChan(Parameter.UnityChanType type) {
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
	}
	private void ChangeUnityChan(Parameter.UnityChanType type) {
		unityChanBlack.SetActive(false);
		unityChanColor.SetActive(false);
		if (type != unityChanType) {
			SetUnityChan(type);
			unityChanType = type;
		} else {
			unityChanType = Parameter.UnityChanType.Off;
		}
	}

	private void NoteOn(MidiChannel channel, int note, float velocity) {
		if (note == Parameter.NoteLyricTypeDown) {
		} else if (note == Parameter.NoteLyricTypeUp) {
		} else if (note == Parameter.NoteLyricFontDown) {
			FontResource.Instance.DecFont();
		} else if (note == Parameter.NoteLyricFontUp) {
			FontResource.Instance.IncFont();
		} else if (note == Parameter.NoteLyricModeToggle) {
			ToggleLyricMode();
		} else if (note == Parameter.NoteParticleSnow) {
			ChangeParticle(Parameter.ParticleType.Snow);
		} else if (note == Parameter.NoteParticleConfetti) {
			ChangeParticle(Parameter.ParticleType.Confetti);
		} else if (note == Parameter.NoteParticleKiraKira) {
			ChangeParticle(Parameter.ParticleType.Zeknova);
		} else if (note == Parameter.NoteParticleRamen) {
			ChangeParticle(Parameter.ParticleType.Ramen);
		} else if (note == Parameter.NoteUnityChanBlack) {
			ChangeUnityChan(Parameter.UnityChanType.Black);
		} else if (note == Parameter.NoteUnityChanColor) {
			ChangeUnityChan(Parameter.UnityChanType.Color);
		}
	}

	private void ApplyEffect(string effect) {
		switch (effect) {
		case "WallRect":
			backGroundController.SetWallType(Parameter.WallType.Rectangle);
			break;
		case "WallCircle":
			backGroundController.SetWallType(Parameter.WallType.Circle);
			break;
		case "WallOff":
			backGroundController.SetWallType(Parameter.WallType.Off);
			break;
		case "Snow":
			ChangeParticle(Parameter.ParticleType.Snow);
			break;
		case "Confetti":
			ChangeParticle(Parameter.ParticleType.Confetti);
			break;
		case "Sakura":
			ChangeParticle(Parameter.ParticleType.Sakura);
			break;
		case "Zeknova":
			ChangeParticle(Parameter.ParticleType.Zeknova);
			break;
		case "Ramen":
			ChangeParticle(Parameter.ParticleType.Ramen);
			break;
		case "UCBlack":
			ChangeUnityChan(Parameter.UnityChanType.Black);
			break;
		case "UCColor":
			ChangeUnityChan(Parameter.UnityChanType.Color);
			break;
		case "UCOff":
			ChangeUnityChan(Parameter.UnityChanType.Off);
			break;
		}
	}

	private void NoteOff(MidiChannel channel, int note) {
		// Debug.Log("NoteOff: " + channel + ", " + note);
	}

	private void knobChanged(MidiChannel ch, int ccNum, float value) {
		// Debug.Log($"knobChanged: ch{ch}, cc{ccNum}: {value}");
	}
}
