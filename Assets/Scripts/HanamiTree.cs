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

public class HanamiTree : MonoBehaviour
{
	[SerializeField] private Dependency<Animator> _animator;

	private Animator animator => _animator.Resolve(this);

	public bool HasBeenWatered
	{
		get => hasBeenWatered; set
		{
			hasBeenWatered = value;
			Level.CheckLevelCompletion();
		}
	}

	private bool hasBeenWatered;
}