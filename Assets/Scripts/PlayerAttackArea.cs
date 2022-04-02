using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Utils;
using Utils.Dependency;
using UnityEngine;
using UnityEngine.UI;
using static Facade;

public class PlayerAttackArea : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (Player.HasWaterBucket)
		{
			if (collision.TryGetComponent(out Bamboo b))
			{
				b.Grow();
			}
			else if (collision.TryGetComponent(out HanamiTree t))
			{
				t.HasBeenWatered = true;
			}
		}
		else
		{
			if (collision.TryGetComponent(out BambooBridge br))
			{
				br.TryBuild();
			}
			else if (collision.TryGetComponent(out Bamboo b))
			{
				b.Reduce();
			}
		}
	}
}