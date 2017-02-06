using UnityEngine;
using System.Collections;

public static class UnityDir 
{
#if UNITY_EDITOR
	//unityでデータを保存する場合のディレクトリ
	public static string dataPath { get { return CheckDirectory(System.IO.Directory.GetCurrentDirectory() + "/dataStorage/"); } }
	public static string BanndleName = "_WebPlayer.unity3d";
	public static string cachePath { get { return CheckDirectory(Application.temporaryCachePath + "/"); } }
	public static string externalPath { get { return dataPath; } }
#elif UNITY_IPHONE
	//iPhoneの場合
	//unity以外でデータを保存する場合のディレクトリ
	public static string dataPath { get { return CheckDirectory(Application.persistentDataPath + "/"); } }
	public static string BanndleName = "_iOS.unity3d";
	public static string cachePath { get { return CheckDirectory(Application.temporaryCachePath + "/"); } }
	public static string externalPath { get { return dataPath; } }
#elif UNITY_ANDROID
	//Androidの場合
	//unity以外でデータを保存する場合のディレクトリ
	public static string BanndleName = "_android.unity3d";
	public static string dataPath{ get {
			using( AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer") )
			{
				using( AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity") )
				{
					using( AndroidJavaObject filesDir = currentActivity.Call<AndroidJavaObject>( "getFilesDir" ) )
					{
						return CheckDirectory(filesDir.Call<string>("getCanonicalPath") + "/");
					}
				}
			}
		}
	}
	public static string cachePath {
		get {
			using( AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer") )
			{
				using( AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity") )
				{
					using( AndroidJavaObject cacheDir = currentActivity.Call<AndroidJavaObject>( "getCacheDir" ) )
					{
						return CheckDirectory(cacheDir.Call<string>("getCanonicalPath") + "/");
					}
				}
			}
		}
	}
	
	public static string externalPath {
		get {
			using( AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer") )
			{
				using( AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity") )
				{
					using( AndroidJavaObject externalFilesDir = currentActivity.Call<AndroidJavaObject>("getExternalFilesDir",null ) )
					{
						return CheckDirectory(externalFilesDir.Call<string>("getCanonicalPath") + "/");
					}
				}
			}
		}
	}

#else
	//unityでデータを保存する場合のディレクトリ
	public static string dataPath { get { return CheckDirectory(Application.temporaryCachePath + "/"); } }
	public static string BanndleName = "_WebPlayer.unity3d";
	public static string cachePath { get { return CheckDirectory(Application.temporaryCachePath + "/"); } }
	public static string externalPath { get { return dataPath; } }
#endif

	public static string CheckDirectory(string path)
	{
		if (System.IO.Directory.Exists(path) == false)
		{
			System.IO.Directory.CreateDirectory(path);
		}

		return path;
	}
}
