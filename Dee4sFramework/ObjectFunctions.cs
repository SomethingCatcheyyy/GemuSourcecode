using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.Reflection;
using MTM101BaldAPI.ObjectCreation;
using Newtonsoft.Json;
using Gemu.JSON;
using HarmonyLib;
using UnityEngine.UI;

namespace Gemu
{
	public class SoundMetadata
	{
		public SoundType type;
		public Color subcolor;
		public SoundMetadata(SoundType type, Color subcolor)
		{
			this.type = type;
			this.subcolor = subcolor;
		}
	}

	public class ModContents
	{
		public Dictionary<string, WeightedNPC> newNpcs = new Dictionary<string, WeightedNPC> ();
		public Dictionary<string, WeightedItemObject> newItems = new Dictionary<string, WeightedItemObject> ();
		public Dictionary<string, WeightedRoomAsset> newRooms = new Dictionary<string, WeightedRoomAsset> ();
		public Dictionary<string, WeightedRandomEvent> newEvents = new Dictionary<string, WeightedRandomEvent> ();
		public Dictionary<string, GameObject> modPrefabs = new Dictionary<string, GameObject> ();

		public string imagePath;
		public string audioPath;

		public ModContents()
		{
			Debug.Log ("Mod Setup Done");
		}


		public Dictionary<string, PosterObject> npcPoster = new Dictionary<string, PosterObject> ();

		/// <summary> Creates a npc poster object </summary>
		[Obsolete("use PosterObject method ya dingus")]
		public void CreateNPCPoster(string npcName, string file, string nameKey, string descKey, string path)
		{
			npcPoster.Add(npcName, ObjectCreators.CreateCharacterPoster (
				AssetLoader.TextureFromFile (
					string.Concat (
						path,
						"/",
						file,
						".png"
					)), nameKey, descKey));
		}

		public PosterObject CreateNPCPoster(string file, string nameKey, string descKey, string path)
		{
			return ObjectCreators.CreateCharacterPoster (
				AssetLoader.TextureFromFile (
					string.Concat (
						path,
						"/",
						file,
						".png"
					)), nameKey, descKey);
		}
	}

	public class ObjectFunctions
	{

		/// <summary> Creates a sprite </summary>
		public static Sprite CreateSprite(string name, string path, float offset, float ppu)
		{
			Texture2D tex = AssetLoader.TextureFromFile (path + "/" + name + ".png");
			Sprite spr = Sprite.Create(tex, 
				new Rect(0, 0, tex.width, tex.height), 
				new Vector2(0.5f, offset), ppu );
			return spr;
		}

		public static Sprite CreateSprite(string name, string path)
		{
			return CreateSprite (name, path, 0.5f, 32f);
		}

		/// <summary> Creates a array of sound objects </summary>
		public static SoundObject[] CreateSoundArray(string name, string path, int length, SoundMetadata metadata, string Subtitle, string format = "wav", bool captioned = true)
		{
			string pather = path + "/" + name;

			SoundObject[] output = new SoundObject[length];

			for (int i = 1; i < length + 1; i++) {
				int index = i - 1;
				/*AudioClip audioasset = AssetLoader.AudioClipFromFile (pather + i.ToString() + "." + format);

				output [index] = ObjectCreators.CreateSoundObject (audioasset, string.Concat (Subtitle, i), metadata.type, metadata.subcolor);

				output [index].subtitle = captioned;*/
				output [index] = CreateSound (string.Concat (name, i), path, metadata, string.Concat (Subtitle, i), format, captioned);

			}
			return output;
		}
	

		/// <summary> Creates a sound object </summary>
		public static SoundObject CreateSound(string name, string path, SoundMetadata metadata, string Subtitle, string format = "wav", bool useCaption = true)
		{
			string pather = path + "/" + name;

			AudioClip audioasset = AssetLoader.AudioClipFromFile (pather + "." + format);

			SoundObject fuckMe = ObjectCreators.CreateSoundObject (
				                     audioasset, Subtitle, metadata.type, metadata.subcolor);
			fuckMe.subtitle = useCaption;

		
			return fuckMe;
		}

