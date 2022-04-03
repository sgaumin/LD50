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

public class Level8Animations : MonoBehaviour
{
	[SerializeField] private AudioClip clip;

	public void PlayEathquake()
	{
		Level.GenerateImpulse();
		Level.InverseColor();
		Level.PlayBlokerSound();
		Music.ForceMusic(clip);
	}
}