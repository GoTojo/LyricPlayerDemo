/// LyricGenUnder1Line.cs
/// 表示エリア一番下に1Lineの歌詞を表示する
/// Copyright (c) 2025 gotojo

using UnityEngine;
using TMPro;

class LyricGenControl 
{
	public string curWord = "";
	public int curmeas = 0;
	public int measInterval = 2000;
	public int lyricNum = 0;
	private MidiWatcherBase midiWatcher;

	public LyricGenControl(MidiWatcherBase midiWatcher)
	{
		this.midiWatcher = midiWatcher;
		midiWatcher.onMidiIn += MIDIIn;
		midiWatcher.onLyricIn += LyricIn;
		midiWatcher.onBeatIn += BeatIn;
		midiWatcher.onMeasureIn += MeasureIn;
		midiWatcher.onEventIn += EventIn;
	}

	public void Release()
	{
		midiWatcher.onMidiIn -= MIDIIn;
		midiWatcher.onLyricIn -= LyricIn;
		midiWatcher.onBeatIn -= BeatIn;
		midiWatcher.onMeasureIn -= MeasureIn;
		midiWatcher.onEventIn -= EventIn;
	}

	public GameObject CreateText(string word, TMP_FontAsset font, Color color, Vector2 sizeDelta, Vector3 position, float scale, float rotate)
	{
		GameObject	simpleLyric = new GameObject("SimpleLyric");
		simpleLyric.AddComponent<TextMeshPro>();
		TextMeshPro text = simpleLyric.GetComponent<TextMeshPro>();
		text.font = font;
		text.text = word;
		text.color = color;
		text.fontSize = 12;
		text.fontSizeMax = 12;
		text.fontSizeMin = 12;
		text.autoSizeTextContainer = false;
		text.alignment = TextAlignmentOptions.Center;
		Transform transform = text.GetComponent<Transform>();
		RectTransform rectTransform =	simpleLyric.GetComponent<RectTransform>();
		rectTransform.sizeDelta = sizeDelta;
		transform.position = position;
		transform.Rotate(0.0f, 0.0f, rotate);
		transform.localScale = new Vector3(scale, scale, scale);
		return simpleLyric;
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
	public Rect area = new Rect(-10, -3, 20, 6);
	public bool active = true;
	class LyricGenUnder1LineControl : LyricGenControl
	{
		private LyricGenUnder1Line lyricGen;
		private SentenceList sentenceList;
		private bool measureChanged = false;
		private TextMeshPro text;
		private int curMeas = 0;
		private int map;
		public bool active = true;
		public LyricGenUnder1LineControl(Vector3 position, LyricGenUnder1Line lyricGen, MidiWatcherBase midiWatcher) : base(midiWatcher)
		{
			this.map = midiWatcher.GetMap();
			this.lyricGen = lyricGen;
			this.sentenceList = lyricGen.sentenceList;
			TMP_FontAsset font = FontResource.Instance.GetFont();
			Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
			float scale = 1f;
			float rotate = 0;
			Vector2 size = new Vector2(20, 2);
			GameObject simpleLyric = CreateText("", font, color, size, position, scale, rotate);
			this.text = simpleLyric.GetComponent<TextMeshPro>();
			simpleLyric.transform.parent = lyricGen.transform;
		}

		protected override void OnMIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) { }
		protected override void OnLyricIn(int track, string lyric, float position, uint currentMsec)
		{
			if (!sentenceList.IsActive(track, map)) return;
			if (measureChanged) {
				LyricData lyricData = sentenceList.GetSentence(track, curMeas, map);
				string sentence = lyricData.sentence;
				if (sentence[0] == lyric[0]) {
					text.font = FontResource.Instance.GetFont();
					text.text = sentence;
					measureChanged = false;
				}
			}
		}
		protected override void OnBeatIn(int numerator, int denominator, uint currentMsec) { }
		protected override void OnMeasureIn(int measure, int measureInterval, uint currentMsec)
		{
			curMeas = measure;
			if (active && sentenceList.IsExist(measure, map)) {
				measureChanged = true;
			} else {
				measureChanged = false;
				text.text = "";
			}
		}
		protected override void OnEventIn(MIDIHandler.Event playerEvent) { }
	};
	LyricGenUnder1LineControl control;
	LyricGenUnder1LineControl controlSub;
	private string lyric = "";
	public SentenceList sentenceList;

	void Start()
	{
		GameObject mainObj = GameObject.Find("MainGameObject");
		sentenceList = mainObj.GetComponent<SentenceList>();
		control = new LyricGenUnder1LineControl(new Vector3(0, -6, -1.0f), this, MidiWatcher.Instance);
		controlSub = new LyricGenUnder1LineControl(new Vector3(0, -4, -1.0f), this, SubMidiWatcher.Instance);
	}

	void Update()
	{
		controlSub.active = (PlayerPrefs.GetInt("LyricMode") == 0);
	}
}
