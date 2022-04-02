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

public class HUDManager : Singleton<HUDManager>
{
	[Header("References")]
	[SerializeField] private IconHUD bucketIcon;
	[SerializeField] private IconHUD bambooIcon;

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