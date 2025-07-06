// LyricGenBase.cs
// 小節毎の歌詞を取得、SMFPlayerからのイベントで適切なタイミングを判断、歌詞の切り替えを行う
using UnityEngine;
using Unity.VisualScripting;
using TMPro;
using System;

public class LyricGenBase {
	public bool active = false;
	public int curMeas = 0;
	public int lastSentenceMeas = -1;
	public int measInterval = 2000;
	private int map;
	private SentenceList sentenceList;
	private string sentence = "";
	private MidiWatcherBase midiWatcher;
	protected bool autoSizeTextContainer = false;
	protected float fontSize = 12;
	public LyricGenBase(SentenceList sentenceList, int map, MidiWatcherBase midiWatcher) {
		this.midiWatcher = midiWatcher;
		midiWatcher.onMidiIn += MIDIIn;
		midiWatcher.onLyricIn += LyricIn;
		midiWatcher.onBeatIn += BeatIn;
		midiWatcher.onMeasureIn += MeasureIn;
		midiWatcher.onEventIn += EventIn;
		this.map = map;
		this.sentenceList = sentenceList;
	}

	public void Release() {
		midiWatcher.onMidiIn -= MIDIIn;
		midiWatcher.onLyricIn -= LyricIn;
		midiWatcher.onBeatIn -= BeatIn;
		midiWatcher.onMeasureIn -= MeasureIn;
		midiWatcher.onEventIn -= EventIn;
	}
	public GameObject CreateText(string word, TMP_FontAsset font, Color color, TextAlignmentOptions align, Vector2 sizeDelta, Vector3 position, float scale, float rotate) {
		if (!active) return null;
		GameObject simpleLyric = new GameObject("SimpleLyric");
		simpleLyric.AddComponent<TextMeshPro>();
		TextMeshPro text = simpleLyric.GetComponent<TextMeshPro>();
		text.font = font;
		text.text = word;
		text.color = color;
		text.fontSize = fontSize;
		text.fontSizeMax = fontSize;
		text.fontSizeMin = fontSize;
		text.autoSizeTextContainer = autoSizeTextContainer;
		text.alignment = align;
		text.lineSpacing = -30;
		Transform transform = text.GetComponent<Transform>();
		RectTransform rectTransform = simpleLyric.GetComponent<RectTransform>();
		rectTransform.sizeDelta = sizeDelta;
		// rectTransform.pivot = new Vector2(0.5f, 1);
		transform.position = position;
		transform.Rotate(0.0f, 0.0f, rotate);
		simpleLyric.transform.localScale = new Vector3(scale, scale, scale);
		return simpleLyric;
	}
	public void MIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) {
		OnMIDIIn(track, midiEvent, position, currentMsec);
	}
	private void GetSentence(int track, int measure) {
		if (measure == curMeas) {
			measure += 1;
		} else {
			measure = curMeas;
		}
		LyricData lyricData = sentenceList.GetSentence(track, measure, map);
		sentence = lyricData.sentence;
		lastSentenceMeas = measure;
	}
	public void LyricIn(int track, string lyric, float position, uint currentMsec) {
		if (sentence.Length == 0) {
			GetSentence(track, lastSentenceMeas);
			if (sentence.Length == 0) {
				GetSentence(track, lastSentenceMeas);
			}
			OnTextChanged(sentence);
		}
		if (sentence.Length >= lyric.Length) {
			sentence = sentence.Substring(lyric.Length);
		}
		OnLyricIn(track, lyric, position, currentMsec);
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec) {
		OnBeatIn(numerator, denominator, currentMsec);
	}
	public void MeasureIn(int measure, int measureInterval, uint currentMsec) {
		curMeas = measure;
		OnMeasureIn(measure, measureInterval, currentMsec);
	}
	public void EventIn(MIDIHandler.Event playerEvent) {
		OnEventIn(playerEvent);
	}

	protected virtual void OnMIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) { }
	protected virtual void OnLyricIn(int track, string lyric, float position, uint currentMsec) {}
	protected virtual void OnBeatIn(int numerator, int denominator, uint currentMsec) {}
	protected virtual void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {}
	protected virtual void OnEventIn(MIDIHandler.Event playerEvent) {}
	protected virtual void OnTextChanged(string sentence) {}
}