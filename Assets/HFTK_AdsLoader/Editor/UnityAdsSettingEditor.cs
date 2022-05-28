using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
[CustomEditor(typeof(UnityAdsSetting))]
public class UnityAdsSettingEditor : UnityEditor.Editor
{
	[MenuItem("Assets/Unity Mobile Ads/Settings...")]
	public static void OpenInspector()
	{
		Selection.activeObject = UnityAdsSetting.Instance;
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("Google Mobile Ads App ID", EditorStyles.boldLabel);
		EditorGUI.indentLevel++;

		UnityAdsSetting.Instance.UnityAndroidGameId =
				EditorGUILayout.TextField("Android",
						UnityAdsSetting.Instance.UnityAndroidGameId);

		UnityAdsSetting.Instance.UnityIOSGameId =
				EditorGUILayout.TextField("iOS",
						UnityAdsSetting.Instance.UnityIOSGameId);


		EditorGUI.indentLevel--;
		EditorGUILayout.Separator();

		EditorGUI.indentLevel++;

		EditorGUI.BeginChangeCheck();

		EditorGUI.indentLevel--;
		EditorGUILayout.Separator();

		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty((UnityAdsSetting)target);
			AssetDatabase.SaveAssets();
		}
	}
}

