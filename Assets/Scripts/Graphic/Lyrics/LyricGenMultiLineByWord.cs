/// LyricGenMultiLine.cs
/// 任意のエリアに複数Lineの歌詞を表示する
/// Copyright (c) 2025 gotojo

using UnityEngine;
using TMPro;

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
		private int numOfWord = 0;
		private float measureInterval = 0;
		public LyricGenMultiLineControl(Rect area, float textHeight, float textWidth, TMP_FontAsset font, Transform transform, SentenceList sentenceList) : base(area, textHeight, textWidth, font, transform, sentenceList) {
		}
		protected override void GetPosition(ref float x, ref float y) {
			if (vertical) {
				y -= textHeight * numOfWord;
			} else {
				x += textWidth * numOfWord;
			}
		}
		protected override void OnCleared() {
			numOfWord = 0;
		}
		protected override void OnLyricIn(int track, string lyric, float position, uint currentMsec) {
			GameObject obj = CreateText(lyric);
			if (obj) Destroy(obj, measureInterval * 2);
			numOfWord += lyric.Length;
		}
		protected override void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {
			this.measureInterval = measureInterval / 1000f;
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
		control = new LyricGenMultiLineControl(area, textHeight, textWidth, font, this.transform, sentenceList);
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
