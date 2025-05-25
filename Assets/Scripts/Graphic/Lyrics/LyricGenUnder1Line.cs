/// LyricGenUnder1Line.cs
/// 表示エリア一番下に1Lineの歌詞を表示する
/// Copyright (c) 2025 gotojo

using UnityEngine;

class LyricGenControl 
{
	public string curWord = "";
	public int curmeas = 0;
	public int measInterval = 2000;
	public int lyricNum = 0;

	public LyricGenControl()
	{
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMidiIn += MIDIIn;
		midiWatcher.onLyricIn += LyricIn;
		midiWatcher.onBeatIn += BeatIn;
		midiWatcher.onMeasureIn += MeasureIn;
		midiWatcher.onEventIn += EventIn;
	}

	public void Release()
	{
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMidiIn -= MIDIIn;
		midiWatcher.onLyricIn -= LyricIn;
		midiWatcher.onBeatIn -= BeatIn;
		midiWatcher.onMeasureIn -= MeasureIn;
		midiWatcher.onEventIn -= EventIn;
	}

	public void MIDIIn(int track, byte[] midiEvent, float position, uint currentMsec)
	{
		OnMIDIIn(track, midiEvent, position, currentMsec);
	}

	public void LyricIn(int track, string lyric, float position, uint currentMsec)
	{
		curWord = lyric;
		OnLyricIn(track, lyric, position, currentMsec);
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec)
	{
		OnBeatIn(numerator, denominator, currentMsec);
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec)
	{
		curmeas = measure;
		measInterval = measureInterval;
		lyricNum = 0;
		OnMeasureIn(measure, measureInterval, currentMsec);
	}

	public void EventIn(MIDIHandler.Event playerEvent)
	{
		OnEventIn(playerEvent);
	}

	protected virtual void OnMIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) {}
	protected virtual void OnLyricIn(int track, string lyric, float position, uint currentMsec) {}
	protected virtual void OnBeatIn(int numerator, int denominator, uint currentMsec) {}
	protected virtual void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {}
	protected virtual void OnEventIn(MIDIHandler.Event playerEvent) {}
}

public class LyricGenUnder1Line : MonoBehaviour
{
	string lyric = "";

	void Start()
	{
	}

	void Update()
	{
	}

	public void SetLyric(string lyric)
	{
		this.lyric = lyric;
	}
}

class LyricGenUnder1LineControl : LyricGenControl
{
	private GameObject gameObject;
	public LyricGenUnder1LineControl()
	{
	}
	void Init()
	{
		gameObject = GameObject.Find("LyricGenUnder1Line");
	}

	protected override void OnMIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) {}
	protected override void OnLyricIn(int track, string lyric, float position, uint currentMsec) {}
	protected override void OnBeatIn(int numerator, int denominator, uint currentMsec) {}
	protected override void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {
	}
	protected override void OnEventIn(MIDIHandler.Event playerEvent) {}
}