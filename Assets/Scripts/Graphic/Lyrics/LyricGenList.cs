/// LyricGenList.cs
/// LiricGenBaseを継承したLyricGenのリスト
/// Copyright (c) 2025 gotojo

using System.Collections.Generic;

class LyricGenList {
	static public List<LyricGenBase> lyricGens = new List<LyricGenBase>();
	static public void Start(int meas) {
		Lyrics.Reset();
		foreach (var lyricGen in lyricGens) {
			lyricGen.Start(meas);
		}
	}
	static public void Clear() {
		foreach (var lyricGen in lyricGens) {
			lyricGen.Clear();
		}
	}
}