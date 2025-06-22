/// LyricGenMultiLine.cs
/// 任意のエリアに複数Lineの歌詞を表示する
/// Copyright (c) 2025 gotojo

using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;

public class LyricGenMultiLine : MonoBehaviour {
	public Rect area = new Rect(-6, -4, 20, 6);
	public TMP_FontAsset font;
	public float scale = 0.7f;
	public float textHight = 1.0f;
	public float textWidth = 1f;

	public bool vertical = false;
	public int maxLine = 5;
	public SentenceList sentenceList;
	public bool active = true;
	class LyricGenMultiLineControl : LyricGenLineBase {
		public int maxLine = 5;
		public float scale = 1f;
		public bool vertical = false;
		private List<GameObject> lyrics = new List<GameObject>();
		private TMP_FontAsset font;
		private LyricGenMultiLine lyricGen;
		private Rect area;
		private float textHight = 2f;
		private float textWidth = 2f;
		private int lyricCount = 0;
		public LyricGenMultiLineControl(Rect area, float textHight, float textWidth, TMP_FontAsset font, LyricGenMultiLine lyricGen) : base(lyricGen.sentenceList, MidiWatcher.Instance) {
			this.area = area;
			this.textHight = textHight;
			this.textWidth = textWidth;
			this.font = font;
			this.lyricGen = lyricGen;
		}
		private void CreateText(string text) {
			if (!active) return;
			Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
			float rotate = 0;
			float x;
			float y;
			float w;
			float h;
			TextAlignmentOptions alignment;
			if (vertical) {
				w = textWidth;
				h = area.height;
				x = area.xMin - textWidth * lyricCount;
				y = area.yMax;
				alignment = TextAlignmentOptions.Top;
				text = ToVertical(text);
			} else {
				w = area.width;
				h = textHight;
				x = area.x;
				y = area.yMax - h * lyricCount;
				alignment = TextAlignmentOptions.TopLeft;
			}
			Vector2 size = new Vector2(w, h);
			Vector3 position = new Vector3(x, y, 0);
			GameObject lyric = CreateText(text, font, color, alignment, size, position, scale, rotate);
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
		control = new LyricGenMultiLineControl(area, textHight, textWidth, font, this);
		control.maxLine = maxLine;
		control.scale = scale;
		control.vertical = vertical;
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
