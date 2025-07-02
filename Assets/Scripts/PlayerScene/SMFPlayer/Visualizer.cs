using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using MidiJack;
using System;
using System.Collections.Generic;

public class Visualizer : MonoBehaviour {
	public Parameter parameter;
	public Rect area = new Rect(-2, 5, 4, 10);
	public SMFPlayer smfPlayer;
	public SMFPlayer kanjiPlayer;
	public UIPanelControl uiPanelControl;
	public GameObject infoPanel;
	// private LyricMode lyricMode;
	MidiEventMapAccessor eventMap;
	public EffectSwitcher effectSwitcher;

	public GameObject unityChanBlack;
	public GameObject unityChanColor;
	public GameObject unityChanRunning;
	public GameObject unityChanShift;
	public ZeknovaController zeknovaController;
	public ParticleController particleController;
	public SunController sun;
	public GameObject ramenDisk;
	public GameObject ramenFloor;
	public GameObject night;
	public GameObject wave;
	public BackGroundController backGroundController;
	public SimpleLyricGen simpleLyricGen;
	public SimpleLyricGen simpleLyricGenUp;
	public LyricGenMultiLine multiLineL;
	public LyricGenMultiLine multiLineR;
	public LyricGenMultiLine multiLineL1;
	public LyricGenMultiLine multiLineR1;
	public TextMeshPro titleCenter;
	public RamenController ramenController;

	public Parameter.ParticleType particleType;
	public Parameter.UnityChanType unityChanType;
	public GameObject blackOut;

	public RotateContentsZ rotateContentsZ;
	public RotateContentsZ rotateContentsZ1;
	public Bulb bulb;
	public Rocket rocket;
	public Icons icons;
	public ShootingStar shootingStar;
	public Naruto naruto;
	public UFO ufo;
	private int particleMeasCount = 0;
	private SentenceList sentenceList;
	private int newMeasure = -1;
	private int curMeasure = 0;
	private int curBeat = 0;
	private bool beatEffectApplied = true;
	private float beatInterval = 0.5f;
	private float measureInterval = 2f;
	private bool showInfo = false;

	public enum LyricMode {
		Original,
		Kanji
	}

	void Awake() {
		// lyricMode = LyricMode.Kanji;
		// particleType = (Parameter.ParticleType)PlayerPrefs.GetInt("Parameter.ParticleType");
		// unityChanType = (Parameter.UnityChanType)PlayerPrefs.GetInt("Parameter.UnityChanType");

		MidiMaster.noteOnDelegate += NoteOn;
		MidiMaster.noteOffDelegate += NoteOff;
		MidiMaster.knobDelegate += knobChanged;
		MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMidiIn += MIDIIn;
		midiWatcher.onLyricIn += LyricIn;
		midiWatcher.onTempoIn += TempoIn;
		midiWatcher.onBeatIn += BeatIn;
		midiWatcher.onMeasureIn += MeasureIn;
	}
	void OnDestroy() {
		MidiMaster.noteOnDelegate -= NoteOn;
		MidiMaster.noteOffDelegate -= NoteOff;
		MidiMaster.knobDelegate -= knobChanged;
		// Debug.Log("Destract");
	}
	public void BackupParams() {
		// PlayerPrefs.SetInt("LyricMode", (int)LyricMode.Kanji);
		// PlayerPrefs.SetInt("ParticleType", (int)particleType);
		// PlayerPrefs.SetInt("UnityChanType", (int)unityChanType);
	}
	void Start() {
		GameObject mainObj = GameObject.Find("MainGameObject");
		sentenceList = mainObj.GetComponent<SentenceList>();
	}
	// Update is called once per frame
	void Update() {
		if (!beatEffectApplied) {
			for (var track = 0; track < sentenceList.GetNumOfTrack(); track++) {
				if (sentenceList.IsActive(track, 1)) {
					LyricData lyricData = sentenceList.GetSentence(track, newMeasure, 1);
					foreach (string control in lyricData.beats[curBeat].controls) {
						ApplyControlDelayed(control);
					}
				}
			}
			beatEffectApplied = false;
		}
	}

