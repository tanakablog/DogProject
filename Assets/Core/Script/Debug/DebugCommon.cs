using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;
using System.IO;

/// <summary>
/// デバッグ便利関係
/// </summary>
public static class DebugCommon
{
	/// <summary>
	/// 条件をチェックし、その条件が false の場合は例外を投げます
	/// </summary>
	/// <param name="condition">評価する条件式</param>
	public static void ASSERT(bool condition)
	{
		ASSERT(condition, "");
	}

	/// <summary>
	/// 条件をチェックし、その条件が false の場合は例外を投げます
	/// </summary>
	/// <param name="condition">評価する条件式</param>
	/// <param name="message">送信するメッセージ</param>
	public static void ASSERT(bool condition, string message)
	{
		if (!condition)
		{

			StackFrame CallStack = new StackFrame(1, true);

			string SourceFile = CallStack.GetFileName();
			int SourceLine = CallStack.GetFileLineNumber();
			string mes = ("ASSERT: " + message + " - File: " + SourceFile + " Line: " + SourceLine.ToString());

			// ログを出す
			UnityEngine.Debug.LogError(mes);

			// ログ保存
			DebugLogger.instance.SaveLog();

			// すべてのログを取得
			string saveMes = DebugLogger.instance.GetAllLog();

			// 写真撮影
			DebugLogger.instance.StartCoroutine(takeSS(saveMes));
		}
	}

	static IEnumerator takeSS(string message)
	{
		string fileName = UnityDir.externalPath + "temp.png";

		yield return new WaitForEndOfFrame();

		// スクショの撮影
		Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
		texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		texture.Apply();

		byte [] pngData = texture.EncodeToPNG();
		File.WriteAllBytes(fileName, pngData);

		yield return new WaitForSeconds(0.1f);

		// 一応クリップボードへ
		Core.Utility.Clipboard.clipBoard = message;

#if !UNITY_EDITOR
		SocialConnector.Share(message, null, fileName);
#endif
		yield return 0;

		// Unity停止
		UnityEngine.Debug.Break();
		
		// 終了
		yield return 0;
	}
}
