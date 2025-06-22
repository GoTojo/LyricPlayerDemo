/// LyricGenUnder1Line.cs
/// 表示エリア一番下に1Lineの歌詞を表示する
/// Copyright (c) 2025 gotojo

using UnityEngine;
using TMPro;

public class LyricGenUnder1Line : MonoBehaviour
{
	public Rect area = new Rect(-10, -3, 20, 6);
	public bool active = true;
	class LyricGenUnder1LineControl : LyricGenLineBase {
		public bool active = false;
		private TextMeshPro text;
		private int waitCount = 3;
		private int waitClear = 0;
		private string sentence = "";
		private int sentenceLength = 0;
		public LyricGenUnder1LineControl(Vector3 position, LyricGenUnder1Line lyricGen, MidiWatcherBase midiWatcher) : base(lyricGen.sentenceList, midiWatcher) {
			TMP_FontAsset font = FontResource.Instance.GetFont();
			Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
			float scale = 1f;
			float rotate = 0;
			Vector2 size = new Vector2(20, 2);
			GameObject simpleLyric = CreateText("", font, color, TextAlignmentOptions.Center, size, position, scale, rotate);
			this.text = simpleLyric.GetComponent<TextMeshPro>();
			simpleLyric.transform.parent = lyricGen.transform;
		}
		protected override void OnLyricIn(int track, string lyric, float position, uint currentMsec) {
			if (sentence.Length == 0) {
				sentence = GetSentence();
				sentenceLength = sentence.Length;
				if (sentenceLength > 0) {
					text.font = FontResource.Instance.GetFont();
					text.text = sentence;
					waitClear = waitCount;
				}
			}
			// Debug.Log($"{lyricNum}/{sentenceLength}: {curWord}");
			if (sentence.Length >= lyric.Length) {
				sentence = sentence.Substring(lyric.Length);
			}
		}
		protected override void OnEventIn(MIDIHandler.Event playerEvent) { }
		protected override void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {
			if (waitClear > 0) {
				waitClear--;
				if (waitClear <= 0) {
					sentence = "";
					text.text = "";
					sentenceLength = 0;
				}
			}
		}
	};
	LyricGenUnder1LineControl control;
	LyricGenUnder1LineControl controlSub;
	public SentenceList sentenceList;

	void Start()
	{
		GameObject mainObj = GameObject.Find("MainGameObject");
		sentenceList = mainObj.GetComponent<SentenceList>();
		control = new LyricGenUnder1LineControl(new Vector3(0, -6, -4.0f), this, MidiWatcher.Instance);
		controlSub = new LyricGenUnder1LineControl(new Vector3(0, -4, -14.0f), this, SubMidiWatcher.Instance);
	}

	void Update()
	{
		controlSub.active = (PlayerPrefs.GetInt("LyricMode") == 0);
	}
}