		/// <summary> Creates a fog object </summary>
		/// <param name="color">The color of the fog</param>
		/// <param name="strength">The strength of the fog</param>
		/// <param name="maxDist">The maxinum distance of the fog</param>
		/// <param name="priority">The priority/layer of the fog</param>
		/// <param name="startDist">The starting distance of the fog</param>

		public static Fog createFog(Color color, float strength, float maxDist, int priortiy, float startDist)
		{
			return new Fog () {
				color = color,
				strength = strength,
				maxDist = maxDist,
				startDist = startDist,
				priority = priortiy
			};
		}

		public static Sprite[] CreateSpriteArray(string name, float units, float offset, int count, string path)
		{
			List<Sprite> listofsprites = new List<Sprite> ();
			for (int num = 0; num < count; num++) {
				listofsprites.Add (
					CreateSprite(string.Concat(name, num), path, offset, units)
				);
			}
			return listofsprites.ToArray ();
		}

		/// <summary> Find a speciific resource </summary>
		/// <param name="name">The resources name</param>
		/// <param name="plugin">The mod's asset manager(Optional)</param>
		public static T FindResourceOfName<T>(string name, AssetManager plugin = null) where T : UnityEngine.Object
		{
			T[] stuffs = UnityEngine.Resources.FindObjectsOfTypeAll<T> ();
			foreach (T thingy in stuffs) {
				if (thingy.name == name) {
					return thingy;
				}
			}
			if (plugin != null) {
				try {
					return plugin.Get<T> (name);
				} catch {
					throw new NotImplementedException ("YOU DONT HAVE THAT YET");
				}
			}
			return null;
		}

		/// <summary> Creates a entity object </summary>
		public static Entity CreateEntity(GameObject entityObject, float height, bool triggerBool, Collider collider, Collider trigger, ActivityModifier actMod, Transform renderBase)
		{
			Entity entity = entityObject.AddComponent<Entity> ();
			entity.SetHeight (height);
			entity.SetTrigger (triggerBool);
			AccessTools.Field (typeof(Entity), "collider").SetValue (entity, collider);
			AccessTools.Field (typeof(Entity), "trigger").SetValue (entity, trigger);
			AccessTools.Field (typeof(Entity), "externalActivity").SetValue (entity, actMod);
			AccessTools.Field (typeof(Entity), "rendererBase").SetValue (entity, renderBase);
			return entity;
		}


		public static void SwapPositions(Transform a, Transform b)
		{
			Vector3 origPosition = a.position;
			a.position = b.position;
			b.position = origPosition;
		}

		/// <summary> Creates a NPC </summary>
		public static NPC CreateBasicNPC<T>(string filePath, BepInEx.PluginInfo info, ModContents mod, NPCFlags flag, RoomCategory[] categorys, params WeightedRoomAsset[] roomAssets) where T : NPC
		{
			JsonNPC dataParsed = JsonConvert.DeserializeObject<JsonNPC> (
				System.IO.File.ReadAllText(filePath)
			);

			NPCBuilder<T> builder = new NPCBuilder<T> (info)
				.SetName (dataParsed.name)
				.SetEnum (dataParsed.character)
				.SetMinMaxAudioDistance (10f, 250f)
				.AddSpawnableRoomCategories (categorys)
				.AddMetaFlag (flag)
				.SetPoster (
					mod.CreateNPCPoster(dataParsed.poster, dataParsed.posterName, dataParsed.posterDesc, mod.imagePath)
				)
				.IgnorePlayerOnSpawn();

			if (dataParsed.airborne) {
				builder = builder.SetAirborne ();
			}
			if (dataParsed.hasLooker) {
				builder = builder.AddLooker ();
			}
			if (dataParsed.hasTrigger) {
				builder = builder.AddTrigger ();
			}
			if (dataParsed.ignoreBelts) {
				builder = builder.IgnoreBelts ();
			}
			if (dataParsed.stationary) {
				builder = builder.SetStationary ();
			}
			if (roomAssets.Length > 0) {
				builder = builder.AddPotentialRoomAssets (roomAssets);
			}

			return builder.Build ();
		}

