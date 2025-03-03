﻿using SimpleJSON;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TrombLoader.Class_Patches
{
    public class CustomSavedLevel : SavedLevel
	{
		//Extra track data, moved from SongData to here for a cleaner chart file
		public string trackRef;
		public string name;
		public string shortName;
		public string author;
		public string genre;
		public string description;
		public int difficulty;
		public int year;
		public int unk1;

		//Create new Custom level data from a base
		public CustomSavedLevel(SavedLevel baseChart)
		{
			this.savedleveldata = baseChart.savedleveldata;
			this.bgdata = baseChart.bgdata;
			this.lyricspos = baseChart.lyricspos;
			this.lyricstxt = baseChart.lyricstxt;
			this.endpoint = baseChart.endpoint;
			this.note_color_start = baseChart.note_color_start;
			this.note_color_end = baseChart.note_color_end;
			this.savednotespacing = baseChart.savednotespacing;

			this.timesig = baseChart.timesig;
			this.tempo = baseChart.tempo;
		}
		//Empty ctor
		public CustomSavedLevel()
		{

		}

		//Load json from path
		public CustomSavedLevel(string path)
		{
			var jsonString = File.ReadAllText(path);
			var jsonObject = JSON.Parse(jsonString);
			this.Deserialize(jsonObject);
		}

		//For debugging, saves to a file
		public JSONNode Serialize()
		{
			Plugin.LogDebug("Nyx: Serializing Chart!!");
			JSONObject jsonobject = new JSONObject();
			JSONArray asArray = jsonobject["notes"].AsArray;
			foreach (float[] array in this.savedleveldata)
			{
				JSONArray jsonarray = new JSONArray();
				foreach (float n in array)
				{
					jsonarray.Add(n);
				}
				asArray.Add(jsonarray);
			}
			jsonobject["endpoint"] = this.endpoint;
			jsonobject["savednotespacing"] = this.savednotespacing;
			jsonobject["tempo"] = this.tempo;
			jsonobject["timesig"] = this.timesig;

			jsonobject["trackRef"] = this.trackRef;
			jsonobject["name"] = this.name;
			jsonobject["shortName"] = this.shortName;
			jsonobject["author"] = this.author;
			jsonobject["genre"] = this.genre;
			jsonobject["description"] = this.description;
			jsonobject["difficulty"].AsInt = this.difficulty;
			jsonobject["year"].AsInt = this.year;
			jsonobject["UNK1"] = this.unk1;


			JSONArray lyricsArray = new JSONArray();
			int i = 0;
			foreach (var text in this.lyricstxt)
			{
				var lyricNode = new JSONObject();
				lyricNode["text"] = text;
				lyricNode["bar"] = this.lyricspos[i][0];
				//ypos is unused
				lyricsArray.Add(lyricNode);
				i++;
			}
			jsonobject["lyrics"] = lyricsArray;

			return jsonobject;
		}

		public void Deserialize(JSONNode jsonObject, bool getNotes = true)
		{

			if (getNotes) this.savedleveldata = GetNotes(jsonObject);
			this.endpoint = jsonObject["endpoint"].AsFloat;
			this.savednotespacing = jsonObject["savednotespacing"].AsInt;
			this.tempo = jsonObject["tempo"].AsFloat;
			this.timesig = jsonObject["timesig"].AsInt;

			this.trackRef = jsonObject["trackRef"];
			this.name = jsonObject["name"];
			this.shortName = jsonObject["shortName"];
			this.author = jsonObject["author"];
			this.genre = jsonObject["genre"];
			this.description = jsonObject["description"];
			this.difficulty = jsonObject["difficulty"].AsInt;
			this.year = jsonObject["year"].AsInt;
			this.unk1 = jsonObject["UNK1"];

			this.lyricstxt = new List<string>();
			this.lyricspos = new List<float[]>();

			Plugin.LogDebug("Deserializing Chart:");
			foreach (JSONObject node in jsonObject["lyrics"].AsArray)
			{
				Plugin.LogDebug(node.ToString());
				this.lyricstxt.Add(node["text"].ToString());
				var aux = new List<float>();
				aux.Add(node["bar"].AsFloat);
				aux.Add(0);
				this.lyricspos.Add(aux.ToArray());
			}

		}

		public List<float[]> GetNotes(JSONNode jsonObject)
		{
			List<float[]> notes = new List<float[]>();
			foreach (KeyValuePair<string, JSONNode> keyValuePair in jsonObject["notes"])
			{
				JSONArray currentNote = keyValuePair.Value.AsArray;
				notes.Add(new List<float>
				{
					currentNote[0].AsFloat,
					currentNote[1].AsFloat,
					currentNote[2].AsFloat,
					currentNote[3].AsFloat,
					currentNote[4].AsFloat
				}.ToArray());
			}

			return notes;
		}
	}
}