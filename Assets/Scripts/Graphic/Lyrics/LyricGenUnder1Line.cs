/// LyricGenUnder1Line.cs
/// 表示エリア一番下に1Lineの歌詞を表示する
/// Copyright (c) 2025 gotojo

using UnityEngine;
using TMPro;

class LyricGenControl {
	public string curWord = "";
	public int curMeas = 0;
	public int measInterval = 2000;
	public int lyricNum = 0;
	public bool active = true;
	private int map;
	private SentenceList sentenceList;
	private string sentence = "";

	private MidiWatcherBase midiWatcher;
	private bool reserveDisplay = false;
	private bool updateSentence = false;
	public LyricGenControl(SentenceList sentenceList, MidiWatcherBase midiWatcher) {
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

	public GameObject CreateText(string word, TMP_FontAsset font, Color color, Vector2 sizeDelta, Vector3 position, float scale, float rotate) {
		GameObject simpleLyric = new GameObject("SimpleLyric");
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
		RectTransform rectTransform = simpleLyric.GetComponent<RectTransform>();
		rectTransform.sizeDelta = sizeDelta;
		transform.position = position;
		transform.Rotate(0.0f, 0.0f, rotate);
		transform.localScale = new Vector3(scale, scale, scale);
		return simpleLyric;
	}

	public void MIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) {
		OnMIDIIn(track, midiEvent, position, currentMsec);
	}

	public void LyricIn(int track, string lyric, float position, uint currentMsec) {
		curWord = lyric;
		if (!sentenceList.IsActive(track, map)) return;
		if (updateSentence) {
			GetSentence(track, curMeas);
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
		sentence = lyricData.sentence;
		if (!string.IsNullOrEmpty(sentence)) {
			reserveDisplay = true;
			updateSentence = false;
		}
	}
	public void MeasureIn(int measure, int measureInterval, uint currentMsec) {
		curMeas = measure;
		measInterval = measureInterval;
		lyricNum = 0;
		curMeas = measure;
		if (active && sentenceList.IsExist(measure, map)) {
			updateSentence = true;
		} else {
			updateSentence = false;
			OnTextChanged("");
		}
		OnMeasureIn(measure, measureInterval, currentMsec);
	}

	public void EventIn(MIDIHandler.Event playerEvent) {
		OnEventIn(playerEvent);
	}

	protected virtual void OnMIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) {}
	protected virtual void OnLyricIn(int track, string lyric, float position, uint currentMsec) {}
	protected virtual void OnBeatIn(int numerator, int denominator, uint currentMsec) {}
	protected virtual void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {}
	protected virtual void OnEventIn(MIDIHandler.Event playerEvent) {}
	protected virtual void OnTextChanged(string sentence) {}
}

public class LyricGenUnder1Line : MonoBehaviour
{
	public Rect area = new Rect(-10, -3, 20, 6);
	public bool active = true;
	class LyricGenUnder1LineControl : LyricGenControl {
		private TextMeshPro text;
		public LyricGenUnder1LineControl(Vector3 position, LyricGenUnder1Line lyricGen, MidiWatcherBase midiWatcher) : base(lyricGen.sentenceList, midiWatcher) {
			TMP_FontAsset font = FontResource.Instance.GetFont();
			Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
			float scale = 1f;
			float rotate = 0;
			Vector2 size = new Vector2(20, 2);
			GameObject simpleLyric = CreateText("", font, color, size, position, scale, rotate);
			this.text = simpleLyric.GetComponent<TextMeshPro>();
			simpleLyric.transform.parent = lyricGen.transform;
		}
		protected override void OnEventIn(MIDIHandler.Event playerEvent) {}
		protected override void OnTextChanged(string sentence) {
			text.font = FontResource.Instance.GetFont();
			text.text = sentence;
		}
	};
	LyricGenUnder1LineControl control;
	LyricGenUnder1LineControl controlSub;
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
