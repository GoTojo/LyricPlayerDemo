///
///	Parameter.cs
/// Copyright (c) 2025 gotojo, All Rights Reserved.

using UnityEngine;

// [CreateAssetMenu(menuName = "ParameterData")]
// public class Parameter : ScriptableObject
public class Parameter {
	public static int NoteRocketLaunch = 0x2F + 12; // B3
	public static int NoteUFO = 0x30 + 12; // C3
	public static int Note31 = 0x31 + 12;
	public static int NoteNaruto = 0x32 + 12;	// D3
	public static int Note33 = 0x33 + 12;
	public static int NoteShootingStar = 0x34 + 12;	// E3
	public static int NoteComet = 0x35 + 12;	// F3
	public static int Note36 = 0x36 + 12;
	public static int NoteParticleSnow = 0x37 + 12;	// G3
	public static int Note38 = 0x38 + 12;
	public static int NoteParticleConfetti = 0x39 + 12;			// A3
	public static int Note3A = 0x3A + 12;
	public static int NoteParticleKiraKira = 0x3B + 12;			// B3

	public static int NoteUnityChanOff = 0x3C + 12;	// C4
	public static int Note3D = 0x3D + 12;
	public static int NoteUnityChanBlack = 0x3E + 12;	// D4
	public static int Note3F = 0x3F + 12;
	public static int NoteUnityChanColor = 0x40 + 12;	// E4
	public static int Note41 = 0x41 + 12;	// F4
	public static int Note42 = 0x42 + 12;
	public static int Note43 = 0x43 + 12;		// G4
	public static int Note44 = 0x44 + 12;
	public static int NoteSongDown = 0x45 + 12;			// A4
	public static int Note46 = 0x46 + 12;
	public static int NoteSongUp = 0x47 + 12;		// B4
	public static int NoteStartStop = 0x48 + 12;	// C5

	public const int CCSetFont = 0x5b; 
	public const int CCEffectSelect = 0x4B; 
	public const int CCRGBShiftAmount = 0x48; 
	public const int CCRGBShiftAngle = 0x1e;
	public const int CCRamenRotate = 0x49;
 
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