		public static CustomNPC CreateCustomNPC<T>(string filePath, BepInEx.PluginInfo info, ModContents mod, NPCFlags flag, RoomCategory[] categorys, params WeightedRoomAsset[] roomAssets) where T : CustomNPC
		{
			JsonNPC dataParsed = JsonConvert.DeserializeObject<JsonNPC> (
				System.IO.File.ReadAllText(filePath)
			);

			NPCBuilder<T> builder = new NPCBuilder<T> (info)
				.SetName (dataParsed.name)
				.SetEnum (dataParsed.character)
				.SetMinMaxAudioDistance (10f, 250f)
				.AddSpawnableRoomCategories (categorys)
				.AddMetaFlag (flag).SetPoster (
					mod.CreateNPCPoster(dataParsed.poster, dataParsed.posterName, dataParsed.posterDesc, mod.imagePath)
				)
				.IgnorePlayerOnSpawn();

			if (dataParsed.airborne) {
				builder = builder.SetAirborne ();
			}
			if (dataParsed.hasLooker) {
				builder = builder.AddLooker ();
			}
			if (dataParsed.hasTrigger) {
				builder = builder.AddTrigger ();
			}
			if (dataParsed.ignoreBelts) {
				builder = builder.IgnoreBelts ();
			}
			if (dataParsed.stationary) {
				builder = builder.SetStationary ();
			}
			if (roomAssets.Length > 0) {
				builder = builder.AddPotentialRoomAssets (roomAssets);
			}
			T t = builder.Build ();
			SpriteBank bank = new SpriteBank (dataParsed.name);
			Debug.Log (string.Concat ("Created bank ", bank.bankName, " now do the funnies"));
			/*List<string> iHateMyLife = new List<string> ();
			List<Sprite> andIWannaDie = new List<Sprite> ();*/
			if (dataParsed.sprites.Length > 0) {
				foreach (JsonSprite sprite in dataParsed.sprites) {
					Debug.Log ("Registering " + sprite.name + " " + sprite.filename);
					bank.AddSprite(sprite.name,
						CreateSprite(sprite.filename, mod.imagePath, sprite.center, sprite.pixelsPerUnit)
					);
				}
			}
			SpriteBanks.banks.Add (dataParsed.name, bank);
			t.npcName = dataParsed.name;
			t.UpdateSprite ("idle");
			return t;
		}

		/// <summary> Adds a list to an array </summary>
		public static T[] AddToArray<T>(List<T> gaye, T[] og)
		{

			List<T> news = new List<T> ();

			for (int i = 0; i < og.Length; i++) {
				news.Add (og [i]);
			}
			for (int j = 0; j < gaye.Count; j++) {
				news.Add (gaye [j]);
			}
			return news.ToArray ();
		}

		public static Transform CreateBasicMesh(PrimitiveType mesh, Vector3 scale, Material material, Vector3 collisionScale)
		{
			Transform cube = GameObject.CreatePrimitive (mesh).transform;
			cube.localScale = scale;
			cube.GetComponent<MeshRenderer> ().material = material;
			cube.GetComponent<BoxCollider> ().size = collisionScale;
			return cube;
		}

		/// <summary> Creates a sprite list from a atlas</summary>
		public static Sprite[] GenerateAtlas(string name, string path, int sizeX, int sizeZ, float height, float ppu)
		{
			string filePath = string.Concat (
				path,
				name,
				".png"
			);

			List<Sprite> outputFile = new List<Sprite> ();

			Texture2D spriteAtlas = AssetLoader.TextureFromFile (filePath);

			float scaleX = (float)(spriteAtlas.height / sizeX);
			float scaleZ = (float)(spriteAtlas.width / sizeZ);

			for (int i = 0; i < sizeX; i++) {
				for (int j = 0; j < sizeZ; j++) {
					Rect eRECTion = new Rect (((float)j * scaleZ), ((float)i * scaleX), scaleZ, scaleX);
					Sprite newSprite = Sprite.Create(spriteAtlas, 
						eRECTion, 
						new Vector2(0.5f, height), ppu );
					outputFile.Add (newSprite);
				}
			}
			return outputFile.ToArray ();
		}

