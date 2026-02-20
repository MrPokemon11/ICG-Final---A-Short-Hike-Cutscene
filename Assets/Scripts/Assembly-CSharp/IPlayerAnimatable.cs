using UnityEngine;

public interface IPlayerAnimatable
{
	Collider collider { get; }

	Transform transform { get; }

	GameObject heldItem { get; }

	Transform nearbyInteractable { get; }

	WaterRegion waterRegion { get; }

	bool isGrounded { get; }

	bool isSwimming { get; }

	bool isGliding { get; }

	bool isClimbing { get; }

	bool isRunning { get; }

	bool isSliding { get; }

	bool isMounted { get; }

	float maximumWalkSpeed { get; }

	float waterY { get; }

	Vector3 relativeVelocity { get; }

	Rigidbody movingPlatform { get; }

	Vehicle mountedVehicle { get; }

	Vector3 GetDesiredMovementVector();

	Vector3 GetWorldLookDirection();

	Vector2 GetInputLookDirection();

	void OnSwimArmStroke();
}
