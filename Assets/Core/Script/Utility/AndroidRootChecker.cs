using UnityEngine;
using System.Collections;

namespace Core
{
	public class AndroidRootChecker : Base.MonoSingleton<AndroidRootChecker>
	{
		float CurrentTime = 0f;
		const float CheckTime = 30f;	// 30秒に一回チェック

		/// <summary>
		/// アプリを中断・再開した時に呼ばれる
		/// </summary>
		/// <param name="pauseStatus">中断フラグ</param>
		void OnApplicationPause(bool pauseStatus)
		{
			// 中断
			if (pauseStatus)
			{
			}
			// 再開
			else
			{
				CurrentTime = 0f;
				RootCheck();
			}
		}

		/// <summary>
		/// 更新時
		/// </summary>
		void Update()
		{
			CurrentTime += Time.deltaTime;

			if (CurrentTime >= CheckTime)
			{
				CurrentTime = 0f;

				RootCheck();
			}
		}

		void RootCheck()
		{
			// ルートチェック
			if (checkRoot())
			{
				// メッセージ表示
				showRootMessage();
				// ゲーム強制終了
				Application.Quit();
			}
		}

		/// <summary>
		/// ルート状態をチェックします。
		/// </summary>
		/// <returns></returns>
		public static bool checkRoot()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
		// Root権限確認
		AndroidJavaClass plugin = new AndroidJavaClass("jp.co.visualize.unitypush.RootChecker");
//		Debug.Log("RootCheck");
		if (plugin != null)
		{
			if (plugin.CallStatic<bool>("isInappropriateEnvSuspected"))
			{
//				Debug.Log("RootDevice!!");
				return true;
			}
		}
#endif
			return false;
		}

		/// <summary>
		/// ルートメッセージの表示
		/// </summary>
		public static void showRootMessage()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass plugin = new AndroidJavaClass("jp.co.visualize.unitypush.RootChecker");
		if (plugin != null)
		{
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			if (unityPlayer != null)
			{
				AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				if (activity != null)
				{
					plugin.CallStatic("ShowRootMessageHandler", activity);
				}
			}
		}
#endif
		}
	}

}