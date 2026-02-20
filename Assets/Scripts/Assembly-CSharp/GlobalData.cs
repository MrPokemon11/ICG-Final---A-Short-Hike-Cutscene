using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using QuickUnityTools.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalData : Singleton<GlobalData>
{
	[Serializable]
	public class GameData
	{
		public delegate void CollectionChangedHandler(CollectableItem item, int newAmount, bool equipAction);

		public const string SAVED_MAP_TAG = "SAVED_MAP";

		public string fileName = currentSaveFile;

		public Tags tags = new Tags();

		public CollectionInventory inventory = new CollectionInventory();

		public Dictionary<string, List<PlayerReplayFrame>> playerReplayData = new Dictionary<string, List<PlayerReplayFrame>>();

		private List<string> allCollectedNames = new List<string>();

		[NonSerialized]
		private HashSet<CollectableItem> allCollected;

		public string savedMap
		{
			get
			{
				string text = tags.GetString("SAVED_MAP");
				if (string.IsNullOrEmpty(text) || !text.StartsWith("Game"))
				{
					return "GameScene";
				}
				return text;
			}
			set
			{
				if (!value.StartsWith("Game"))
				{
					Debug.LogError("Cannot save map " + value + " because it doesn't start with the prefix Game");
				}
				else
				{
					tags.SetString("SAVED_MAP", value);
				}
			}
		}

		[field: NonSerialized]
		private event CollectionChangedHandler onAnyCollectionChanged;

		public GameData Clone()
		{
			return new GameData
			{
				fileName = fileName.ToString(),
				tags = tags.Clone(),
				inventory = inventory.Clone(),
				playerReplayData = playerReplayData.ToDictionary((KeyValuePair<string, List<PlayerReplayFrame>> p) => p.Key.ToString(), (KeyValuePair<string, List<PlayerReplayFrame>> p) => p.Value.ToList()),
				allCollectedNames = allCollectedNames.ToList()
			};
		}

		public void EnsureAllCollectedInitalized()
		{
			if (allCollected == null)
			{
				allCollected = new HashSet<CollectableItem>(from name in allCollectedNames
					select CollectableItem.Load(name) into i
					where i != null
					select i);
			}
		}

		public IEnumerable<CollectableItem> GetAllCollected()
		{
			EnsureAllCollectedInitalized();
			return allCollected;
		}

		public int GetCollected(CollectableItem item)
		{
			return tags.GetInt(item.saveTag);
		}

		public void AddCollected(CollectableItem item, int amount, bool equipAction = false)
		{
			int num = tags.GetInt(item.saveTag) + amount;
			tags.SetInt(item.saveTag, num);
			this.onAnyCollectionChanged?.Invoke(item, num, equipAction);
			UpdateAllCollectionCache(item, num);
		}

		private void UpdateAllCollectionCache(CollectableItem item, int total)
		{
			EnsureAllCollectedInitalized();
			if (total > 0)
			{
				if (!allCollected.Contains(item))
				{
					allCollected.Add(item);
					allCollectedNames.Add(item.name);
				}
			}
			else if (allCollected.Contains(item))
			{
				allCollected.Remove(item);
				allCollectedNames.Remove(item.name);
			}
		}

		public void WatchCollected(CollectableItem item, Action<int> onChange)
		{
			tags.WatchInt(item.saveTag, onChange);
		}

		public void UnwatchCollected(CollectableItem item, Action<int> onChange)
		{
			tags.UnwatchInt(item.saveTag, onChange);
		}

		public void WatchAllCollections(CollectionChangedHandler onCollectionChange)
		{
			onAnyCollectionChanged += onCollectionChange;
		}

		public void UnwatchAllCollections(CollectionChangedHandler onCollectionChange)
		{
			onAnyCollectionChanged -= onCollectionChange;
		}
	}

	[Serializable]
	public class CollectionInventory
	{
		[NonSerialized]
		private static CollectableItem FISH_ITEM = CollectableItem.Load("Fish");

		private const int MAX_FISH = 200;

		private List<Fish> heldFish = new List<Fish>();

		private Dictionary<string, Fish> biggestFish = new Dictionary<string, Fish>();

		private Dictionary<string, int> catchCount = new Dictionary<string, int>();

		public CollectionInventory Clone()
		{
			return new CollectionInventory
			{
				heldFish = heldFish.Select((Fish f) => f.Clone()).ToList(),
				biggestFish = biggestFish.ToDictionary((KeyValuePair<string, Fish> p) => p.Key.ToString(), (KeyValuePair<string, Fish> p) => p.Value.Clone()),
				catchCount = catchCount.ToDictionary((KeyValuePair<string, int> p) => p.Key.ToString(), (KeyValuePair<string, int> p) => p.Value)
			};
		}

		public void AddFish(Fish fish)
		{
			if (Singleton<GlobalData>.instance.gameData.GetCollected(FISH_ITEM) >= 200)
			{
				return;
			}
			heldFish.Add(fish);
			string biggestFishKey = GetBiggestFishKey(fish);
			if (biggestFish.ContainsKey(biggestFishKey))
			{
				Fish fish2 = biggestFish[biggestFishKey];
				if (fish.size > fish2.size)
				{
					biggestFish[biggestFishKey] = fish;
				}
			}
			else
			{
				biggestFish.Add(biggestFishKey, fish);
			}
			if (catchCount.ContainsKey(fish.species.name))
			{
				catchCount[fish.species.name]++;
			}
			else
			{
				catchCount.Add(fish.species.name, 1);
			}
			Singleton<GlobalData>.instance.gameData.AddCollected(FISH_ITEM, 1);
		}

		public void RemoveFish(Fish fish)
		{
			heldFish.Remove(fish);
			Singleton<GlobalData>.instance.gameData.AddCollected(FISH_ITEM, -1);
		}

		public IEnumerable<Fish> GetAllFish()
		{
			return heldFish;
		}

		public int GetCatchCount(FishSpecies fishSpecies)
		{
			if (!catchCount.ContainsKey(fishSpecies.name))
			{
				return 0;
			}
			return catchCount[fishSpecies.name];
		}

		public Fish GetBiggestFishRecord(FishSpecies fishSpecies, bool rare)
		{
			string biggestFishKey = GetBiggestFishKey(fishSpecies, rare);
			if (!biggestFish.ContainsKey(biggestFishKey))
			{
				return null;
			}
			return biggestFish[biggestFishKey];
		}

		private string GetBiggestFishKey(Fish fish)
		{
			return GetBiggestFishKey(fish.species, fish.rare);
		}

		private string GetBiggestFishKey(FishSpecies species, bool rare)
		{
			return (rare ? "rare_" : "") + species.name;
		}
	}

	public class LoadGameAsyncOperationBundle : IAsyncOperationBundle
	{
		private GlobalData globalData;

		private Task<GameData> loadFileTask;

		private AsyncOperation sceneAsyncOperation;

		private Action onFail;

		private bool _allowSceneActivation;

		public bool allowSceneActivation
		{
			get
			{
				return _allowSceneActivation;
			}
			set
			{
				_allowSceneActivation = value;
				if (sceneAsyncOperation != null)
				{
					sceneAsyncOperation.allowSceneActivation = value;
				}
			}
		}

		public float progress
		{
			get
			{
				if (sceneAsyncOperation == null)
				{
					return 0f;
				}
				return sceneAsyncOperation.progress;
			}
		}

		public LoadGameAsyncOperationBundle(GlobalData globalData, string filename, Action onFail)
		{
			this.globalData = globalData;
			this.onFail = onFail;
			loadFileTask = Task.Run(delegate
			{
				AutoResetEvent token = new AutoResetEvent(initialState: false);
				GameData result = null;
				FileSystem.LoadObject(filename, delegate(GameData gameData)
				{
					result = gameData;
					token.Set();
				});
				token.WaitOne();
				return result;
			});
			globalData.StartCoroutine(UpdateStatus());
		}

		private IEnumerator UpdateStatus()
		{
			while (!loadFileTask.IsCompleted)
			{
				yield return null;
			}
			if (loadFileTask.Result != null)
			{
				globalData._gameData = loadFileTask.Result;
				sceneAsyncOperation = SceneManager.LoadSceneAsync(globalData._gameData.savedMap);
				sceneAsyncOperation.allowSceneActivation = _allowSceneActivation;
			}
			else
			{
				onFail?.Invoke();
			}
		}
	}

	public const string BEAT_GAME_PREFS_TAG = "BeatGame";

	public const string TITLE_SCENE = "TitleScene";

	public const string CREDITS_SCENE = "CreditsScene";

	public const string GAME_SCENE = "GameScene";

	private static int currentSaveSlot;

	private GameData _gameData;

	private SaveQueue saveQueue;

	public static string currentSaveFile => GetFilenameForSaveSlot(currentSaveSlot);

	public GameData gameData
	{
		get
		{
			EnsureActiveGameDataExists();
			return _gameData;
		}
	}

	public event Action onBeforeSave;

	private void Awake()
	{
		FileSystem.Initalize();
		saveQueue = new SaveQueue(this);
	}

	private void EnsureActiveGameDataExists()
	{
		if (_gameData == null)
		{
			Debug.Log("Loading game data lazily...");
			_gameData = FileSystem.LoadObjectUnsafe<GameData>(currentSaveFile);
			if (_gameData == null)
			{
				_gameData = new GameData();
			}
		}
	}

	public void SaveGame(Action onFinish)
	{
		this.onBeforeSave?.Invoke();
		PlayerPrefsAdapter.Save();
		FileSystem.SaveObject(currentSaveFile, gameData, 1000000, onFinish);
	}

	public SaveQueue SaveGameAsync()
	{
		this.onBeforeSave?.Invoke();
		PlayerPrefsAdapter.Save();
		GameData clone = gameData.Clone();
		saveQueue.Enqueue(clone, 1000000);
		return saveQueue;
	}

	public void LoadGameForCheats()
	{
		_gameData = FileSystem.LoadObjectUnsafe<GameData>(currentSaveFile);
		if (_gameData != null)
		{
			LevelController.loadSaveRegion = true;
			SceneManager.LoadScene(_gameData.savedMap);
		}
	}

	public IAsyncOperationBundle LoadGameAsync(Action onFail)
	{
		_gameData = new GameData();
		LevelController.loadSaveRegion = true;
		return new LoadGameAsyncOperationBundle(this, currentSaveFile, onFail);
	}

	public SimpleAsyncOperationBundle NewGameAsync()
	{
		_gameData = new GameData();
		return new SimpleAsyncOperationBundle(SceneManager.LoadSceneAsync("GameScene"));
	}

	public bool DoesSaveExist()
	{
		return FileSystem.Exists(currentSaveFile);
	}

	public void ResetGameData()
	{
		_gameData = new GameData();
	}

	public static void SetSaveSlot(int slot)
	{
		if (SceneManager.GetActiveScene().name == "TitleScene")
		{
			currentSaveSlot = slot;
			Singleton<GlobalData>.instance._gameData = null;
		}
		else
		{
			Debug.LogWarning("Can only change save slot on the title screen.");
		}
	}

	public static string GetFilenameForSaveSlot(int slot)
	{
		return string.Format("GameSaveNew{0}.mountain", (slot > 0) ? slot.ToString() : "");
	}

	public static void AddSerializationSurrogates(BinaryFormatter bf)
	{
		SurrogateSelector surrogateSelector = new SurrogateSelector();
		Vector3SerializationSurrogate surrogate = new Vector3SerializationSurrogate();
		surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), surrogate);
		QuaternionSerializationSurrogate surrogate2 = new QuaternionSerializationSurrogate();
		surrogateSelector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), surrogate2);
		bf.Binder = new CustomSerializationBinder();
		bf.SurrogateSelector = surrogateSelector;
	}
}
