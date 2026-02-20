using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
	[Header("Particles")]
	public TrailRenderer[] wingTrails;

	public ParticleSystem dust;

	public ParticleSystem slidingDust;

	public TrailRenderer bodyTrail;

	public ParticleSystem midairJumpPuff;

	public ParticleSystem splashParticles;

	public GameObject waterRingPrefab;

	[Header("Water Ring Settings")]
	public float ringSpawnPeriodFast = 0.25f;

	public float ringSpawnPeriodSlow = 2f;

	public float ringSpawnPeriodMaxSpeed = 10f;

	[Header("Sound")]
	public AudioClip jumpSound;

	public AudioClip flapSound;

	public AudioClip splashSound;

	public AudioSource playerSoundsSource;

	public AudioSource airGlideSource;

	public AudioSource slideSoundSource;

	public float airGlideVolumeSpeed = 2f;

	private PlayerIKAnimator animator;

	private IPlayerAnimatable player;

	private float airGlideMaxVolume;

	private Timer trailTimer;

	private float waterRingTime;

	private bool areWaterEffectsActive;

	private void Start()
	{
		player = GetComponentInParent<IPlayerAnimatable>();
		animator = GetComponentInChildren<PlayerIKAnimator>();
		airGlideMaxVolume = airGlideSource.volume;
		airGlideSource.volume = 0f;
		slideSoundSource.volume = 0f;
	}

	private void Update()
	{
		EnableWingTrails(player.isGliding);
		ParticleSystem.EmissionModule emission = dust.emission;
		emission.enabled = player.isGrounded && player.GetDesiredMovementVector() != Vector3.zero && player.relativeVelocity.SetY(0f).sqrMagnitude > (player.maximumWalkSpeed / 2f).Sqr();
		emission = slidingDust.emission;
		emission.enabled = player.isSliding;
		bodyTrail.emitting = trailTimer != null || player.isRunning;
		slideSoundSource.volume = Mathf.MoveTowards(slideSoundSource.volume, player.isSliding ? 1 : 0, Time.deltaTime * airGlideVolumeSpeed);
		StartOrStopSource(slideSoundSource);
		airGlideSource.volume = Mathf.MoveTowards(airGlideSource.volume, player.isGliding ? airGlideMaxVolume : 0f, Time.deltaTime * airGlideVolumeSpeed);
		StartOrStopSource(airGlideSource);
		if (!areWaterEffectsActive && player.isSwimming && !player.isMounted && player.waterY > player.collider.bounds.min.y)
		{
			CreateBigSplashEffect();
			areWaterEffectsActive = true;
		}
		if (areWaterEffectsActive)
		{
			if (player.waterRegion == null || player.waterY < player.collider.bounds.min.y)
			{
				areWaterEffectsActive = false;
			}
			waterRingTime += Time.deltaTime;
			float num = Mathf.Lerp(ringSpawnPeriodSlow, ringSpawnPeriodFast, player.relativeVelocity.magnitude / ringSpawnPeriodMaxSpeed);
			if (waterRingTime > num)
			{
				waterRingTime = 0f;
				SpawnWaterRingDecal();
			}
		}
	}

	public void ShowBodyTrail(float time)
	{
		Timer.Cancel(trailTimer);
		trailTimer = this.RegisterTimer(time, delegate
		{
			trailTimer = null;
		});
	}

	public void Jump()
	{
		PlaySound(jumpSound);
	}

	public void FlapWings(float pitch = 1f)
	{
		animator.FlapWings();
		midairJumpPuff.Play();
		PlaySound(flapSound, pitch);
	}

	private void CreateBigSplashEffect()
	{
		float waterY = player.waterRegion.GetWaterY(splashParticles.transform.position);
		splashParticles.transform.position = splashParticles.transform.position.SetY(waterY);
		splashParticles.Play();
		PlaySound(splashSound);
		SpawnWaterRingDecal();
		waterRingTime = 0f;
	}

	public void SpawnWaterRingDecal()
	{
		if (!(player.waterRegion == null))
		{
			waterRingPrefab.CloneAt(base.transform.position).GetComponent<WaterDecal>().region = player.waterRegion;
		}
	}

	private void EnableWingTrails(bool enable)
	{
		TrailRenderer[] array = wingTrails;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].emitting = enable;
		}
	}

	private void StartOrStopSource(AudioSource source)
	{
		if (source.volume == 0f && source.isPlaying)
		{
			source.Stop();
		}
		else if (source.volume > 0f && !source.isPlaying)
		{
			source.Play();
		}
	}

	private void PlaySound(AudioClip sound, float pitch = 1f)
	{
		playerSoundsSource.pitch = pitch;
		playerSoundsSource.PlayOneShot(sound);
	}
}
