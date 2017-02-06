#define BUTTON_LOG     // ボタンログ表示
#define PRESS_ON       // 長押し中にコールバック関数呼び出し

using UnityEngine;
using System.Collections;
using System;

namespace Core.UI.Button
{
    /// <summary>
    /// ボタン情報クラス
    /// </summary>
	public class ButtonInfo : MonoBehaviour
    {
        /// <summary>
        /// アクションタイプ
        /// </summary>
        public enum eAction
        {
            Down,           // 押された
            Up,             // 離された
            Enter,          // 範囲内へ
            Exit,           // 範囲外へ
            Over,           // 範囲内にいる
            Press,          // 長押し中
            PressStart,     // 長押し開始
            PressEnd,       // 長押し終了
            NotDown,        // 無効状態で押された
            NotUp,          // 無効状態で離された
            NotEnter,       // 無効状態で範囲内へ
            NotExit,        // 無効状態で範囲外へ
            NotOver,        // 無効状態で範囲内にいる
            NotPress,       // 無効状態で長押し中
            NotPressStart,  // 無効状態で長押し開始
            NotPressEnd,    // 無効状態で長押し終了
        }

        /// <summary>
        /// アクションイベント配列
        /// </summary>
        [SerializeField]
        private Action[] m_ActionEvent = new Action[Extension.EnumExtension.GetEnumNum<eAction>()];
        public Action[] actionEvent
        {
            get
            {
                return m_ActionEvent;
            }

            set
            {
                m_ActionEvent = value;
            }
        }

        /// <summary>
        /// ボタンタイプ
        /// </summary>
        /// <remarks>
        /// m_Typeの値がeButton.Allの時のみ特別処理が行われる
        /// ButtonManagerのm_Stateの値がeButton.None以外の場合、そのボタンは有効と見なされる
        /// </remarks>
        [SerializeField]
        private ButtonManager.eButton m_Type = ButtonManager.eButton.All;
        public ButtonManager.eButton type
        {
            get
            {
                return m_Type;
            }

            set
            {
                m_Type = value;
            }
        }

        /// <summary>
        /// 長押し判定時間
        /// </summary>
        [SerializeField]
        private float m_PressTime = 0.2f;
        public float pressTime
        {
            get
            {
                return m_PressTime;
            }

            set
            {
                m_PressTime = value;
            }
        }

        private bool m_IsDown;  

        /// <summary>
        /// 長押しチェックコルーチン
        /// </summary>
        private IEnumerator m_PressCheck;

        /// <summary>
        /// イメージを基準にコライダー2Dリサイズ
        /// </summary>
        public void ResizeBoxCollider2DBasisImage()
        {
			/*
            // コライダー2D & レクトトランスフォーム
            if( cachedBoxCollider2D && cachedRectTransform )
            {
                // リサイズ
                cachedBoxCollider2D.size = cachedRectTransform.sizeDelta;
            }
			*/
        }

        #region 判定

        /// <summary>
        /// 長押しチェック
        /// </summary>
        IEnumerator PressCheck( )
        {
            // 長押し判定時間待機
            yield return new WaitForSeconds( m_PressTime );

            // 長押し開始
            OnPressStart();

            while( true )
            {
                // 長押し中
                OnPress();   

                // 1フレーム待機
                yield return null;
            }
        }

        private void PressEnd()
        {
            // 長押しチェックコルーチン
            if( m_PressCheck != null )
            {
                // 停止
                StopCoroutine( m_PressCheck );

                // 初期化
                m_PressCheck = null;

                // 長押し終了
                OnPressEnd();
            }
        }

        /// <summary>
        /// 長押し中
        /// </summary>
        [System.Diagnostics.Conditional( "PRESS_ON" )]
        void OnPress()
        {
            // 長押しアクション
            if( ButtonManager.instance.CheckButtonState( m_Type ) )
            {
                if( m_ActionEvent[(int)eAction.Press] != null )
                {
                    m_ActionEvent[(int)eAction.Press]();
                }
            }
            else
            {
                if( m_ActionEvent[(int)eAction.NotPress] != null )
                {
                    m_ActionEvent[(int)eAction.NotPress]();
                }
            }
        }

