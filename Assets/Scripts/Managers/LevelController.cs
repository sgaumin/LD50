using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using Utils;
using Utils.Dependency;
using static Facade;

public class LevelController : SceneBase
{
	[Header("Stats")]
	[SerializeField] private int startMaxBucketWater = 1;
	[SerializeField] private int startMaxBambooPack = 3;

	public int StartMaxBucketWater => startMaxBucketWater;
	public int StartMaxBambooPack => startMaxBambooPack;

	private List<HanamiTree> trees = new List<HanamiTree>();

	protected override void Start()
	{
		base.Start();

		trees = FindObjectsOfType<HanamiTree>().ToList();
	}

	protected override void Update()
	{
		base.Update();

		if (Input.GetKeyDown(KeyCode.R))
		{
			ReloadScene();
		}
	}

	public void CheckLevelCompletion()
	{
		if (trees.All(x => x.HasBeenWatered))
		{
			ReloadScene();
		}
	}
}