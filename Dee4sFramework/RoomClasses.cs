using Gemu;
using Gemu.JSON;
using UnityEngine;
using System.Collections;
using System.IO;
using BepInEx;
using MTM101BaldAPI.AssetTools;
using System.Collections.Generic;
using Newtonsoft.Json;
using EditorCustomRooms;


namespace Gemu.RoomTools
{

	public enum RoomObjects
	{
		BigDesk = 0,
		FilingCabinent = 1,
		SmallCabinent = 2,
		Locker = 3,
		Chair = 4,
		Banana = 5,
		Globe = 6,
		Lunch = 7,
		TapePlayer = 8,
		CeilingFan = 9,
		BSODA = 10,
		Zesty = 11,
		Crazy = 12,
		Custom = 4096
	}

	public class RoomParser
	{
		public static Transform[] jernk = new Transform[]
		{
			ObjectFunctions.FindResourceOfName<Transform> ("BigDesk"),
			ObjectFunctions.FindResourceOfName<Transform> ("FilingCabinet_Tall"),
			ObjectFunctions.FindResourceOfName<Transform> ("FilingCabinet_Short"),
			ObjectFunctions.FindResourceOfName<Transform> ("Locker"),
			ObjectFunctions.FindResourceOfName<Transform> ("Chair_Test"),
			ObjectFunctions.FindResourceOfName<Transform> ("Decor_Banana"),
			ObjectFunctions.FindResourceOfName<Transform> ("Decor_Globe"),
			ObjectFunctions.FindResourceOfName<Transform> ("Decor_Lunch"),
			ObjectFunctions.FindResourceOfName<Transform> ("TapePlayer"),
			ObjectFunctions.FindResourceOfName<Transform> ("CeilingFan"),
			ObjectFunctions.FindResourceOfName<Transform> ("SodaMachine"),
			ObjectFunctions.FindResourceOfName<Transform> ("ZestyMachine"),
			ObjectFunctions.FindResourceOfName<Transform> ("CrazyVendingMachineBSODA")
		};
		static Transform getObjectFromType(string type, AssetManager plugin)
		{
			RoomObjects objlol = RoomObjects.Custom;
			switch (type) {
			case "BigDesk":
				objlol = RoomObjects.BigDesk;
				break;
			case "FilingCabinent":
				objlol = RoomObjects.FilingCabinent;
				break;
			case "SmallCabinent":
				objlol = RoomObjects.SmallCabinent;
				break;
			case "Locker":
				objlol = RoomObjects.Locker;
				break;
			case "Chair":
				objlol = RoomObjects.Chair;
				break;
			case "Banana":
				objlol = RoomObjects.Banana;
				break;
			case "Globe":
				objlol = RoomObjects.Globe;
				break;
			case "Lunch":
				objlol = RoomObjects.Lunch;
				break;
			case "TapePlayer":
				objlol = RoomObjects.TapePlayer;
				break;
			case "Fan":
				objlol = RoomObjects.CeilingFan;
				break;
			case "BSODA":
				objlol = RoomObjects.BSODA;
				break;
			case "Zesty":
				objlol = RoomObjects.Zesty;
				break;
			case "Crazy":
				objlol = RoomObjects.Crazy;
				break;
			}
			if ((int)objlol != 4096) {
				return jernk [(int)objlol];
			}
			return plugin.Get<Transform> (type);
		}

		public static List<BasicObjectData> parseRoomData(string data, AssetManager plugin)
		{
			List<BasicObjectData> rofl = new List<BasicObjectData> ();
			//Debug.Log ("initzlaed data");
			JsonLevelData dataParsed = JsonConvert.DeserializeObject<JsonLevelData> (data);
			//Debug.Log ("loaded file " + dataParsed);
			JsonObjectsDatas junk = dataParsed.objects;
			//Debug.Log (string.Concat("loaded objects ", dataParsed.objects));
			foreach (JsonObjectData jsonData in junk.basicObjects) {
				BasicObjectData convertedData = new BasicObjectData ();
				//Debug.Log ("Converting...");
				convertedData.position = new Vector3(jsonData.position[0], jsonData.position[1], jsonData.position[2]);
				convertedData.rotation = new Quaternion(jsonData.rotation[0], jsonData.rotation[1], jsonData.rotation[2], jsonData.rotation[3]);
				//Debug.Log ("Variables Set, Fetching Object...");
				string lol = jsonData.type;
				convertedData.prefab = getObjectFromType (lol, plugin);
				rofl.Add (convertedData);
				//Debug.Log (string.Concat("Converted Object ", jsonData.type, " with a position of : ", convertedData.position));
			}
			return rofl;
		}

		public static List<CellData> parseRoomTiles(string data)
		{
			List<CellData> output = new List<CellData> ();
			//Debug.Log ("initzlaed data");
			JsonTileData dataParsed = JsonConvert.DeserializeObject<JsonTileData> (data);
			//Debug.Log ("loaded file " + dataParsed);
			foreach (JsonCell jsonData in dataParsed.cells) {
				CellData convertedData = new CellData ();
				//Debug.Log ("Converting...");
				convertedData.pos = new IntVector2 (jsonData.position [0], jsonData.position [1]);
				convertedData.type = jsonData.type;
				convertedData.roomId = 0;
				output.Add (convertedData);
				//Debug.Log (string.Concat("Converted Tile ", jsonData.type, " with a position of : ", convertedData.pos));
			}
			return output;
		}


