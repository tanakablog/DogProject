using UnityEngine;
using System.Collections;

/// <summary>
/// スクロールボード制御
/// </summary>
public abstract class ScrollBoardCtrlBase : MonoBehaviour
{

	/// <summary>
	/// インデックス数
	/// </summary>
	protected int m_Index;

	public int index {
		get {
			return m_Index;
		}
	}

	/// <summary>
	/// ボード更新
	/// </summary>
	/// <param name="index">Index.</param>
	public void UpdateBoard (int index)
	{
		// インデックス番号保存
		m_Index = index;

		// 情報更新コールバック
		OnUpdateBoard (index);
	}

	/// <summary>
	/// ボード更新コールバック
	/// </summary>
	/// <param name="index">Index.</param>
	protected virtual void OnUpdateBoard (int index)
	{
	}
}