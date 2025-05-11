using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using MidiJack;

public class LyricPlayer : MonoBehaviour
{
	public Parameter parameter;
	private AudioSource audioSource;
	private static SMFPlayer smfPlayer;
	private static SMFPlayer kanjiPlayer;
	private bool fIsPlaying = false;
	public static MidiWatcher midiWatcher;

	void Awake()
	{
		MidiMaster.noteOnDelegate += NoteOn;
		midiWatcher = new MidiWatcher();
	}
	void OnDestroy()
	{
		MidiMaster.noteOnDelegate -= NoteOn;
	}
	private void NoteOn(MidiChannel channel, int note, float velocity)
	{
		if (note == parameter.NoteStopVideo) {
			End();
		}
	}
	void Start()
	{
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
		smfPlayer = new SMFPlayer(SongInfo.GetSMFPath(songnum, false));
		kanjiPlayer = new SMFPlayer(SongInfo.GetSMFPath(songnum, true));
		smfPlayer.midiHandler = midiWatcher;
		kanjiPlayer.midiHandler = midiWatcher;
		Visualizer visualizer = GetComponent<Visualizer>();
		visualizer.SetSMFPlayer(smfPlayer, kanjiPlayer);
		audioSource.Play();
		fIsPlaying = true;
		smfPlayer.Start();
		kanjiPlayer.Start();
	}

	// Update is called once per frame
	void Update()
	{
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
			End();
		}
	}

	public void End()
	{
		smfPlayer?.Stop();
		kanjiPlayer?.Stop();
		SceneManager.LoadScene("TitleScene");
	}

	public void Stop()
	{
		audioSource.Stop();
		smfPlayer.Stop();
		kanjiPlayer.Stop();
	}
}
