using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

public abstract class ScrollCtrlBase<T> : MonoBehaviour where T : ScrollBoardCtrlBase
{
	/// <summary>
	/// スクロールレクト
	/// </summary>
	[SerializeField]
	protected ScrollRect m_ScrollRect;

	/// <summary>
	/// ボードプレハブ
	/// </summary>
	[SerializeField]
	protected T m_BoardPrefab;

	/// <summary>
	/// ボードサイズ
	/// </summary>
	[SerializeField]
	protected Vector2 m_BoardSize;

	/// <summary>
	/// 横ボード数
	/// </summary>
	[SerializeField]
	protected int m_WidthBoardNum;

	/// <summary>
	/// 縦ボード数
	/// </summary>
	[SerializeField]
	protected int m_HeightBoardNum;

	/// <summary>
	/// トップオフセット
	/// </summary>
	[SerializeField]
	protected float m_TopOffset;

	/// <summary>
	/// ボトムオフセット
	/// </summary>
	[SerializeField]
	protected float m_BottomOffset;

	/// <summary>
	/// コンテンツトランスフォーム
	/// </summary>
	protected RectTransform m_ContentsTransform;

	/// <summary>
	/// ボード制御リスト
	/// </summary>
	protected List<T> m_BoardCtrls = new List<T> ();

	/// <summary>
	/// 現在の開始ライン
	/// </summary>
	protected int m_NowStartLine;

	/// <summary>
	/// 位置の差
	/// </summary>
	protected Vector2 m_Offset;

	/// <summary>
	/// 最大インデックス
	/// </summary>
	protected int m_MaxIndex;

	/// <summary>
	/// 最大スタートライン
	/// </summary>
	protected int m_MaxStartLine;

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="num">要素数</param>
	public virtual void Init (int num)
	{
		foreach (var ctrl in m_BoardCtrls) {
			if (ctrl == null) {
				continue;
			}
			Destroy (ctrl.gameObject);
		}

		if (num <= 0) {
			return;
		}

		m_BoardCtrls.Clear ();
			
		// コンテンツ取得
		m_ContentsTransform = m_ScrollRect.content;
			
		// コンテンツサイズ設定
		var size = m_ContentsTransform.sizeDelta;
		int add_num = num % m_WidthBoardNum == 0 ? 0 : 1;
		size.y = m_BoardSize.y * (num / m_WidthBoardNum + add_num) + m_TopOffset + m_BottomOffset;
		m_ContentsTransform.sizeDelta = size;

		// 最大インデックス取得
		m_MaxIndex = num;

		// 最大開始ライン数算出
		m_MaxStartLine = IndexToLine (m_MaxIndex) - m_HeightBoardNum + 1;
		if (m_MaxStartLine < 0) {
			m_MaxStartLine = 0;
		}

		// オフセット算出
		m_Offset.y = m_BoardSize.y * 0.5f + m_TopOffset;
		m_Offset.x = (m_ScrollRect.GetComponent<RectTransform> ().sizeDelta.x - m_BoardSize.x) * 0.5f;

		int max = m_HeightBoardNum * m_WidthBoardNum;
		max = Mathf.Clamp (max, 0, num);

		// ボード制御生成
		for (int i = 0; i < max; ++i) {
			// ボードプレハブ生成
			m_BoardCtrls.Add (CreateBoard ());
		}

		// スタートライン設定
		var pos = m_ContentsTransform.anchoredPosition;
		pos.y = 0.0f;
		m_ContentsTransform.anchoredPosition = pos;
		SetStartLine (PosToStartLine (0.0f));
	}

	/// <summary>
	/// ボード生成
	/// </summary>
	/// <returns>The board.</returns>
	protected virtual T CreateBoard ()
	{
		// ボードプレハブ生成
		var ctrl = Instantiate (m_BoardPrefab);

		// ボード初期設定
		ctrl.transform.SetParent (m_ContentsTransform);
		ctrl.transform.localScale = Vector3.one;

		// ボード生成コールバック
		OnCreateBoard (ctrl);

		return ctrl;
	}

	/// <summary>
	/// ボード生成コールバック
	/// </summary>
	/// <param name="ctrl">生成されたボード制御</param>
	protected virtual void OnCreateBoard (T ctrl)
	{
		// TODO : ボード生成時に行う処理
	}

