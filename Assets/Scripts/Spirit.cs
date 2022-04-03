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

public class Spirit : MonoBehaviour
{
	[SerializeField] private bool checkLevelCompletion;

	[Space]
	[SerializeField] private AudioExpress fadeSound;
	[SerializeField] private Dependency<Animator> _animator;

	private Animator animator => _animator.Resolve(this);

	public void Fade()
	{
		StartCoroutine(FadeCore());
	}

	private IEnumerator FadeCore()
	{
		fadeSound.Play();
		animator.SetTrigger("Fade");
		Level.SetCameraTarget(transform);
		Level.Zoom(5f, 0.5f, Ease.OutSine);

		Player.CancelInteraction = true;

		yield return new WaitForSeconds(2f);

		Level.ResetZoom(0.5f, Ease.OutSine);
		Level.ResetCameraTarget();

		if (checkLevelCompletion)
		{
			yield return new WaitForSeconds(1f);
			Level.CheckLevelCompletion();
		}

		Player.CancelInteraction = false;
	}
}