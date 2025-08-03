using UnityEngine.SceneManagement;

public class GameplayLoop : BetterSingleton<GameplayLoop>
{
	public PlayerData PlayerData { get; private set; }
	public int Level { get; private set; }
	public int DeathCounter { get; private set; }

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (IsRealInstance)
		{
			TileMapGenerator tileMapGenerator = FindAnyObjectByType<TileMapGenerator>();
			if (tileMapGenerator != null)
			{
				tileMapGenerator.GenerateTileMap(Level);
			}

			PlayerController controller = FindAnyObjectByType<PlayerController>();
			if (controller != null)
			{
				controller.WriteData(PlayerData);
				PlayerData = null;
			}
		}
	}

	public void OnLevelCompleted()
	{
		PlayerController controller = FindAnyObjectByType<PlayerController>();
		PlayerData = controller.ReadData();
		Level++;
		SceneManager.LoadScene(StateManager.GameScreen);
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

	void OnDisable()
	{
		if (IsRealInstance)
			SceneManager.sceneLoaded -= OnSceneLoaded;
	}
}