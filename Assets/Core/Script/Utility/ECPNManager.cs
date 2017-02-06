using System.Collections;
using UnityEngine;


namespace Core
{
	/// <summary>
	/// デバイストークンを取得できるクラス
	/// </summary>
	public class ECPNManager : Base.MonoSingleton<ECPNManager>
	{
		private string deviceToken = "";
		private event System.Action<string> m_registAction = delegate { };

		/// <summary>
		/// デバイストークンの登録
		/// </summary>
		/// <param name="GoogleCloudMessageProjectID">Androidのみ、ProjectID</param>
		/// <param name="registerAction">登録完了後の処理</param>
		public void Register(string GoogleCloudMessageProjectID, System.Action<string> registerAction)
		{
#if UNITY_EDITOR
			Debug.Log("You can only register iOS and android devices, not the editor!");
#else

			// 既に登録されていた・・・
			if (string.IsNullOrEmpty(deviceToken) == false)
			{
				registerAction(deviceToken);
				return;
			}

			// デバイストークンの取得処理  registerAction;
			System.Action<string> regst = null;

			regst = (mes) =>
			{
				registerAction(mes);
				m_registAction -= regst;
			};

			m_registAction += regst;

			// 取得する
			RequestDeviceToken(GoogleCloudMessageProjectID);
#endif

		}

		/// <summary>
		/// 登録処理
		/// </summary>
		/// <param name="token"></param>
		private void RegisterToken(string token)
		{
			deviceToken = token;
			m_registAction(deviceToken);
		}

		/// <summary>
		/// デバイストークンの取得
		/// </summary>
		/// <returns></returns>
		public string getDeviceToken()
		{
			return deviceToken;
		}

#if UNITY_IPHONE && !UNITY_EDITOR
	private bool pollIOSDeviceToken = false;

	void Update()
	{
		// 設定で取得できなかったら、設定が書き換わるまでリクエストしまくる
		if (pollIOSDeviceToken)
		{
			RegisterIOSDevice();
		}
	}
#endif

#if UNITY_IPHONE && !UNITY_EDITOR
		private void RegisterIOSDevice()
		{
			// 本体の設定で登録できないようになってる？
			if (NotificationServices.registrationError != null)
			{
				Debug.Log(NotificationServices.registrationError);
			}

			// デバイストークンが空？
			if (NotificationServices.deviceToken == null)
			{
				return;
			}

			pollIOSDeviceToken = false;
			string hexToken = System.BitConverter.ToString(NotificationServices.deviceToken).Replace("-", string.Empty);
			// 登録完了
			RegisterToken(hexToken);
		}
#endif

		/// <summary>
		/// デバイストークンの取得
		/// </summary>
		private void RequestDeviceToken(string GoogleCloudMessageProjectID = "")
		{
#if UNITY_EDITOR
			Debug.Log("You can only register iOS and android devices, not the editor!");
#endif
#if UNITY_IPHONE && !UNITY_EDITOR
		if(NotificationServices.deviceToken == null) {
			pollIOSDeviceToken = true;
			NotificationServices.RegisterForRemoteNotificationTypes(RemoteNotificationType.Alert | 
			                                                        RemoteNotificationType.Badge | 
			                                                        RemoteNotificationType.Sound);
		} else {
			RegisterIOSDevice();
		}
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass  plugin = new AndroidJavaClass ("jp.co.visualize.unitypush.GCMRegister");
		
		// 初期化確認
		if (plugin.CallStatic<bool>("IsInit") == false)
		{
			// 初期化させる
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			plugin.CallStatic("Init", activity);
		}
		
		if( plugin.CallStatic< bool >("setGCM", GoogleCloudMessageProjectID) )
		{
			string regid = plugin.CallStatic< string >("getRegId");
			RegisterToken(regid);
		}
#endif
		}

		public void RequestUnregisterDevice()
		{
#if UNITY_EDITOR
			Debug.Log("You can only unregister iOS and android devices, not the editor!");
#endif
#if UNITY_IPHONE && !UNITY_EDITOR
			NotificationServices.UnregisterForRemoteNotifications();
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass plugin = new AndroidJavaClass("jp.co.visualize.unitypush.GCMRegister");

			// 初期化確認
			if (plugin.CallStatic<bool>("IsInit") == false)
			{
				// 初期化させる
				AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				plugin.CallStatic("Init", activity);
			}
			plugin.CallStatic("unsetGCM");
			deviceToken = "";
#endif
		}
	}

}