	/// <summary>
	/// 更新
	/// </summary>
	private void Update ()
	{
		if (m_ContentsTransform == null || Convert.ToInt32 (m_ContentsTransform.anchoredPosition.y) < 0) {
			return;
		}

		// スタートライン更新
		UpdateStartLine (PosToStartLine (m_ContentsTransform.anchoredPosition.y));
	}

	/// <summary>
	/// 位置からスタートライン
	/// </summary>
	/// <returns>The to start line.</returns>
	/// <param name="pos">Position.</param>
	public int PosToStartLine (float pos)
	{
		// インデックス算出
		return Convert.ToInt32 (pos) / Convert.ToInt32 (m_BoardSize.y);
	}

	/// <summary>
	/// スタートライン設定
	/// </summary>
	/// <param name="start">スタートライン</param>
	public void SetStartLine (int start)
	{
		// スタートインデックス
		start = Mathf.Clamp (start, 0, m_MaxStartLine) * m_WidthBoardNum;

		for (int i = start; i < start + m_BoardCtrls.Count; ++i) {
			UpdateBoard (m_BoardCtrls [i - start], i);
		}

		m_NowStartLine = start;
	}

	/// <summary>
	/// スタートライン更新
	/// </summary>
	/// <param name="start">スタートライン</param>
	public void UpdateStartLine (int start)
	{	
		// 最大＆最小に丸める
		start = Mathf.Clamp (start, 0, m_MaxStartLine);
			
		// 開始ラインチェック
		if (start == m_NowStartLine) {
			return;
		}

		// ライン数の差
		int line_diff = start - m_NowStartLine;

		// 変更するインデックス数
		int remove_index_num = line_diff * m_WidthBoardNum;

		// インデックス数チェック
		if (Mathf.Abs (remove_index_num) >= m_BoardCtrls.Count) {
			SetStartLine (start);
			return;
		}

		// 現在のインデックス
		int index = LineToIndex (m_NowStartLine);

		// ライン数の差をチェック
		if (line_diff > 0) {
			// 上側のボードから変更
			for (int i = index; i < index + remove_index_num; ++i) {
				int num = i + m_BoardCtrls.Count;

				// インデックス数から制御スクリプト取得
				var ctrl = m_BoardCtrls.FirstOrDefault (x => x.index == i);

				if (ctrl == null) {
					continue;
				}

				if (num < 0 || num >= m_MaxIndex) {
					continue;
				}

				// ボード更新
				UpdateBoard (ctrl, num);
			}
		} else {
			// 下側のボードから変更
			index += m_BoardCtrls.Count - 1;
			for (int i = index; i > index + remove_index_num; --i) {
				int num = i - m_BoardCtrls.Count;

				// インデックス数から制御スクリプト取得
				var ctrl = m_BoardCtrls.FirstOrDefault (x => x.index == i);

				if (ctrl == null) {
					continue;
				}

				if (num < 0 || num >= m_MaxIndex) {
					continue;
				}

				UpdateBoard (ctrl, num); 
			}
		}

		m_NowStartLine = start;
	}

	/// <summary>
	/// ボード更新
	/// </summary>
	/// <param name="ctrl">ボード制御</param>
	/// <param name="index">更新するインデックス</param>
	protected virtual void UpdateBoard (T ctrl, int index)
	{
		if (index < 0 || index >= m_MaxIndex) {
			return;
		}
			
		ctrl.UpdateBoard (index);
		ctrl.transform.localPosition = IndexToPos (index);
		ctrl.gameObject.SetActive (true);
	}

	/// <summary>
	/// インデックスから位置へ変換
	/// </summary>
	/// <returns>The index position.</returns>
	/// <param name="index">Index.</param>
	protected Vector2 IndexToPos (int index)
	{
		int x = index % m_WidthBoardNum;
		int y = index / m_WidthBoardNum;

		Vector2 pos;

		pos.y = Convert.ToSingle (y) * -m_BoardSize.y;
		pos.x = Convert.ToSingle (x) * m_BoardSize.x;

		pos -= m_Offset;

		return pos;
	}

	/// <summary>
	/// ライン番号からインデックス番号
	/// </summary>
	/// <returns>The to index.</returns>
	/// <param name="line">Line.</param>
	protected int LineToIndex (int line)
	{
		return line * m_WidthBoardNum;
	}

	/// <summary>
	/// インデックス番号からライン番号
	/// </summary>
	/// <returns>The to line.</returns>
	/// <param name="index">Index.</param>
	protected int IndexToLine (int index)
	{
		return index / m_WidthBoardNum;
	}
}