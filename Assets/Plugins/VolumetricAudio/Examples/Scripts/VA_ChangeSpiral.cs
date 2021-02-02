using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(VA_ChangeSpiral))]
public class VA_ChangeSpiral_Editor : VA_Editor<VA_ChangeSpiral>
{
	protected override void OnInspector()
	{
		DrawDefault("AngleStepA");
		DrawDefault("AngleStepB");
		BeginError(Any(t => t.Interval <= 0.0f));
			DrawDefault("Interval");
		EndError();
	}
}
#endif

// This component changes a spiral's AngleStep to make it animated
[RequireComponent(typeof(VA_Spiral))]
[AddComponentMenu("Volumetric Audio/VA Change Spiral")]
public class VA_ChangeSpiral : MonoBehaviour
{
	[Tooltip("The minimum AngleStep value")]
	public float AngleStepA = 10.0f;
	
	[Tooltip("The maximum AngleStep value")]
	public float AngleStepB = -10.0f;
	
	[Tooltip("The amount of seconds it takes to go from AngleStep A and B")]
	public float Interval = 5.0f;
	
	// Current interpolation position
	[SerializeField]
	private float position;

	[System.NonSerialized]
	private VA_Spiral spiral;
	
	protected virtual void Update()
	{
		if (Interval > 0.0f)
		{
			position += Time.deltaTime;

			if (spiral == null) spiral = GetComponent<VA_Spiral>();

			spiral.AngleStep = Mathf.Lerp(AngleStepA, AngleStepB, Mathf.PingPong(position, Interval) / Interval);

			spiral.Regenerate();
		}
	}
}