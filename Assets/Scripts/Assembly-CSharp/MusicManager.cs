using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

[ResourceSingleton("MusicManager")]
public class MusicManager : Singleton<MusicManager>
{
	public AnimationCurve globalFadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public AudioMixerGroup mixerGroup;

	private SortedList<StackResourceSortingKey, MusicSet> musicSetStack = new SortedList<StackResourceSortingKey, MusicSet>();

	private IList<MusicSet> musicSetList;

	private Dictionary<MusicSet, ActiveMusicSet> activeMusicSets = new Dictionary<MusicSet, ActiveMusicSet>();

	public MusicSet currentMusicSet
	{
		get
		{
			if (musicSetList.Count != 0)
			{
				return musicSetList[0];
			}
			return null;
		}
	}

	public int currentMusicPriority
	{
		get
		{
			if (musicSetList.Count != 0)
			{
				return musicSetStack.Keys[0].priority;
			}
			return -100;
		}
	}

	public List<IMusicLayerController> musicLayerControllers { get; private set; }

	public event Action<IMusicLayerController> onLayerControllerAdded;

	public event Action<IMusicLayerController> onLayerControllerRemoved;

	private void Awake()
	{
		musicLayerControllers = new List<IMusicLayerController>();
		musicSetList = musicSetStack.Values;
	}

	public StackResourceSortingKey RegisterSilenece(int priority)
	{
		return RegisterMusicSet(priority, null);
	}

	public StackResourceSortingKey RegisterMusicSet(int priority, MusicSet set)
	{
		StackResourceSortingKey stackResourceSortingKey = new StackResourceSortingKey(priority, delegate(StackResourceSortingKey releaseKey)
		{
			UnregisterMusicSet(releaseKey);
		});
		MusicSet previous = currentMusicSet;
		musicSetStack.Add(stackResourceSortingKey, set);
		UpdateActiveMusicSet(previous, currentMusicSet);
		return stackResourceSortingKey;
	}

	public void UnregisterAll()
	{
		StackResourceSortingKey[] array = musicSetStack.Keys.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ReleaseResource();
		}
	}

	public void UnregisterMusicSet(StackResourceSortingKey key)
	{
		MusicSet previous = currentMusicSet;
		musicSetStack.Remove(key);
		UpdateActiveMusicSet(previous, currentMusicSet);
	}

	public void RegisterLayerController(IMusicLayerController controller)
	{
		musicLayerControllers.Add(controller);
		this.onLayerControllerAdded?.Invoke(controller);
	}

	public void UnregisterLayerController(IMusicLayerController controller)
	{
		musicLayerControllers.Remove(controller);
		this.onLayerControllerRemoved?.Invoke(controller);
	}

	public ActiveMusicSet GetActiveMusicSet(MusicSet musicSet)
	{
		return activeMusicSets[musicSet];
	}

	public void TrimRetiredActiveMusicSets(float fadeOutTime)
	{
		foreach (ActiveMusicSet value in activeMusicSets.Values)
		{
			if (value.alive)
			{
				value.TrimRetirementTime(fadeOutTime);
			}
		}
	}

	public bool AreOtherSetsAlive(ActiveMusicSet activeMusicSet)
	{
		foreach (ActiveMusicSet value in activeMusicSets.Values)
		{
			if (value != activeMusicSet && value.alive)
			{
				return true;
			}
		}
		return false;
	}

	private void UpdateActiveMusicSet(MusicSet previous, MusicSet next)
	{
		if (previous == next)
		{
			return;
		}
		if (previous != null)
		{
			activeMusicSets[previous].Retire();
		}
		if (next != null)
		{
			if (!activeMusicSets.ContainsKey(next))
			{
				activeMusicSets[next] = new ActiveMusicSet(next, this);
			}
			activeMusicSets[next].Play();
		}
	}

	private void Update()
	{
		foreach (ActiveMusicSet value in activeMusicSets.Values)
		{
			if (value.alive)
			{
				value.Update();
			}
		}
	}
}