	public void SetSMFPlayer(SMFPlayer player, SMFPlayer _kanjiPlayer) {
		smfPlayer = player;
		kanjiPlayer = _kanjiPlayer;
		eventMap = GetComponent<MidiEventMapAccessor>();
		eventMap.Init(smfPlayer, kanjiPlayer);
		eventMap.SetCurrentMap(1);
		kanjiPlayer.mute = false;
		ChangeParticle(particleType);
	}
	public void SetTitle(String title) {
		titleCenter.text = title;
	}

	public void MIDIIn(int track, byte[] midiEvent, float position, uint currentMsec) {
		byte status = (byte)(midiEvent[0] & 0xF0);
		int ch = (status & 0xF0) >> 4;
		switch (status) {
		case 0x90:
			if (midiEvent[2] == 0) {
				// Debug.Log($"NoteOff, position: {position}");
			} else {
				// Debug.Log($"NoteOn, position: {position}");
			}
			break;
		case 0x80:
			// Debug.Log($"NoteOff, position: {position}");
			break;
		default:
			// Debug.Log($"MIDIData Status = {status}");
			break;
		}
	}

	public void LyricIn(int track, string lyric, float position, uint currentMsec) {
		// Debug.Log($"lyric: {lyric}, position: {position}");
	}
	public void TempoIn(float msecPerQuaterNote, uint tempo, uint currentMsec) {
	}

	public void BeatIn(int numerator, int denominator, uint currentMsec) {
		curBeat = numerator;
		beatEffectApplied = false;
		if (numerator == 0) return;  // Beat0のcontrolはMeasureInで適用済
		for (var track = 0; track < sentenceList.GetNumOfTrack(); track++) {
			if (sentenceList.IsActive(track, 1)) {
				LyricData lyricData = sentenceList.GetSentence(track, curMeasure, 1);
				foreach (string control in lyricData.beats[numerator].controls) {
					ApplyControlNow(control);
				}
			}
		}
	}

	public void MeasureIn(int measure, int measureInterval, uint currentMsec) {
		beatInterval = (float)measureInterval / (float)kanjiPlayer.beat.count / 1000f;
		this.measureInterval = (float)measureInterval / 1000f;
		if (particleMeasCount > 0) {
			particleMeasCount--;
			if (particleMeasCount == 0) {
				ChangeParticle(Parameter.ParticleType.Off);
			}
		}
		// beat0のcontrolはMeasureInで適用する
		for (var track = 0; track < sentenceList.GetNumOfTrack(); track++) {
			if (sentenceList.IsActive(track, 1)) {
				LyricData lyricData = sentenceList.GetSentence(track, measure, 1);
				foreach (string control in lyricData.beats[0].controls) {
					ApplyControlNow(control);
				}
			}
		}
		newMeasure = measure;
		curMeasure = measure;
	}

	private void ChangeParticle(Parameter.ParticleType type) {
		particleController.Stop();
		zeknovaController.Stop();
		if (type != particleType) {
			particleMeasCount = 2;
			switch (type) {
			case Parameter.ParticleType.Snow:
				particleController.Play(ParticleController.Type.Snow);
				break;
			case Parameter.ParticleType.Confetti:
				particleController.Play(ParticleController.Type.Confetti);
				break;
			case Parameter.ParticleType.Sakura:
				// not yet
				break;
			case Parameter.ParticleType.Zeknova:
				zeknovaController.Play();
				break;
			case Parameter.ParticleType.Ramen:
				particleController.Play(ParticleController.Type.Ramen);
				break;
			default:
				break;
			}
			particleType = type;
		} else {
			particleType = Parameter.ParticleType.Off;
		}
	}
	private void SetUnityChan(Parameter.UnityChanType type) {
		switch (type) {
		case Parameter.UnityChanType.Black:
			unityChanColor.SetActive(false);
			unityChanBlack.SetActive(true);
			break;
		case Parameter.UnityChanType.Color:
			unityChanBlack.SetActive(false);
			unityChanColor.SetActive(true);
			break;
		case Parameter.UnityChanType.Off:
			unityChanBlack.SetActive(false);
			unityChanColor.SetActive(false);
			break;
		default:
			break;
		}
	}

