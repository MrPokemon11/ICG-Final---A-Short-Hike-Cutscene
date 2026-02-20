using UnityEngine;

public interface IFloatingBoxContent
{
	GameObject gameObject { get; }

	Vector2 extraFloatPadding { get; }

	void Configure(Transform target, TextBoxStyleProfile styleProfile);
}
