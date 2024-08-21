using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;

public class DebugMenu : MonoBehaviour
{

	EnvironmentController ec;

	PlayerManager pm;

	bool Initalzied = false;

	bool TurnedON = false;

	public List<string> debugLogs;

	public static DebugMenu instance;

	public NPC[] npcs;
	public ItemObject[] items;

	public void Setup(EnvironmentController ec, PlayerManager pm)
	{
		debugLogs = new List<string> ();
		this.ec = ec;
		this.pm = pm;
		npcs = Resources.FindObjectsOfTypeAll<NPC> ();
		items = Resources.FindObjectsOfTypeAll<ItemObject> ();
		Initalzied = true;
		instance = this;
	}

	public static void LogEvent(object eventToLog)
	{
		if (instance != null) {
			if (instance.Initalzied) {
				instance.debugLogs.Add (eventToLog.ToString ());
				if (instance.debugLogs.Count > 20) {
					instance.debugLogs.RemoveAt (0);
				}
				Debug.Log (eventToLog);
			}
		}
	}

	bool ncVal = false;

	void OnGUI()
	{
		if (Initalzied) {
			if (TurnedON) {
				GUI.Label (new Rect (30f, 0f, 160f, 20f), "Press N to (Un)Lock Mosue");
				for (int i = 0; i < npcs.Length; i++) {
					if (GUI.Button (new Rect (230f, 20f + (20f * (float)i), 160f, 15f), string.Concat ("Spawn ", npcs [i].name, " here"))) {
						SpawnNpcDebug (npcs [i]);
					}
				}

				for (int j = 0; j < items.Length; j++) {
					if (GUI.Button (new Rect (430f, 20f + (20f * (float)j), 160f, 15f), string.Concat ("Spawn ", items [j].name, " here"))) {
						Pickup pickupPre = (Pickup)AccessTools.Field (typeof(EnvironmentController), "pickupPre").GetValue (ec);


						Pickup doodoo = UnityEngine.Object.Instantiate<Pickup>(pickupPre, ec.transform);
					
						doodoo.item = items [j];
						doodoo.transform.position = pm.transform.position;
					}
				}
				if (GUI.Button (new Rect (30f, 20f, 160f, 30f), "Fill map")) {
					ec.map.CompleteMap ();
				}

				for (int k = 0; k < debugLogs.Count; k++) {
					GUI.Label (new Rect (30f, (float)(Screen.height - 20) - (20f * (float)k), 230f, 20f), debugLogs[k]);
				}
				RaycastHit hit;
				Transform player = Singleton<CoreGameManager>.Instance.GetPlayer (0).transform;
				if (Physics.Raycast (player.position, player.forward, out hit, 1000f)) {
					if (hit.transform != null) {
						GUI.Label (new Rect (630f, 20f, 360f, 30f), string.Concat (hit.transform.name, hit.transform.gameObject.layer));
					}
				}
				IntVector2 intVector = IntVector2.GetGridPosition (player.position);
				GUI.Label (new Rect (630f, 40f, 360f, 30f), string.Concat (intVector.x, ", ", intVector.z));
			}
		}
	}

	void SpawnNpcDebug(NPC npc)
	{
		ec.SpawnNPC (npc, IntVector2.GetGridPosition (pm.transform.position));
	}

	bool unlocked = false;
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.F5)) {
			TurnedON = !TurnedON;
		}
		if (Input.GetKeyDown (KeyCode.N)) {
			unlocked = !unlocked;
			Cursor.visible = unlocked;
			if (unlocked) {
				Cursor.lockState = CursorLockMode.None;
				return;
			}
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
}

