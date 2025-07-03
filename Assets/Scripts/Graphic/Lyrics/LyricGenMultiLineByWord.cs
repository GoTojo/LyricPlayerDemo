/// LyricGenMultiLine.cs
/// 任意のエリアに複数Lineの歌詞を表示する
/// Copyright (c) 2025 gotojo

using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using System;
using UnityEngine.AI;

public class LyricGenMultiLineByWord : MonoBehaviour {
	public Rect area = new Rect(-6, -4, 20, 6);
	public TMP_FontAsset font;
	public float scale = 0.7f;
	public float textHeight = 1.0f;
	public float textWidth = 1f;

	public bool vertical = false;
	public int maxLine = 5;
	public SentenceList sentenceList;
	public bool active = true;
	class LyricGenMultiLineControl : LyricGenMultiLineBase {
		public bool active = false;
		public int maxLine = 5;
		public float scale = 1f;
		public bool vertical = false;
		private List<GameObject> lyrics = new List<GameObject>();
		private TMP_FontAsset font;
		private LyricGenMultiLineByWord lyricGen;
		private Rect area;
		private float textHeight = 2f;
		private float textWidth = 2f;
		private int line = 0;
		private int numOfWord = 0;
		private float measureInterval = 0;
		public LyricGenMultiLineControl(Rect area, float textHeight, float textWidth, TMP_FontAsset font, LyricGenMultiLineByWord lyricGen) : base(area, textHeight, textWidth, lyricGen.sentenceList) {
			this.area = area;
			this.textHeight = textHeight;
			this.textWidth = textWidth;
			this.font = font;
			this.lyricGen = lyricGen;
		}
		private void CreateText(string word) {
			if (!active) return;
			int length = word.Length;
			Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
			float rotate = 0;
			TextAlignmentOptions alignment;
			Vector2 size = new Vector2();
			Vector3 position = new Vector3();
			GetTextArea(line, vertical, ref position, ref size);
			if (vertical) {
				alignment = TextAlignmentOptions.Top;
				position.y -= textHeight * numOfWord;
				word = ToVertical(word);
			} else {
				alignment = TextAlignmentOptions.TopLeft;
				position.x += textWidth * numOfWord;
			}
			GameObject lyric = CreateText(word, font, color, alignment, size, position, scale, rotate);
			lyric.transform.SetParent(lyricGen.transform);
			lyrics.Add(lyric);
			Destroy(lyric, measureInterval * 2);
			numOfWord += length;
		}
		public void Clear() {
			foreach (var lyric in lyrics) {
				Destroy(lyric);
			}
			lyrics.Clear();
			line = 0;
			numOfWord = 0;
		}
		protected override void OnLyricIn(int track, string lyric, float position, uint currentMsec) {
			CreateText(lyric);
		}
		protected override void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {
			this.measureInterval = measureInterval / 1000f;
		}
		protected override void OnEventIn(MIDIHandler.Event playerEvent) {
		}
		protected override void OnTextChanged(string sentence) {
			if (line >= maxLine) {
				Clear();
			} else {
				numOfWord = 0;
				line++;
			}
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
