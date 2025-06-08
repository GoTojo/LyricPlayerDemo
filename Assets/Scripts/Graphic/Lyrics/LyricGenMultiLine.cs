/// LyricGenMultiLine.cs
/// 任意のエリアに複数Lineの歌詞を表示する
/// Copyright (c) 2025 gotojo

using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LyricGenMultiLine : MonoBehaviour {
	public Rect area = new Rect(-6, -4, 20, 6);
	public TMP_FontAsset font;
	public float scale = 0.7f;
	public float textHight = -1.0f;
	public int maxLine = 5;
	public SentenceList sentenceList;
	public bool active = true;
	class LyricGenMultiLineControl : LyricGenControl {
		public int maxLine = 5;
		public float scale = 1f;
		private List<GameObject> lyrics = new List<GameObject>();
		private TMP_FontAsset font;
		private LyricGenMultiLine lyricGen;
		private Rect area;
		private float textHeight = 2f;
		private int lyricCount = 0;
		public LyricGenMultiLineControl(Rect area, float textHeight, TMP_FontAsset font, LyricGenMultiLine lyricGen) : base(lyricGen.sentenceList, MidiWatcher.Instance) {
			this.area = area;
			this.textHeight = textHeight;
			this.font = font;
			this.lyricGen = lyricGen;
		}
		private void CreateText(string text) {
			if (!active) return;
			Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
			float rotate = 0;
			Vector2 size = new Vector2(area.width, textHeight);
			float y = area.yMax - textHeight * lyricCount;
			Vector3 position = new Vector3(area.x, y, 0);
			GameObject lyric = CreateText(text, font, color, TextAlignmentOptions.Left, size, position, scale, rotate);
			lyric.transform.parent = lyricGen.transform;
			lyrics.Add(lyric);
			lyricCount++;
		}
		public void Clear() {
			foreach (var lyric in lyrics) {
				Destroy(lyric);
			}
			lyrics.Clear();
			lyricCount = 0;
		}
		protected override void OnEventIn(MIDIHandler.Event playerEvent) { }
		protected override void OnTextChanged(string text) {
			if (lyricCount >= maxLine) Clear();
			if (!string.IsNullOrEmpty(text)) {
				CreateText(text);
			}
		}
	};
	LyricGenMultiLineControl control;

	void Start() {
		control = new LyricGenMultiLineControl(area, textHight, font, this);
		control.maxLine = maxLine;
		control.scale = scale;
		control.active = active;
	}

	public void Clear() {
		control.Clear();
	}

	public void SetActive(bool f) {
		control.Clear();
		active = f;
		control.active = f;
	}
}
