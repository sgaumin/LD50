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

public class BambooPack : MonoBehaviour
{
	[SerializeField] private float collectTime = 0.5f;
	[SerializeField, FloatRangeSlider(0f, 500f)] private FloatRange throwDirectionX = new FloatRange(350f, 450f);
	[SerializeField, FloatRangeSlider(0f, 500f)] private FloatRange throwDirectionY = new FloatRange(250f, 350f);

	[Header("Animations")]
	[SerializeField] private float moveToIconDuration = 0.5f;
	[SerializeField] private float moveToIconScale = 0.3f;
	[SerializeField] private Ease moveToIconEase = Ease.OutSine;

	[Header("References")]
	[SerializeField] private Dependency<Collider2D> _collider2D;
	[SerializeField] private Collider2D circleCollider;
	[SerializeField] private Dependency<Rigidbody2D> _body;

	private Collider2D box => _collider2D.Resolve(this);
	private Rigidbody2D body => _body.Resolve(this);

	private bool canBeCollected;
	private bool hasBeenColleted;

	public void Throw()
	{
		StartCoroutine(ReadyCollect());

		var factor = Player.transform.position.x > transform.position.x ? -1f : 1f;
		body.AddForce(new Vector2(factor * throwDirectionX.RandomValue, throwDirectionY.RandomValue));
	}

	private IEnumerator ReadyCollect()
	{
		yield return new WaitForSeconds(collectTime);
		canBeCollected = true;
		body.Freeze();
		circleCollider.enabled = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		CollisionCheck(collision);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		CollisionCheck(collision);
	}

	private void CollisionCheck(Collider2D collision)
	{
		if (hasBeenColleted) return;

		if (canBeCollected && collision.gameObject == Player.gameObject && !Player.FullOfBambooPack)
		{
			hasBeenColleted = true;
			Player.CollectBambooPack();

			box.enabled = false;
			body.Freeze();
			transform.DOMove(HUD.BambooPosition, moveToIconDuration).SetEase(moveToIconEase).OnComplete(() => Destroy(gameObject));
			transform.DOScale(moveToIconScale, moveToIconDuration);
		}
	}
}