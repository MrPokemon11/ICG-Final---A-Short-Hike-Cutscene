using System;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour, IFloater
{
	public float floatForce = 10f;

	public float maxFloatRiseSpeed = 3f;

	public float floatDrag = 1f;

	public float floatRollDrag = 5f;

	public float currentForce = 10f;

	[Header("Optional Effects")]
	public GameObject waterDecalPrefab;

	public AudioClip enterWaterClip;

	public ParticleSystem splash;

	public Renderer freezeIfInvisible;

	private List<WaterRegion> regions = new List<WaterRegion>();

	private Rigidbody body;

	public bool inWater => regions.Count > 0;

	public WaterRegion waterRegion
	{
		get
		{
			if (regions.Count != 0)
			{
				return regions[0];
			}
			return null;
		}
	}

	public event Action<WaterRegion> onWaterRegionChanged;

	private void Awake()
	{
		body = GetComponent<Rigidbody>();
	}

	public void RegisterWaterRegion(WaterRegion region)
	{
		regions.Add(region);
		if (regions.Count == 1)
		{
			CreateWaterPlopEffects();
			this.onWaterRegionChanged?.Invoke(waterRegion);
		}
	}

	public void UnregisterWaterRegion(WaterRegion region)
	{
		regions.Remove(region);
		if (regions.Count == 0)
		{
			this.onWaterRegionChanged?.Invoke(waterRegion);
		}
	}

	private void CreateWaterPlopEffects()
	{
		if (waterDecalPrefab != null)
		{
			for (int i = 0; i < 3; i++)
			{
				this.RegisterTimer(0.15f * (float)i, delegate
				{
					SpawnWaterRipple();
				});
			}
		}
		if (enterWaterClip != null)
		{
			enterWaterClip.Play();
		}
		if (splash != null)
		{
			splash.Play();
		}
	}

	public WaterDecal SpawnWaterRipple()
	{
		if (waterDecalPrefab != null && regions.Count > 0)
		{
			WaterDecal component = waterDecalPrefab.CloneAt(base.transform.position).GetComponent<WaterDecal>();
			component.region = regions[0];
			return component;
		}
		return null;
	}

	private void FixedUpdate()
	{
		if (freezeIfInvisible != null)
		{
			if (!freezeIfInvisible.isVisible)
			{
				if (!body.isKinematic && body.linearVelocity.sqrMagnitude < 25f)
				{
					body.isKinematic = true;
				}
				return;
			}
			if (body.isKinematic)
			{
				body.isKinematic = false;
			}
		}
		if (regions.Count > 0)
		{
			WaterRegion waterRegion = regions[0];
			float waterY = waterRegion.GetWaterY(base.transform.position);
			Vector3 velocity = body.linearVelocity;
			if (velocity.y < maxFloatRiseSpeed)
			{
				float maxDelta = Mathf.Max(0f, waterY - base.transform.position.y) * floatForce * Time.fixedDeltaTime / body.mass;
				velocity.y = Mathf.MoveTowards(velocity.y, maxFloatRiseSpeed, maxDelta);
			}
			velocity.y *= 1f - Time.fixedDeltaTime * floatDrag;
			if (base.transform.position.y < waterY)
			{
				velocity.x *= 1f - Time.fixedDeltaTime * floatRollDrag;
				velocity.z *= 1f - Time.fixedDeltaTime * floatRollDrag;
			}
			body.linearVelocity = velocity;
			if ((bool)waterRegion.current)
			{
				Vector3 current = waterRegion.current.GetCurrent(base.transform.position);
				body.AddForce(current * currentForce);
			}
		}
	}
}
