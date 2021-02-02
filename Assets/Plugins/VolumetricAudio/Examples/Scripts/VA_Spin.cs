using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(VA_Spin))]
public class VA_Spin_Editor : VA_Editor<VA_Spin>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.DegreesPerSecond.sqrMagnitude == 0.0f));
			DrawDefault("DegreesPerSecond");
		EndError();
	}
}
#endif

// This component spins the current GameObject
[AddComponentMenu("Volumetric Audio/VA Spin")]
public class VA_Spin : MonoBehaviour
{
	[Tooltip("The amount of degrees this GameObject is rotated by each second in world space")]
	public Vector3 DegreesPerSecond;
	
	protected virtual void Update()
	{
		transform.Rotate(DegreesPerSecond * Time.deltaTime);
	}
}