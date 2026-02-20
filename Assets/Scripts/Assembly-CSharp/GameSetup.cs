using InControl;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

public class GameSetup : MonoBehaviour
{
	[FormerlySerializedAs("managerPrefab")]
	public GameObject inputManagerPrefab;

	public GameObject touchscreenInputPrefab;

	public ShaderVariantCollection warmUpShaders;

	public bool isSetupFinished { get; private set; }

	private void Awake()
	{
		PlayerPrefsAdapter.Initalize();
	}

	public void Start()
	{
		if (!InputManager.IsSetup)
		{
			SetupInputManager();
			IWaitable waitable = GameSettings.LoadSettingsPrefs();
			ExtendedQualitySettings.LoadSettingsPrefs();
			Addressables.InitializeAsync();
			warmUpShaders.WarmUp();
			WaitFor.WithCoroutine(this, waitable, delegate
			{
				isSetupFinished = true;
			});
		}
		else
		{
			isSetupFinished = true;
		}
	}

	private void SetupInputManager()
	{
		inputManagerPrefab.Clone();
		if (Application.isMobilePlatform)
		{
			Object.DontDestroyOnLoad(touchscreenInputPrefab.Clone());
		}
		CrossPlatform.ConfigureInputManager();
	}

	public void EnableXInput()
	{
		Object.Destroy(Object.FindObjectOfType<InControlManager>().gameObject);
		inputManagerPrefab.GetComponent<InControlManager>().enableXInput = true;
		SetupInputManager();
	}
}
