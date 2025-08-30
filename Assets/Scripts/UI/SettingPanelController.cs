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
	private string [] controlTypes;
	private string [] fontTypes;
	private LyricBase targetLyric;
	private bool activated = false;
	void Awake() {
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
		LyricBase lyric = GetLyricObj((LyricControl.Type)edititem.value);
		if (lyric == null) return;
		targetLyric = lyric;
		fontSelector.value = (int)FontResource.Instance.GetFontType(targetLyric.font.name);
		xinput.text = targetLyric.GetPosX().ToString();
		yinput.text = targetLyric.GetPosY().ToString();
		winput.transform.parent.gameObject.SetActive(targetLyric.HasArea());
		hinput.transform.parent.gameObject.SetActive(targetLyric.HasArea());
		winput.text = targetLyric.GetAreaW().ToString();
		hinput.text = targetLyric.GetAreaH().ToString();
	}
	public void OnEditItemChanged(int num) {
		targetLyric.Hide();
		GetParams();
		targetLyric.Show();
	}
	public void OnFontSelectChanged() {
		targetLyric.SetFont(FontResource.Instance.GetFont((FontResource.Type)fontSelector.value));
	}
	public void OnInputEndX() {
		string text = xinput.text;
		float x = float.Parse(text);
		targetLyric.SetPosX(x);
	}
	public void OnInputEndY() {
		string text = yinput.text;
		float y = float.Parse(text);	
		targetLyric.SetPosY(y);
	}
	public void OnInputEndW() {
		string text = winput.text;
		float w = float.Parse(text);
		targetLyric.SetPosW(w);
	}
	public void OnInputEndH() {
		string text = hinput.text;
		float h = float.Parse(text);	
		targetLyric.SetPosH(h);
	}
	public void OnInputEndSampleText(string text) {

	}
	public void OnShowTextButtonClicked() {
		string text = sampleText.text;
		string [] lines = text.Split('\n');
		targetLyric.ShowSampleText(lines);
	}
}
