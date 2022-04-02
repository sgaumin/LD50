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

public class WaterBucket : MonoBehaviour
{
	public Action OnCollect;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject == Player.gameObject && !Player.FullOfBucketWater)
		{
			Player.CollectWaterBucket();
			OnCollect?.Invoke();
			Destroy(gameObject);
		}
	}
}