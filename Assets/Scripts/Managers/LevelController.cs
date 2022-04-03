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
	[SerializeField] private GameObject blocker;

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
			StartCoroutine(InactiveBlocker());
		}
	}

	public void ForceInactiveBlocker()
	{
		StartCoroutine(InactiveBlocker());
	}

	private IEnumerator InactiveBlocker()
	{
		Player.CancelInteraction = true;
		SetCameraTarget(blocker.transform);
		yield return new WaitForSeconds(1f);
		blocker.SetActive(false);
		Level.GenerateImpulse();
		yield return new WaitForSeconds(1f);
		ResetCameraTarget();
		Player.CancelInteraction = false;
	}
}