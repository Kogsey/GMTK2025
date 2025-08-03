using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public record Settings
{
	public KeyCode Left = KeyCode.A;
	public KeyCode Right = KeyCode.D;
	public KeyCode Up = KeyCode.W;
	public KeyCode Down = KeyCode.S;
	public KeyCode Jump = KeyCode.Space;
	public KeyCode Dodge = KeyCode.S;
	public KeyCode Primary = KeyCode.Mouse0;
	public KeyCode Secondary = KeyCode.Mouse1;

	public float Volume = 0.5f;
	public bool MuteMusic = false;

	public static readonly Settings DefaultSettings = new();
	public static readonly Settings CelesteStyleSettings = new()
	{
		Left = KeyCode.LeftArrow,
		Right = KeyCode.RightArrow,
		Up = KeyCode.UpArrow,
		Down = KeyCode.DownArrow,
		Jump = KeyCode.C,
		Dodge = KeyCode.DownArrow,
		Primary = KeyCode.Z,
		Secondary = KeyCode.S,
	};
	public static Settings CurrentSettings { get; set; } = DefaultSettings;

	public void CopyControlsFrom(Settings settings)
	{
		Left = settings.Left;
		Right = settings.Right;
		Up = settings.Up;
		Down = settings.Down;

		Jump = settings.Jump;
		Dodge = settings.Dodge;
		Primary = settings.Primary;
		Secondary = settings.Secondary;
	}
}

public class StateManager : BetterSingleton<StateManager>
{
	public const string MainMenuScreen = "Menu";
	public const string HomeScreen = "Home";
	public const string GameScreen = "Primary Level";
	public const string WinScreen = "Win Game";
	public const string LoseScreen = "Lose Game";
	public const string BossScreen = "Boss Level";

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (SceneManager.GetActiveScene().name == MainMenuScreen)
				QuitGame();
			else if (SceneManager.GetActiveScene().name == GameScreen)
				SpawnQuitGamePlayPopUp();
			else if (SceneManager.GetActiveScene().name == HomeScreen)
				SpawnQuitGamePlayPopUp();
		}
	}

	public void SpawnQuitGamePlayPopUp() // TODO: low priority: end game pop-up
		=> SceneManager.LoadScene(MainMenuScreen);

	public static void GameOver()
		=> SceneManager.LoadScene(LoseScreen);

	public static void WinGame()
		=> SceneManager.LoadScene(WinScreen);

	public static void QuitGame()
		=> Application.Quit();

	//SaveSystem.Save();
}