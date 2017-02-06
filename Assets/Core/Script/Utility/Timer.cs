//
//  Timer.cs
//
//  Author:
//       Takatoshi Maehara
//
//  Copyright (c) 2014 visualize
using System;

/// <summary>
/// タイマークラス
/// </summary>
public sealed class Timer
{
		private float m_time;	// カウンター.
		private float m_limit;	// リミット.
		private bool m_pause;	// ポーズ.

		public float Count{ get { return m_time; } }

		public float Limit{ get { return m_limit; } }

		public bool IsPause{ get { return m_pause; } }
	
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Timer ()
		{
				Reset ();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="limit">Limit.</param>
		public Timer (float limit)
		{
				Reset ();

				SetLimit (limit);
		}

		/// <summary>
		/// リミットの設定
		/// </summary>
		/// <param name="limit">Limit.</param>
		public void SetLimit (float limit)
		{
				m_limit = limit;
		}

		/// <summary>
		/// リセット
		/// </summary>
		public void Reset ()
		{
				m_time = 0;
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="deltaTime">Delta time.</param>
		public void Update (float deltaTime)
		{
				if (m_pause)
						return;

				m_time += deltaTime;
				if (m_time >= m_limit) {
						m_time = m_limit;
				}
		}

		/// <summary>
		/// タイマーポーズ
		/// </summary>
		public void TimerPause ()
		{
				m_pause = true;
		}

		/// <summary>
		/// タイマー再開
		/// </summary>
		public void TimerResume ()
		{
				m_pause = false;
		}

		/// <summary>
		/// タイマー終了判定
		/// </summary>
		/// <returns><c>true</c> if this instance is end; otherwise, <c>false</c>.</returns>
		public bool IsEnd ()
		{
				if (m_time >= m_limit)
						return true;

				return false;
		}

}
