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