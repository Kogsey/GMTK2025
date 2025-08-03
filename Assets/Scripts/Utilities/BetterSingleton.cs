using UnityEngine;

public class BetterSingleton<T> : MonoBehaviour where T : BetterSingleton<T>
{
	public static T Instance { get; private set; }
	protected bool IsBrokenInstance => this != Instance;
	protected bool Uninitialized => Instance == null;

	/// <summary> Use to check if this instance is the real singleton. False if this is one created for a scene and then deleted </summary>
	public bool IsRealInstance => !IsBrokenInstance;

	private void Awake()
		=> AwakeInternal();

	protected virtual void AwakeInternal()
	{
		if (Uninitialized)
		{
			Instance = (T)this;
		}
		else if (IsBrokenInstance)
		{
			DestroyImmediate(this);
			return;
		}

		DontDestroyOnLoad(this);
	}
}