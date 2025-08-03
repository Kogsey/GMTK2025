using UnityEngine.SceneManagement;

public class GameplayLoop : BetterSingleton<GameplayLoop>
{
	public PlayerData PlayerData { get; private set; }
	public int Level { get; private set; }
	public int DeathCounter { get; private set; }

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if ((IsRealInstance && scene.name == StateManager.GameScreen) || scene.name == StateManager.BossScreen)
		{
			PlayerController controller = FindAnyObjectByType<PlayerController>();
			if (controller != null)
			{
				controller.WriteData(PlayerData);
				PlayerData = null;
			}

			TileMapGenerator tileMapGenerator = FindAnyObjectByType<TileMapGenerator>();
			if (tileMapGenerator != null)
			{
				tileMapGenerator.GenerateTileMap(Level);
			}
		}
	}

	public void OnLevelCompleted()
	{
		PlayerController controller = FindAnyObjectByType<PlayerController>();
		PlayerData = controller.ReadData();
		Level++;

		if (Level >= 10)
			SceneManager.LoadScene(StateManager.BossScreen);
		else
			SceneManager.LoadScene(StateManager.GameScreen);
	}

	public void OnBossBeaten()
	{
		PlayerController controller = FindAnyObjectByType<PlayerController>();
		PlayerData = controller.ReadData();
		Level = 0;
		SceneManager.LoadScene(StateManager.WinScreen);
	}

	public void OnDeath()
	{
		Level = 1;
		DeathCounter++;
		SceneManager.LoadScene(StateManager.LoseScreen);
	}

	private void OnEnable()
	{
		if (IsRealInstance)
			SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		if (IsRealInstance)
			SceneManager.sceneLoaded -= OnSceneLoaded;
	}
}