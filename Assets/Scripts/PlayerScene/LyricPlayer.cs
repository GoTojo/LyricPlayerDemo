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
	private uint currentMsec = 0;
	private float endTimer = 0;
	public GameObject editPanel;
	public GameObject transportPanel;
	public GameObject settingPanel;
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
			End();
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
	}
	void OnDestroy() {
		MidiMaster.noteOnDelegate -= NoteOn;
		MidiWatcher.Instance.Clear();
	}
	private void NoteOn(MidiChannel channel, int note, float velocity) {
		if (note == Parameter.NoteStartStop) {
			End();
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
		if (startWait > 0) {
			startWait -= Time.deltaTime;
			if (startWait <= 0) {
				StartPlayer();
			}
			return;
		}
		if (Input.GetKey(KeyCode.Space)) {
			End();
		} else if (fIsPlaying) {
			fIsPlaying = smfPlayer.Update();
			kanjiPlayer.Update();
			if (!fIsPlaying) {
				// SongEnd
			}
		} else {
			// End();
		}
		if (!audioSource.isPlaying) {
			blackOut.SetActive(true);
			endTimer -= Time.deltaTime;
			if (endTimer <= 0) {
				End();
			}
		}
	}

	public void End() {
		smfPlayer?.Stop();
		kanjiPlayer?.Stop();
		Visualizer visualizer = GetComponent<Visualizer>();
		visualizer.BackupParams();
		// SceneManager.LoadScene("TitleScene");
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
	}
	private void PlayStart() {
		LyricData data = SentenceList.Instance.GetSentence(0, measure);
		LyricGenList.Start(measure);
		currentMsec = data.msec;
		smfPlayer.Start(currentMsec);
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
}
