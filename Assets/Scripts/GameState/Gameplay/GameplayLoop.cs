using UnityEngine.SceneManagement;

public class GameplayLoop : BetterSingleton<GameplayLoop>
{
	public PlayerData PlayerData;
	public int Level { get; private set; }

	protected override void AwakeInternal()
	{
		base.AwakeInternal();
		TileMapGenerator tileMapGenerator = FindAnyObjectByType<TileMapGenerator>();
		if (tileMapGenerator != null)
		{
			tileMapGenerator.GenerateTileMap(Level);
		}

		PlayerController controller = FindAnyObjectByType<PlayerController>();
		if (controller != null)
		{
			controller.UpdateData(PlayerData);
		}
	}

	public void OnLevelCompleted()
	{
		Level++;
		SceneManager.LoadScene(StateManager.GameScreen);
	}

	public void OnDeath()
	{
		Level = 1;
		SceneManager.LoadScene(StateManager.LoseScreen);
	}
}