using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using MidiJack;
using UnityEngine.UI;
using TMPro;
public class LyricPlayer : MonoBehaviour {
	public Parameter parameter;
	private AudioSource audioSource;
	private static SMFPlayer smfPlayer;
	private static SMFPlayer kanjiPlayer;
	private bool fIsPlaying = false;
	private const float startWaitTime = 0.1f;
	private float startWait = startWaitTime;
	public GameObject blackOut;
	public int measure = 0;
	private int numOfMeas = 100;
	private uint currentMsec = 0;
	private float endTimer = 0;
	public GameObject editPanel;
	public GameObject transportPanel;
	public GameObject settingPanel;
	public EditPanel editPanelControl;
	public Button playButton;
	public Button repeatButton;
	public Slider curPos;
	public TextMeshProUGUI textPos;
	public Slider pointA;
	private TextMeshProUGUI textA;
	public Slider pointB;
	private TextMeshProUGUI textB;
	private bool fRepeat = false;
	private Image playButtonImage;
	private Image repeatButtonImage;
	private bool wasPlaying = false;
	void Awake() {
		MidiMaster.noteOnDelegate += NoteOn;
		int songnum = PlayerPrefs.GetInt("Song");
		audioSource = GetComponent<AudioSource>();
		string clipname = SongInfo.GetAudioClipName(songnum);
		// Debug.Log($"clipname = {clipname}");
		AudioClip clip = Resources.Load<AudioClip>(clipname);
		if (clip != null && audioSource != null) {
			audioSource.clip = clip;
		} else {
			Debug.LogWarning($"AudioClip または AudioSource {SongInfo.GetBaseName(songnum)}.mp3 が見つかりません。");
			//End();
			return;
		}
		smfPlayer = new SMFPlayer(SongInfo.GetSMFPath(songnum, false), SongInfo.numOfMeasure[songnum]);
		kanjiPlayer = new SMFPlayer(SongInfo.GetSMFPath(songnum, true), SongInfo.numOfMeasure[songnum]);
		smfPlayer.midiHandler = SubMidiWatcher.Instance;
		kanjiPlayer.midiHandler = MidiWatcher.Instance;
		Visualizer visualizer = GetComponent<Visualizer>();
		visualizer.SetSMFPlayer(smfPlayer, kanjiPlayer);
		visualizer.SetTitle(SongInfo.GetTitle(songnum));
		foreach (LyricList lyricList in LyricLists.Instance.lists) {
			lyricList.Init();
		}
		FontResource fontResource = FontResource.Instance;
		fontResource.LoadFont();
		SentenceList.Instance.Init();

		textPos = curPos.handleRect.GetComponentInChildren<TextMeshProUGUI>();
		textPos.text = curPos.value.ToString();
		textA = pointA.handleRect.GetComponentInChildren<TextMeshProUGUI>();
		textA.text = pointA.value.ToString();
		textB = pointB.handleRect.GetComponentInChildren<TextMeshProUGUI>();
		textB.text = pointB.value.ToString();
		numOfMeas = SongInfo.numOfMeasure[songnum];
		// if (numOfMeas < 0) {
		// 	numOfMeas = LyricLists.Instance.tracks[0].lyrics.Count;
		// }
		curPos.minValue = 0;
		curPos.maxValue = numOfMeas;
		pointA.minValue = 0;
		pointA.maxValue = numOfMeas - 1;
		pointA.value = 0;
		pointB.minValue = 1;
		pointB.maxValue = numOfMeas;
		pointB.value = numOfMeas;
		playButtonImage = playButton.GetComponent<Image>();
		repeatButtonImage = repeatButton.GetComponent<Image>();
	}
	void OnDestroy() {
		MidiMaster.noteOnDelegate -= NoteOn;
		MidiWatcher.Instance.Clear();
		Lyrics.lyrics.Clear();
	}
	private void NoteOn(MidiChannel channel, int note, float velocity) {
		if (note == Parameter.NoteStartStop) {
			//End();
		}
	}
	void Start() {
		// fader = FindObjectOfType<FadeController>();
		// fader.FadeIn();
	}

	void StartPlayer() {
		audioSource.Play();
		fIsPlaying = true;
		smfPlayer.Start();
		kanjiPlayer.Start();
		endTimer = 1f;
		blackOut.SetActive(false);
	}

