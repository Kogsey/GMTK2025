﻿using UnityEngine;
using Utilities.Timing;

namespace Utilities.Movement
{
	public class GroundCheck : MonoBehaviour
	{
		private bool onGround;
		private Rigidbody2D rigidBody;

		[Header("Collider Settings")]
		[SerializeField][Tooltip("Length of the ground-checking collider")] private float groundLength = 0.2f;

		[SerializeField][Tooltip("Distance between the ground-checking colliders")] private float colliderOffsetX = 0.18f;
		[SerializeField][Tooltip("Distance between the ground-checking colliders")] private float colliderOffsetY = -0.72f;
		[SerializeField][Tooltip("Cannot be grounded if moving upwards")] private bool accountForVelocity;
		[SerializeField][Tooltip("The maximum speed that can be moved upwards and still be grounded")] private float maxVelocity;
		[SerializeField][Tooltip("The timer for coyote time")] private CountUpTimer airTimer;

		[SerializeField][Tooltip("Checks for what to count as ground")] private ContactFilter2D contactFilter;

		private Vector3 LColliderOffset => new(colliderOffsetX, colliderOffsetY);
		private Vector3 RColliderOffset => new(-colliderOffsetX, colliderOffsetY);

		private void Start() => rigidBody = GetComponent<Rigidbody2D>();

		// Update is called once per frame
		private void Update()
		{
			airTimer.Tick();
			onGround = VelocityCheck && (Raycast(transform.position + LColliderOffset)
				|| Raycast(transform.position + RColliderOffset));
			if (onGround)
				airTimer.Reset();
		}

		private RaycastHit2D Raycast(Vector2 start)
			=> Physics2D.defaultPhysicsScene.Raycast(start, Vector2.down, groundLength, contactFilter);

		private void OnDrawGizmos()
		{
			//Draw the ground colliders on screen for debug purposes
			Gizmos.color = onGround ? Color.green : Color.red;
			Gizmos.DrawLine(transform.position + LColliderOffset, transform.position + LColliderOffset + (Vector3.down * groundLength));
			Gizmos.DrawLine(transform.position + RColliderOffset, transform.position + RColliderOffset + (Vector3.down * groundLength));
		}

		/// <summary> Remove double jump on floor glitch </summary>
		private bool VelocityCheck
			=> !accountForVelocity || rigidBody.velocityY < maxVelocity;

		//Send ground detection to other scripts
		public bool GetGrounded()
			=> !airTimer.Check();

		public bool GetGroundedPure()
			=> onGround;

		public float TimeSinceGrounded()
			=> airTimer.Time;
	}
}