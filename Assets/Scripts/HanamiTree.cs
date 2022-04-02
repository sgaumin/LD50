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

public class HanamiTree : MonoBehaviour
{
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
			spirit.Fade();
			spriteRenderer.sprite = wateredSprite;
			Level.CheckLevelCompletion();
		}
	}

	private bool hasBeenWatered;
}