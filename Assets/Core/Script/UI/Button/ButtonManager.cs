using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Core.UI.Button
{

    /// <summary>
    /// ボタン管理
    /// </summary>
    public class ButtonManager
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        private static ButtonManager m_Instance; 

        /// <summary>
        /// インスタンス取得
        /// </summary>
        public static ButtonManager instance
        {
            get
            {
                // インスタンスチェック
                if( m_Instance == null )
                {
                    // インスタンス生成
                    m_Instance = new ButtonManager();
                }

                return m_Instance;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private ButtonManager()
        {
            // 初期は全有効
            m_State.Set( eButton.All );
        }

        /// <summary>
        /// ボタン状態タイプ
        /// </summary>
        [Flags]
        public enum eButton
        {
            None = 0,				// 全無効
            Demo = 1,
            Demo2 = 1 << 1,

            All = int.MaxValue		// 全有効（CheckButtonState関数で特別処理有り）
        }

        /// <summary>
        /// 状態変更タイプ
        /// </summary>
        public enum eStateChange
        {
            Set,    // 設定
            Add,    // 追加
            Delete, // 削除
        }

        /// <summary>
        /// 状態変更デリゲート
        /// </summary>
        /// <param name="button">ボタンタイプ</param>
        public delegate void StateChange( eButton button );

        /// <summary>
        /// 状態変更イベント配列
        /// </summary>
        private StateChange[] m_StateChangeEvent = new StateChange[Extension.EnumExtension.GetEnumNum<eStateChange>()];

        /// <summary>
        /// ボタン状態
        /// </summary>
        private Utility.Flag<eButton> m_State = new Utility.Flag<eButton>();

        #region 公開メソッド

        /// <summary>
        /// ボタン状態イベント追加
        /// </summary>
        /// <param name="type">イベントタイプ</param>
        /// <param name="state">追加イベント</param>
        public void AddStateChangeEvent( eStateChange type, StateChange state )
        {
            m_StateChangeEvent[(int)type] += state;
        }

        /// <summary>
        /// ボタン状態イベント削除
        /// </summary>
        /// <param name="type">イベントタイプ</param>
        /// <param name="state">削除イベント</param>
        public void RemoveStateChangeEvent( eStateChange type, StateChange state )
        {
            m_StateChangeEvent[(int)type] -= state;
        }

        /// <summary>
        /// ボタン状態設定
        /// </summary>
        /// <param name="state">設定する状態</param>
        public void SetButtonState( eButton state )
        {
            // 状態設定
            m_State.Set( state );

            // ボタン状態変更イベント
            if( m_StateChangeEvent[(int)eStateChange.Set] != null )
            {
                m_StateChangeEvent[(int)eStateChange.Set]( state );
            }
        }

        /// <summary>
        /// ボタン状態追加
        /// </summary>
        /// <param name="state">追加する状態</param>
        public void AddButtonState( eButton state )
        {
            // 状態追加
            m_State.Add( state );

            // ボタン状態変更イベント
            if( m_StateChangeEvent[(int)eStateChange.Add] != null )
            {
                m_StateChangeEvent[(int)eStateChange.Add]( state );
            }
        }

        /// <summary>
        /// ボタン状態削除
        /// </summary>
        /// <param name="state">削除する状態</param>
        public void DeleteButtonState( eButton state )
        {
            // 状態削除
            m_State.Delete( state );

            // ボタン状態変更イベント
            if( m_StateChangeEvent[(int)eStateChange.Delete] != null )
            {
                m_StateChangeEvent[(int)eStateChange.Delete]( state );
            }
        }

        /// <summary>
        /// ボタン状態確認
        /// </summary>
        /// <param name="state">確認する状態</param>
        /// <returns>true = 有効, false = 無効</returns>
        /// <remarks>state = m_Info.eButton.All だった場合、m_Stateがm_Info.eButton.None以外ならtrueを返す</remarks>
        public bool CheckButtonState( eButton state )
        {
            if( state == eButton.All )
            {
                return ( m_State.GetValue() > 0 );
            }

            return m_State.Check( state );
        }

        #endregion
    }
}