        /// <summary>
        /// 長押し開始
        /// </summary>
        void OnPressStart( )
        {
            // ボタン状態チェック
            if( ButtonManager.instance.CheckButtonState( m_Type ) )
            {
                if( m_ActionEvent[(int)eAction.PressStart] != null )
                {
                    m_ActionEvent[(int)eAction.PressStart]();
                }
            }
            else
            {
                if( m_ActionEvent[(int)eAction.NotPressStart] != null )
                {
                    m_ActionEvent[(int)eAction.NotPressStart]();
                }
            }
        }

        /// <summary>
        /// 長押し終了
        /// </summary>
        void OnPressEnd( )
        {
            // ボタン状態チェック
            if( ButtonManager.instance.CheckButtonState( m_Type ) )
            {
                if( m_ActionEvent[(int)eAction.PressEnd] != null )
                {
                    m_ActionEvent[(int)eAction.PressEnd]();
                }
            }
            else
            {
                if( m_ActionEvent[(int)eAction.NotPressEnd] != null )
                {
                    m_ActionEvent[(int)eAction.NotPressEnd]();
                }
            }
        }

        /// <summary>
        /// マウスボタンを押された
        /// </summary>
        void OnMouseDown( )
        {
            // 押下フラグ
            m_IsDown = true;

            // ボタン状態チェック
            if( ButtonManager.instance.CheckButtonState( m_Type ) )
            {
                if( m_ActionEvent[(int)eAction.Down] != null )
                {
                    m_ActionEvent[(int)eAction.Down]();
                }
            }
            else
            {
                if( m_ActionEvent[(int)eAction.NotDown] != null )
                {
                    m_ActionEvent[(int)eAction.NotDown]();
                }
            }

            // 長押しチェックコルーチン停止
            if( m_PressCheck != null )
            {
                // 停止
                StopCoroutine( m_PressCheck );
            }

            // 長押しチェックコルーチン再設定
            m_PressCheck = PressCheck();

            // 長押しチェックコルーチン開始
            StartCoroutine( m_PressCheck );
        }

        /// <summary>
        /// マウスボタンが離された
        /// </summary>
        void OnMouseUp( )
        {
            if( !m_IsDown )
                return;

            // ボタン状態チェック
            if( ButtonManager.instance.CheckButtonState( m_Type ) )
            {
                if( m_ActionEvent[(int)eAction.Up] != null )
                {
                    m_ActionEvent[(int)eAction.Up]();
                }
            }
            else
            {
                if( m_ActionEvent[(int)eAction.NotUp] != null )
                {
                    m_ActionEvent[(int)eAction.NotUp]();
                }
            }

            // 長押し終了
            PressEnd();
        }

        /// <summary>
        /// マウスボタンが範囲内へ移動した
        /// </summary>
        void OnMouseEnter( )
        {
            // ボタン状態チェック
            if( ButtonManager.instance.CheckButtonState( m_Type ) )
            {
                if( m_ActionEvent[(int)eAction.Enter] != null )
                {
                    m_ActionEvent[(int)eAction.Enter]();
                }
            }
            else
            {
                if( m_ActionEvent[(int)eAction.NotEnter] != null )
                {
                    m_ActionEvent[(int)eAction.NotEnter]();
                }
            }
        }

        /// <summary>
        /// マウスボタンが範囲外へ移動した
        /// </summary>
        void OnMouseExit( )
        {
            // 押下フラグ
            m_IsDown = false;

            // 長押し終了
            PressEnd();

            // ボタン状態チェック
            if( ButtonManager.instance.CheckButtonState( m_Type ) )
            {
                if( m_ActionEvent[(int)eAction.Exit] != null )
                {
                    m_ActionEvent[(int)eAction.Exit]();
                }
            }
            else
            {
                if( m_ActionEvent[(int)eAction.NotExit] != null )
                {
                    m_ActionEvent[(int)eAction.NotExit]();
                }
            }

            // 長押し終了
            PressEnd();
        }

        /// <summary>
        /// マウスボタンが範囲内と重なっている
        /// </summary>
        void OnMouseOver( )
        {
            // ボタン状態チェック
            if( ButtonManager.instance.CheckButtonState( m_Type ) )
            {
                if( m_ActionEvent[(int)eAction.Over] != null )
                {
                    m_ActionEvent[(int)eAction.Over]();
                }
            }
            else
            {
                if( m_ActionEvent[(int)eAction.NotOver] != null )
                {
                    m_ActionEvent[(int)eAction.NotOver]();
                }
            }
        }

        #endregion
    }
}