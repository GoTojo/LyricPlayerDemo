///
///	Parameter.cs
/// Copyright (c) 2025 gotojo, All Rights Reserved.

using UnityEngine;

// [CreateAssetMenu(menuName = "ParameterData")]
// public class Parameter : ScriptableObject
public class Parameter {
	public static int NoteLyricTypeDown = 0x30; // C3
	public static int NoteLyricModeToggle = 0x31;
	public static int NoteLyricTypeUp = 0x32;	// D3
	public static int Note33 = 0x33;
	public static int NoteLyricFontDown = 0x34;	// E3
	public static int NoteLyricFontUp = 0x35;	// F3
	public static int NoteParticleSnow = 0x36;
	public static int NoteParticleRamen = 0x37;	// G3
	public static int NoteParticleConfetti = 0x38;
	public static int Note39 = 0x39;			// A3
	public static int NoteParticleKiraKira = 0x3A;
	public static int Note3B = 0x3B;			// B3

	public static int NoteWallTypeRect = 0x3C;	// C4
	public static int NoteUnityChanBlack = 0x3D;
	public static int NoteWallTypeCircle = 0x3E;	// D4
	public static int NoteUnityChanColor = 0x3F;	
	public static int NoteRamenStart = 0x40;	// E4
	public static int NoteEffectOnOff = 0x41;	// F4
	public static int NoteEffectDown = 0x42;
	public static int NOteEffectUp = 0x43;		// G4
	public static int NoteSongDown = 0x44;
	public static int Note45 = 0x45;			// A4
	public static int NoteSongUp = 0x46;
	public static int NoteStopVideo = 0x47;		// B4
	public static int NoteStartVideo = 0x48;	// C5

	public const int CCRGBShiftAmount = 14; 
	public const int CCRGBShiftAngle = 15;
	public const int CCRamenRotate = 16;

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

