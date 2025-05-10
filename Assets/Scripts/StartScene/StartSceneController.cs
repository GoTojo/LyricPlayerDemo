using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MidiJack;

public class StartSceneController : MonoBehaviour
{
	private int songnum = 0;
	private const int numOfSong = 5;
    private static GameObject loadingPanel;

	[SerializeField] public static int NoteSongTabeyo = 0x3D;
	[SerializeField] public static int NoteSongMadakana = 0x3F;
	[SerializeField] public static int NoteSongHaruka = 0x42;
	[SerializeField] public static int NoteSongSanpun = 0x44;
	[SerializeField] public static int NoteSongYakusoku = 0x46;
	[SerializeField] public static int NoteStartVideo = 0x48;

	// Start is called before the first frame update
	void Awake()
	{
		loadingPanel = GameObject.Find("LoadingPanel");
		loadingPanel.SetActive(true);
		MidiMaster.noteOnDelegate += NoteOn;
	}
	void OnDestroy()
	{
		MidiMaster.noteOnDelegate -= NoteOn;
	}
	// Update is called once per frame
	void Update()
	{
		
	}

    private void LoadMainScene()
    {
        SceneManager.LoadScene("PlayerScene");
        loadingPanel.SetActive(true);
    }

	private void SelectSong(int num)
	{
		if (num < 0) return;
		if (num >= numOfSong) return;
		PlayerPrefs.SetInt("Song", songnum);
	}

	private void NoteOn(MidiChannel channel, int note, float velocity)
	{
		if (note == NoteStartVideo) {
			LoadMainScene();
		} else if (note == NoteSongTabeyo) {
			SelectSong(0);
		} else if (note == NoteSongMadakana) {
			SelectSong(1);
		} else if (note == NoteSongHaruka) {
			SelectSong(2);
		} else if (note == NoteSongSanpun) {
			SelectSong(3);
		} else if (note == NoteSongYakusoku) {
			SelectSong(4);
		}
	}
}
