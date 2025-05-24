using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MidiJack;

public class StartSceneController : MonoBehaviour
{
	private const int numOfSong = 5;
    private GameObject loadingPanel;
	private TMP_Text songtitle;
	public GameObject titlePanel;
	private int currentSong = 0;
	public Parameter parameter;

	// Start is called before the first frame update
	void Awake()
	{
		loadingPanel = GameObject.Find("LoadingPanel");
		loadingPanel.SetActive(false);
		titlePanel.SetActive(true);
		songtitle = GameObject.Find("SongTitle").GetComponent<TMP_Text>();;
		SelectSong(PlayerPrefs.GetInt("Song"));
		MidiMaster.noteOnDelegate += NoteOn;
	}
	void OnDestroy()
	{
		MidiMaster.noteOnDelegate -= NoteOn;
	}
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space)) {
            LoadMainScene();
        }
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			SelectSong(currentSong - 1);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			SelectSong(currentSong + 1);
		}
	}

    private void LoadMainScene()
    {
        SceneManager.LoadScene("PlayerScene");
		titlePanel.SetActive(false);
        loadingPanel.SetActive(true);
    }

	private void SelectSong(int num)
	{
		if (num < 0) return;
		if (num >= numOfSong) return;
		songtitle.SetText(SongInfo.GetTitle(num));
		PlayerPrefs.SetInt("Song", num);
		currentSong = num;
	}

	private void NoteOn(MidiChannel channel, int note, float velocity)
	{
		if (note == Parameter.NoteStartVideo) {
			LoadMainScene();
		} else if (note == Parameter.NoteSongDown) {
			SelectSong(currentSong - 1);
		} else if (note == Parameter.NoteSongUp) {
			SelectSong(currentSong + 1);
		}
	}
}
