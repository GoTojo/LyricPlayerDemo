// LyricGenLineBase.cs
// 小節毎の歌詞を取得、SMFPlayerからのイベントで適切なタイミングを判断、歌詞の切り替えを行う
using UnityEngine;
using TMPro;
using System.Collections.Generic;

class LyricGenLineBase : LyricGenBase {
	private Rect area;
	protected float textHeight = 2f;
	protected float textWidth = 2f;
	protected List<GameObject> lyrics = new List<GameObject>();
	protected TMP_FontAsset font;
	protected Transform transform;
	protected int line = 0;
	public int maxLine = 5;
	public float scale = 1;

	public bool vertical = false;
	public LyricGenLineBase(Rect area, float textHeight, float textWidth, TMP_FontAsset font, Transform transform, SentenceList sentenceList) : base(sentenceList, SentenceList.kanjiMap, MidiWatcher.Instance) {
		this.area = area;
		this.textHeight = textHeight;
		this.textWidth = textWidth;
		this.font = font;
		this.transform = transform;
	}
	public string ToVertical(string input) {
		var builder = new System.Text.StringBuilder();

		foreach (char c in input) {
			if (c == 'ー' || c == '～' || c == '―' || c == '…') {
				builder.Append("<rotate=90>");
				builder.Append(c);
				builder.Append("</rotate>");
			} else {
				builder.Append(c);
			}
			builder.Append('\n');
		}

		return builder.ToString();
	}
	public void GetTextArea(int num, bool vertical, ref Vector3 position, ref Vector2 size) {
		float x;
		float y;
		float w;
		float h;
		if (vertical) {
			w = textWidth;
			h = area.height;
			x = area.xMin - textWidth * num;
			y = area.yMax;
		} else {
			w = area.width;
			h = textHeight;
			x = area.x;
			y = area.yMax - h * num;
		}
		size = new Vector2(w, h);
		position = new Vector3(x, y, 0);
	}
	protected GameObject CreateText(string text) {
		if (!active) return null;
		Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		float rotate = 0;
		TextAlignmentOptions alignment;
		Vector2 size = new Vector2();
		Vector3 position = new Vector3();
		GetTextArea(line, vertical, ref position, ref size);
		GetPosition(ref position.x, ref position.y);
		if (vertical) {
			alignment = TextAlignmentOptions.Top;
			text = ToVertical(text);
		} else {
			alignment = TextAlignmentOptions.TopLeft;
		}
		GameObject lyric = CreateText(text, font, color, alignment, size, position, scale, rotate);
		lyric.transform.SetParent(transform);
		lyrics.Add(lyric);
		return lyric;
	}
	protected virtual void GetPosition(ref float x, ref float y) {
	}
	public void Clear() {
		foreach (var lyric in lyrics) {
			Object.Destroy(lyric);
		}
		lyrics.Clear();
		line = 0;
		OnCleared();
	}
	protected virtual void OnCleared() {
	}
};