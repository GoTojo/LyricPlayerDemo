/// LyricGenMultiLine.cs
/// 任意のエリアに複数Lineの歌詞を表示する
/// Copyright (c) 2025 gotojo

using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using System;
using UnityEngine.AI;

public class LyricGenMultiLine : MonoBehaviour {
	public Rect area = new Rect(-6, -4, 20, 6);
	public TMP_FontAsset font;
	public float scale = 0.7f;
	public float textHeight = 1.0f;
	public float textWidth = 1f;

	public bool vertical = false;
	public int maxLine = 5;
	public SentenceList sentenceList;
	public bool active = true;
	class LyricGenMultiLineControl : LyricGenLineBase {
		public bool active = false;
		public int maxLine = 5;
		public float scale = 1f;
		public bool vertical = false;
		private List<GameObject> lyrics = new List<GameObject>();
		private TMP_FontAsset font;
		private LyricGenMultiLine lyricGen;
		private Rect area;
		private float textHeight = 2f;
		private float textWidth = 2f;
		private int lyricCount = 0;
		private int waitCount = 3;
		private int waitClear = 0;
		public LyricGenMultiLineControl(Rect area, float textHeight, float textWidth, TMP_FontAsset font, LyricGenMultiLine lyricGen) : base(lyricGen.sentenceList, SentenceList.kanjiMap, MidiWatcher.Instance) {
			this.area = area;
			this.textHeight = textHeight;
			this.textWidth = textWidth;
			this.font = font;
			this.lyricGen = lyricGen;
		}
		private void GetTextArea(string text, ref Vector3 position, ref Vector2 size) {
			float x;
			float y;
			float w;
			float h;
			if (vertical) {
				w = textWidth;
				h = area.height;
				x = area.xMin - textWidth * lyricCount;
				y = area.yMax;
			} else {
				w = area.width;
				h = textHeight;
				x = area.x;
				y = area.yMax - h * lyricCount;
			}
			size = new Vector2(w, h);
			position = new Vector3(x, y, 0);
		}
		private void CreateText(string text) {
			if (!active) return;
			Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
			float rotate = 0;
			TextAlignmentOptions alignment;
			Vector2 size = new Vector2();
			Vector3 position = new Vector3();
			GetTextArea(text, ref position, ref size);
			if (vertical) {
				alignment = TextAlignmentOptions.Top;
				text = ToVertical(text);
			} else {
				alignment = TextAlignmentOptions.TopLeft;
			}
			GameObject lyric = CreateText(text, font, color, alignment, size, position, scale, rotate);
			lyric.transform.SetParent(lyricGen.transform);
			lyrics.Add(lyric);
			lyricCount++;
			waitClear = waitCount;
		}
		public void Clear() {
			foreach (var lyric in lyrics) {
				Destroy(lyric);
			}
			lyrics.Clear();
			lyricCount = 0;
		}
		protected override void OnLyricIn(int track, string lyric, float position, uint currentMsec) {
		}
		protected override void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {
			if (waitClear > 0) {
				waitClear--;
				if (waitClear <= 0) {
					Clear();
				}
			}
		}
		protected override void OnEventIn(MIDIHandler.Event playerEvent) {
		}
		protected override void OnTextChanged(string sentence) {
			if (lyricCount >= maxLine) {
				Clear();
			}
			CreateText(sentence);
		}
	};
	LyricGenMultiLineControl control;

	void Start() {
		control = new LyricGenMultiLineControl(area, textHeight, textWidth, font, this);
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
		control.vertical = vertical;
	}
}
