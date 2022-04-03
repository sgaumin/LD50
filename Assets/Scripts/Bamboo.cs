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

[ExecuteAlways]
public class Bamboo : MonoBehaviour
{
	[SerializeField] private int startStateIndex = 1;
	[SerializeField] private float evolutionFactor = 0.1f;
	[SerializeField, IntRangeSlider(0, 5)] private IntRange bambooStackCreation = new IntRange(0, 3);

	[Header("References")]
	[SerializeField] private Sprite emptySprite;
	[SerializeField] private Sprite fullSprite;
	[SerializeField] private Dependency<BoxCollider2D> _box;
	[SerializeField] private Dependency<SpriteRenderer> _spriteRenderer;

	private int currentStateIndex;
	private bool hasBeenTouched;

	private int CurrentStateIndex
	{
		get => currentStateIndex;
		set
		{
			currentStateIndex = Mathf.Max(0, value);
			UpdateState();
		}
	}
	private SpriteRenderer spriteRenderer => _spriteRenderer.Resolve(this);
	private BoxCollider2D box => _box.Resolve(this);

	private void Start()
	{
		CurrentStateIndex = startStateIndex;
	}

	private void Update()
	{
		if (Application.IsPlaying(gameObject))
		{
			if (CurrentStateIndex == 0) return;

			var reference = new Vector2(1f, CurrentStateIndex);
			spriteRenderer.size += evolutionFactor * (reference - spriteRenderer.size);

			if (box.size != reference)
				box.size += evolutionFactor * (reference - box.size);

			var offsetReference = new Vector2(0f, CurrentStateIndex / 2f);
			if (box.offset != offsetReference)
				box.offset += evolutionFactor * (offsetReference - box.offset);
		}
		else
		{
			box.isTrigger = startStateIndex == 0;
			spriteRenderer.sprite = startStateIndex == 0 ? emptySprite : fullSprite;

			if (startStateIndex == 0)
			{
				box.offset = new Vector2(0f, -0.2f);
				box.size = new Vector2(0.2f, 0.2f);
			}
			else
			{
				var reference = new Vector2(1f, startStateIndex);
				spriteRenderer.size = reference;
				box.size = reference;

				var offsetReference = new Vector2(0f, startStateIndex / 2f);
				box.offset = offsetReference;
			}
		}
	}

	[ContextMenu("Reduce")]
	public void Reduce()
	{
		if (!hasBeenTouched)
		{
			hasBeenTouched = true;
			CurrentStateIndex = startStateIndex;
		}

		if (CurrentStateIndex > 0)
		{
			int count = bambooStackCreation.RandomValue;
			for (int i = 0; i < count; i++)
			{
				var b = Instantiate(Prefabs.bambooPackPrefab);
				b.transform.position = (Vector2)transform.position + new Vector2(0f, CurrentStateIndex / 2f);
				b.Throw();
			}
			CurrentStateIndex--;
		}
	}

	[ContextMenu("Grow")]
	public void Grow()
	{
		CurrentStateIndex++;
	}

	private void UpdateState()
	{
		box.isTrigger = CurrentStateIndex == 0;
		spriteRenderer.sprite = CurrentStateIndex == 0 ? emptySprite : fullSprite;

		if (CurrentStateIndex == 0)
		{
			box.offset = new Vector2(0f, -0.2f);
			box.size = new Vector2(0.2f, 0.2f);
		}
	}
}