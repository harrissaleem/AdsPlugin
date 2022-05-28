using System.IO;
using UnityEditor;
using UnityEngine;

public class UnityAdsSetting : ScriptableObject
{
	private const string MobileAdsSettingsResDir = "Assets/HFTK_AdsLoader/Resources";

	private const string MobileAdsSettingsFile = "UnityAdsSettings";

	private const string MobileAdsSettingsFileExtension = ".asset";

	private static UnityAdsSetting instance;

	[SerializeField]
	private string unityAndroidGameId = string.Empty;

	[SerializeField]
	private string unityIOSGameId = string.Empty;

	public string UnityAndroidGameId
	{
		get { return Instance.unityAndroidGameId; }

		set { Instance.unityAndroidGameId = value; }
	}

	public string UnityIOSGameId
	{
		get { return Instance.unityIOSGameId; }

		set { Instance.unityIOSGameId = value; }
	}

	public static UnityAdsSetting Instance
	{
		get
		{
			if (instance != null)
			{
				return instance;
			}

			instance = Resources.Load<UnityAdsSetting>(MobileAdsSettingsFile);

			if (instance != null)
			{
				return instance;
			}

			Directory.CreateDirectory(MobileAdsSettingsResDir);

			instance = ScriptableObject.CreateInstance<UnityAdsSetting>();

			string assetPath = Path.Combine(MobileAdsSettingsResDir, MobileAdsSettingsFile);
			string assetPathWithExtension = Path.ChangeExtension(
													assetPath, MobileAdsSettingsFileExtension);
			AssetDatabase.CreateAsset(instance, assetPathWithExtension);

			AssetDatabase.SaveAssets();

			return instance;
		}
	}
}

