using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(VA_Freeflight))]
public class VA_Freeflight_Editor : VA_Editor<VA_Freeflight>
{
	protected override void OnInspector()
	{
		DrawDefault("EulerAngles");

		Separator();

		DrawDefault("LinearSpeed");
		BeginError(Any(t => t.AngularDampening < 0.0f));
			DrawDefault("LinearDampening");
		EndError();

		Separator();

		DrawDefault("AngularSpeed");
		BeginError(Any(t => t.AngularDampening < 0.0f));
			DrawDefault("AngularDampening");
		EndError();
	}
}
#endif

// This component allows you to control this GameObject using freeflight camera controls
[ExecuteInEditMode]
[AddComponentMenu("Volumetric Audio/VA Freeflight")]
public class VA_Freeflight : MonoBehaviour
{
	[Tooltip("The current Euler rotation in degrees")]
	public Vector3 EulerAngles;

	[Tooltip("The maximum linear speed")]
	public float LinearSpeed = 10.0f;

	[Tooltip("The speed at which the maximum linear speed is reached")]
	public float LinearDampening = 5.0f;

	[Tooltip("The maximum angular speed")]
	public float AngularSpeed = 1000.0f;

	[Tooltip("The speed at which the maximum angular speed is reached")]
	public float AngularDampening = 5.0f;

	[SerializeField]
	private Vector3 linearVelocity;

	[SerializeField]
	private Vector3 angularVelocity;

	private bool lastMouseDown;

	protected virtual void Update()
	{
		var mouseDown = Input.GetMouseButton(0);

		// Only update controls if playing
		if (Application.isPlaying == true)
		{
			linearVelocity += transform.right   * Input.GetAxis("Horizontal") * LinearSpeed * Time.deltaTime;
			linearVelocity += transform.forward * Input.GetAxis("Vertical")   * LinearSpeed * Time.deltaTime;

			if (mouseDown == true && lastMouseDown == true)
			{
				angularVelocity.y += Input.GetAxis("Mouse X") * AngularSpeed * Time.deltaTime;
				angularVelocity.x -= Input.GetAxis("Mouse Y") * AngularSpeed * Time.deltaTime;
			}

			linearVelocity  = VA_Helper.Dampen3( linearVelocity, Vector3.zero,  LinearDampening, Time.deltaTime, 0.1f);
			angularVelocity = VA_Helper.Dampen3(angularVelocity, Vector3.zero, AngularDampening, Time.deltaTime, 0.1f);
		}

		EulerAngles += angularVelocity * Time.deltaTime;
		EulerAngles.x = Mathf.Clamp(EulerAngles.x, -89.0f, 89.0f);

		transform.position += linearVelocity * Time.deltaTime;
		transform.rotation  = Quaternion.Euler(EulerAngles);

		lastMouseDown = mouseDown;
	}
}