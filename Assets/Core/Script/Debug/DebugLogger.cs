using UnityEngine;
using System.Collections;

public class DebugLogger : Core.Base.MonoSingleton<DebugLogger>
{
	static int maxLines = 300;
	static string myLog = "";
	static Queue myLogQueue = new Queue();
//	string stack = "";

	protected override void OnInitialize()
	{
		Application.RegisterLogCallback(HandleLog);
		Application.RegisterLogCallbackThreaded(HandleLog);
	}

	protected override void OnFinalize()
	{
		Application.RegisterLogCallback(null);
		Application.RegisterLogCallbackThreaded(null);
		base.OnFinalize();
	}


	void HandleLog(string logString, string stackTrace, LogType type)
	{
//		stack = stackTrace;
		string newString = "[" + type + "] :「" + logString + "」" + "\n";

		myLogQueue.Enqueue(newString);

		if (type == LogType.Exception)
		{
			newString = "　　StackTrace:「" + stackTrace + "」" + "\n";
			myLogQueue.Enqueue(newString);
		}

		while (myLogQueue.Count > maxLines)
		{
			myLogQueue.Dequeue();
		}

		myLog = string.Empty;
		foreach (string s in myLogQueue)
		{
			myLog += s;
		}
	}

	public void SaveLog()
	{
		System.IO.StreamWriter sw = new System.IO.StreamWriter(
			UnityDir.dataPath + "/Log.log",
			false,
			System.Text.Encoding.GetEncoding("utf-8"));
		sw.Write(myLog);
		sw.Close();
	}

	public string GetAllLog()
	{
		return myLog;
	}

}
