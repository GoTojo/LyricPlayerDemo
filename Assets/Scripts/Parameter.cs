///
///	Parameter.cs
/// Copyright (c) 2025 gotojo, All Rights Reserved.

using UnityEngine;

[CreateAssetMenu(menuName = "ParameterData")]
public class Parameter : ScriptableObject
{
    public int NoteSongDown = 0x44;
    public int NoteSongUp = 0x46;
    public int NoteStopVideo = 0x47;
    public int NoteStartVideo = 0x48;
    public int NoteWallTypeRect = 0x3C;
    public int NoteWallTypeCircle = 0x3E;
    public int NoteLyricModeOriginal = 0x36;
    public int NoteLyricModeKanji = 0x38;
    public enum WallType {
        Rectangle,
        Circle
    }
}