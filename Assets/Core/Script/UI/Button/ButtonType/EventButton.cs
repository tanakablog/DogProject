using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

namespace Core.UI.Button
{
    /// <summary>
    /// イベントボタンクラス
    /// </summary>
    [System.Serializable]
    public class EventButton : Private.ButtonBase
    {
        /// <summary>
        /// イベント配列
        /// </summary>
        [SerializeField]
        private UnityEngine.Events.UnityEvent[] m_Events = new UnityEngine.Events.UnityEvent[Extension.EnumExtension.GetEnumNum<ButtonInfo.eAction>()];
        public UnityEngine.Events.UnityEvent[] events
        {
            get
            {
                return m_Events;
            }

            set
            {
                m_Events = value;
            }
        }

        /// <summary>
        /// 描画フラグ配列
        /// </summary>
        [SerializeField]
        public bool[] m_IsDraw = new bool[Extension.EnumExtension.GetEnumNum<ButtonInfo.eAction>()];
        public bool[] isDraw
        {
            get
            {
                return m_IsDraw;
            }

            set
            {
                m_IsDraw = value;
            }
        }

        void Start()
        {
            for( int i = 0; i < m_IsDraw.Length; ++i )
            {
                if( !m_IsDraw[i] )
                {
                    m_Events[i].RemoveAllListeners();
                }
            }
        }

        /// <summary>
        /// イベント追加
        /// </summary>
        /// <param name="action">イベントタイプ</param>
        /// <param name="add_event">追加するイベント</param>
        public void AddEvent( ButtonInfo.eAction action, UnityEngine.Events.UnityAction add_event )
        {
            m_Events[(int)action].AddListener( add_event );
            m_IsDraw[(int)action] = true;
        }   
        
        /// <summary>
        /// イベント削除
        /// </summary>
        /// <param name="action">イベントタイプ</param>
        /// <param name="add_event">削除するイベント</param>
        public void DeleteEvent( ButtonInfo.eAction action, UnityEngine.Events.UnityAction del_event )
        {
            m_Events[(int)action].RemoveListener( del_event );
        }

        /// <summary>
        /// クリアイベント
        /// </summary>
        /// <param name="action">イベントタイプ</param>
        public void ClearEvent( ButtonInfo.eAction action )
        {
            m_Events[(int)action].RemoveAllListeners();
            m_IsDraw[(int)action] = false;
        }

        #region 有効アクション

        /// <summary>
        /// 押された
        /// </summary>
        protected override void OnDownAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.Down] )
            {
                m_Events[(int)ButtonInfo.eAction.Down].Invoke();
            }    
        }

        /// <summary>
        /// 離された
        /// </summary>
        protected override void OnUpAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.Up] )
            {
                m_Events[(int)ButtonInfo.eAction.Up].Invoke();
            }
        }

        /// <summary>
        /// 範囲内へ移動した
        /// </summary>
        protected override void OnEnterAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.Enter] )
            {
                m_Events[(int)ButtonInfo.eAction.Enter].Invoke();
            }
        }

        /// <summary>
        /// 範囲外へ移動した
        /// </summary>
        protected override void OnExitAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.Exit] )
            {
                m_Events[(int)ButtonInfo.eAction.Exit].Invoke();
            }
        }

        /// <summary>
        /// 範囲内と重なっている
        /// </summary>
        protected override void OnOverAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.Over] )
            {
                m_Events[(int)ButtonInfo.eAction.Over].Invoke();
            }
        }

        /// <summary>
        /// 長押し
        /// </summary>
        protected override void OnPressAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.Press] )
            {
                m_Events[(int)ButtonInfo.eAction.Press].Invoke();
            }
        }

        /// <summary>
        /// 長押し開始
        /// </summary>
        protected override void OnPressStartAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.PressStart] )
            {
                m_Events[(int)ButtonInfo.eAction.PressStart].Invoke();
            }
        }

        /// <summary>
        /// 長押し終了
        /// </summary>
        protected override void OnPressEndAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.PressEnd] )
            {
                m_Events[(int)ButtonInfo.eAction.PressEnd].Invoke();
            }
        }

        #endregion

        #region 無効アクション

        /// <summary>
        /// 無効状態で押された
        /// </summary>
        protected override void OnNotDownAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.NotDown] )
            {
                m_Events[(int)ButtonInfo.eAction.NotDown].Invoke();
            }
        }

        /// <summary>
        /// 無効状態で離された
        /// </summary>
        protected override void OnNotUpAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.NotUp] )
            {
                m_Events[(int)ButtonInfo.eAction.NotUp].Invoke();
            }
        }

        /// <summary>
        /// 無効状態で範囲内へ移動した
        /// </summary>
        protected override void OnNotEnterAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.NotEnter] )
            {
                m_Events[(int)ButtonInfo.eAction.NotEnter].Invoke();
            }
        }

        /// <summary>
        /// 無効状態で範囲外へ移動した
        /// </summary>
        protected override void OnNotExitAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.NotExit] )
            {
                m_Events[(int)ButtonInfo.eAction.NotExit].Invoke();
            }
        }

        /// <summary>
        /// 無効状態で範囲内と重なっている
        /// </summary>
        protected override void OnNotOverAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.NotOver] )
            {
                m_Events[(int)ButtonInfo.eAction.NotOver].Invoke();
            }
        }

        /// <summary>
        /// 無効状態で長押し
        /// </summary>
        protected override void OnNotPressAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.NotPress] )
            {
                m_Events[(int)ButtonInfo.eAction.NotPress].Invoke();
            }
        }

        /// <summary>
        /// 無効状態で長押し開始
        /// </summary>
        protected override void OnNotPressStartAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.NotPressStart] )
            {
                m_Events[(int)ButtonInfo.eAction.NotPressStart].Invoke();
            }
        }

        /// <summary>
        /// 無効状態で長押し終了
        /// </summary>
        protected override void OnNotPressEndAction( )
        {
            if( m_IsDraw[(int)ButtonInfo.eAction.NotPressEnd] )
            {
                m_Events[(int)ButtonInfo.eAction.NotPressEnd].Invoke();
            }
        }

        #endregion
    }
}