		public static SpriteRenderer CreateSpriteRender(string name, bool billboarded, Transform baseObject)
		{
			GameObject srobject = new GameObject (name);
			srobject.transform.parent = baseObject;
			srobject.transform.position = baseObject.position;
			srobject.AddComponent<SpriteRenderer> ();
			SpriteRenderer rend = srobject.GetComponent<SpriteRenderer> ();

			if (billboarded) {
				rend.material = ObjectFunctions.FindResourceOfName<Material> ("SpriteStandard_Billboard");

			} else {
				rend.material = ObjectFunctions.FindResourceOfName<Material> ("SpriteWithFog_Forward_NoBillboard");

			}

			return rend;
		}

		/// <summary> Finds a object in a scene </summary>
		public static GameObject findObjectInScene(string name)
		{
			GameObject[] objectz = SceneManager.GetActiveScene ().GetRootGameObjects ();
			foreach(GameObject x in objectz)
			{
				if (x.name == name) {
					return x;
				}
			}
			return null;
		}
			
		public static GameObject CreateFlooredSprite(string name, string spriteFile, bool prefab, ModContents mod)
		{
			GameObject objectLol = new GameObject (name);
			SpriteRenderer mainObject = CreateSpriteRender (name + "_Sprite", false, objectLol.transform);
			mainObject.sprite = CreateSprite (spriteFile, mod.imagePath);
			if (prefab) {
				objectLol.ConvertToPrefab (true);
			}
			return objectLol;
		}

		public static RoomFunction CreateRoomFunction<T>(string name, bool post) where T : RoomFunction
		{
			GameObject XD = new GameObject (name);
			RoomFunction LMAO = XD.AddComponent<T> ();
			if (!post) {
				XD.ConvertToPrefab (true);
			}
			return LMAO;
		}

		public static RoomFunctionContainer CreateRoomFunctionContainer (string name, bool post, params RoomFunction[] functions)
		{
			GameObject XD2 = new GameObject (name);
			RoomFunctionContainer container = XD2.AddComponent<RoomFunctionContainer> ();
			foreach (RoomFunction function in functions) {
				container.AddFunction (function);
			}
			if (!post) {
				XD2.ConvertToPrefab (true);
			}
			return container;
		}
	}

	/*public class PathManager
	{
		public static PathManager instance;

		public string imagePath;
		public string audioPath;
		public string roomPath;
		public string itemPath;
		public string npcPath;

		public PathManager(string imagePath, string audioPath, string roomPath, string itemPath, string npcPath)
		{
			this.imagePath = imagePath;
			this.audioPath = audioPath;
			this.itemPath = itemPath;
			this.roomPath = roomPath;
			this.npcPath = npcPath;
			instance = this;
		}

		public string imageFile(string name)
		{
			return string.Concat (imagePath, "/", name, ".png");
		}
		public string audioFile(string name, string format)
		{
			if (format == "ogg") {
				return oggFile (name);
			}
			return wavFile (name);

		}
		public string oggFile(string name)
		{
			return string.Concat (audioPath, "/", name, ".ogg");
		}
		public string wavFile(string name)
		{
			return string.Concat (audioPath, "/", name, ".wav");
		}
		public string roomMetaFile(string name)
		{
			return string.Concat (roomPath, "/", name, ".json");
		}
		public string npcMetaFile(string name)
		{
			return string.Concat (npcPath, "/", name, ".json");
		}
		public string lootTable(string name)
		{
			return string.Concat (itemPath, "/", name, ".json");
		}
	}*/

	public class ItemFunctions
	{

		/// <summary> Creates a item gameobject </summary>
		public static GameObject CreateItemObject<T>(string name, bool prefabPost = false, bool setActive = true) where T : Item
		{
			GameObject itemObj = new GameObject (name);
			itemObj.AddComponent<T> ();
			if (!prefabPost) {
				itemObj.ConvertToPrefab (setActive);
			}
			return itemObj;
		}


		/// <summary> Parse an item loot table </summary>
		public static List<WeightedItemObject> parseItemTable(string data, AssetManager plugin)
		{
			List<WeightedItemObject> output = new List<WeightedItemObject> ();
			//Debug.Log ("initzlaed data");
			JsonItemTable dataParsed = JsonConvert.DeserializeObject<JsonItemTable> (data);
			//Debug.Log ("loaded file " + dataParsed);
			foreach (JsonItem item in dataParsed.items) 
			{
				output.Add(new WeightedItemObject {
					selection = ObjectFunctions.FindResourceOfName<ItemObject>(item.name, plugin),
					weight = item.weight
				});
			}
			return output;
		}
	}

