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

public class BambooBridge : MonoBehaviour
{
	[SerializeField] private Dependency<Collider2D> _box;
	[SerializeField] private Dependency<SpriteRenderer> _spriteRenderer;

	private bool isBuilt;

	private SpriteRenderer spriteRenderer => _spriteRenderer.Resolve(this);
	private Collider2D box => _box.Resolve(this);

	public bool IsBuilt => isBuilt;

	private void Start()
	{
		box.isTrigger = true;
		spriteRenderer.enabled = false;
	}

	public void TryBuild()
	{
		if (isBuilt || !Player.HasBambooPack) return;

		isBuilt = true;
		Player.BambooPackCount--;
		box.isTrigger = false;
		spriteRenderer.enabled = true;
	}
}