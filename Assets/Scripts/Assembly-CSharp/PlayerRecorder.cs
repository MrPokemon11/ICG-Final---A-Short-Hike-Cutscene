using UnityEngine;

public class PlayerRecorder : MonoBehaviour
{
	public float frameTimeLength = 0.1f;

	public bool enableDebugKeys;

	private float recordTime;

	private Player player;

	private float frameCooldown;

	public bool recording { get; private set; }

	public PlayerReplayData replayData { get; private set; }

	private void Start()
	{
		player = Singleton<GameServiceLocator>.instance.levelController.player;
	}

	private void Update()
	{
		if (enableDebugKeys)
		{
			UpdateDebugCommands();
		}
		if (recording)
		{
			if (frameCooldown <= 0f && (replayData.frames.Count <= 0 || replayData.lastFrame.time != recordTime))
			{
				replayData.RecordFrame(player, recordTime);
				frameCooldown = frameTimeLength;
			}
			frameCooldown -= Time.deltaTime;
			recordTime += Time.deltaTime;
		}
	}

	private void UpdateDebugCommands()
	{
		if (Input.GetKeyDown(KeyCode.F8))
		{
			Debug.Log("Started recording...");
			StartRecording();
		}
		if (Input.GetKeyDown(KeyCode.F9))
		{
			Debug.Log("Stopped recording...");
			StopRecording();
		}
		if (Input.GetKeyDown(KeyCode.F10))
		{
			PlayerReplay component = GetComponent<PlayerReplay>();
			if ((bool)component)
			{
				Debug.Log("Playing recording...");
				component.data = replayData;
				component.Play();
			}
		}
		if (Input.GetKeyDown(KeyCode.F11))
		{
			ExportRecording();
		}
		if (Input.GetKeyDown(KeyCode.F12))
		{
			Debug.Log("New recording...");
			NewRecording();
		}
	}

	public void StartRecording()
	{
		if (!recording)
		{
			if (replayData == null)
			{
				NewRecording();
			}
			recording = true;
			player.onWingsFlapped += OnPlayerWingsFlapped;
			player.onGroundJumped += OnPlayerJumped;
		}
	}

	public void ForceCurrentFrame(PlayerReplayFrame.Event frameEvent = (PlayerReplayFrame.Event)0)
	{
		RecordEvent(frameEvent);
	}

	public void StopRecording()
	{
		if (recording)
		{
			recording = false;
			player.onWingsFlapped -= OnPlayerWingsFlapped;
			player.onGroundJumped -= OnPlayerJumped;
		}
	}

	public void NewRecording()
	{
		recordTime = 0f;
		if (replayData != null)
		{
			Object.Destroy(replayData);
		}
		replayData = ScriptableObject.CreateInstance<PlayerReplayData>();
	}

	private void OnPlayerWingsFlapped()
	{
		RecordEvent(PlayerReplayFrame.Event.FlapWings);
	}

	private void OnPlayerJumped()
	{
		RecordEvent(PlayerReplayFrame.Event.Jump);
	}

	private void RecordEvent(PlayerReplayFrame.Event frameEvent)
	{
		if (replayData.lastFrame.time != recordTime)
		{
			replayData.RecordFrame(player, recordTime);
		}
		PlayerReplayFrame lastFrame = replayData.lastFrame;
		lastFrame.eventFlags |= frameEvent;
		replayData.lastFrame = lastFrame;
	}

	public void ExportRecording()
	{
	}
}
