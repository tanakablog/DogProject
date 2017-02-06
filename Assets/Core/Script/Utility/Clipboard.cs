using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
namespace Core.Utility
{
	// クリップボードにコピー
	public class Clipboard
	{
		#if UNITY_EDITOR
		private static PropertyInfo systemCopyBufferProperty = null;
		private static PropertyInfo GetSystemCopyBuffer()
		{
			if (systemCopyBufferProperty == null)
			{
				Type T = typeof(GUIUtility);
				systemCopyBufferProperty = T.GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
				if (systemCopyBufferProperty == null)
					throw new Exception("Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
			}
			return systemCopyBufferProperty;
		}
		public static string clipBoard
		{
			get
			{
				PropertyInfo P = GetSystemCopyBuffer();
				return (string)P.GetValue(null,null);
			}
			set
			{
				PropertyInfo P = GetSystemCopyBuffer();
				P.SetValue(null,value,null);
			}
		}
		#elif UNITY_IPHONE
		// ネイティブコードplugin/iOS/Clipboard.mmを呼び出し
		[DllImport("__Internal")]
		private static extern void _setClip(string value);
		public static string clipBoard
		{
			get{
				return "";
			}
			set{
				_setClip(value);
			}
		}
		#elif UNITY_ANDROID
		private static string setValue;
		public static string clipBoard
		{
			get{
				return "";
			}
			set{
				setValue = value;
				AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");

				activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
				{
					AndroidJavaObject clipboardManager = activity.Call<AndroidJavaObject>("getSystemService","clipboard");
					//clipboardManager.Call("setText", exportData);
					AndroidJavaClass clipDataClass = new AndroidJavaClass("android.content.ClipData");
					AndroidJavaObject clipData = clipDataClass.CallStatic<AndroidJavaObject>("newPlainText","simple text", setValue);
					clipboardManager.Call("setPrimaryClip",clipData);
				}));
			}
		}
		#endif
	}
}