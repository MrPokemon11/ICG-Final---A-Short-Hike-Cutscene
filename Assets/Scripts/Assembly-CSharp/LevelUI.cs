using UnityEngine;

public class LevelUI : ServiceMonoBehaviour
{
	public FPSCounter fpsCounter;

	public GameObject loadingCover;

	public StatusBarUI statusBar;

	public CollectableItem placeholderItem;

	public GameObject saveIcon;

	[SerializeField]
	private PauseMenu pauseMenu;

	[SerializeField]
	private GameObject uiElements;

	public bool pauseMenuOpen => pauseMenu.gameObject.activeSelf;

	public bool isHidden => !uiElements.gameObject.activeSelf;

	private void Start()
	{
		statusBar.ShowCollection(placeholderItem);
		this.RegisterTimer(0.01f, delegate
		{
			statusBar.HideCollection(placeholderItem);
		});
		loadingCover.SetActive(value: true);
		this.RegisterTimer(0.25f, delegate
		{
			loadingCover.SetActive(value: false);
		});
		ShowSaveIcon(show: false);
	}

	public void ShowSaveIcon(bool show)
	{
		saveIcon.SetActive(show);
	}

	public void HideUI(bool hidden)
	{
		uiElements.gameObject.SetActive(!hidden);
	}

	public void OpenPauseMenu()
	{
		pauseMenu.Open();
	}
}
