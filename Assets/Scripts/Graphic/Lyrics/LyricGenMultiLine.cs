/// LyricGenMultiLine.cs
/// 任意のエリアに複数Lineの歌詞を表示する
/// Copyright (c) 2025 gotojo

using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using System;
using UnityEngine.AI;

public class LyricGenMultiLine : LyricBase {
	public Rect area = new Rect(-6, -4, 20, 6);
	public float scale = 0.7f;
	public float textHeight = 1.0f;
	public float textWidth = 1f;

	public bool vertical = false;
	public int maxLine = 5;
	class LyricGenMultiLineControl : LyricGenLineBase {
		private int waitCount = 3;
		private int waitClear = 0;
		public LyricGenMultiLineControl(Rect area, float textHeight, float textWidth, TMP_FontAsset font, Transform transform) : base(area, textHeight, textWidth, font, transform) {
		}
		protected override void OnCleared() {
		}
		protected override void OnMeasureIn(int measure, int measureInterval, uint currentMsec) {
			if (waitClear > 0) {
				waitClear--;
				if (waitClear <= 0) {
					Clear();
				}
			}
		}
		protected override void OnTextChanged(string sentence) {
			if (!active) return;
			if (line >= maxLine) {
				Clear();
			}
			for (numOfWord = 0; numOfWord < sentence.Length; numOfWord++) {
				CreateText(sentence.Substring(numOfWord, 1));
			}
			line++;
			waitClear = waitCount;
		}
		public void SetText(string[] text) {
			Clear();
			for (var i = 0; i < maxLine; i++) {
				if (text.Length <= i) break;
				for (numOfWord = 0; numOfWord < text[i].Length; numOfWord++) {
					CreateText(text[i].Substring(numOfWord, 1));
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
