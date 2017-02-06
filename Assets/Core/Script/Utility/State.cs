using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 状態管理を行うクラスです。
/// 
/// enumはすべてint型で定義してください。
/// 
/// </summary>
/// <typeparam name="TEnum"></typeparam>
public class State<TEnum> where TEnum : struct
{
	#region 変数宣言領域

	/// <summary>
	/// 現在の状態
	/// </summary>
	int m_State;

	/// <summary>
	/// 前の状態
	/// </summary>
	int m_PrevState;

	public TEnum NowState
	{
		get
		{
			return (TEnum)Enum.ToObject(typeof(TEnum), m_State);
		}
	}
	public TEnum PrevState
	{
		get
		{
			return (TEnum)Enum.ToObject(typeof(TEnum), m_PrevState);
		}
	}
	#endregion

	#region 関数宣言領域
	/// <summary>
	/// コンストラクタ.
	/// </summary>
	public State()
	{
		Reset();
	}

	/// <summary>
	/// リセット
	/// </summary>
	public void Reset()
	{
		m_State = 0;
		m_PrevState = 0;
	}

	/// <summary>
	/// 次の状態へ
	/// </summary>
	/// <returns>値オーバー</returns>
	public bool NextState()
	{
		m_PrevState = m_State;

		++m_State;

		// 状態が最大値を超えたか
        if( Core.Extension.EnumExtension.GetEnumNum<TEnum>() <= m_State )
		{
			m_State = 0;
			return true;
		}

		return false;
	}

	/// <summary>
	/// 前の状態へ
	/// </summary>
	public void SetPrevState()
	{
		// 前の状態を一時保存
		int tmp = m_PrevState;

		m_PrevState = m_State;

		m_State = tmp;
	}

	/// <summary>
	/// 状態の登録
	/// </summary>
	/// <param name='state'>
	/// State.
	/// </param>
	public void SetState(TEnum state)
	{
		m_PrevState = m_State;
		m_State = Convert.ToInt32(state);
	}


	#endregion
}
