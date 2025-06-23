// LyricGenLineBase.cs
// 小節毎の歌詞を取得、SMFPlayerからのイベントで適切なタイミングを判断、歌詞の切り替えを行う
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

class LyricGenLineBase : LyricGenBase
{
	public LyricGenLineBase(SentenceList sentenceList, int map, MidiWatcherBase midiWatcher) : base(sentenceList, map, midiWatcher)
	{
	}
	public string ToVertical(string input)
	{
		var builder = new System.Text.StringBuilder();

		foreach (char c in input)
		{
			if (c == 'ー' || c == '～' || c == '―' || c == '…')
			{
				builder.Append("<rotate=90>");
				builder.Append(c);
				builder.Append("</rotate>");
			}
			else
			{
				builder.Append(c);
			}
			builder.Append('\n');
		}

		return builder.ToString();
	}
	public GameObject CreateText(string word, TMP_FontAsset font, Color color, TextAlignmentOptions align, Vector2 sizeDelta, Vector3 position, float scale, float rotate)
	{
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
		text.alignment = align;
		text.lineSpacing = -30;
		Transform transform = text.GetComponent<Transform>();
		RectTransform rectTransform = simpleLyric.GetComponent<RectTransform>();
		rectTransform.sizeDelta = sizeDelta;
		// rectTransform.pivot = new Vector2(0.5f, 1);
		transform.position = position;
		transform.Rotate(0.0f, 0.0f, rotate);
		simpleLyric.transform.localScale = new Vector3(scale, scale, scale);
		return simpleLyric;
	}
}