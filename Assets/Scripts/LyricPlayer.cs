using UnityEngine;
using UnityEngine.SceneManagement;

public class LyricPlayer : MonoBehaviour
{
	private AudioSource audioSource;
	private static SMFPlayer smfPlayer;
	private static SMFPlayer kanjiPlayer;
	private bool fIsPlaying = false;
	private string [] songbasenames = new string [] {
		@"らーめんたべよう",
		@"らーめんまだかな",
		@"遥かな旅路",
		@"3分間のトキメキ",
		@"約束の場所へ"
	};
	private const string kanji = @"(漢字)";
	string GetAudioFileBasename()
	{
		int num = PlayerPrefs.GetInt("Song");
		string basename = songbasenames[0];
		if ((num >= 0) && (num < songbasenames.Length)) {
			basename = songbasenames[num];
		}
		return basename;
	}
	void Start()
	{
		string basename = GetAudioFileBasename();
		audioSource = GetComponent<AudioSource>();
		AudioClip clip = Resources.Load<AudioClip>($"Audio/{basename}");
		if (clip != null && audioSource != null) {
			audioSource.clip = clip;
		} else {
			Debug.LogWarning($"AudioClip または AudioSource {basename}.mp3 が見つかりません。");
			End();
			return;
		}
		Visualizer visualizer = GetComponent<Visualizer>();
		MidiWatcher midiWatcher = new MidiWatcher(visualizer);
		string smfPath = $"{Application.streamingAssetsPath}/{basename}.mid";
		string kanjiPath = $"{Application.streamingAssetsPath}/{basename}{kanji}.mid";
		smfPlayer = new SMFPlayer(smfPath);
		kanjiPlayer = new SMFPlayer(kanjiPath);
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
		// SceneManager.LoadScene("StartScene");
	}

	public void Stop()
	{
		audioSource.Stop();
		smfPlayer.Stop();
		kanjiPlayer.Stop();
	}
}
