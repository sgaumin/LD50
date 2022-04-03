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

public class HUDManager : Singleton<HUDManager>
{
	[Header("References")]
	[SerializeField] private IconHUD bucketIcon;
	[SerializeField] private IconHUD bambooIcon;
	[SerializeField] private Dependency<DialogueBox> _dialog;

	private DialogueBox dialog => _dialog.Resolve(this);

	public Vector2 BucketPosition => bucketIcon.transform.position;
	public Vector2 BambooPosition => bambooIcon.transform.position;

	public void DisplayTexts(string[] lines, UnityEvent onCompletion = null)
	{
		dialog.DisplayTexts(lines, onCompletion);
	}

	public void SetupBambooIcon(int value)
	{
		bambooIcon.Setup(value);
	}

	public void SetupBucketIcon(int value)
	{
		bucketIcon.Setup(value);
	}

	public void UpdateBucketUsage(int value)
	{
		bucketIcon.UpdateUsage(value);
	}

	public void UpdateBambooUsage(int value)
	{
		bambooIcon.UpdateUsage(value);
	}
}