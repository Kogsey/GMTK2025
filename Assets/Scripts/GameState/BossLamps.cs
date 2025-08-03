using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossLamps : MonoBehaviour
{
	private readonly List<LightHelper> lights = new();
	public LightHelper HangingLampPrefab;
	public Tilemap Foreground;

	public int roomRadiusTiles;
	private float RoomRadius => roomRadiusTiles * Foreground.cellSize.x;
	private Rect SpawnArea => Rect.MinMaxRect(-RoomRadius, MinLightHeight, RoomRadius, MaxLightHeight);

	public bool RegenLights;
	public int MinLights;
	public int MaxLights;
	public float MinLightHeight;
	public float MaxLightHeight;

	private void Start()
		=> GenerateLights();

	private void ClearLights()
	{
		lights.ForEach(light => Destroy(light.gameObject));
		lights.Clear();
	}

	public void GenerateLights()
	{
		int count = Random.Range(MinLights, MaxLights + 1);

		for (int i = 0; i < count; i++)
		{
			LightHelper light = Instantiate(HangingLampPrefab);
			Vector2 lightSize = light.SpriteRenderer.bounds.size;
			float lightPPU = light.SpriteRenderer.sprite.pixelsPerUnit;
			//Vector2 lightPivot = light.SpriteRenderer.sprite.pivot * lightPPU;
			//float lightTopOffset = lightSize.y - lightPivot.y;

			light.transform.position = GetLightPosition(SpawnArea, lightSize, lightPPU);
			Vector2 lightTopCentre = new(light.SpriteRenderer.bounds.center.x, light.SpriteRenderer.bounds.max.y);
			RaycastHit2D hit = Physics2D.Raycast(lightTopCentre, Vector2.up);
			light.SetChainHeight(hit.distance);

			lights.Add(light);
		}
	}

	private static Vector2 GetLightPosition(Rect roomBounds, Vector2 lightSize, float PPU)
	{
		float pixelPadding = 3f / PPU;

		float xPadding = (lightSize.x / 2) + pixelPadding;
		float yPadding = (lightSize.y / 2) + pixelPadding;
		float x = Random.Range(roomBounds.xMin + xPadding, roomBounds.xMax - xPadding);
		float y = Extensions.RandomGaussianMinMax(roomBounds.yMin + yPadding, roomBounds.yMax - yPadding);

		return Extensions.PixelPerfectRound(new Vector2(x, y), PPU);
	}

	private void Update()
	{
		if (RegenLights)
		{
			ClearLights();
			GenerateLights();
			RegenLights = false;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 left = new(-RoomRadius, 3 * Foreground.cellSize.x);
		Vector3 right = new(RoomRadius, 3 * Foreground.cellSize.x);

		Gizmos.DrawLine(left, right);
		Gizmos.DrawLine(new Vector3(0, MinLightHeight, 0), new Vector3(0, MaxLightHeight, 0));
	}
}