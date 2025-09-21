using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;
using Unity.VisualScripting;
using System.Collections.Generic;

public class EventSettingPanel : MonoBehaviour {
	public GameObject eventSettingPanel;
	private TMP_Dropdown commandSelector;
	private Vector3 position = new Vector3(-700, 50, 0);
	private float width = 300;
	private GameObject selectorPrefab;
	private GameObject inputPrefab;
	private string [] commands;
	private List<TMP_Dropdown> optionSelectors = new List<TMP_Dropdown>();
	private TextMeshProUGUI infotext;
	private int track;
	private int measure;
	private int beat;
	private int num;
	void Awake() {
		selectorPrefab = (GameObject)Resources.Load("Prefab/UI/ParamSelector");
		inputPrefab = (GameObject)Resources.Load("Prefab/UI/ValueInput");
		commands = Enum.GetNames(typeof(Parameter.Command));
		string [] options = new string [commands.Length + 1];
		options[0] = "-----";
		Array.Copy(commands, 0, options, 1, commands.Length);
		commandSelector = CreateSelector(position, options);
		commandSelector.onValueChanged.AddListener((value) => OnValueChanged());
		infotext = eventSettingPanel.GetComponentInChildren<TextMeshProUGUI>();
	}
	public void Show(int track, int measure, int beat, int num) {
		eventSettingPanel.transform.SetAsLastSibling();
		eventSettingPanel.SetActive(true);
		Setup(track, measure, beat, num);
	}
	private TMP_Dropdown CreateSelector(Vector3 position, string [] options) {
		GameObject instantiate = Instantiate(selectorPrefab, eventSettingPanel.transform);
		instantiate.transform.localPosition = position;
		TMP_Dropdown dropdown = instantiate.GetComponent<TMP_Dropdown>();
		for (var i = 0; i < options.Length; i++) {
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(options[i]);
			dropdown.options.Add(optionData);
		}
		return dropdown;
	}
	private TMP_Dropdown CreateOption(int num, string [] options) {
		float x = position.x + (width + 10) * (num + 1);
		TMP_Dropdown option = CreateSelector(new Vector3(x, position.y, position.z), options);
		optionSelectors.Add(option);
		return option;
	}
	private void CreateOptions() {
		List<int> optionValues = new List<int>();
		for (var i = 0; i < optionSelectors.Count; i++) {
			optionValues.Add(optionSelectors[i].value);
		}
		ClearOptions();
		var value = commandSelector.value;
		if (value == 0) return;
		string command = commands[value - 1];
		for (var i = 0; true; i++) {
			string[] options = Parameter.GetOptions(command, i);
			if (options == null) break;
			int selected = (i < optionValues.Count) ? optionValues[i] : 0;
			CreateOption(i, options);
			if (options[selected] == "VARIABLE") {
				GameObject instantiate = Instantiate(inputPrefab, optionSelectors[i].transform);
				TMP_InputField inputField = instantiate.GetComponentInChildren<TMP_InputField>();
				float val = 0;
				inputField.text = $"{val}";
			} else {
				optionSelectors[i].value = selected;
				optionSelectors[i].onValueChanged.AddListener((value) => OnParamChanged());
			}
			command = $"{command}_{options[selected]}";
		}
	}
	private void Setup(int track, int measure, int beat, int num) {
		this.track = track;
		this.measure = measure;
		this.beat = beat;
		this.num = num;
		infotext.text = $"track: {track + 1}   measure: {measure}   beat: {beat}";
		LyricData data = SentenceList.Instance.GetSentence(track, measure);
		ControlList list = data.beats[beat];
		if (num < list.controls.Count) {
			string control = list.controls[num];
			string[] args = control.Split("_");
			int commandIndex = Array.IndexOf(commands, args[0]);
			if (commandIndex >= 0) {
				// commandのvalueを選択するとcallbackが返ってCreateOptionsが呼ばれる
				commandSelector.value = commandIndex + 1;
				string command = commands[commandIndex];
				for (var i = 0; true; i++) {
					string[] options = Parameter.GetOptions(command, i);
					if (options == null) break;
					if (options[0] == "VARIABLE") {
						TMP_InputField input = optionSelectors[i].transform.parent.GetComponentInChildren<TMP_InputField>();
						if (input) input.text = args[i + 1];
					} else if (i + 1 < args.Length) {
						int selected = Array.IndexOf(options, args[i + 1]);
						if (selected >= 0) {
							if (optionSelectors.Count > i) optionSelectors[i].value = selected;
						}
					}
					command += $"_{args[i + 1]}";
				}
			}
		}
	}
	private void ClearOptions() {
		foreach (TMP_Dropdown obj in optionSelectors) {
			Destroy(obj.gameObject);
		}
		optionSelectors.Clear();
	}
	private void ResetItems() {
		eventSettingPanel.SetActive(false);
		ClearOptions();
		commandSelector.value = 0;
	}
	public void OnEventSettingSubmit() {
		string commandtext = "";
		if (commandSelector.value > 0) {
			string command = commands[commandSelector.value - 1];
			commandtext = command;
			for (var i = 0; i < optionSelectors.Count; i++) {
				TMP_Dropdown obj = optionSelectors[i];
				string[] options = Parameter.GetOptions(commandtext, i);
				if (options != null) {
					if (options[obj.value] == "VARIABLE") {
						TMP_InputField input = obj.transform.parent.GetComponentInChildren<TMP_InputField>();
						commandtext += $"_{input.text}";
					} else {
						commandtext += $"_{options[obj.value]}";
					}
				}
			}
		}
		ResetItems();
		LyricLists.Instance.lists[1].SetControl(track + 1, measure, beat, num, commandtext);
	}
	public void OnValueChanged() {
		CreateOptions();
	}
	public void OnParamChanged() {
		CreateOptions();
	}
	public void OnEventSettingCancel() {
		ResetItems();
	}
	public bool IsActive() {
		return eventSettingPanel.activeSelf;
	}
}
