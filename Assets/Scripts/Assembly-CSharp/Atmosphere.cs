using System;
using QuickUnityTools.Audio;
using UnityEngine;

[Serializable]
public struct Atmosphere
{
	[Flags]
	public enum Effects
	{
		Wind = 1,
		Snow = 2,
		Rain = 4,
		SunsetGlow = 8,
		NorthernLights = 0x10,
		WindySnow = 0x20
	}

	public Color fogColor;

	public Color shadowColor;

	[ColorUsage(true, true)]
	public Color lightColor;

	public Color edgeColor;

	public float saturation;

	[EnumFlag]
	public Effects effects;

	public Soundscape ambientSounds;

	public Color cameraColor;
}
