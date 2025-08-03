using UnityEngine;

namespace Utilities.Movement
{
	public class TrackThisTo : MonoBehaviour
	{
		public float TimeUntilStart = 0f;
		public float TimeToReturn = 0.5f;
		public Transform tracked;
		public Vector3 Offset;
		private Vector3 Target => new Vector3(tracked.position.x, tracked.position.y) + Offset;

		[Range(0.1f, 30f)]
		public float trackingStrength = 15;

		private Vector3 StartPos;

		private void Start()
			=> StartPos = transform.position;

		// Update is called once per frame
		private void Update()
		{
			TimeUntilStart -= Time.deltaTime;
			if (TimeUntilStart > 0)
			{
				if (TimeUntilStart < TimeToReturn)
					transform.position = Vector3.Lerp(Target, StartPos, Extensions.SinLerpFactor(TimeUntilStart / TimeToReturn));
			}
			else
				transform.position = Target;
		}
	}
}