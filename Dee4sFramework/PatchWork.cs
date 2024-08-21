using UnityEngine;
using System.Collections;
using HarmonyLib;
using BepInEx.Configuration;
using BepInEx;
using MTM101BaldAPI;

[BepInPlugin("dee4.games.baldiplus.gemuengine", "Gemu Mod Engine", "1.0.0")]
[BepInDependency("mtm101.rulerp.bbplus.baldidevapi", BepInDependency.DependencyFlags.HardDependency)]
public class PatchWork : BaseUnityPlugin
{

	void Awake()
	{
		Harmony harmony = new Harmony ("dee4.games.baldiplus.gemuengine");
		harmony.PatchAllConditionals ();
		Debug.Log ("Patcher was successful");
	}
}

[HarmonyPatch(typeof(BaseGameManager))]
[HarmonyPatch("BeginPlay")]
public class DebugPatch
{

	public static void Prefix(BaseGameManager __instance)
	{
		if (Singleton<PlayerFileManager>.Instance.fileName == "USERDEV") {
			__instance.gameObject.AddComponent<DebugMenu> ();

			__instance.gameObject.GetComponent<DebugMenu> ().Setup (__instance.Ec, Singleton<CoreGameManager>.Instance.GetPlayer (0));
		}
	}

}

public class ApiPatches
{

	[HarmonyPatch(typeof(Notebook))]
	[HarmonyPatch("Clicked")]
	[HarmonyPostfix]
	public static void NotebookCollect()
	{
		APIActions.onNotebookCollect.Invoke ();
	}


	[HarmonyPatch(typeof(ItemManager))]
	[HarmonyPatch("UseItem")]
	[HarmonyPostfix]
	public static void ItemUse(ItemManager __instance)
	{
		if (__instance.items [__instance.selectedItem].itemType != Items.None) {
			APIActions.onItemUse.Invoke ();
		}
	}


	[HarmonyPatch(typeof(Pickup))]
	[HarmonyPatch("Collect")]
	[HarmonyPostfix]
	public static void ItemCollect(Pickup __instance)
	{
		if (__instance.item.itemType != Items.None) {
			APIActions.onItemCollect.Invoke ();

		}
	}

	[HarmonyPatch(typeof(Principal))]
	[HarmonyPatch("SendToDetention")]
	[HarmonyPostfix]
	public static void Detention()
	{
		APIActions.onDetentionGet.Invoke ();
	}


	[HarmonyPatch(typeof(EnvironmentController))]
	[HarmonyPatch("SpawnNPCs")]
	[HarmonyPostfix]
	public static void Spawned()
	{
		APIActions.onNpcSpawns.Invoke ();
	}


	[HarmonyPatch(typeof(BaseGameManager))]
	[HarmonyPatch("BeginPlay")]
	[HarmonyPostfix]
	public static void BeginGame()
	{
		APIActions.onGameStart.Invoke ();
	}
}
