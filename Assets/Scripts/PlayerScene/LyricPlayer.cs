using UnityEngine;
using UnityEngine.SceneManagement;

public class LyricPlayer : MonoBehaviour
{
	private AudioSource audioSource;
	private static SMFPlayer smfPlayer;
	private static SMFPlayer kanjiPlayer;
	private bool fIsPlaying = false;
	void Start()
	{
		int songnum = PlayerPrefs.GetInt("Song");
		audioSource = GetComponent<AudioSource>();
		AudioClip clip = Resources.Load<AudioClip>(SongInfo.GetAudioClipName(songnum));
		if (clip != null && audioSource != null) {
			audioSource.clip = clip;
		} else {
			Debug.LogWarning($"AudioClip または AudioSource {SongInfo.GetBaseName(songnum)}.mp3 が見つかりません。");
			End();
			return;
		}
		smfPlayer = new SMFPlayer(SongInfo.GetSMFPath(songnum, false));
		kanjiPlayer = new SMFPlayer(SongInfo.GetSMFPath(songnum, true));
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
		SceneManager.LoadScene("StartScene");
	}

	public void Stop()
	{
		audioSource.Stop();
		smfPlayer.Stop();
		kanjiPlayer.Stop();
	}
}
