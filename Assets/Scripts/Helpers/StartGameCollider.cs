// Ignore Spelling: Collider

using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameCollider : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out PlayerController _))
			SceneManager.LoadScene(StateManager.GameScreen);
	}
}