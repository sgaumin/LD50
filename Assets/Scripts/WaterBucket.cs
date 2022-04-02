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

public class WaterBucket : MonoBehaviour
{
	public Action OnCollect;

	[Header("Animations")]
	[SerializeField] private float moveToIconDuration = 0.5f;
	[SerializeField] private float moveToIconScale = 0.3f;
	[SerializeField] private Ease moveToIconEase = Ease.OutSine;

	[Header("References")]
	[SerializeField] private Dependency<Collider2D> _collider2D;

	private Collider2D box => _collider2D.Resolve(this);

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject == Player.gameObject && !Player.FullOfBucketWater)
		{
			Player.CollectWaterBucket();
			OnCollect?.Invoke();

			box.enabled = false;
			transform.DOMove(HUD.BucketPosition, moveToIconDuration).SetEase(moveToIconEase).OnComplete(() => Destroy(gameObject));
			transform.DOScale(moveToIconScale, moveToIconDuration);
		}
	}
}