using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public class EditPanel : MonoBehaviour {
	public GameObject eventPanel;
	public TMP_Dropdown trackInput;
	public TMP_InputField lyric;
	public TMP_Text text;
	public EventSettingPanel eventSettingPanel;
	private int measure = -1;
	private GameObject textTrackNumber;
	private GameObject eventButton;
	private List<GameObject> textTrackNumbers = new List<GameObject>();
	public LyricPlayer player;

	// Start is called before the first frame update
	void Start() {
		trackInput.options?.Clear();
		for (var i = 0; i < LyricLists.Instance.lists[1].tracks.Count; i++) {
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData((i + 1).ToString());
			trackInput.options.Add(optionData);
		}
		trackInput.value = 1;
		trackInput.captionText.text = "1";
		textTrackNumber = (GameObject)Resources.Load("Prefab/UI/TrackNumber");
		eventButton = (GameObject)Resources.Load("Prefab/UI/EventButton");
	}
	public void OnTrackValueChanged(int value) {
		measure = -1;
	}
	public void UpdateLyric(string text) {
		LyricLists.Instance.lists[1].SetSentence(trackInput.value, measure, text);
	}
	// Update is called once per frame
	void Update() {
		if (player.measure != measure) {
			measure = player.measure;
			LyricData data = LyricLists.Instance.lists[1].GetSentence(trackInput.value + 1, measure);
			lyric.text = data.sentence;
			if (!eventSettingPanel.IsActive()) CreateOption();
		}
	}
	public void OnButtonClick(int beat, int num) {
		eventSettingPanel.Show(trackInput.value, measure, beat, num);
	}
	public void OnEventSettingSubmit() {
		CreateOption();
	}
	public void OnEventSettingCancel() {
		CreateOption();
	}
	private void CreateOption() {
		LyricData data = LyricLists.Instance.lists[1].GetSentence(trackInput.value + 1, measure);
		float y = 230;
		for (var beat = 0; beat < textTrackNumbers.Count; beat++) {
			Destroy(textTrackNumbers[beat]);
		}
		textTrackNumbers.Clear();
		for (var beat = 0; beat < data.beats.Count; beat++) {
			int beatID = beat;
			ControlList list = data.beats[beat];
			Vector3 position = new Vector3(-750, y, 0);
			GameObject trackNum = Instantiate(textTrackNumber, eventPanel.transform);
			TextMeshProUGUI tmpro = trackNum.GetComponent<TextMeshProUGUI>();
			tmpro.text = (beat + 1).ToString();
			trackNum.transform.localPosition = position;
			float buttonX = 30;
			for (var i = 0; i < list.controls.Count + 1; i++) {
				int buttonID = i;
				GameObject buttonObj = Instantiate(eventButton, trackNum.transform);
				Vector3 buttonPosition = new Vector3(buttonX, 10, 0);
				buttonObj.transform.localPosition = buttonPosition;
				Button button = buttonObj.GetComponent<Button>();
				TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
				if (i < list.controls.Count) text.text = list.controls[i];
				button.onClick.AddListener(() => OnButtonClick(beatID, buttonID));
				buttonX += 200;
			}
			textTrackNumbers.Add(trackNum);
			y -= 50;
		}
	}
}
