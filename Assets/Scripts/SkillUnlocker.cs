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
using UnityEngine.Events;

public class SkillUnlocker : MonoBehaviour
{
	private enum SkillUnlockerType
	{
		Attack,
		Bucket,
		Collect,
		AttackAir,
		MoreBucket
	}

	[SerializeField] private SkillUnlockerType type;
	[SerializeField] private string[] lines;
	[SerializeField] private UnityEvent onCompletion;
	[SerializeField] private bool updateMusic;
	[SerializeField] private AudioClip newMusic;
	[SerializeField] private AudioExpress collectSound;

	private bool hasBeenTrigered;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (hasBeenTrigered) return;

		if (collision.gameObject == Player.gameObject)
		{
			hasBeenTrigered = true;

			switch (type)
			{
				case SkillUnlockerType.Attack:
					Player.ForceAllowAttack();
					break;

				case SkillUnlockerType.Bucket:
					Player.MaxBucketWater = 1;
					break;

				case SkillUnlockerType.Collect:
					Player.MaxBambooPack = 3;
					break;

				case SkillUnlockerType.MoreBucket:
					Player.MaxBucketWater = 3;
					break;

				case SkillUnlockerType.AttackAir:
					Player.ForceAllowAirAttack();
					break;
			}

			Level.InverseColor();
			Level.GenerateImpulse();
			Level.FreezeTime();

			if (updateMusic)
			{
				Music.FadOut(0.2f);
			}
			collectSound.Play();

			if (lines.Length > 0)
			{
				HUD.DisplayTexts(lines, onCompletion);
			}
		}
	}

	public void UpdateMusic()
	{
		Music.ForceMusic(newMusic);
	}

	[ContextMenu("UpperCase Lines")]
	public void UpperCaseLines()
	{
		List<string> temp = new List<string>();
		foreach (string line in lines)
		{
			temp.Add(line.ToUpper());
		}
		lines = temp.ToArray();
	}
}