	public class LevelFunctions
	{

		public static List<WeightedNPC> ProcessNPCList(JsonWeight[] weights, AssetManager plugin)
		{
			List<WeightedNPC> output = new List<WeightedNPC> ();
			foreach (JsonWeight w in weights) {
				output.Add (new WeightedNPC {
					selection = ObjectFunctions.FindResourceOfName<NPC> (w.value, plugin),
					weight = w.weight
				});
			}
			return output;
		}

		public static WeightedItemObject[] ProcessItemList(JsonWeight[] weights, AssetManager plugin)
		{
			List<WeightedItemObject> output = new List<WeightedItemObject> ();
			foreach (JsonWeight w in weights) {
				output.Add (new WeightedItemObject {
					selection = ObjectFunctions.FindResourceOfName<ItemObject> (w.value, plugin),
					weight = w.weight
				});
			}
			return output.ToArray();
		}

		public static LevelObject CreateObject(string data, AssetManager plugin)
		{
			LevelObject theLevel = ScriptableObject.CreateInstance<LevelObject> ();


			JsonGenerationData penis = JsonConvert.DeserializeObject<JsonGenerationData> (data);

			LevelObject referenceLevel = ObjectFunctions.FindResourceOfName<LevelObject> (penis.baseLevel, plugin);
				
			theLevel.minSize = new IntVector2 (penis.mapSizeMin [0], penis.mapSizeMin [1]);
			theLevel.maxSize = new IntVector2 (penis.mapSizeMax [0], penis.mapSizeMax [1]);

			theLevel.minPlots = penis.plotRange [0];
			theLevel.maxPlots = penis.plotRange [1];
			theLevel.minPlotSize = penis.plotSizeMin [0];

			theLevel.deadEndBuffer = penis.deadEndBuffer;
			theLevel.edgeBuffer = penis.edgeBuffer;
			theLevel.hallBuffer = penis.hallBuffer;
			theLevel.outerEdgeBuffer = penis.outerEdgeBuffer;

			theLevel.minClassRooms = penis.classRange [0];
			theLevel.maxClassRooms = penis.classRange [1];
			theLevel.classStickToHallChance = penis.classStick;

			theLevel.minFacultyRooms = penis.facultyRange [0];
			theLevel.maxFacultyRooms = penis.facultyRange [1];
			theLevel.facultyStickToHallChance = penis.facultyStick;

			theLevel.minOffices = penis.officeRange[0];
			theLevel.maxOffices = penis.officeRange [1];
			theLevel.officeStickToHallChance = penis.officeStick;

			theLevel.minExtraRooms = penis.extraRange [0];
			theLevel.maxExtraRooms = penis.extraRange [1];
			theLevel.extraStickToHallChance = penis.extraStick;

			theLevel.minSpecialRooms = penis.specialRange [0];
			theLevel.maxSpecialRooms = penis.specialRange [1];
			theLevel.specialRoomsStickToEdge = penis.specialStick;

			theLevel.exitCount = penis.exitCount;
			theLevel.hallPriorityDampening = penis.hallDamp;
			theLevel.elevatorPre = referenceLevel.elevatorPre;
			theLevel.elevatorRoom = referenceLevel.elevatorRoom;
			theLevel.standardDoorMat = ObjectFunctions.FindResourceOfName<StandardDoorMats>(penis.doorMaterial, plugin);

			theLevel.additionalNPCs = penis.npcCount;
			theLevel.potentialNPCs = ProcessNPCList (penis.npcs, plugin);
			theLevel.potentialBaldis = ProcessNPCList (penis.baldis, plugin).ToArray();

			theLevel.noHallItemVal = penis.itemNoHallVal;
			theLevel.singleEntranceItemVal = penis.itemEntranceVal;
			theLevel.items = ProcessItemList (penis.items, plugin);

			return theLevel;
		}


