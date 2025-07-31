using UnityEngine;

namespace Utilities.Movement
{
	public class TrackThisTo : MonoBehaviour
	{
		public Transform tracked;
		public Vector3 Offset;
		private Vector3 Target => tracked.position + Offset;

		public Vector3 TruePosition;

		[Range(0.1f, 30f)]
		public float trackingStrength = 15;

		private void Start()
			=> TruePosition = transform.position;

		// Update is called once per frame
		private void Update()
			=> transform.position = Target;

		/*		{
					TruePosition = Extensions.SmoothInterpolate((Vector2)TruePosition, (Vector2)Target, trackingStrength);
					TruePosition.z = Target.z;
					transform.position = TruePosition*//*.PixelPerfectClamp()*//*;
				}*/
	}
}