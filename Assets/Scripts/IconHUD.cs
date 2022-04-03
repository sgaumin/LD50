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

public class IconHUD : MonoBehaviour
{
	[SerializeField] private List<Image> usages = new List<Image>();
	[SerializeField] private Sprite usageOn;
	[SerializeField] private Sprite usageOff;

	public void Setup(int count)
	{
		if (count == 0)
		{
			gameObject.SetActive(false);
			return;
		}

		gameObject.SetActive(true);

		int i = 0;
		foreach (var item in usages)
		{
			item.gameObject.SetActive(i++ < count);
		}
	}

	public void UpdateUsage(int count)
	{
		int i = 0;
		foreach (var item in usages)
		{
			item.sprite = i < count ? usageOn : usageOff;
			i++;
		}
	}
}