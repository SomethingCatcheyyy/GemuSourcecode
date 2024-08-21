using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gemu.JSON;
using Gemu;

public class SpriteBank
{
	public string bankName;
	public Dictionary<string, Sprite> allSprites = new Dictionary<string, Sprite>();


	public SpriteBank(string name)
	{
		bankName = name;
	}

	public void AddSprite(string name, Sprite sprite)
	{
		allSprites.Add (name, sprite);
	}

	public Sprite GetSprite(string name)
	{
		if (allSprites.ContainsKey (name)) {
			return allSprites [name];
		}
		Debug.LogError (string.Concat (
			"No Sprite called ",
			name,
			" exists in ",
			allSprites,
			" Size:",
			allSprites.Count));
		return null;
	}
}

public class CustomNPC : NPC
{
	public SpriteBank bank;
	public string npcName;

	public override void Initialize()
	{
		base.Initialize ();
		bank = SpriteBanks.getBank (npcName);
		spriteBase = transform.Find ("SpriteBase").gameObject;
		spriteRenderer = new SpriteRenderer[]
		{
			spriteBase.transform.GetChild(0).GetComponent<SpriteRenderer>()
		};
		//this shits a fucking joke
	}

	public void UpdateSprite(string name)
	{
		this.bank = SpriteBanks.getBank (npcName);
		if (bank != null) {
			if (spriteRenderer.Length > 0) {
				if(bank.allSprites.ContainsKey(name))
				{
					spriteRenderer [0].sprite = bank.GetSprite (name);
				}
			}
		}
	}

	public void PlayAnimation(string name, float fps, int length, System.Action onFinish)
	{
		StopCoroutine ("playLoopedAnim");
		if (loopAnim) {

			StartCoroutine (playLoopedAnim (name, fps, length));
			return;
		}
		StartCoroutine (playAnimInternal (name, fps, length, onFinish));
	}

	public bool loopAnim = false;

	IEnumerator playAnimInternal(string n, float fps, int l, System.Action fin)
	{
		for (int i = 0; i < l; i++) {
			UpdateSprite (string.Concat (n, i));
			yield return new WaitForSeconds (1f / fps);
		}
		fin.Invoke ();
		yield break;
	}

	IEnumerator playLoopedAnim(string n, float fps, int l)
	{
		int i = 0;
		while (loopAnim) {
			while (i < l) {
				UpdateSprite (string.Concat (n, i));
				yield return new WaitForSeconds (1f / fps);
				i++;
			}
			i = 0;
		}
		yield break;
	}
}

