/// SMFPlayer.cs
/// SMF with lyrics player for C#
/// Copyright (c) 2025 gotojo, All Rights Reserved.

using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class SMFPlayer
{
	private UInt16 format = 0;
	public UInt32 tempo = 120;
	public UInt32 tpqn = 96;
	public float usecPerQuarterNote = 60000000 / 120;
	public bool isValid = false;
	public bool mute = false;
	private UInt16 nTracks = 0;
	private List<TrackData> tracks = new List<TrackData>();
	private List<TrackPlayer> players = new List<TrackPlayer>();
	public MIDIHandler midiHandler = new MIDIHandler();
	private bool playing = false;
	private bool isEnd = false;
	public struct Beat
	{
		public int unit;
		public int count;
	};
	public Beat beat;
	public int numOfMeasure = 0;
	public int numOfTrack = 0;
	private Stopwatch stopWatch = new Stopwatch();
	private UInt32 nextEventTime = 0;
	private UInt32 startTime = 0;
	private UInt32 lastMeasTime = 0;

	public static UInt32 BEReader(BinaryReader reader, int len)
	{
		UInt32 value = 0;
		for (int i = 0; i < len; i++) {
			byte data = reader.ReadByte();
			value <<= 8;
			value += data;
		}
		return value;
	}
	public SMFPlayer(string filepath, int totalMeasure = -1)
	{
		if (string.IsNullOrEmpty(filepath)) {
			UnityEngine.Debug.Log("File path is null or empty.");
			return;
		}
		if (filepath.IndexOfAny(Path.GetInvalidPathChars()) >= 0) {
			UnityEngine.Debug.Log("Invalid characters found in filepath.");
		}
		if (!File.Exists(filepath)) {
			UnityEngine.Debug.Log("File does not exist: " + filepath);
			return;
		}
		isValid = true;
		playing = false;
		isEnd = false;
		tracks.Clear();
		players.Clear();
		// UnityEngine.Debug.Log("Loading SMF: " + filepath);
		// UnityEngine.Debug.Log(filepath);
		using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
		{
			BinaryReader reader = new BinaryReader(fs);
			isValid = ParseChunk(reader, totalMeasure);
			foreach (TrackData track in tracks)
			{
				TrackPlayer player = new TrackPlayer(track, this);
				players.Add(player);
			}
		}
		numOfTrack = tracks.Count;
		// UnityEngine.Debug.Log("complete parsing SMF");
	}
	public void Reset()
	{
		beat.count = 4;
		beat.unit = 4; // default is 4 / 4
		nextEventTime = 0;
		startTime = 0;
		lastMeasTime = 0;
		isEnd = false;
		foreach (TrackPlayer player in players){
			player.Reset();
		}
		midiHandler.EventIn(MIDIHandler.Event.Reset);
	}
	public bool isPlaying()
	{
		return this.playing;
	}
	private UInt32 tickup()
	{
		UInt32 nexttime = UInt32.MaxValue;
		foreach (TrackPlayer player in players) {
			if (player.isEnd) {
				continue;
			}
			UInt32 _nexttime = player.Tickup();
			if (_nexttime < nexttime) {
				nexttime = _nexttime;
			}
		}
		// isEnd = true;
		return nexttime;
	}

	public bool Start(int _startTime = -1)
	{
		// UnityEngine.Debug.Log("Play Start");
		if (!isValid) {
			return false;
		}
		playing = true;
		midiHandler.EventIn(MIDIHandler.Event.Start);
		Reset();
		stopWatch.Start();
		if (_startTime < 0) {
			startTime = (UInt32)stopWatch.ElapsedMilliseconds;
		} else {
			startTime = (UInt32)_startTime;
		}
		UInt32 nexttime = tickup();
		if (nexttime == UInt32.MaxValue) {
			playing = false;
		}
		UInt32 nextEventTime = nexttime + startTime;
		// UnityEngine.Debug.Log($"nextEventTime: {nextEventTime}");
		return playing;
	}

	public bool Update(UInt32 currentTime = 0)
	{
		if (playing == false) {
			return false;
		}
		if (currentTime == 0) {
			currentTime = (UInt32)stopWatch.ElapsedMilliseconds;
		}
		// UnityEngine.Debug.Log($"currentTime: {currentTime}");
		while (currentTime >= nextEventTime) {
			UInt32 nexttime = tickup();
			if (nexttime == UInt32.MaxValue) {
				Stop();
				break;
			}
			nextEventTime = nexttime + startTime;
			// UnityEngine.Debug.Log($"currentTime: {currentTime}, nextEventTime: {nextEventTime}");
		}
		return playing;
	}

	public bool Stop()
	{
		if (!isValid) {
			return false;
		}
		stopWatch.Stop();
		playing = false;
		midiHandler.EventIn(isEnd ? MIDIHandler.Event.End : MIDIHandler.Event.Stop);
		return true;
	}

	public float GetMsecForMeasure()
	{
		float msecForMeasure = beat.count * usecPerQuarterNote * 4 / beat.unit / 1000;
		// UnityEngine.Debug.Log($"GetMsecForMeasure: {msecForMeasure}");
		return msecForMeasure;
	}

	public float GetPosition(UInt32 msec)
	{
		float current = (float)((msec > lastMeasTime) ? msec - lastMeasTime : 0);
		float msecForMeasure = GetMsecForMeasure();
		// UnityEngine.Debug.Log($"GetPosition: lastMeas: {lastMeasTime}, msec: {msec}, position: {current / msecForMeasure}");
		float position = current / msecForMeasure;
		return (position < 1.0f) ? position : 1.0f;
	}

	private bool ParseChunk(BinaryReader reader, int totalMeasure = -1) 
	{
		char[] type = new char[4];
		int trackid = 0;
		do {
			reader.Read(type, 0, 4);
			string typeString = new string(type);
			switch (typeString) {
			case "MThd":
				if (!ParseHeader(reader)) {
					return false;
				}
				break;
			case "MTrk":
				// UnityEngine.Debug.Log("Track Chunk");
				TrackParser parser = new TrackParser(trackid, reader, this);
				if (parser.isValid) {
					tracks.Add(parser);
					trackid++;
				}
				break;
			default:
				// UnityEngine.Debug.Log("Unknown Chunk");
				UInt32 size = BEReader(reader, 4);
				reader.BaseStream.Seek(size, SeekOrigin.Current);
				break;
			}
		} while (reader.BaseStream.Position < reader.BaseStream.Length);
		BeatTrack beatTrack = new BeatTrack(trackid, tracks, this, totalMeasure);
		tracks.Insert(0, beatTrack);
		numOfMeasure = beatTrack.numOfMeasure;
		// UnityEngine.Debug.Log($"numOfMeasure: {numOfMeasure}");
		trackid++;
		return true;
	}

	private bool ParseHeader(BinaryReader reader)
	{
		UInt32 size = BEReader(reader, 4);
		if (size != 6) {
			return false;
		}
		format = (UInt16)BEReader(reader, 2);
		nTracks = (UInt16)BEReader(reader, 2);
		tpqn = (UInt16)BEReader(reader, 2);
		return true;
	}

	class TrackData {
		protected int id;
		private SMFPlayer player;
		protected bool isEnd;
		public struct MIDIEvent
		{
			public UInt32 deltaTime;
			public byte[] data;
			public UInt32 msec;
		};
		protected List<MIDIEvent> midiEvents = new List<MIDIEvent>();
		private UInt32 currentEventIndex = 0;
		private UInt32 currentTime = 0;

		public TrackData(int trackid, SMFPlayer player)
		{
			id = trackid;
			this.player = player;
			Reset();
		}
		public void Clear()
		{
			Reset();
			midiEvents.Clear();
		}
		public bool Add(UInt32 deltaTime, byte[] data)
		{
			UInt32 deltaMSec = (UInt32)(deltaTime * player.usecPerQuarterNote / player.tpqn / 1000);
			MIDIEvent midiEvent = new MIDIEvent();
			if (deltaMSec >= (UInt32.MaxValue - currentTime)) {
				midiEvent.deltaTime = UInt32.MaxValue;
				midiEvent.msec = UInt32.MaxValue;
				midiEvents.Add(midiEvent);
				return false;
			}
			midiEvent.deltaTime = deltaTime;
			midiEvent.data = data;
			currentTime += deltaMSec;
			midiEvent.msec = currentTime;
			midiEvents.Add(midiEvent);
			return true;
		}
		public virtual void Reset() {
			currentEventIndex = 0;
			currentTime = 0;
			isEnd = false;
		}
		public bool Next() {
			UInt32 index = currentEventIndex + 1;
			if (index >= midiEvents.Count) {
				isEnd = true;
				return false;
			}
			currentEventIndex = index;
			return true;
		}
		public UInt32 GetDeltaTime()
		{
			if (midiEvents.Count <= currentEventIndex) {
				return UInt32.MaxValue;
			}
			return midiEvents[(int)currentEventIndex].deltaTime;
		}
		public byte[] GetData()
		{
			if (midiEvents.Count <= currentEventIndex) {
				byte[] dummy = new byte[0];
				return dummy;
			}
			return midiEvents[(int)currentEventIndex].data;
		}
		public UInt32 GetMsec()
		{
			if (midiEvents.Count <= currentEventIndex) {
				return UInt32.MaxValue;
			}
			return midiEvents[(int)currentEventIndex].msec;
		}
		public bool IsEnd()
		{
			return isEnd;
		}

		public virtual void DoEvent()
		{

		}
	};

	class TrackParser : TrackData {
		public bool isValid = true;
		private SMFPlayer smfPlayer;
		private byte runningStatus = 0;
		public TrackParser(int trackid, BinaryReader reader, SMFPlayer player, int totalMeasure = -1) : base(trackid, player) {
			smfPlayer = player;
			runningStatus = 0;
			UInt32 size = SMFPlayer.BEReader(reader, 4);
			if (size == 0) {
				isValid = false;
				return;
			}
			Clear();
			byte[] data = new byte[size];
			reader.Read(data, 0, (int)size);
			long endPosition = reader.BaseStream.Position + size;
			BinaryReader bufferReader = new BinaryReader(new MemoryStream(data));
			ParseBody(bufferReader, endPosition, totalMeasure);
		}
		private void ParseBody(BinaryReader reader, long endPosition, int totalMeasure = -1) {
			while (reader.BaseStream.Position < endPosition) {
				UInt32 deltaTime = ParseDeltaTime(reader);
				// UnityEngine.Debug.Log($"deltaTime: {deltaTime}");
				byte[] eventData = ParseEvent(reader);
				if (!Add(deltaTime, eventData)) {
					break;
				}
				if (isEnd) {
					break;
				}
			}
		}
		private UInt32 ParseDeltaTime(BinaryReader reader)
		{
			return ParseVariableLength(reader);
		}
		private UInt32 ParseVariableLength(BinaryReader reader)
		{
			UInt32 value = 0;
			byte b;
			do {
				b = reader.ReadByte();
				value = (value << 7) | (UInt32)(b & 0x7F);
			} while ((b & 0x80) != 0);
			return value;
		}
		private byte[] ParseEvent(BinaryReader reader)
		{
			byte status = reader.ReadByte();
			if (status < 0x80) {
				// Running Status
				reader.BaseStream.Seek(-1, SeekOrigin.Current);
				status = runningStatus;
			} else {
				runningStatus = status;
			}
			switch (status) {
				case 0xFF:
					// Meta Event
					byte metaType = reader.ReadByte();
					UInt32 size = ParseVariableLength(reader);
					byte[] metaData = new byte[size + 2];
					metaData[0] = status;
					metaData[1] = metaType;
					reader.Read(metaData, 2, (int)size);
					if (metaType == 0x2F) {
						// End of Track
						isEnd = true;
					} else if (metaType == 0x51) {
						UInt32 usecPerQuarterNote = (UInt32)(metaData[2] << 16 | metaData[3] << 8 | metaData[4]);
						usecPerQuarterNote &= 0x00FFFFFF;
						smfPlayer.tempo = 60000000 / usecPerQuarterNote;
						smfPlayer.usecPerQuarterNote = (float)usecPerQuarterNote;
						// UnityEngine.Debug.Log("Tempo: " + smfPlayer.tempo);
					} else if (metaType == 0x58) {
						// Time Signature
						smfPlayer.beat.count = metaData[2];
						smfPlayer.beat.unit = 2 ^ metaData[3];
						// UnityEngine.Debug.Log($"Beat:  {smfPlayer.beat.count} / {smfPlayer.beat.unit}");
					}
					return metaData;
				case 0xF0:
				case 0xF7:
					// SysEx Event
					UInt32 sysexSize = ParseVariableLength(reader);
					byte[] sysexData = new byte[sysexSize + 1];
					sysexData[0] = status;
					reader.Read(sysexData, 1, (int)sysexSize);
					return sysexData;
				case 0x80:
				case 0x90:
				case 0xE0:
					// 3byte events
					byte[] data3 = new byte[3];
					data3[0] = status;
					data3[1] = reader.ReadByte();
					data3[2] = reader.ReadByte();
					return data3;
				case 0xA0:
				case 0xB0:
				case 0xC0:
				case 0xD0:
					// 2byte events
					byte[] data2 = new byte[2];
					data2[0] = status;
					data2[1] = reader.ReadByte();
					return data2;
				default:
					// Unknown event
					// UnityEngine.Debug.Log("Unknown Event");
					byte[] unknownData = new byte[1];
					unknownData[0] = status;
					return unknownData;
			}
		}
		private string GetMetaText(byte[] data)
		{
			String text = System.Text.Encoding.UTF8.GetString(data, 2, data.Length - 2);
			return text;
		}
		public override void DoEvent()
		{
			byte[] data = GetData();
			if (data[0] == 0xFF) {
				// Meta Event
				switch (data[1]) {
					case 0x51:
						// Set Tempo
						UInt32 usecPerQuarterNote = (UInt32)(data[2] << 16 | data[3] << 8 | data[4]);
						usecPerQuarterNote &= 0x00FFFFFF;
						smfPlayer.tempo = 60000000 / usecPerQuarterNote;
						smfPlayer.usecPerQuarterNote = usecPerQuarterNote;
						if (!smfPlayer.mute) {
							smfPlayer.midiHandler?.TempoIn(usecPerQuarterNote / 1000, smfPlayer.tempo, GetMsec());
						}
						// UnityEngine.Debug.Log("Tempo: " + smfPlayer.tempo);
						break;
					case 0x58:
						// Time Signature
						smfPlayer.beat.count = data[2];
						smfPlayer.beat.unit = 2 ^ data[3];
						// UnityEngine.Debug.Log($"Beat:  {smfPlayer.beat.count} / {smfPlayer.beat.unit}");
						break;
					case 0x2F:
						// End of Track
						// UnityEngine.Debug.Log("End of Track");
						isEnd = true;
						break;
					case 0x5:
						//Lyric Event
						if (!smfPlayer.mute) {
							smfPlayer.midiHandler?.LyricIn(id, GetMetaText(data), smfPlayer.GetPosition(GetMsec()), GetMsec());
						}
						break;
					default:
						// UnityEngine.Debug.Log("Meta Event: " + data[1]);
						break;
				}
			} else {
				// MIDI Event
				// UnityEngine.Debug.Log("MIDI Event: " + data[0]);
				if (!smfPlayer.mute) {
					smfPlayer.midiHandler?.MIDIIn(id, data, smfPlayer.GetPosition(GetMsec()), GetMsec());
				}
			}
		}
	};

	class BeatTrack : TrackData {
		private struct Beat
		{
			public int unit;
			public int count;
		};
		private Beat beat;
		private SMFPlayer player;
		private int ticksForMeasure;
		private int ticksForBeat;
		private int counterForMeasure;
		private int counterForBeat;
		private int currentBeat;
		private int currentMeasure;
		private const byte typeBeat = 0;
		private const byte typeMeasure = 1;
		private const byte typeTimeSignature = 2;
		public int numOfMeasure = 0;
		public BeatTrack(int id, List<TrackData> trackData, SMFPlayer player, int totalMeasure = -1) : base(id, player) {
			this.player = player;
			UInt32 currentTick = 0;
			UInt32[] nextEventTick = new UInt32[trackData.Count];
			beat.unit = 4;
			beat.count = 4;
			ticksForBeat = (int)player.tpqn * 4 / 4;
			ticksForMeasure = ticksForBeat * 4;
			for (int i = 0; i < trackData.Count; i++) {
				trackData[i].Reset();
				nextEventTick[i] = trackData[i].GetDeltaTime();
			}
			Reset();
			AddMeasure(0);
			bool allIsEnd = true;
			do {
				allIsEnd = true;
				for (int i = 0; i < trackData.Count; i++) {
					if (trackData[i].IsEnd()) {
						continue;
					}
					allIsEnd = false;
					while (nextEventTick[i] == currentTick) {
						ParseData(trackData[i].GetData());
						if (!trackData[i].Next()) {
							break;
						}
						nextEventTick[i] += trackData[i].GetDeltaTime();
					}
				}
				CheckBeat();
				currentTick++;
			} while (!allIsEnd);
			{
				byte[] data = midiEvents[midiEvents.Count - 1].data;
				if (data[0] == typeBeat && data[1] == 0) {
					// 最後のデータがMeasureとBeat0の場合は削除する　（EndOfTrackで作られたと思われる)
					midiEvents.RemoveAt(midiEvents.Count - 1);
					midiEvents.RemoveAt(midiEvents.Count - 1);
					currentMeasure--;
				}
			}
			int realMeasure = currentMeasure;
			if (currentMeasure < totalMeasure) {
				byte[] data;
				int lastMeasureIndex = midiEvents.Count - 1;
				while (lastMeasureIndex >= 0) {
					data = midiEvents[lastMeasureIndex].data;
					if (data[0] == typeMeasure) break;
					lastMeasureIndex--;
				}
				int numOfEvent = midiEvents.Count - lastMeasureIndex;
				MIDIEvent[] measEvents = new MIDIEvent[numOfEvent];
				midiEvents.CopyTo(lastMeasureIndex, measEvents, 0, numOfEvent);
				for (; currentMeasure <= totalMeasure; currentMeasure++) {
					for (var i = 0; i < measEvents.Length; i++) {
						byte[] newData = new byte[measEvents[i].data.Length];
						Array.Copy(measEvents[i].data, newData, measEvents[i].data.Length);
						if (newData[0] == typeMeasure) {
							newData[1] = (byte)currentMeasure;
						}
						Add(measEvents[i].deltaTime, newData);
					}
				}
			}
			numOfMeasure = currentMeasure;
			// UnityEngine.Debug.Log("---- Show Final Events ----");
			// Reset();
			// while (!IsEnd()) {
			// 	string type;
			// 	switch (GetData()[0]) {
			// 	case typeBeat:
			// 		type = "Beat";
			// 		break;
			// 	case typeMeasure:
			// 		type = $"Measure";
			// 		break;
			// 	case typeTimeSignature:
			// 		type = "timeSignature";
			// 		break;
			// 	default:
			// 		type = "Unknown";
			// 		break;
			// 	}
			// 	UnityEngine.Debug.Log($"deltaTime: {GetDeltaTime()}, data: {type}, {GetData()[1]}, msec: {GetMsec()}");
			// 	if (!Next()) {
			// 		break;
			// 	}
			// }
			// UnityEngine.Debug.Log($"realMeasure: {realMeasure}, numOfMeasure: {numOfMeasure}");
		}
		public override void Reset()
		{
			currentMeasure = 0;
			currentBeat = 0;
			base.Reset();
		}
		private void CheckBeat()
		{
			counterForMeasure--;
			counterForBeat--;
			if (counterForMeasure == 0) {
				AddMeasure(ticksForBeat);
			} else if (counterForBeat == 0) {
				AddBeat(ticksForBeat);
			}
		}
		private void AddBeat(int deltaTime)
		{
			byte [] data = new byte [2];
			data[0] = typeBeat;
			data[1] = (byte)currentBeat;
			currentBeat++;
			Add((UInt32)deltaTime, data);
			counterForBeat = ticksForBeat;
		}
		private void AddMeasure(int deltaTime) {
			byte[] data = new byte[2];
			data[0] = typeMeasure;
			data[1] = (byte)currentMeasure;
			currentMeasure++;
			currentBeat = 0;
			Add((uint)deltaTime, data);
			AddBeat(0);
			counterForMeasure = ticksForMeasure;
		}
		private void SetBeat(int unit, int count)
		{
			beat.unit = unit;
			beat.count = count;
			ticksForBeat = (int)player.tpqn * 4 / unit;
			ticksForMeasure = ticksForBeat * beat.count;

			byte [] data = new byte [3];
			data[0] = typeTimeSignature;
			data[1] = (byte)unit;
			data[2] = (byte)count;
			Add(0, data);			
			AddMeasure(counterForBeat);
		}
		private void ParseData(byte[] data)	
		{
			if (data[0] == 0xFF) {
				// Meta Event
				if (data[1] == 0x58) {
					// Time Signature
					SetBeat(2 ^ data[3], data[2]);
				} 
			}
		}
		public override void DoEvent()
		{
			byte[] data = GetData();
			switch (data[0]) {
			case typeBeat:
				if (!player.mute) {
					player.midiHandler?.BeatIn(data[1], beat.unit, GetMsec());
				}
				currentBeat++;
				if (currentBeat >= beat.count) {
					currentBeat = 0;
				}
				break;
			case typeMeasure:
				player.lastMeasTime = GetMsec();
				if (!player.mute) {
					player.midiHandler?.MeasureIn(data[1], (int)player.GetMsecForMeasure(), GetMsec());
				}
				currentMeasure++;
				break;
			case typeTimeSignature:
				if (data.Length > 3) {
					beat.unit = data[1];
					beat.count = data[2];
				}
				currentBeat = 0;
				break;
			default:
				break;
			}
		} 
	}

	class TrackPlayer {
		public bool isEnd = false;
		private TrackData midiEvents;
		private SMFPlayer smfPlayer;
		private UInt32 currentTick = 0;
		private UInt32 nextEventTick = 0;
		private UInt32 nextEventMsec = 0; 
		public TrackPlayer(TrackData data, SMFPlayer player) {
			midiEvents = data;
			smfPlayer = player;
		}
		public void Reset()
		{
			midiEvents.Reset();
			isEnd = false;
			nextEventTick = midiEvents.GetDeltaTime();
			nextEventMsec = midiEvents.GetMsec();
			currentTick = 0;
		}
		public UInt32 Tickup()
		{
			if (isEnd) {
				return UInt32.MaxValue;
			}
			currentTick += 1;
			if (midiEvents.IsEnd()) {
				isEnd = true;
				return UInt32.MaxValue;
			}
			while (currentTick >= nextEventTick) {
				midiEvents.DoEvent();
				if (!midiEvents.Next()) {
					nextEventTick = UInt32.MaxValue;
					nextEventMsec = UInt32.MaxValue;
					isEnd = true;
					break;
				}
				nextEventTick = currentTick + midiEvents.GetDeltaTime();
				nextEventMsec = midiEvents.GetMsec();
				// UnityEngine.Debug.Log($"currentTick: {currentTick}, nextEventTick:{nextEventTick}, nextEventMsec:{nextEventMsec}");
			}
			return nextEventMsec;
		}
	};
}
