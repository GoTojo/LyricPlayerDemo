using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class SettingPanelController : MonoBehaviour
{
	public GameObject settingPanel;
	public TMP_Dropdown edititem;
	public TMP_Dropdown fontSelector;
	public TMP_InputField xinput;
	public TMP_InputField yinput;
	public TMP_InputField winput;
	public TMP_InputField hinput;
	public TMP_InputField sampleText;
	public TitleControl titleControl;
	public LyricGenUnder1Line line;
	public SimpleLyricGen words;
	public LyricGenMultiLine multiL;
	public LyricGenMultiLine multiR;
	public LyricGenMultiLine multiVL;
	public LyricGenMultiLine multiVR;
	public LyricGenMultiLineByWord multiWordL;
	public LyricGenMultiLineByWord multiWordR;
	public LyricGenMultiLineByWord multiWordVL;
	public LyricGenMultiLineByWord multiWordVR;
	public LyricControl lyricControl;
	private string[] controlTypes;
	private string [] fontTypes;
	private LyricBase targetLyric;
	private int curLyric;
	private bool activated = false;
	void Start() {
		controlTypes = Enum.GetNames(typeof(LyricControl.Type));
		for (var i = 0; i < controlTypes.Length; i++) {
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(controlTypes[i]);
			edititem.options.Add(optionData);
		}
		fontTypes = Enum.GetNames(typeof(Parameter.Font));
		for (var i = 0; i < fontTypes.Length; i++) {
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(fontTypes[i]);
			fontSelector.options.Add(optionData);
		}
		sampleText.text = "さんぷるてきすと";
		RestoreParams();
		GetParams();
	}
	void Update() {
		if (activated != settingPanel.activeSelf) {
			activated = settingPanel.activeSelf;
			if (settingPanel.activeSelf) {
				GetParams();
				targetLyric.Show();
			}
		}
	}
	private LyricBase GetLyricObj(LyricControl.Type type) {
		LyricBase lyric = null;
		switch (type) {
		case LyricControl.Type.Title:
			lyric = titleControl;
			break;
		case LyricControl.Type.Line:
			lyric = line;
			break;
		case LyricControl.Type.Words:
			lyric = words;
			break;
		case LyricControl.Type.MultiL:
			lyric = multiL;
			break;
		case LyricControl.Type.MultiR:
			lyric = multiR;
			break;
		case LyricControl.Type.MultiVL:
			lyric = multiVL;
			break;
		case LyricControl.Type.MultiVR:
			lyric = multiVR;
			break;
		case LyricControl.Type.MultiWordL:
			lyric = multiWordL;
			break;
		case LyricControl.Type.MultiWordR:
			lyric = multiWordR;
			break;
		case LyricControl.Type.MultiWordVL:
			lyric = multiWordVL;
			break;
		case LyricControl.Type.MultiWordVR:	
			lyric = multiWordVR;
			break;
		default:
			lyric = null;
			break;
		}
		return lyric;
	}
	public void OnExitButtonClicked() {
		settingPanel.SetActive(false);
	}
	private void GetParams() {
		curLyric = edititem.value; 
		LyricBase lyric = GetLyricObj((LyricControl.Type)curLyric);
		if (lyric == null) return;
		targetLyric = lyric;
		fontSelector.value = (int)FontResource.Instance.GetFontType(targetLyric.font.name);
		xinput.text = targetLyric.GetPosX().ToString();
		yinput.text = targetLyric.GetPosY().ToString();
		winput.transform.parent.gameObject.SetActive(targetLyric.HasArea());
		hinput.transform.parent.gameObject.SetActive(targetLyric.HasArea());
		winput.text = targetLyric.GetPosW().ToString();
		hinput.text = targetLyric.GetPosH().ToString();
	}
	public void OnEditItemChanged(int num) {
		targetLyric.Hide();
		GetParams();
		targetLyric.Show();
	}
	public void OnFontSelectChanged() {
		int font = fontSelector.value;
		SetFont(targetLyric, (FontResource.Type)font);
		StoreParam("FONT", font);
	}
	private void SetFont(LyricBase lyric, FontResource.Type type) {
		lyric.SetFont(FontResource.Instance.GetFont(type));
	}
	public void OnInputEndX() {
		string text = xinput.text;
		float x = float.Parse(text);
		lyricControl.SetPosition(targetLyric, "POSX", x);
		StoreParam("POSX", x);
	}
	public void OnInputEndY() {
		string text = yinput.text;
		float y = float.Parse(text);	
		lyricControl.SetPosition(targetLyric, "POSY", y);
		StoreParam("POSY", y);
	}
	public void OnInputEndW() {
		string text = winput.text;
		float w = float.Parse(text);
		lyricControl.SetPosition(targetLyric, "POSW", w);
		StoreParam("POSW", w);
	}
	public void OnInputEndH() {
		string text = hinput.text;
		float h = float.Parse(text);
		lyricControl.SetPosition(targetLyric, "POSH", h);
		StoreParam("POSH", h);
	}
	private void StoreParam(string param, float value) {
		int songnum = PlayerPrefs.GetInt("Song");
		PlayerPrefs.SetFloat($"SONG{songnum}_{controlTypes[curLyric]}_{param}", value);
	}
	private void StoreParam(string param, int value) {
		int songnum = PlayerPrefs.GetInt("Song");
		PlayerPrefs.SetInt($"SONG{songnum}_{controlTypes[curLyric]}_{param}", value);
	}
	private void SetPosition(LyricBase lyricObj, string type, string param) {
		int songnum = PlayerPrefs.GetInt("Song");
		string key = $"SONG{songnum}_{type}_{param}";
		if (PlayerPrefs.HasKey(key)) {
			lyricControl.SetPosition(lyricObj, param, PlayerPrefs.GetFloat(key));
		}
	}
	private void RestoreParams() {
		int songnum = PlayerPrefs.GetInt("Song");
		for (var type = 0; type < controlTypes.Length; type++) {
			LyricBase lyricObj = GetLyricObj((LyricControl.Type)type);
			string lyricType = controlTypes[type];
			SetPosition(lyricObj, lyricType, "POSX");
			SetPosition(lyricObj, lyricType, "POSY");
			SetPosition(lyricObj, lyricType, "POSW");
			SetPosition(lyricObj, lyricType, "POSH");
			string key = $"SONG{songnum}_{lyricType}_FONT";
			if (PlayerPrefs.HasKey(key)) {
				SetFont(lyricObj, (FontResource.Type)PlayerPrefs.GetInt(key));
			}
		}
	}
	public void OnInputEndSampleText(string text) {

	}
	public void OnShowTextButtonClicked() {
		string text = sampleText.text;
		string [] lines = text.Split('\n');
		targetLyric.ShowSampleText(lines);
	}
}
