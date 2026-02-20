using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InControl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : CheatCodeDetector
{
	public const string DISABLE_LEADERBOARDS = "DisableLeaderboardsCheats";

	public static string[] RELEASE_MENU_CHEATS = new string[9] { "stuffplz", "imstuckplz", "showtimeplz", "nopeplz", "nouiplz", "loadgameplz", "saveplz", "newgameplz", "sitdownplz" };

	public GameObject recorder;

	public Material white;

	public static string[] OPTION_MENU_CHEATS => RELEASE_MENU_CHEATS;

	public static bool cheatsActive { get; private set; }

	private void Start()
	{
		if (cheatsActive || Application.isEditor || Debug.isDebugBuild)
		{
			RegisterAllCheats();
		}
		else if (GameSettings.allowConsoleCheats)
		{
			RegisterConsoleReleaseCheats();
		}
		RegisterCheat("cheatsplz", delegate
		{
			if (!cheatsActive)
			{
				this.RegisterTimer(0.1f, delegate
				{
					RegisterAllCheats();
				});
			}
		});
	}

	private void RegisterAllCheats()
	{
		cheatsActive = true;
		RegisterConsoleReleaseCheats();
		RegisterCheat("bundleplz", delegate
		{
			TerrainBaker.BUNDLE_DRAW_CALLS = true;
		});
		RegisterCheat("unbundleplz", delegate
		{
			TerrainBaker.BUNDLE_DRAW_CALLS = false;
		});
		RegisterCheat("oldterrainplz", delegate
		{
			GameSettings.useBakedTerrain = false;
		});
		RegisterCheat("newterrainplz", delegate
		{
			GameSettings.useBakedTerrain = true;
		});
		RegisterCheat("occonplz", delegate
		{
			Camera.main.useOcclusionCulling = true;
		});
		RegisterCheat("occoffplz", delegate
		{
			Camera.main.useOcclusionCulling = false;
		});
		RegisterCheat("cullonplz", delegate
		{
			GameSettings.useCullingGroups = true;
		});
		RegisterCheat("culloffplz", delegate
		{
			GameSettings.useCullingGroups = false;
		});
		RegisterCheat("pixoffplz", delegate
		{
			Canvas[] array = UnityEngine.Object.FindObjectsOfType<Canvas>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].pixelPerfect = false;
			}
		});
		RegisterCheat("pixonplz", delegate
		{
			Canvas[] array = UnityEngine.Object.FindObjectsOfType<Canvas>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].pixelPerfect = true;
			}
		});
		RegisterCheat("lowqplz", delegate
		{
			TerrainBaker[] array = UnityEngine.Object.FindObjectsOfType<TerrainBaker>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetQuality(highQuality: false);
			}
		});
		RegisterCheat("highqplz", delegate
		{
			TerrainBaker[] array = UnityEngine.Object.FindObjectsOfType<TerrainBaker>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetQuality(highQuality: true);
			}
		});
		RegisterCheat("hidetreesplz", delegate
		{
			TerrainBaker.HIDE_TREES = true;
		});
		RegisterCheat("showtreesplz", delegate
		{
			TerrainBaker.HIDE_TREES = false;
		});
		RegisterCheat("terrianbadplz", delegate
		{
			CullingTerrain.ERROR_MULTIPLIER *= 2f;
		});
		RegisterCheat("terraingoodplz", delegate
		{
			CullingTerrain.ERROR_MULTIPLIER /= 2f;
		});
		RegisterCheat("treebanplz", delegate
		{
			Terrain[] array = UnityEngine.Object.FindObjectsOfType<Terrain>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].treeDistance = 5f;
			}
		});
		RegisterCheat("treebackplz", delegate
		{
			Terrain[] array = UnityEngine.Object.FindObjectsOfType<Terrain>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].treeDistance = 350f;
			}
		});
		RegisterCheat("animplz", delegate
		{
			int num = 0;
			Animator[] array = UnityEngine.Object.FindObjectsOfType<Animator>();
			foreach (Animator animator in array)
			{
				if (!(animator.runtimeAnimatorController == null) && animator.enabled && animator.gameObject.activeInHierarchy)
				{
					bool flag = false;
					Renderer[] componentsInChildren = animator.GetComponentsInChildren<Renderer>();
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						if (componentsInChildren[j].isVisible)
						{
							flag = true;
							break;
						}
					}
					if (flag || animator.cullingMode == AnimatorCullingMode.AlwaysAnimate)
					{
						Debug.Log("Active (" + animator.cullingMode.ToString() + ") animator: " + animator.name, animator.gameObject);
						num++;
					}
				}
			}
			Debug.Log("Found total: " + num);
		});
		RegisterCheat("60fpsplz", delegate
		{
			Application.targetFrameRate = 15;
			QualitySettings.vSyncCount = 0;
		});
		RegisterCheat("120fpsplz", delegate
		{
			Application.targetFrameRate = 120;
			QualitySettings.vSyncCount = 0;
		});
		RegisterCheat("omgresplz", delegate
		{
			Singleton<ServiceLocator>.instance.Locate<PixelFilterAdjuster>().SetPixelSize(7000, 1750);
		});
		RegisterCheat("speedrunplz", delegate
		{
			LevelController.speedrunClockActive = !LevelController.speedrunClockActive;
		});
		RegisterCheat("swapiplz", delegate
		{
			InputMapperExtensions.swapInteractButtons = !InputMapperExtensions.swapInteractButtons;
		});
		RegisterCheat("mapsplz", delegate
		{
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("TreasureMap"), 4);
			Singleton<GlobalData>.instance.gameData.tags.SetBool("TMap1");
			Singleton<GlobalData>.instance.gameData.tags.SetBool("TMap2");
			Singleton<GlobalData>.instance.gameData.tags.SetBool("TMap3");
			Singleton<GlobalData>.instance.gameData.tags.SetBool("TMap4");
		});
		RegisterCheat("fishplz", delegate
		{
			FishingActions.FISH_CHEAT_MULTIPLIER = ((FishingActions.FISH_CHEAT_MULTIPLIER == 10f) ? 1f : 10f);
		});
		RegisterCheat("allspeciesplz", delegate
		{
			FishSpecies[] array = FishSpecies.LoadAll();
			foreach (FishSpecies fishSpecies in array)
			{
				Singleton<GlobalData>.instance.gameData.inventory.AddFish(new Fish(fishSpecies, rare: false));
			}
		});
		RegisterCheat("allrareplz", delegate
		{
			FishSpecies[] array = FishSpecies.LoadAll();
			foreach (FishSpecies fishSpecies in array)
			{
				Singleton<GlobalData>.instance.gameData.inventory.AddFish(new Fish(fishSpecies, rare: true));
			}
		});
		RegisterCheat("coinsplz", delegate
		{
			_ = Singleton<GameServiceLocator>.instance.levelController.player;
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("Coin"), 50);
		});
		RegisterCheat("sunhatplz", delegate
		{
			_ = Singleton<GameServiceLocator>.instance.levelController.player;
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("Sunhat"), 1);
		});
		RegisterCheat("featherplz", delegate
		{
			_ = Singleton<GameServiceLocator>.instance.levelController.player;
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("GoldenFeather"), 1);
		});
		RegisterCheat("shellsplz", delegate
		{
			_ = Singleton<GameServiceLocator>.instance.levelController.player;
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("Shell"), 14);
		});
		RegisterCheat("shovelplz", delegate
		{
			_ = Singleton<GameServiceLocator>.instance.levelController.player;
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("Shovel"), 1);
		});
		RegisterCheat("shoesplz", delegate
		{
			_ = Singleton<GameServiceLocator>.instance.levelController.player;
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("RunningShoes"), 1);
		});
		RegisterCheat("greedyplz", delegate
		{
			GlobalData.GameData gameData = Singleton<GlobalData>.instance.gameData;
			CollectableItem[] array = Resources.LoadAll<CollectableItem>("Items/");
			foreach (CollectableItem item in array)
			{
				gameData.AddCollected(item, 1);
			}
			DisableLeaderboards();
		});
		RegisterCheat("markrulz", delegate
		{
			Player player = Singleton<GameServiceLocator>.instance.levelController.player;
			player.runMultiplier = 2.5f;
			player.jumpSpeed = 45f;
			TriggerCheat("stuffplz");
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("GoldenFeather"), 5);
			DisableLeaderboards();
		});
		RegisterCheat("whiteplz", delegate
		{
			IEnumerable<MonoBehaviour> first = UnityEngine.Object.FindObjectsOfType<CollectOnTouch>().Cast<MonoBehaviour>();
			IEnumerable<MonoBehaviour> second = UnityEngine.Object.FindObjectsOfType<Chest>().Cast<MonoBehaviour>();
			IEnumerable<MonoBehaviour> second2 = UnityEngine.Object.FindObjectsOfType<CollectOnInteract>().Cast<MonoBehaviour>();
			foreach (MonoBehaviour item2 in Enumerable.Concat(second: UnityEngine.Object.FindObjectsOfType<Holdable>().Cast<MonoBehaviour>(), first: first.Concat(second).Concat(second2)))
			{
				MeshRenderer[] componentsInChildren = item2.gameObject.GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer obj in componentsInChildren)
				{
					Texture mainTexture = obj.sharedMaterial.mainTexture;
					obj.sharedMaterial = white;
					obj.material.mainTexture = mainTexture;
				}
			}
		});
		RegisterCheat("ezbballplz", delegate
		{
			VolleyballGameController.EASY_MODE = !VolleyballGameController.EASY_MODE;
			DisableLeaderboards();
		});
		RegisterCheat("boatplz", delegate
		{
			Motorboat motorboat = UnityEngine.Object.FindObjectOfType<Motorboat>();
			motorboat.tankControls = !motorboat.tankControls;
		});
		RegisterCheat("fasterplz", delegate
		{
			Time.timeScale *= 2f;
			DisableLeaderboards();
		});
		RegisterCheat("slowerplz", delegate
		{
			Time.timeScale /= 2f;
			DisableLeaderboards();
		});
		RegisterCheat("tagplz", delegate
		{
			StartCoroutine(DoSetTagRoutine());
		});
		RegisterCheat("falseplz", delegate
		{
			StartCoroutine(DoSetTagRoutine(value: false));
		});
		RegisterCheat("dumptagsplz", delegate
		{
			Debug.Log(Singleton<GlobalData>.instance.gameData.tags.DumpTagData());
		});
		RegisterCheat("deletemytagsplz", delegate
		{
			Singleton<GlobalData>.instance.gameData.tags.Clear();
		});
		RegisterCheat("restoreplz", delegate
		{
			Singleton<GameServiceLocator>.instance.levelController.player.RestoreFeathers();
			DisableLeaderboards();
		});
		RegisterCheat("credsplz", delegate
		{
			SceneManager.LoadScene("CreditsScene");
		});
		RegisterCheat("mainroomplz", delegate
		{
			SceneManager.LoadScene("GameScene");
		});
		RegisterCheat("testroomplz", delegate
		{
			SceneManager.LoadScene("GameTestScene");
		});
		RegisterCheat("oldmovementplz", delegate
		{
			Player player = Singleton<GameServiceLocator>.instance.levelController.player;
			if (player.movementForce == 450f)
			{
				player.maxSpeed = 20.5f;
				player.movementForce = 800f;
			}
			else
			{
				player.maxSpeed = 21f;
				player.movementForce = 450f;
			}
		});
		RegisterCheat("whosawakeplz", delegate
		{
			Rigidbody[] array = UnityEngine.Object.FindObjectsOfType<Rigidbody>();
			foreach (Rigidbody rigidbody in array)
			{
				if (!rigidbody.IsSleeping())
				{
					Debug.Log(rigidbody.name + "is awake", rigidbody);
				}
			}
		});
		RegisterCheat("noactiveplz", delegate
		{
			Rigidbody[] array = UnityEngine.Object.FindObjectsOfType<Rigidbody>();
			foreach (Rigidbody rigidbody in array)
			{
				if (rigidbody.name != "Player" && !rigidbody.IsSleeping())
				{
					rigidbody.gameObject.SetActive(value: false);
				}
			}
		});
		RegisterCheat("noaudiosync", delegate
		{
			ActiveMusicLayer.RESYNC_LAYERS = !ActiveMusicLayer.RESYNC_LAYERS;
		});
		RegisterCheat("debugaudio", delegate
		{
			ActiveMusicLayer.DEBUG_AUDIO = !ActiveMusicLayer.DEBUG_AUDIO;
		});
		RegisterCheat("updatebase1plz", delegate
		{
			BaseResolutionHandler.SetBaseResolution(426, 240);
		});
		RegisterCheat("updatebase2plz", delegate
		{
			BaseResolutionHandler.SetBaseResolution(384, 216);
		});
		RegisterCheat("aunttalk1plz", delegate
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool("$AuntCheat1");
		});
		RegisterCheat("aunttalk2plz", delegate
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool("$AuntCheat2");
		});
		RegisterCheat("screenshotplz", delegate
		{
			string text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/screenshot" + UnityEngine.Random.Range(0, 1000) + ".png";
			ScreenCapture.CaptureScreenshot(text);
			Debug.Log("Screenshot at: " + text);
			Resources.Load<AudioClip>("Camera").Play();
		});
		RegisterCheat("cinemaplz", delegate
		{
			GameObject cinemaCamera = Singleton<GameServiceLocator>.instance.levelController.cinemaCamera;
			cinemaCamera.SetActive(!cinemaCamera.activeSelf);
			if (cinemaCamera.activeSelf)
			{
				TriggerCheat("hideuiplz");
			}
			else
			{
				TriggerCheat("showuiplz");
			}
		});
		RegisterCheat("happyplz", delegate
		{
			StartCoroutine(DoHappyRoutine());
		});
		RegisterCheat("aunteplz", delegate
		{
			StartCoroutine(DoAuntRoutine());
		});
		RegisterCheat("tripplz", delegate
		{
			FishingActions.ALWAYS_TRIP = !FishingActions.ALWAYS_TRIP;
		});
		RegisterCheat("hideuiplz", delegate
		{
			Singleton<GameServiceLocator>.instance.levelUI.HideUI(hidden: true);
		});
		RegisterCheat("showuiplz", delegate
		{
			Singleton<GameServiceLocator>.instance.levelUI.HideUI(hidden: false);
		});
		RegisterCheat("showalluiplz", delegate
		{
			Singleton<GameServiceLocator>.instance.levelUI.gameObject.SetActive(value: true);
		});
		RegisterCheat("hidealluiplz", delegate
		{
			Singleton<GameServiceLocator>.instance.levelUI.gameObject.SetActive(value: false);
		});
		RegisterCheat("recordplz", delegate
		{
			recorder.gameObject.SetActive(!recorder.activeSelf);
		});
		RegisterCheat("randomplz", delegate
		{
			Randomize();
		});
	}

	public void RegisterConsoleReleaseCheats()
	{
		cheatsActive = true;
		RegisterCheat("stuffplz", delegate
		{
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("RunningShoes"), 1);
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("Shovel"), 1);
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("GoldenFeather"), 5);
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("Bucket"), 1);
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("Coin"), 300);
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("FishingRod"), 1);
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("Pickaxe"), 1);
		});
		RegisterCheat("imstuckplz", delegate
		{
			LevelController levelController = Singleton<GameServiceLocator>.instance.levelController;
			levelController.player.body.position = levelController.defaultSpawn.transform.position;
		});
		RegisterCheat("nopeplz", delegate
		{
			_ = Singleton<GameServiceLocator>.instance.levelController.player;
			Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("GoldenFeather"), -1);
		});
		RegisterCheat("showtimeplz", delegate
		{
			GameObject obj = GameObject.Find("FriendlyCameraSetup").transform.GetChild(0).gameObject;
			obj.SetActive(!obj.activeSelf);
		});
		RegisterCheat("toggleuiplz", delegate
		{
			LevelUI levelUI = Singleton<GameServiceLocator>.instance.levelUI;
			levelUI.HideUI(!levelUI.isHidden);
		});
		RegisterCheat("nouiplz", delegate
		{
			StartCoroutine(ToggleUISafe());
		});
		RegisterCheat("setposplz", delegate
		{
			Player player = Singleton<GameServiceLocator>.instance.levelController.player;
			player.startPosition = player.transform.position;
		});
		RegisterCheat("returnplz", delegate
		{
			Player player = Singleton<GameServiceLocator>.instance.levelController.player;
			player.body.position = player.startPosition;
		});
		RegisterCheat("loadgameplz", delegate
		{
			Singleton<GlobalData>.instance.LoadGameForCheats();
		});
		RegisterCheat("newgameplz", delegate
		{
			WaitFor.WithCoroutine(Singleton<GlobalData>.instance, Singleton<GlobalData>.instance.NewGameAsync(), delegate
			{
				Singleton<GlobalData>.instance.ResetGameData();
			});
		});
		RegisterCheat("saveplz", delegate
		{
			Singleton<GlobalData>.instance.SaveGame(null);
		});
		RegisterCheat("creditsplz", delegate
		{
			SceneManager.LoadScene("CreditsScene");
		});
		RegisterCheat("sitdownplz", delegate
		{
			StartCoroutine(DoSitDownRoutine());
		});
	}

	private IEnumerator TestFishingRod()
	{
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		if ((bool)player.heldItem && (bool)player.heldItem.GetComponent<FishingActions>())
		{
			while (!Input.GetKey(KeyCode.P))
			{
				player.UseItem();
				yield return new WaitForSeconds(0.1f);
			}
		}
	}

	private IEnumerator DoSetTagRoutine(bool value = true)
	{
		string tagName = "";
		yield return new WaitForSeconds(0.1f);
		while (!Input.inputString.Contains("\r"))
		{
			if (Input.inputString.Contains("\b") && tagName.Length > 0)
			{
				tagName = tagName.Remove(tagName.Length - 1);
			}
			else if (Input.inputString.Length > 0)
			{
				tagName += Input.inputString;
			}
			if (Input.inputString.Length > 0)
			{
				Debug.Log("typing: " + tagName);
			}
			yield return null;
		}
		Singleton<GlobalData>.instance.gameData.tags.SetBool(tagName, value);
		Debug.Log("Setting " + tagName);
		Singleton<ServiceLocator>.instance.Locate<LevelUI>().statusBar.ShowCollection(CollectableItem.Load("Toast")).HideAndKill(1f);
		DisableLeaderboards();
	}

	private void DisableLeaderboards()
	{
		Singleton<GlobalData>.instance.gameData.tags.SetBool("DisableLeaderboardsCheats");
	}

	private IEnumerator ToggleUISafe()
	{
		Singleton<GameServiceLocator>.instance.levelUI.HideUI(hidden: true);
		yield return new WaitUntil(() => InputManager.ActiveDevice.MenuWasPressed);
		Singleton<GameServiceLocator>.instance.levelUI.HideUI(hidden: false);
	}

	private IEnumerator DoSitDownRoutine()
	{
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		yield return new WaitUntil(player.input.IsRunHeld);
		Action releasePose = player.ikAnimator.Pose(Pose.Sit);
		yield return new WaitUntil(player.input.IsJumpHeld);
		releasePose();
	}

	private IEnumerator DoAuntRoutine()
	{
		IEmotionAnimator aunt = GameObject.Find("AuntMayNPC").GetComponentInChildren<IEmotionAnimator>();
		StackResourceSortingKey key = null;
		while (true)
		{
			if (InputManager.ActiveDevice.LeftBumper.WasPressed)
			{
				StackResourceSortingKey.Release(key);
				key = aunt.ShowEmotion(Emotion.EyesClosed);
			}
			if (InputManager.ActiveDevice.Action4.WasPressed)
			{
				StackResourceSortingKey.Release(key);
				key = aunt.ShowEmotion(Emotion.Surprise);
				this.RegisterTimer(1.2f, delegate
				{
					StackResourceSortingKey.Release(key);
					key = aunt.ShowEmotion(Emotion.EyesClosed);
					this.RegisterTimer(0.3f, delegate
					{
						StackResourceSortingKey.Release(key);
					});
				});
			}
			if (InputManager.ActiveDevice.LeftStickButton.WasPressed)
			{
				StackResourceSortingKey.Release(key);
			}
			if (!InputManager.ActiveDevice.RightStickButton.WasPressed)
			{
				yield return null;
				continue;
			}
			break;
		}
	}

	private IEnumerator DoHappyRoutine()
	{
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		yield return new WaitUntil(player.input.IsDropTapped);
		Action releasePose = player.ikAnimator.Pose(Pose.RaiseArms);
		StackResourceSortingKey releaseEmotion = player.ikAnimator.ShowEmotion(Emotion.Happy);
		yield return new WaitUntil(player.input.IsJumpHeld);
		releasePose();
		releaseEmotion.ReleaseResource();
	}

	private void Randomize()
	{
		string path = Application.streamingAssetsPath + "/random.txt";
		if (File.Exists(path))
		{
			string text = File.ReadAllText(path);
			if (!string.IsNullOrEmpty(text.Trim()))
			{
				UnityEngine.Random.InitState(text.GetHashCode());
				Debug.Log("setting seed to " + text);
			}
		}
		List<GameObject> list = new List<GameObject>();
		List<GameObject> list2 = new List<GameObject>();
		List<GameObject> list3 = new List<GameObject>();
		Chest[] array = UnityEngine.Object.FindObjectsOfType<Chest>();
		foreach (Chest chest in array)
		{
			list.Add(chest.gameObject);
		}
		CollectOnTouch[] array2 = UnityEngine.Object.FindObjectsOfType<CollectOnTouch>();
		foreach (CollectOnTouch collectOnTouch in array2)
		{
			if (collectOnTouch.collectable.name.StartsWith("GoldenFeather") || collectOnTouch.collectable.name.StartsWith("SilverFeather"))
			{
				list.Add(collectOnTouch.gameObject);
			}
			else if (collectOnTouch.collectable.name.StartsWith("Coin"))
			{
				list2.Add(collectOnTouch.gameObject);
			}
		}
		BuriedChest[] array3 = UnityEngine.Object.FindObjectsOfType<BuriedChest>();
		foreach (BuriedChest buriedChest in array3)
		{
			if (buriedChest.name != "BuriedChest (3) _lighthouse")
			{
				list3.Add(buriedChest.gameObject);
			}
		}
		BuriedCollectable[] array4 = UnityEngine.Object.FindObjectsOfType<BuriedCollectable>();
		foreach (BuriedCollectable buriedCollectable in array4)
		{
			list3.Add(buriedCollectable.gameObject);
		}
		IList<Vector3> list4 = list.Select((GameObject x) => x.transform.position).ToList().Shuffle();
		for (int num = 0; num < list.Count; num++)
		{
			list[num].transform.position = list4[num];
		}
		IList<Vector3> list5 = list2.Select((GameObject x) => x.transform.position).ToList().Shuffle();
		for (int num2 = 0; num2 < list2.Count; num2++)
		{
			list2[num2].transform.position = list5[num2];
		}
		IList<Vector3> list6 = list3.Select((GameObject x) => x.transform.position).ToList().Shuffle();
		for (int num3 = 0; num3 < list3.Count; num3++)
		{
			list3[num3].transform.position = list6[num3];
		}
		List<Renderer> renderers = list.SelectMany((GameObject o) => o.GetComponentsInChildren<MeshRenderer>()).Cast<Renderer>().ToList();
		renderers.AddRange(from b in list3
			where b.transform.Find("Mesh") != null
			select b.transform.Find("Mesh").GetComponent<Renderer>());
		CullingRegion[] array5 = UnityEngine.Object.FindObjectsOfType<CullingRegion>();
		foreach (CullingRegion obj in array5)
		{
			List<Renderer> list7 = obj.renderers.ToList();
			list7.RemoveAll((Renderer r) => renderers.Contains(r));
			obj.renderers = list7.ToArray();
		}
		foreach (Renderer item in renderers)
		{
			item.enabled = true;
		}
	}

	protected override void OnCheatActivation(string code)
	{
		base.OnCheatActivation(code);
		Debug.Log("Cheat Activated: " + code);
		LevelUI levelUI = Singleton<ServiceLocator>.instance.Locate<LevelUI>();
		if ((bool)levelUI)
		{
			levelUI.statusBar.ShowCollection(CollectableItem.Load("Toast")).HideAndKill(1f);
		}
	}
}
