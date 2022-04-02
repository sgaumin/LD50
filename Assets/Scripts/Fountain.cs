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

public class Fountain : MonoBehaviour
{
	[SerializeField] private Transform spawnPoint;
	[SerializeField, FloatRangeSlider(0f, 10f)] private FloatRange preparringTime = new FloatRange(2f, 5f);

	private Coroutine preparring;

	private void Start()
	{
		SpawnBucket();
	}

	private void SpawnBucket()
	{
		var b = Instantiate(Prefabs.waterBucketPrefab);
		b.transform.parent = transform;
		b.transform.position = spawnPoint.position;
		b.OnCollect += PrepareNextBucket;
	}

	private void PrepareNextBucket()
	{
		this.TryStartCoroutine(PrepareNextBucketCore(), ref preparring);
	}

	private IEnumerator PrepareNextBucketCore()
	{
		yield return new WaitForSeconds(preparringTime.RandomValue);
		SpawnBucket();
	}
}