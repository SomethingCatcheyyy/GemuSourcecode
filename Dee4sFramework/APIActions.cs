using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class APIActions
{
	public static Dictionary<string, System.Action> customActions = new Dictionary<string, System.Action>();

	public static Action onNotebookCollect;

	public static Action onDetentionGet;

	public static Action onGameStart;

	public static Action onNpcSpawns;

	public static Action onItemUse;

	public static Action onItemCollect;

	public static void InvokeCustom(string name)
	{
		if (customActions.ContainsKey (name)) {
			customActions [name].Invoke ();
		}
	}

	public static void CreateCustom(string name, Action initAction = null)
	{
		if (!customActions.ContainsKey (name)) {
			customActions.Add (name, initAction);
		}
	}

	public static void AddToCustom(string name, Action action)
	{
		if (customActions.ContainsKey (name)) {
			customActions [name] += action;
		}
	}
}