	//
	//	Measureイベントですぐにactiveにする
	//	Measureイベントを受けて処理を行うタイプに使用する
	//
	private void ApplyControlNow(string command) {
		string[] args = command.Split("_");
		switch (args[0]) {
		case "SimpleLyricOn":
			simpleLyricGen.active = true;
			break;
		case "SimpleLyricOff":
			simpleLyricGen.active = false;
			break;
		case "SimpleLyricUpOn":
			simpleLyricGenUp.active = true;
			break;
		case "SimpleLyricUpOff":
			simpleLyricGenUp.active = false;
			break;
		case "MultiLine":
		case "MultiRow":
			if (args.Length < 2) break;
			LyricGenMultiLine multiLine;
			if (args[1] == "L") {
				multiLine = multiLineL;
			} else if (args[1] == "R") {
				multiLine = multiLineR;
			} else if (args[1] == "L1") {
				multiLine = multiLineL1;
			} else if (args[1] == "R1") {
				multiLine = multiLineR1;
			} else {
				multiLineL.SetActive(false);
				multiLineR.SetActive(false);
				multiLineL1.SetActive(false);
				multiLineR1.SetActive(false);
				break;
			}
			if (args[0] == "MultiRow") {
				multiLine.vertical = true;
			} else if (args[2] == "Off") {
				multiLine.vertical = false;
			}
			if (args[2] == "On") {
				multiLine.SetActive(true);
			} else if (args[2] == "Off") {
				multiLine.SetActive(false);
			} else if (args[2] == "Clear") {
				multiLine.Clear();
			}
			break;
		case "RamenDiskOn":
			ramenDisk.SetActive(true);
			break;
		case "RamenDiskOff":
			ramenDisk.SetActive(false);
			break;
		case "RamenFloorOn":
			ramenFloor.SetActive(true);
			break;
		case "RamenFloorOff":
			ramenFloor.SetActive(false);
			break;
		case "WallRect":
			backGroundController.SetWallType(Parameter.WallType.Rectangle);
			break;
		case "WallCircle":
			backGroundController.SetWallType(Parameter.WallType.Circle);
			break;
		case "WallOff":
			backGroundController.SetWallType(Parameter.WallType.Off);
			break;
		case "UCBlack":
			SetUnityChan(Parameter.UnityChanType.Black);
			break;
		case "UCColor":
			SetUnityChan(Parameter.UnityChanType.Color);
			break;
		case "UCOff":
			SetUnityChan(Parameter.UnityChanType.Off);
			break;
		case "WaveFormOn":
			wave.SetActive(true);
			break;
		case "WaveFormOff":
			wave.SetActive(false);
			break;
		case "Effect":
			effectSwitcher.ChangeEffect(args, beatInterval);
			break;
		case "BlackOut":
			if (args.Length < 2) break;
			if (args[1] == "On") {
				blackOut.SetActive(true);
			} else if (args[1] == "Off") {
				blackOut.SetActive(false);
			}
			break;
		case "Bulb":
			bulb.Create(new Vector3(2.6f, 6, 0), beatInterval * 2);
			break;
		case "Rocket":
			rocket.Launch();
			break;
		case "Dango":
			icons.Create("Dango", measureInterval * 2);
			break;
		case "Cake":
			icons.Create("Cake", measureInterval * 2);
			break;
		case "Donburi":
			icons.Create("Donburi", measureInterval * 2);
			break;
		case "ShootingStar":
			shootingStar.Trigger("ShootingStar", 12, 0.3f, false);
			break;
		case "Comet":
			shootingStar.Trigger("Comet", 12, 0.3f, false);
			break;
		case "NarutoStar":
			shootingStar.Trigger("Naruto", 12, 0.1f, true);
			break;
		case "Naruto":
			naruto.Begin(beatInterval * 2);
			break;
		case "UFO":
			ufo.Create(measureInterval * 2);
			break;
		}
	}
	private void ApplyControlDelayed(string command) {
		string[] args = command.Split("_");
		switch (args[0]) {
		case "TitleCenterOn":
			titleCenter.enabled = true;
			break;
		case "TitleCenterOff":
			titleCenter.enabled = false;
			break;
		case "RamenCupAuto":
			ramenController.CreateRamen();
			break;
		case "Snow":
			ChangeParticle(Parameter.ParticleType.Snow);
			break;
		case "Confetti":
			ChangeParticle(Parameter.ParticleType.Confetti);
			break;
		case "Sakura":
			ChangeParticle(Parameter.ParticleType.Sakura);
			break;
		case "Zeknova":
			ChangeParticle(Parameter.ParticleType.Zeknova);
			break;
		case "Ramen":
			ChangeParticle(Parameter.ParticleType.Ramen);
			break;
		case "SimpleLyricOn":
			simpleLyricGen.active = true;
			break;
		case "SimpleLyricOff":
			simpleLyricGen.active = false;
			break;
		case "SimpleLyricUpOn":
			simpleLyricGenUp.active = true;
			break;
		case "SimpleLyricUpOff":
			simpleLyricGenUp.active = false;
			break;
		case "Sun":
			if (args.Length >= 2) sun.SetCommand(args[1]);
			break;
		case "UnityChanRunOn":
			unityChanRunning.SetActive(true);
			break;
		case "UnityChanRunOff":
			unityChanRunning.SetActive(false);
			break;
		case "UnityChanShiftOn":
			unityChanShift.SetActive(true);
			break;
		case "UnityChanShiftOff":
			unityChanShift.SetActive(false);
			break;
		case "Night":
			if (args.Length < 2) break;
			if (args[1] == "On") {
				night.SetActive(true);
			} else if (args[1] == "Off") {
				night.SetActive(false);
			}
			break;
		default:
			break;
		}
	}