		public static List<PosterData> parsePosterData(string data, AssetManager plugin)
		{
			List<PosterData> output = new List<PosterData> ();
			//Debug.Log ("initzlaed data");
			JsonPosterFile dataParsed = JsonConvert.DeserializeObject<JsonPosterFile> (data);
			//Debug.Log ("loaded file " + dataParsed);
			foreach (JsonPosterData jsonData in dataParsed.posters) {
				PosterData convertedData = new PosterData ();
				//Debug.Log ("Converting...");
				convertedData.position = new IntVector2 (jsonData.position [0], jsonData.position [1]);
				convertedData.direction = (Direction)jsonData.direction;
				convertedData.poster = plugin.Get<PosterObject> (jsonData.name);
				output.Add (convertedData);
				//Debug.Log (string.Concat("Converted Tile ", jsonData.type, " with a position of : ", convertedData.pos));
			}
			return output;
		}


		public static List<IntVector2> convertVectors(JsonVector[] array)
		{
			List<IntVector2> result = new List<IntVector2> ();
			foreach (JsonVector vector in array) {
				result.Add (new IntVector2 (
					vector.x,
					vector.z));
			}
			return result;
		}

		public static RoomAsset CreateRoomAsset(string path, string tileName, string objectName, string metaName, Transform lightPre, RoomCategory category, Texture2D[] textures, StandardDoorMats doorTex, AssetManager plugin)
		{
			RoomAsset finalRoom = RoomAsset.CreateInstance<RoomAsset> ();
			finalRoom.cells = parseRoomTiles (File.ReadAllText (
				string.Concat(path, "/", tileName, ".json")
			));
			finalRoom.basicObjects = parseRoomData (File.ReadAllText (
				string.Concat(path, "/", objectName, ".json")
			), plugin);
			string metadata = File.ReadAllText (
				string.Concat (path, "/", metaName, ".json")
			);
			JsonRoomMetadata dataParsed = JsonConvert.DeserializeObject<JsonRoomMetadata> (metadata);
			finalRoom.hasActivity = dataParsed.hasActivity;
			finalRoom.activity = new ActivityData ();
			finalRoom.potentialDoorPositions = convertVectors (dataParsed.doorPositions);
			finalRoom.entitySafeCells = convertVectors (dataParsed.entityPositions);
			finalRoom.eventSafeCells = convertVectors (dataParsed.eventPositions);
			finalRoom.color = new Color (
				dataParsed.color[0] / 255f,
				dataParsed.color[1] / 255f,
				dataParsed.color[2] / 255f
			);
			finalRoom.keepTextures = dataParsed.keepTextures;
			finalRoom.posterChance = dataParsed.posterChance;
			finalRoom.lightPre = lightPre;
			finalRoom.standardLightCells = convertVectors (dataParsed.lightPositions);
			finalRoom.category = category;
			finalRoom.type = RoomType.Room;
			finalRoom.florTex = textures [0];
			finalRoom.wallTex = textures [1];
			finalRoom.ceilTex = textures [2];
			finalRoom.doorMats = doorTex;
			return finalRoom;
		}



		/*public static RoomAsset CreateRoomFromLevel(string path, int itemVal, bool offlimits, string metaName, RoomCategory category, Texture2D[] textures, StandardDoorMats doorTex, Texture2D mapIcon, bool secret, AssetManager plugin, RoomFunctionContainer function = null)
		{	
			string metadata = File.ReadAllText (
				string.Concat (path, "/", metaName, ".json")
			);

			JsonRoomMetadata dataParsed = JsonConvert.DeserializeObject<JsonRoomMetadata> (metadata);
			
			RoomAsset finalRoom = RoomFactory.CreateAssetFromPath (path, itemVal, offlimits, function, false, secret, mapIcon, dataParsed.keepTextures); 

			finalRoom.hasActivity = dataParsed.hasActivity;
			finalRoom.activity = new ActivityData ();
			finalRoom.color = new Color (
				dataParsed.color[0] / 255f,
				dataParsed.color[1] / 255f,
				dataParsed.color[2] / 255f
			);
			finalRoom.posterChance = dataParsed.posterChance;
			finalRoom.category = category;
			finalRoom.type = RoomType.Room;
			finalRoom.florTex = textures [0];
			finalRoom.wallTex = textures [1];
			finalRoom.ceilTex = textures [2];
			finalRoom.doorMats = doorTex;
			return finalRoom;
		}

		public static RoomAsset CreateHallAsset(string path, AssetManager plugin)
		{	
			RoomAsset finalRoom = RoomFactory.CreateAssetFromPath (path, 0, false, null, true); 

			return finalRoom;
		}*/

	}
}

