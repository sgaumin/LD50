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

public class Level10Controller : MonoBehaviour
{
	[SerializeField] private int growingTarget;
	[SerializeField] private float durationBetweenGrow = 0.2f;
	[SerializeField] private AudioClip clip;

	[Header("References")]
	[SerializeField] private Bamboo bamboo;
	[SerializeField] private CanvasGroup group;

	public void StartSequence()
	{
		Player.ForceDisallowAttack();
		StartCoroutine(StartSequenceCore());
	}

	private IEnumerator StartSequenceCore()
	{
		Music.FadOut(0.2f);

		yield return new WaitForSeconds(1f);
		Level.GenerateImpulse();
		Level.PlayBlokerSound();

		for (int i = 0; i < growingTarget; i++)
		{
			yield return new WaitForSeconds(durationBetweenGrow);
			Level.GenerateImpulse();
			Level.PlayBlokerSound();
			bamboo.Grow();
		}

		Player.MaxBambooPack = 0;
		Player.MaxBucketWater = 0;
		Player.WaterBucketCount = 0;

		bamboo.enabled = false;
		Music.ForceMusic(clip);

		yield return new WaitForSeconds(1f);
		group.DOFade(1f, 0.5f).SetEase(Ease.OutSine);
	}
}