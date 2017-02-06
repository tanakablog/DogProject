using UnityEngine;
using System.Collections;
using System;

namespace Core.UI.Button.Private
{

    /// <summary>
    /// ボタンクラス
    /// </summary>
    [RequireComponent( typeof( ButtonInfo ) )]
    public abstract class ButtonBase : MonoBehaviour
    {
        /// <summary>
        /// ボタン情報
        /// </summary>
        protected ButtonInfo m_Info;

        /// <summary>
        /// ボタン情報プロパティ
        /// </summary>
        public ButtonInfo info
        {
            get
            {
                return info; 
            }
        }

        #region 非公開メソッド

        /// <summary>
        /// Unity Awake
        /// </summary>
        void Awake( )
        {
            // ボタン情報取得
            m_Info = this.GetComponent<ButtonInfo>();

            // アクション登録
            RegisterAction();

            // 状態変更登録
            RegisterStateChange();

            OnAwake();
        }

        protected void OnAwake()
        {
            
        }

        /// <summary>
        /// 削除された
        /// </summary>
        void OnDestroy( )
        {
            // アクション解除
            ReleaseAction();

            // 状態変更解除
            ReleaseStateChange();
        }

        /// <summary>
        /// アクションイベント登録
        /// </summary>
        void RegisterAction( )
        {
            // ボタン情報アクションイベント登録
            if( m_Info != null )
            {
                m_Info.actionEvent[(int)ButtonInfo.eAction.Down] += OnDownAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.Up] += OnUpAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.Enter] += OnEnterAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.Exit] += OnExitAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.Press] += OnPressAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.PressStart] += OnPressStartAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.PressEnd] += OnPressEndAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.NotDown] += OnNotDownAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.NotUp] += OnNotUpAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.NotEnter] += OnNotEnterAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.NotExit] += OnNotExitAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.NotPress] += OnNotPressAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.NotPressStart] += OnNotPressStartAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.NotPressEnd] += OnNotPressEndAction;
            }
        }

        /// <summary>
        /// アクションイベント解除
        /// </summary>
        void ReleaseAction( )
        {
            // ボタン情報アクションイベント登録
            if( m_Info != null )
            {
                m_Info.actionEvent[(int)ButtonInfo.eAction.Down] -= OnDownAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.Up] -= OnUpAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.Enter] -= OnEnterAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.Exit] -= OnExitAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.Press] -= OnPressAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.PressStart] -= OnPressStartAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.PressEnd] -= OnPressEndAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.NotDown] -= OnNotDownAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.NotUp] -= OnNotUpAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.NotEnter] -= OnNotEnterAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.NotExit] -= OnNotExitAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.NotPress] -= OnNotPressAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.NotPressStart] -= OnNotPressStartAction;
                m_Info.actionEvent[(int)ButtonInfo.eAction.NotPressEnd] -= OnNotPressEndAction;
            }
        }

        /// <summary>
        /// 状態変更イベント登録
        /// </summary>
        void RegisterStateChange( )
        {
            ButtonManager.instance.AddStateChangeEvent( ButtonManager.eStateChange.Set, SetButtonStateChange );
            ButtonManager.instance.AddStateChangeEvent( ButtonManager.eStateChange.Add, AddButtonStateChange );
            ButtonManager.instance.AddStateChangeEvent( ButtonManager.eStateChange.Delete, DeleteButtonStateChange );
        }

        /// <summary>
        /// 状態変更イベント解除
        /// </summary>
        void ReleaseStateChange( )
        {
            ButtonManager.instance.RemoveStateChangeEvent( ButtonManager.eStateChange.Set, SetButtonStateChange );
            ButtonManager.instance.RemoveStateChangeEvent( ButtonManager.eStateChange.Add, AddButtonStateChange );
            ButtonManager.instance.RemoveStateChangeEvent( ButtonManager.eStateChange.Delete, DeleteButtonStateChange );
        }

        #region ボタン状態変更コールバック

        /// <summary>
        /// ボタン状態設定変更
        /// </summary>
        /// <param name="state">設定されたボタン状態</param>
        protected virtual void SetButtonStateChange( ButtonManager.eButton state )
        {
#if BUTTON_LOG
        Core.Debug.DebugLog.Log(this.gameObject.name + ":SetButtonStateChange " + state );
#endif
        }

        /// <summary>
        /// ボタン状態追加変更
        /// </summary>
        /// <param name="state">追加されたボタン状態</param>
        protected virtual void AddButtonStateChange( ButtonManager.eButton state )
        {
#if BUTTON_LOG
        Core.Debug.DebugLog.Log(this.gameObject.name + ":AddButtonStateChange " + state );
#endif
        }

        /// <summary>
        /// ボタン状態削除変更
        /// </summary>
        /// <param name="state">削除されたボタン状態</param>
        protected virtual void DeleteButtonStateChange( ButtonManager.eButton state )
        {
#if BUTTON_LOG
        Core.Debug.DebugLog.Log(this.gameObject.name + ":DleteButtonStateChange " + state);
#endif
        }

        #endregion

        #region 有効アクション

        /// <summary>
        /// 押された
        /// </summary>
        protected virtual void OnDownAction( )
        {
#if BUTTON_LOG
		Core.Debug.DebugLog.Log( this.gameObject.name + ":OnDownAction" );
#endif
        }

        /// <summary>
        /// 離された
        /// </summary>
        protected virtual void OnUpAction( )
        {
#if BUTTON_LOG
		Core.Debug.DebugLog.Log( this.gameObject.name + ":OnUpAction" );
#endif
        }

        /// <summary>
        /// 範囲内へ移動した
        /// </summary>
        protected virtual void OnEnterAction( )
        {
#if BUTTON_LOG
		Core.Debug.DebugLog.Log( this.gameObject.name + ":OnEnterAction" );
#endif
        }

        /// <summary>
        /// 範囲外へ移動した
        /// </summary>
        protected virtual void OnExitAction( )
        {
#if BUTTON_LOG
		Core.Debug.DebugLog.Log( this.gameObject.name + ":OnExitAction" );
#endif
        }

        /// <summary>
        /// 範囲内と重なっている
        /// </summary>
        protected virtual void OnOverAction( )
        {
#if BUTTON_LOG
		Core.Debug.DebugLog.Log( this.gameObject.name + ":OnOverAction" );
#endif
        }

        /// <summary>
        /// 長押し
        /// </summary>
        protected virtual void OnPressAction( )
        {
#if BUTTON_LOG
		Core.Debug.DebugLog.Log( this.gameObject.name + ":OnPressAction" );
#endif
        }

        /// <summary>
        /// 長押し開始
        /// </summary>
        protected virtual void OnPressStartAction( )
        {
#if BUTTON_LOG
		Core.Debug.DebugLog.Log( this.gameObject.name + ":OnPressStartAction" );
#endif
        }

        /// <summary>
        /// 長押し終了
        /// </summary>
        protected virtual void OnPressEndAction( )
        {
#if BUTTON_LOG
		Core.Debug.DebugLog.Log( this.gameObject.name + ":OnPressEndAction" );
#endif
        }

        #endregion

        #region 無効アクション

        /// <summary>
        /// 無効状態で押された
        /// </summary>
        protected virtual void OnNotDownAction( )
        {
        }

        /// <summary>
        /// 無効状態で離された
        /// </summary>
        protected virtual void OnNotUpAction( )
        {
        }

        /// <summary>
        /// 無効状態で範囲内へ移動した
        /// </summary>
        protected virtual void OnNotEnterAction( )
        {
        }

        /// <summary>
        /// 無効状態で範囲外へ移動した
        /// </summary>
        protected virtual void OnNotExitAction( )
        {
        }

        /// <summary>
        /// 無効状態で範囲内と重なっている
        /// </summary>
        protected virtual void OnNotOverAction( )
        {
        }

        /// <summary>
        /// 無効状態で長押し
        /// </summary>
        protected virtual void OnNotPressAction( )
        {
        }

        /// <summary>
        /// 無効状態で長押し開始
        /// </summary>
        protected virtual void OnNotPressStartAction( )
        {
        }

        /// <summary>
        /// 無効状態で長押し終了
        /// </summary>
        protected virtual void OnNotPressEndAction( )
        {
        }

        #endregion

        #endregion
    }
}