		public static LevelObject getLevel(string name)
		{
			return Enumerable.ToList<LevelObject> (Resources.FindObjectsOfTypeAll<LevelObject> ()).Find ((LevelObject x) => x.name == name);
		}
	}

	public class SpriteBanks
	{
		public static Dictionary<string, SpriteBank> banks = new Dictionary<string, SpriteBank>();

		public static SpriteBank getBank(string name)
		{
			if (banks.ContainsKey (name)) {
				//Debug.Log (string.Concat ("bank found for ", name, " using now"));
				return banks [name];
			}
			//Debug.Log (string.Concat ("No bank found for ", name, " using placeholder instead"));
			return banks ["Carmella"];
		}
	}

	public class NPCHotSpot : MonoBehaviour, IClickable<int>
	{

		public Action action;

		public void Clicked(int playerNumber)
		{
			PlayerManager pm = Singleton<CoreGameManager>.Instance.GetPlayer (playerNumber);
			action.Invoke ();
			gameObject.SetActive (false);
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00002492 File Offset: 0x00000692
		public void ClickableSighted(int player)
		{
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00002492 File Offset: 0x00000692
		public void ClickableUnsighted(int player)
		{
		}

		// Token: 0x0600018C RID: 396 RVA: 0x000037FE File Offset: 0x000019FE
		public bool ClickableHidden()
		{
			return false;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00003801 File Offset: 0x00001A01
		public bool ClickableRequiresNormalHeight()
		{
			return true;
		}
	}

}

public static class GemuExtensions
{
	public static NPC GetNPC(this EnvironmentController ec, string character) 
	{
		foreach (NPC npc in ec.Npcs) {
			if (npc.Character.ToString ().ToLower () == character.ToLower ()) {
				return npc;
			}
		}
		Debug.LogError ("No npc exists with that name, try again.");
		return null;
	}

	public static Color from255 (this Color color, float R, float G, float B)
	{
		return new Color (R / 255f, G / 255f, B / 255f);
	}


	public static void Freeze(this NPC npc, MovementModifier moveMod, Color color = default(Color))
	{
		npc.spriteRenderer [0].color = color;
		npc.GetComponent<ActivityModifier> ().moveMods.Add (moveMod);
	}
	public static void Unfreeze(this NPC npc, MovementModifier moveMod)
	{
		npc.spriteRenderer [0].color = Color.white;
		npc.GetComponent<ActivityModifier> ().moveMods.Remove (moveMod);
	}

	public static void baldi_Enable(this BaldiTV tv, bool enabled)
	{
		Image img = (Image)tv.ReflectionGetVariable ("baldiImage");
		if (enabled) {
			img.color = Color.white;
			return;
		}
		img.color = Color.clear;
	}

	public static void SetSubcolor(this AudioManager audioManager, Color color)
	{
		audioManager.ReflectionSetVariable ("subtitleColor", color);
	}

	public static void SetSuboveride(this AudioManager audioManager, bool oride)
	{
		audioManager.ReflectionSetVariable("overrideSubtitleColor", oride);
	}

	public static float Height(this Entity entity, bool useBase = true)
	{
		return entity.get_Height (useBase);
	}

	public static float get_Height(this Entity entity, bool useBase = true)
	{
		if (useBase) {
			return entity.BaseHeight;
		}
		return entity.InternalHeight;
	}

	public static void AddAffector<T>(this Entity entity) where T : Affector
	{
		entity.gameObject.AddComponent<T> ();
		Affector affector = entity.GetComponent<Affector> ();
		affector.affectedEntity = entity;
	}

	public static void EnableEffector(this Entity entity, bool val)
	{
		if (entity.GetComponent<Affector> () != null) {
			entity.GetComponent<Affector> ().Enable (val);
		}
	}

	public static void AddMapBackground(this RoomAsset room, Texture2D texture)
	{
		room.mapMaterial = new Material (room.mapMaterial);
		room.mapMaterial.SetTexture ("_MapBackground", texture);
		room.mapMaterial.shaderKeywords = new string[] {
			"_KEYMAPSHOWBACKGROUND_ON"
		};
		room.mapMaterial.name = room.name;
	}
}
	