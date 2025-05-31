///
///	Parameter.cs
/// Copyright (c) 2025 gotojo, All Rights Reserved.

using UnityEngine;

// [CreateAssetMenu(menuName = "ParameterData")]
// public class Parameter : ScriptableObject
public class Parameter
{
	public static int NoteLyricTypeDown = 0x30;
		public static int NoteLyricModeOriginal = 0x31;
	public static int NoteLyricTypeUp = 0x32;
		public static int NoteLyricModeKanji = 0x33;
	public static int NoteLyricFontDown = 0x34;
	public static int NoteLyricFontUp = 0x35;
		public static int NoteParticleSnow = 0x36;
	public static int NoteRamenDown = 0x37;
		public static int NoteParticleConfetti = 0x38;
	public static int NoteRamenUp = 0x39;
		public static int NoteParticleKiraKira = 0x3A;
	public static int Note3B = 0x3B;

	public static int NoteWallTypeRect = 0x3C;
		public static int NoteUnityChanBlack = 0x3D;
	public static int NoteWallTypeCircle = 0x3E;
		public static int NoteUnityChanColor = 0x3F;
	public static int Note40 = 0x40;
	public static int Note41 = 0x41;
		public static int Note42 = 0x42;
	public static int Note43 = 0x43;
		public static int NoteSongDown = 0x44;
	public static int Note45 = 0x45;
		public static int NoteSongUp = 0x46;
	public static int NoteStopVideo = 0x47;
	public static int NoteStartVideo = 0x48;

	public string [,] sentence;

	public enum WallType {
		Rectangle,
		Circle
	}
	public enum ParticleType {
		Off,
		Snow,
		Confetti,
		Sakura,
		Zeknova,
	}
	public enum UnityChanType {
		Off,
		Black,
		Color
	}
}

