using UnityEngine;
using System.Collections;
using System.Linq;

namespace Core.UI.Button
{
    /// <summary>
    /// スケールボタンクラス
    /// </summary>
    public class ScaleButton : Private.ButtonBase
    {
        /// <summary>
        /// 変更スケールタイプ
        /// </summary>
        public enum eScale
        {
            Down,
            Up,
            Disable,
        }

        /// <summary>
        /// 変更時間
        /// </summary>
        public float changeTime = 0.2f;

        /// <summary>
        /// 変更拡大率
        /// </summary>
        public Vector3[] changeScale = 
        {
            new Vector3( 0.8f, 0.8f, 0.8f ),
            Vector3.one,
            Vector3.one
        };

        /// <summary>
        /// 拡大率変更コルーチン
        /// </summary>
        private IEnumerator m_ChangeScaleCoroutine;

        /// <summary>
        /// 有効フラグ
        /// </summary>
        private bool m_IsEnable;

        /// <summary>
        /// 拡大率リフレッシュ
        /// </summary>
        public void RefreshScale()
        {
            m_IsEnable = ButtonManager.instance.CheckButtonState( this.m_Info.type );

			/*
            if( m_IsEnable )
            {
                cachedTransform.localScale = changeScale[(int)eScale.Up];
            }
            else
            {
                cachedTransform.localScale = changeScale[(int)eScale.Disable];
            }
            */
        }

        #region 非公開メソッド

        /// <summary>
        /// Unity Start
        /// </summary>
        void Start()
        {
            RefreshScale();
        }

        /// <summary>
        /// 拡大率変更設定
        /// </summary>
        /// <param name="scale"></param>
        void ChangeScaleSetting( Vector3 scale )
        {
            // コルーチン停止
            if( m_ChangeScaleCoroutine != null )
            {
                StopCoroutine( m_ChangeScaleCoroutine );
            }

            if( !this.enabled )
            {
                return;
            }

            // コルーチン設定
            m_ChangeScaleCoroutine = ChangeScale( scale, changeTime );

            // コルーチン開始
            StartCoroutine( m_ChangeScaleCoroutine );
        }

        /// <summary>
        /// 拡大率変更
        /// </summary>
        /// <param name="change">変更拡大率</param>
        /// <param name="time">変更時間</param>
        IEnumerator ChangeScale( Vector3 change, float time )
        {
			/*
            // 変更時間チェック
            if( time > 0 && cachedTransform.localScale != change )
            {
                // 1秒間の変化量
                Vector3 frame = ( change - cachedTransform.localScale ) / time;

                // タイマー
                float time_count = 0.0f;

                while( time > time_count )
                {
                    // 1フレーム待機
                    yield return null;

                    // 拡大率変更
                    cachedTransform.localScale += frame * Time.deltaTime;

                    // タイマーを進める
                    time_count += Time.deltaTime;
                }
            }

            // 変化後拡大率を設定
            cachedTransform.localScale = change;
			*/
			yield return null;
        }

        void DisableAction( )
        {
            if( !ButtonManager.instance.CheckButtonState( this.m_Info.type ) )
            {
                ChangeScaleSetting( changeScale[(int)eScale.Disable] );

                m_IsEnable = false;
            }
        }

        void EnableAction( )
        {
            if( ButtonManager.instance.CheckButtonState( this.m_Info.type ) )
            {
                ChangeScaleSetting( changeScale[(int)eScale.Up] );

                m_IsEnable = true;
            }
        }

        #region ボタン状態変更コールバック

        protected override void SetButtonStateChange( ButtonManager.eButton state )
        {
            if( m_IsEnable )
            {
                DisableAction();
            }
            else
            {
                EnableAction();
            }
        }

        protected override void AddButtonStateChange( ButtonManager.eButton state )
        {
            if( !m_IsEnable )
            {
                EnableAction();
            }
        }

        protected override void DeleteButtonStateChange( ButtonManager.eButton state )
        {
            if( m_IsEnable )
            {
                DisableAction();
            }
            else
            {
                EnableAction();
            }
        }

        #endregion

        #region 有効アクション

        protected override void OnDownAction( )
        {
            ChangeScaleSetting( changeScale[(int)eScale.Down] );
        }

        protected override void OnUpAction( )
        {
            ChangeScaleSetting( changeScale[(int)eScale.Up] );
        }

        protected override void OnExitAction( )
        {
            ChangeScaleSetting( changeScale[(int)eScale.Up] );
        }

        #endregion

        #endregion
    }
}