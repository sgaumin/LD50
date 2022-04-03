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

public class LoadNextLevelArea : MonoBehaviour
{
	[SerializeField] private Vector2 movementDirection;
	[SerializeField] private float waitDuration = 1f;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject == Player.gameObject)
		{
			StartCoroutine(LoadNextCore());
		}
	}

	private IEnumerator LoadNextCore()
	{
		Player.CancelInteraction = true;
		Player.ForceMovement(movementDirection);

		yield return new WaitForSeconds(waitDuration);

		Level.LoadNextScene();
	}
}