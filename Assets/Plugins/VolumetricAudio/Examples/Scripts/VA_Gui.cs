using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(VA_Gui))]
public class P3D_Gui_Editor : VA_Editor<VA_Gui>
{
	protected override void OnInspector()
	{
		DrawDefault("Header");
		DrawDefault("Footer");
	}
}
#endif

// This component is used in all example scenes to drive the GUI buttons
public class VA_Gui : MonoBehaviour
{
	public string Header;
	public string Footer;

	public void ClickReload()
	{
		LoadLevel(GetCurrentLevel());
	}

	public void ClickPrev()
	{
		var index = GetCurrentLevel() - 1;

			if (index < 0)
			{
				index = GetLevelCount() - 1;
			}

			LoadLevel(index);
	}

	public void ClickNext()
	{
		var index = GetCurrentLevel() + 1;

		if (index >= GetLevelCount())
		{
			index = 0;
		}

		LoadLevel(index);
	}

#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
	private static int GetCurrentLevel()
	{
		return Application.loadedLevel;
	}

	private static int GetLevelCount()
	{
		return Application.levelCount;
	}

	private static void LoadLevel(int index)
	{
		Application.LoadLevel(index);
	}
#else
	private static int GetCurrentLevel()
	{
		return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
	}

	private static int GetLevelCount()
	{
		return UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
	}

	private static void LoadLevel(int index)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(index);
	}
#endif
}