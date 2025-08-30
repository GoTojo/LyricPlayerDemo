/// LyricGenMultiLine.cs
/// 任意のエリアに複数Lineの歌詞を表示する
/// Copyright (c) 2025 gotojo

using UnityEngine;
using TMPro;

public class LyricGenMultiLineByWord : LyricBase {
	public Rect area = new Rect(-6, -4, 20, 6);
	public float scale = 0.7f;
	public float textHeight = 1.0f;
	public float textWidth = 1f;

	public bool vertical = false;
	public int maxLine = 5;
	public bool autoclear = true;
	class LyricGenMultiLineControl : LyricGenLineBase {
		public bool autoclear = true;
		private float measureInterval = 0;
		public LyricGenMultiLineControl(Rect area, float textHeight, float textWidth, TMP_FontAsset font, Transform transform) : base(area, textHeight, textWidth, font, transform) {
		}
		protected override void OnCleared() {
			numOfWord = 0;
		}
		protected override void OnLyricIn(int track, string lyric, float position, uint currentMsec) {
			GameObject obj = CreateText(lyric);
			if (obj && autoclear) Destroy(obj, measureInterval * 2);
			numOfWord += lyric.Length;
		}
		protected override void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {
			this.measureInterval = measureInterval / 1000f;
		}
		protected override void OnTextChanged(string sentence) {
			if (line >= maxLine) {
				Clear();
			} else if (numOfWord != 0) {
				numOfWord = 0;
				line++;
			}
		}
		public void SetText(string[] text) {
			Clear();
			for (var i = 0; i < maxLine; i++) {
				if (text.Length < i) break;
				string sentence = text[i];
				for (numOfWord = 0; numOfWord < sentence.Length; numOfWord++) {
					CreateText(sentence.Substring(numOfWord, 1));
				}
				line++;
			}
		}
	};
	LyricGenMultiLineControl control;

	void Start() {
		control = new LyricGenMultiLineControl(area, textHeight, textWidth, font, this.transform);
	}

	public override void OnParamChanged() {
		control.area = area;
		control.maxLine = maxLine;
		control.scale = scale;
		control.vertical = vertical;
		control.autoclear = autoclear;
		control.active = active;
		control.font = font;
	}
	public override void Clear() {
		control.Clear();
	}
	public override void ShowSampleText(string[] text) {
		control.SetText(text);
	}

	public override void Hide() {
		Clear();
	}
	public override void SetPosX(float x) {
		area.x = x;
		control.area = area;
	}
	public override void SetPosY(float y) {
		area.y = y;
		control.area = area;
	}
	public override float GetPosX() {
		return area.x;
	}
	public override float GetPosY() {
		return area.y;
	}
}