	private void NoteOn(MidiChannel channel, int note, float velocity) {
		string paramText = "";
		if (note == Parameter.NoteParticleSnow) {
			ChangeParticle(Parameter.ParticleType.Snow);
		} else if (note == Parameter.NoteParticleConfetti) {
			ChangeParticle(Parameter.ParticleType.Confetti);
		} else if (note == Parameter.NoteParticleKiraKira) {
			ChangeParticle(Parameter.ParticleType.Zeknova);
		} else if (note == Parameter.NoteUnityChanBlack) {
			SetUnityChan(Parameter.UnityChanType.Black);
		} else if (note == Parameter.NoteUnityChanColor) {
			SetUnityChan(Parameter.UnityChanType.Color);
		} else if (note == Parameter.NoteUnityChanOff) {
			SetUnityChan(Parameter.UnityChanType.Off);
		} else if (note == Parameter.NoteComet) {
			shootingStar.Trigger("Comet", 6, 0.3f, false);
		} else if (note == Parameter.NoteShootingStar) {
			shootingStar.Trigger("ShootingStar", 6, 0.3f, false);
		} else if (note == Parameter.NoteUFO) {
			ufo.Create(measureInterval * 2);
		} else if (note == Parameter.NoteRocketLaunch) {
			rocket.Launch();
		} else if (note == Parameter.NoteNaruto) {
			naruto.Begin(beatInterval * 2);
		} else if (note == Parameter.NoteSongDown || note == Parameter.NoteSongUp) {
			showInfo = !showInfo;
			infoPanel.SetActive(showInfo);
		}
		if (paramText.Length != 0) {
			uiPanelControl.Show(paramText, measureInterval);
		}
	}

	private void NoteOff(MidiChannel channel, int note) {
		// Debug.Log("NoteOff: " + channel + ", " + note);
	}

	private void knobChanged(MidiChannel ch, int ccNum, float value) {
		string paramText = "";
		switch (ccNum) {
		case Parameter.CCSetFont:
			FontResource.Type fontType = (FontResource.Type)((FontResource.Instance.numOfFontType() - 1) * value);
			FontResource.Instance.SetCurFont(fontType);
			paramText = fontType.ToString();
			break;
		case Parameter.CCRGBShiftAmount:
		case Parameter.CCRGBShiftAngle:
			paramText = effectSwitcher.ChangeParameter((int)ch, ccNum, value, 2 * measureInterval);
			break;
		case Parameter.CCEffectSelect:
			paramText = effectSwitcher.ChangeParameter((int)ch, ccNum, value, 3 * measureInterval);
			break;
		case Parameter.CCRamenRotate:
			paramText = "Rotate Speed";
			rotateContentsZ.ChangeRotationTime(value);
			rotateContentsZ1.ChangeRotationTime(value);
			break;
		default:
			break;
		}
		if (paramText.Length != 0) {
			uiPanelControl.Show(paramText, measureInterval);
		}
	}
}
