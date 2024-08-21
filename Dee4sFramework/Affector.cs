using UnityEngine;
using System.Collections;

public class Affector : MonoBehaviour
{
	public Entity affectedEntity;
	public bool EnableStat = false;

	public void Enable(bool val)
	{
		EnableStat = val;
	}

	public void Update()
	{
		if (EnableStat && affectedEntity != null) {
			UpdateStat ();
		}
	}

	public virtual void UpdateStat()
	{
		
	}
}

