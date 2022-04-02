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

public class Bamboo : MonoBehaviour
{
	[Serializable]
	private struct BambooStateData
	{
		public Sprite Sprite;
		public Vector2 offset;
		public Vector2 size;
	}

	[SerializeField] private int startStateIndex = 1;
	[SerializeField] private float evolutionFactor = 0.1f;
	[SerializeField] private List<BambooStateData> states = new List<BambooStateData>();
	[SerializeField, IntRangeSlider(0, 5)] private IntRange bambooStackCreation = new IntRange(0, 3);

	[Header("References")]
	[SerializeField] private Dependency<BoxCollider2D> _box;
	[SerializeField] private Dependency<SpriteRenderer> _spriteRenderer;

	private int currentStateIndex = 0;

	private int CurrentStateIndex
	{
		get => currentStateIndex;
		set
		{
			currentStateIndex = value;
			UpdateState();
		}
	}
	private BambooStateData CurrentData => states[CurrentStateIndex];
	private SpriteRenderer spriteRenderer => _spriteRenderer.Resolve(this);
	private BoxCollider2D box => _box.Resolve(this);

	private void Start()
	{
		CurrentStateIndex = startStateIndex;
	}

	private void FixedUpdate()
	{
		if (box.offset != CurrentData.offset)
			box.offset += evolutionFactor * (CurrentData.offset - box.offset);

		if (box.size != CurrentData.size)
			box.size += evolutionFactor * (CurrentData.size - box.size);
	}

	[ContextMenu("Reduce")]
	public void Reduce()
	{
		if (CurrentStateIndex > 0)
		{
			int count = bambooStackCreation.RandomValue;
			for (int i = 0; i < count; i++)
			{
				var b = Instantiate(Prefabs.bambooPackPrefab);
				b.transform.position = transform.position;
				b.Throw();
			}
		}

		CurrentStateIndex = Mathf.Max(0, CurrentStateIndex - 1);
	}

	[ContextMenu("Grow")]
	public void Grow()
	{
		CurrentStateIndex = Mathf.Min(states.Count - 1, CurrentStateIndex + 1);
	}

	private void UpdateState()
	{
		box.isTrigger = CurrentStateIndex == 0;

		var data = states[CurrentStateIndex];
		if (data.Sprite != null) spriteRenderer.sprite = data.Sprite;
	}
}