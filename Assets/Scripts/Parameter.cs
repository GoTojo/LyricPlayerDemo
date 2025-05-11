///
///	Parameter.cs
/// Copyright (c) 2025 gotojo, All Rights Reserved.

using UnityEngine;

[CreateAssetMenu(menuName = "ParameterData")]
public class Parameter : ScriptableObject
{
    public int NoteSongTabeyo = 0x3D;
    public int NoteSongMadakana = 0x3F;
    public int NoteSongHaruka = 0x42;
    public int NoteSongSanpun = 0x44;
    public int NoteSongYakusoku = 0x46;
    public int NoteStopVideo = 0x47;
    public int NoteStartVideo = 0x48;
}