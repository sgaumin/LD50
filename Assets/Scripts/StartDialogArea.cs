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

public class StartDialogArea : MonoBehaviour
{
	[SerializeField] private string[] lines;
	[SerializeField] private UnityEvent onCompletion;

	private bool hasBeenTrigered;

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (!gameObject.activeSelf || hasBeenTrigered) return;

		if (collision.gameObject == Player.gameObject)
		{
			hasBeenTrigered = true;
			HUD.DisplayTexts(lines, onCompletion);
		}
	}

	[ContextMenu("UpperCase Lines")]
	public void UpperCaseLines()
	{
		List<string> temp = new List<string>();
		foreach (string line in lines)
		{
			temp.Add(line.ToUpper());
		}
		lines = temp.ToArray();
	}
}