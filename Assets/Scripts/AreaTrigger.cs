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

public class AreaTrigger : MonoBehaviour
{
	[SerializeField] private UnityEvent onTriggerEnter;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject == Player.gameObject)
		{
			onTriggerEnter?.Invoke();
			gameObject.SetActive(false);
		}
	}
}