using UnityEngine;
using System.Collections;

public class YieldFunction
{
	/// <summary>
	/// 数秒間待機させる
	/// </summary>
	/// <param name="waittime">待機させる秒</param>
	/// <param name="action">ラムダ式で定義させるAction</param>
	/// <returns></returns>
	public static IEnumerator WaitYieldFunction(float waittime, System.Action action)
	{
		yield return new WaitForSeconds(waittime);
		action();
		yield return 0;
	}
}
