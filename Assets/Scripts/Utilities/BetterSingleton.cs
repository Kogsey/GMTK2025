using UnityEngine;

public class BetterSingleton<T> : MonoBehaviour where T : BetterSingleton<T>
{
	public static T Instance { get; private set; }

	private void Awake()
		=> AwakeInternal();

	protected virtual void AwakeInternal()
	{
		if (Instance == null)
		{
			Instance = (T)this;
		}
		else if (Instance != this)
		{
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(this);
	}
}