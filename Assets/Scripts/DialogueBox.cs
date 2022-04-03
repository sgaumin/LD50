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

public class DialogueBox : MonoBehaviour
{
	[SerializeField] private float displayLetterDuration = 0.05f;
	[SerializeField] private AudioExpress bipSound;
	[SerializeField] private AudioExpress bipTextSound;

	[Header("References")]
	[SerializeField] private Dependency<TextMeshProUGUI> _text;
	[SerializeField] private Dependency<CanvasGroup> _group;

	private CanvasGroup group => _group.Resolve(this);
	private TextMeshProUGUI text => _text.Resolve(this);

	private Coroutine displaying;

	private void Start()
	{
		group.interactable = false;
		group.alpha = 0f;
	}

	public void DisplayTexts(string[] lines, UnityEvent onCompletion = null)
	{
		if (displaying == null)
			displaying = StartCoroutine(DisplayTextsCore(lines, onCompletion));
	}

	private IEnumerator DisplayTextsCore(string[] lines, UnityEvent onCompletion = null)
	{
		group.DOFade(1f, 0.2f).SetEase(Ease.OutSine);
		Player.CancelInteraction = true;
		text.text = "";

		yield return new WaitForSeconds(0.5f);

		foreach (var line in lines)
		{
			text.text = "";
			foreach (var letter in line)
			{
				text.text += letter;
				bipTextSound.Play();
				yield return new WaitForSeconds(displayLetterDuration);
			}

			// Waiting player input
			while (!Input.anyKeyDown)
			{
				yield return null;
			}

			bipSound.Play();
		}

		group.DOFade(0f, 0.2f).SetEase(Ease.OutSine);
		Player.CancelInteraction = false;

		onCompletion?.Invoke();
		displaying = null;
	}
}