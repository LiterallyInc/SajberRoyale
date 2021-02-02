using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(VA_PressToThrow))]
public class VA_PressToThrow_Editor : VA_Editor<VA_PressToThrow>
{
	protected override void OnInspector()
	{
		DrawDefault("Camera");
		DrawDefault("Requires");
		BeginError(Any(t => t.Speed < 0.0f));
			DrawDefault("Speed");
		EndError();
		BeginError(Any(t => t.Prefabs == null || t.Prefabs.Count == 0 || t.Prefabs.Exists(p => p == null) == true));
			DrawDefault("Prefabs");
		EndError();
	}
}
#endif

// This component spins the current GameObject
[AddComponentMenu("Volumetric Audio/VA Press To Throw")]
public class VA_PressToThrow : MonoBehaviour
{
	[Tooltip("The camera the throw will come from (default = MainCamera)")]
	public Camera Camera;

	[Tooltip("The key that must be held down to mouse look")]
	public KeyCode Requires = KeyCode.Mouse0;

	[Tooltip("The layer mask used when raycasting into the scene")]
	public float Speed = 10.0f;

	[Tooltip("The prefabs that will be thrown")]
	public List<GameObject> Prefabs;

	// Called every frame
	protected virtual void Update()
	{
		// Make sure the camera exists
		Camera = VA_Helper.GetCamera(Camera);

		if (Camera != null && Prefabs != null)
		{
			// The required key is down?
			if (Input.GetKeyDown(Requires) == true)
			{
				// Find the ray for this screen position
				var ray      = Camera.ScreenPointToRay(Input.mousePosition);
				var rotation = Quaternion.LookRotation(ray.direction);

				// Loop through all prefabs and spawn them
				for (var i = Prefabs.Count - 1; i >= 0; i--)
				{
					var prefab = Prefabs[i];

					if (prefab != null)
					{
						var clone = (GameObject)Instantiate(prefab, ray.origin, rotation);
							
						// Throw with velocity?
						var cloneRigidbody = clone.GetComponent<Rigidbody>();

						if (cloneRigidbody != null)
						{
							cloneRigidbody.velocity = clone.transform.forward * Speed;
						}
					}
				}
			}
		}
	}
}