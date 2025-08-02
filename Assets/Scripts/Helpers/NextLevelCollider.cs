// Ignore Spelling: Collider

using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class NextLevelCollider : MonoBehaviour
{
	public void SetBounds(Rect rect)
	{
		BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
		boxCollider2D.transform.position = rect.center;
		boxCollider2D.size = rect.size;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out PlayerController _))
			BetterSingleton<GameplayLoop>.Instance.OnLevelCompleted();
	}
}