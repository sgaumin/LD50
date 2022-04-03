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

public class HanamiTree : MonoBehaviour
{
	[SerializeField] private UnityEvent onWattered;

	[SerializeField] private bool forceWatteredAtStart;

	[Space]
	[SerializeField] private Spirit spirit;
	[SerializeField] private Sprite wateredSprite;
	[SerializeField] private ParticleSystem flowerEffect;

	[Header("References")]
	[SerializeField] private Dependency<SpriteRenderer> _spriteRenderer;

	private SpriteRenderer spriteRenderer => _spriteRenderer.Resolve(this);

	public bool HasBeenWatered
	{
		get => hasBeenWatered; set
		{
			hasBeenWatered = value;
			flowerEffect.Play();

			if (spirit != null && spirit.gameObject.activeSelf)
			{
				spirit.Fade();
			}

			spriteRenderer.sprite = wateredSprite;

			onWattered?.Invoke();
		}
	}

	private bool hasBeenWatered;

	private void Start()
	{
		if (forceWatteredAtStart)
		{
			HasBeenWatered = true;
		}
	}
}