	// Update is called once per frame
	void Update() {
		smfPlayer.Update();
		kanjiPlayer.Update();
		if (smfPlayer.isPlaying()) {
			if (!audioSource.isPlaying) {
				endTimer -= Time.deltaTime;
				if (endTimer <= 0) {
					PlayStop();
				}
			}
			measure = smfPlayer.currentMeasure;
			if (fRepeat && measure >= pointB.value) {
				PlayStop();
				LyricGenList.Clear();
				measure = (int)pointA.value;
				PlayStart();
			} else {
				curPos.value = measure;
				textPos.text = curPos.value.ToString();
			}
		}
		// get key
		if (!editPanelControl.isLyricEditing) {
			if (Input.GetKeyDown(KeyCode.Q)) {
				End();
			}
			if (Input.GetKeyDown(KeyCode.L)) {
				settingPanel.SetActive(!settingPanel.activeSelf);
			}
			if (!settingPanel.activeSelf) {
				if (Input.GetKeyDown(KeyCode.Space)) {
					if (Input.GetKey(KeyCode.LeftShift)) {
						measure = 0;
						LyricGenList.Clear();
					}
					OnPlayClicked();
				}
				if (Input.GetKeyDown(KeyCode.T)) {
					transportPanel.SetActive(!transportPanel.activeSelf);
				}
				if (Input.GetKeyDown(KeyCode.E)) {
					editPanel.SetActive(!editPanel.activeSelf);
				}
			}
		}
	}

	public void End() {
		smfPlayer?.Stop();
		kanjiPlayer?.Stop();
		Visualizer visualizer = GetComponent<Visualizer>();
		visualizer.BackupParams();
		SceneManager.LoadScene("TitleScene");
	}

	public void Stop() {
		audioSource.Stop();
		smfPlayer.Stop();
		kanjiPlayer.Stop();
	}

	private void PlayStop() {
		audioSource.Stop();
		this.currentMsec = smfPlayer.currentMsec;
		this.measure = smfPlayer.currentMeasure;
		smfPlayer.Stop();
		kanjiPlayer.Stop();
	}
	private void PlayStart() {
		if (measure >= numOfMeas - 1) {
			measure = 0;
			blackOut.SetActive(false);
		}
		LyricData data = SentenceList.Instance.GetSentence(0, measure);
		LyricGenList.Start(measure);
		currentMsec = data.msec;
		Visualizer visualizer = GetComponent<Visualizer>();
		visualizer.ResetControl();
		visualizer.UpdateControl(measure);
		smfPlayer.Start(currentMsec);
		kanjiPlayer.Start(currentMsec);
		audioSource.time = currentMsec / 1000f;
		audioSource.Play();
	}
	public void OnPlayClicked() {
		if (audioSource.isPlaying) {
			PlayStop();
		} else {
			endTimer = 1f;
			PlayStart();
		}
		UpdatePlayButtonImage();
	}
	public void OnRepeatClicked() {
		fRepeat = !fRepeat;
		repeatButtonImage.color = fRepeat ? Color.green : Color.gray;
	}
	public void OnCurPosChanged() {
		measure = (int)curPos.value;
		if (textPos) textPos.text = curPos.value.ToString();
	}
	public void OnInPosChanged() {
		if (pointA.value >= pointB.value) {
			pointA.value = pointB.value - 1;
		}
		if (textA) textA.text = pointA.value.ToString();
	}
	public void OnOutPosChanged() {
		if (pointB.value <= pointA.value) {
			pointB.value = pointA.value + 1;
		}
		if (textB) textB.text = pointB.value.ToString();
	}
	public void UpdatePlayButtonImage() {
		playButtonImage.color = smfPlayer.isPlaying() ? Color.green : Color.gray;
	}
	public void OnLyricResetClicked() {
		const int track = 1;
		const int map = 1;
		LyricList lyricList = LyricLists.Instance.lists[map];
		Track trackData = lyricList.tracks[track];
		MidiEventMapAccessor eventMap = MidiEventMapAccessor.Instance;
		for (var meas = 0; meas < trackData.lyrics.Count; meas++) {
			string sentence = eventMap.GetSentence(meas, track, map);
			// Debug.Log($"meas: {meas}, sentence: {sentence}");
			lyricList.SetSentence(track + 1, meas, sentence);
		}
	}
	public void OnLyricReloadClicked() {
		LyricLists.Instance.lists[1].Init();
		SentenceList.Instance.Init();
	}
}
