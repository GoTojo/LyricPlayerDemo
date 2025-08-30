/// Title.cs
/// タイトルを表示する
using UnityEngine;
using TMPro;

public class TitleControl : LyricBase
{
	public TextMeshPro title;
	string titleText;
	void Start() {
		int songnum = PlayerPrefs.GetInt("Song");
		titleText = SongInfo.GetTitle(songnum);
	}
	public override void OnParamChanged() {
		if (active) title.text = titleText;
		title.font = font;
		title.enabled = active;
	}
	public override void Clear() {
		title.text = "";
	}
	public override void ShowSampleText(string [] sampletext) {
		title.text = sampletext[0];
	}
}