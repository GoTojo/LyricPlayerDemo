///
///	Parameter.cs
/// Copyright (c) 2025 gotojo, All Rights Reserved.

using UnityEngine;

// [CreateAssetMenu(menuName = "ParameterData")]
// public class Parameter : ScriptableObject
public class Parameter {
	public static int NoteLyricTypeDown = 0x30 + 12; // C3
	public static int NoteLyricModeToggle = 0x31 + 12;
	public static int NoteLyricTypeUp = 0x32 + 12;	// D3
	public static int NoteBulbOn = 0x33 + 12;
	public static int NoteLyricFontDown = 0x34 + 12;	// E3
	public static int NoteLyricFontUp = 0x35 + 12;	// F3
	public static int NoteParticleSnow = 0x36 + 12;
	public static int NoteParticleRamen = 0x37 + 12;	// G3
	public static int NoteParticleConfetti = 0x38 + 12;
	public static int NoteRocketLaunch = 0x39 + 12;			// A3
	public static int NoteParticleKiraKira = 0x3A + 12;
	public static int Note3B = 0x3B + 12;			// B3

	public static int NoteWallTypeRect = 0x3C + 12;	// C4
	public static int NoteUnityChanBlack = 0x3D + 12;
	public static int NoteWallTypeCircle = 0x3E + 12;	// D4
	public static int NoteUnityChanColor = 0x3F + 12;	
	public static int NoteRamenStart = 0x40 + 12;	// E4
	public static int NoteEffectOnOff = 0x41 + 12;	// F4
	public static int NoteEffectDown = 0x42 + 12;
	public static int NOteEffectUp = 0x43 + 12;		// G4
	public static int NoteSongDown = 0x44 + 12;
	public static int Note45 = 0x45 + 12;			// A4
	public static int NoteSongUp = 0x46 + 12;
	public static int NoteStopVideo = 0x47 + 12;		// B4
	public static int NoteStartVideo = 0x48 + 12;	// C5

	public const int CCSetFont = 0x49; 
	public const int CCRGBShiftAmount = 0x5c; 
	public const int CCRGBShiftAngle = 0x5b;
	public const int CCRamenRotate = 0x47;
 
	public string[,] sentence;

	public enum WallType {
		Off,
		Rectangle,
		Circle
	}
	public enum ParticleType {
		Off,
		Snow,
		Confetti,
		Sakura,
		Zeknova,
		Ramen,
	}
	public enum UnityChanType {
		Off,
		Black,
		Color
	}
}

