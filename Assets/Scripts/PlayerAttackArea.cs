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
	private bool done;

	private void OnEnable()
	{
		done = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!done && collision.TryGetComponent(out Bamboo b) && Player.HasWaterBucket)
		{
			Player.DoWaterBucketAttack(() =>
			{
				b.Grow();
				done = true;
			});
		}
		else if (!done && collision.TryGetComponent(out HanamiTree t) && !t.HasBeenWatered && Player.HasWaterBucket)
		{
			Player.DoWaterBucketAttack(() =>
			{
				t.HasBeenWatered = true;
				done = true;
			});
		}
		else if (!done && collision.TryGetComponent(out BambooBridge br) && !br.IsBuilt)
		{
			Player.DoNormalAttack(() =>
			{
				var bs = FindObjectsOfType<BambooBridge>().Where(x => !x.IsBuilt).OrderBy(x => Vector2.Distance(x.transform.position, Player.transform.position)).FirstOrDefault();
				bs.TryBuild();
				done = true;
			});
		}
		else if (collision.TryGetComponent(out Bamboo ba))
		{
			Player.DoNormalAttack(() =>
			{
				ba.Reduce();
				done = true;
			});
		}
	}

	private void OnDisable()
	{
		if (!done)
		{
			Player.DoNormalAttack(null);
		}
	}
}