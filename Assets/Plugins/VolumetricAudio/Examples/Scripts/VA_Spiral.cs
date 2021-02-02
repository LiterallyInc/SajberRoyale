using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(VA_Spiral))]
public class VA_Spiral_Editor : VA_Editor<VA_Spiral>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.InvalidSegmentCount == true));
			DrawDefault("SegmentCount");
		EndError();
		BeginError(Any(t => t.SegmentThickness == 0.0f));
			DrawDefault("SegmentThickness");
		EndError();
		DrawDefault("InitialAngle");
		DrawDefault("InitialDistance");
		BeginError(Any(t => t.AngleStep == 0.0f));
			DrawDefault("AngleStep");
		EndError();
		DrawDefault("DistanceStep");
	}
}
#endif

// This component generates a flat spiral mesh based on the input settings
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[AddComponentMenu("Volumetric Audio/VA Spiral")]
public class VA_Spiral : MonoBehaviour
{
	[Tooltip("Amount of segments in the spiral")]
	public int SegmentCount = 100;

	[Tooltip("Thickness of the spiral in local space")]
	public float SegmentThickness = 1.0f;

	[Tooltip("Initial angle of the spiral edge in degrees")]
	public float InitialAngle;

	[Tooltip("Initial distance of the spiral inner edge in local space")]
	public float InitialDistance = 1.0f;

	[Tooltip("Angle increment of each spiral segment in degrees")]
	public float AngleStep = 10.0f;

	[Tooltip("Distance increment of each spiral segment in local space")]
	public float DistanceStep = 0.1f;

	[System.NonSerialized]
	private Mesh mesh;

	[System.NonSerialized]
	private Vector3[] positions;

	[System.NonSerialized]
	private Vector2[] uvs;

	[System.NonSerialized]
	private int[] indices;

	[System.NonSerialized]
	private MeshFilter cachedMeshFilter;

	public bool InvalidSegmentCount
	{
		get
		{
			return SegmentCount < 1 || SegmentCount > VA_Helper.MeshVertexLimit / 2 - 2;
		}
	}

	public void Regenerate()
	{
		// Create or clear mesh
		if (mesh == null)
		{
			mesh = new Mesh();

			mesh.name      = "Spiral";
			mesh.hideFlags = HideFlags.DontSave;
		}
		else
		{
			mesh.Clear();
		}

		// Apply mesh to filter
		if (cachedMeshFilter == null) cachedMeshFilter = GetComponent<MeshFilter>();

		cachedMeshFilter.sharedMesh = mesh;

		// Invalid segment count?
		if (InvalidSegmentCount == true)
		{
			return;
		}

		// Allocate arrays?
		var vertexCount = SegmentCount * 2 + 2;

		if (positions == null || positions.Length != vertexCount)
		{
			positions = new Vector3[vertexCount];
		}

		if (uvs == null || uvs.Length != vertexCount)
		{
			uvs = new Vector2[vertexCount];
		}

		// Generate indices?
		if (indices == null || indices.Length != SegmentCount * 6)
		{
			indices = new int[SegmentCount * 6];

			for (var i = 0; i < SegmentCount; i++)
			{
				indices[i * 6 + 0] = i * 2 + 0;
				indices[i * 6 + 1] = i * 2 + 1;
				indices[i * 6 + 2] = i * 2 + 2;
				indices[i * 6 + 3] = i * 2 + 3;
				indices[i * 6 + 4] = i * 2 + 2;
				indices[i * 6 + 5] = i * 2 + 1;
			}
		}

		var angle    = InitialAngle;
		var distance = InitialDistance;

		for (var i = 0; i <= SegmentCount; i++)
		{
			var vertex = i * 2;

			positions[vertex + 0] = VA_Helper.SinCos(angle * Mathf.Deg2Rad) *  distance;
			positions[vertex + 1] = VA_Helper.SinCos(angle * Mathf.Deg2Rad) * (distance + SegmentThickness);

			uvs[vertex + 0] = Vector2.zero;
			uvs[vertex + 1] = Vector2.one;

			angle    += AngleStep;
			distance += DistanceStep;
		}

		// Update mesh
		mesh.vertices  = positions;
		mesh.triangles = indices;
		mesh.uv        = uvs;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}

	protected virtual void Awake()
	{
		Regenerate();
	}

	protected virtual void OnValidate()
	{
		Regenerate();
	}

	protected virtual void Update()
	{
		Regenerate();
	}

	protected virtual void OnDestroy()
	{
		VA_Helper.Destroy(mesh);
	}
}