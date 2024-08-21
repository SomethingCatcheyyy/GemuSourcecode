using UnityEngine;
using System.Collections;
using System;

namespace Gemu.JSON
{

	[Serializable]
	public class JsonGenerationData
	{
		//Config
		public string levelName;
		public string levelID;
		public string baseLevel;
		public int[] mapSizeMin;
		public int[] mapSizeMax;
		public bool fieldTrips;
		public int mapCost;

		//Min max ranges
		public int[] classRange;
		public int[] facultyRange;
		public int[] officeRange;
		public int[] extraRange;
		public int[] specialRange;
		public int[] plotRange;
		public int[] hallRemovalRange;
		public int[] hallReplacementRange;
		public int[] prePhRange;
		public int[] postPhRange;
		public int[] speciaBuilderRange;

		public int[] plotSizeMin;

		//Buffers
		public int deadEndBuffer;
		public int edgeBuffer;
		public int hallBuffer;
		public int outerEdgeBuffer;

		//Values
		public int lightDistance;
		public float posterChance;
		public float prePhChance;
		public float postPhChance;
		public float extraDoorChance;
		public float dijkstraMulti;
		public float dijkstraPower;
		public float doorReqMulti;
		public float hallDamp;
		public float perimeterBase;
		public float centerWeightMulti;
		public float initEventGap;

		//Sticks
		public float classStick;
		public float facultyStick;
		public float extraStick;
		public float officeStick;
		public bool specialStick;

		//Character Values
		public int npcCount;
		public JsonWeight[] baldis;
		public JsonWeight[] forcedNpcs;
		public JsonWeight[] npcs;

		//Item Values
		public JsonWeight[] items;
		public int itemMaxVal;
		public int itemEntranceVal;
		public int itemNoHallVal;

		//Event Values
		public int[] eventRange;
		public JsonWeight[] events;

		//Counters
		public int exitCount;

		//Weighted Stuff
		public JsonWeight[] structures;
		public JsonWeight[] floorSkins;
		public JsonWeight[] wallSkins;
		public JsonWeight[] ceilingSkins;

		public string doorMaterial;
	}

	[Serializable]
	public class JsonWeight
	{
		public string value;
		public int weight;
	}

	[Serializable]
	public class JsonLevelData
	{
		public JsonObjectsDatas objects;
	}


	[Serializable]
	public class JsonItemTable
	{
		public JsonItem[] items;
	}

	[Serializable]
	public class JsonSprite
	{
		public string name;
		public string filename;
		public float center;
		public float pixelsPerUnit;
	}

	[Serializable]
	public class JsonItem
	{
		public string name;
		public int weight;
	}


	[Serializable]
	public class JsonRoomMetadata
	{
		public bool hasActivity;
		public bool keepTextures;

		public JsonVector[] doorPositions;
		public JsonVector[] entityPositions;
		public JsonVector[] eventPositions;
		public JsonVector[] lightPositions;

		public float[] color;
		public float posterChance;
	}

	[Serializable]
	public class JsonObjectsDatas
	{
		public JsonObjectData[] basicObjects;
	}
	[Serializable]
	public class JsonObjectData
	{
		public string type;
		public float[] position;
		public float[] rotation;
	}

	[Serializable]
	public class JsonTileData
	{
		public JsonCell[] cells;
	}

	[Serializable]
	public class JsonPosterFile
	{
		public JsonPosterData[] posters;
	}

	[Serializable]
	public class JsonPosterData
	{
		public string name;
		public int direction;
		public int[] position;
	}

	[Serializable]
	public class JsonVector
	{
		public int x;
		public int z;
	}

	[Serializable]
	public class JsonVector3
	{
		public float x;
		public float y;
		public float z;
	}

	public class JsonCell
	{
		public int type;
		public int[] position;
	}


	[Serializable]
	public class JsonLevelContents
	{
		public JsonVector3 spawnPoint;
		public int spawnDirection;
		public JsonVector levelSize;
		public JsonCell[] tile;
	}


	[Serializable]
	public class JsonNPC
	{
		public string name;
		public bool hasLooker;
		public bool hasTrigger;
		public bool ignoreBelts;
		public bool airborne;
		public bool stationary;
		public string poster;
		public string posterName;
		public string posterDesc;
		public string character;
		public JsonSprite[] sprites;
	}
}

