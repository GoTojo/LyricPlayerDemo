// LyricGenBase.cs
// 小節毎の歌詞を取得、SMFPlayerからのイベントで適切なタイミングを判断、歌詞の切り替えを行う
using UnityEngine;
using Unity.VisualScripting;
using System;

public class LyricGenBase {
	public string curWord = "";
	public int curMeas = 0;
	public int measInterval = 2000;
	private int map;
	private SentenceList sentenceList;
	private string sentence = "";
	private string orgSentence = "";

	private MidiWatcherBase midiWatcher;
	private bool reserveDisplay = false;
	private bool updateSentence = true;
	public bool forceMeasureChange = false;
	public LyricGenBase(SentenceList sentenceList, MidiWatcherBase midiWatcher) {
		this.midiWatcher = midiWatcher;
		midiWatcher.onMidiIn += MIDIIn;
		midiWatcher.onLyricIn += LyricIn;
		midiWatcher.onBeatIn += BeatIn;
		midiWatcher.onMeasureIn += MeasureIn;
		midiWatcher.onEventIn += EventIn;
		this.map = midiWatcher.GetMap();
		this.sentenceList = sentenceList;
	}

	public void Release() {
		midiWatcher.onMidiIn -= MIDIIn;
		midiWatcher.onLyricIn -= LyricIn;
		midiWatcher.onBeatIn -= BeatIn;
		midiWatcher.onMeasureIn -= MeasureIn;
		midiWatcher.onEventIn -= EventIn;
	}

	public void MIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) {
		OnMIDIIn(track, midiEvent, position, currentMsec);
	}

	public void LyricIn(int track, string lyric, float position, uint currentMsec) {
		curWord = lyric;
		if (!sentenceList.IsActive(track, map)) return;
		if (updateSentence) {
			if (sentence.Length == 0 || forceMeasureChange) GetSentence(track, curMeas);
		}
		if (sentence.StartsWith(lyric)) {
			if (reserveDisplay) {
				OnTextChanged(sentence);
				reserveDisplay = false;
			}
			sentence = sentence.Substring(lyric.Length);
			if (sentence.Length == 0) {
				GetSentence(track, curMeas + 1);
			}
		}
		OnLyricIn(track, lyric, position, currentMsec);
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec) {
		OnBeatIn(numerator, denominator, currentMsec);
	}
	private void GetSentence(int track, int measure) {
		LyricData lyricData = sentenceList.GetSentence(track, measure, map);
		orgSentence = lyricData.sentence;
		sentence = lyricData.sentence;
		if (!string.IsNullOrEmpty(sentence)) {
			reserveDisplay = true;
			updateSentence = false;
		}
	}
	public void MeasureIn(int measure, int measureInterval, uint currentMsec) {
		curMeas = measure;
		measInterval = measureInterval;
		curMeas = measure;
		if (sentenceList.IsExist(measure, map)) {
			updateSentence = true;
		} else {
			updateSentence = false;
		}
		OnMeasureIn(measure, measureInterval, currentMsec);
	}

	public void EventIn(MIDIHandler.Event playerEvent) {
		OnEventIn(playerEvent);
	}

	public String GetSentence() {
		return orgSentence;
	}

	protected virtual void OnMIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) { }
	protected virtual void OnLyricIn(int track, string lyric, float position, uint currentMsec) {}
	protected virtual void OnBeatIn(int numerator, int denominator, uint currentMsec) {}
	protected virtual void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {}
	protected virtual void OnEventIn(MIDIHandler.Event playerEvent) {}
	protected virtual void OnTextChanged(string sentence) {}
}