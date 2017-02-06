using UnityEngine;
using System.Collections;

namespace Core.UI.Button
{

    /// <summary>
    /// 色変更ボタン
    /// </summary>
    public class ColorButton : Private.ButtonBase
    {
        /// <summary>
        /// 色変更タイプ
        /// </summary>
        public enum eColor
        {
            Down,
            Up,
            Disable,
        }

        /// <summary>
        /// フェード時間
        /// </summary>
        [SerializeField]
        private float m_FadeTime = 0.2f;
        public float fadeTime
        {
            get
            {
                return m_FadeTime;
            }
                                                        
            set
            {
                m_FadeTime = value;
            }
        }
            
        /// <summary>       
        /// 変更カラー
        /// </summary>
        [SerializeField]
        private Color[] m_ChangeColor =
        {
            new Color(0.5f, 0.5f, 0.5f),
            Color.white,
            new Color(0.2f, 0.2f, 0.2f)
        };
        public Color[] changeColor
        {
            get
            {
                return m_ChangeColor; 
            }

            set
            {
                m_ChangeColor = value;
            }
        }

        /// <summary>
        /// 有効状態フラグ
        /// </summary>
        protected bool m_IsEnable;

        /// <summary>
        /// フェードカラーコルーチン
        /// </summary>
        protected IEnumerator m_FadeColorCoroutine;

        /// <summary>
        /// リフレッシュカラー
        /// </summary>
        public virtual void RefreshColor( )
        {
            m_IsEnable = ButtonManager.instance.CheckButtonState( this.m_Info.type );
			/*
            if( m_IsEnable )
            {
                cachedRenderer.material.color = m_ChangeColor[(int)eColor.Up];
            }
            else
            {
                cachedRenderer.material.color = m_ChangeColor[(int)eColor.Disable];
            }
            */
        }

        #region 非公開メソッド

        /// <summary>
        /// Unity Start
        /// </summary>
        private void Start( )
        {
            RefreshColor();
        }

        /// <summary>
        /// フェードカラー設定
        /// </summary>
        /// <param name="color">変更色</param>
        void FadeColorSetting( Color color )
        {
            // コルーチン停止
            if( m_FadeColorCoroutine != null )
            {
                StopCoroutine( m_FadeColorCoroutine );
            }

            // 有効チェック
            if( !this.enabled )
            {
                return;
            }

            // コルーチン設定
            m_FadeColorCoroutine = FadeColor( color, m_FadeTime );

            // コルーチン開始
            StartCoroutine( m_FadeColorCoroutine );
        }

        /// <summary>
        /// フェードカラー
        /// </summary>
        /// <param name="change">変更色</param>
        /// <param name="time">変更時間</param>
        protected virtual IEnumerator FadeColor( Color change, float time )
        {
			yield return null;
			/*
            // 変更時間チェック
            if( time > 0 && cachedRenderer.material.color != change )
            {
                // 1秒間の変化量
                Color frame = ( change - cachedRenderer.material.color ) / time;

                // タイマー
                float time_count = 0.0f;

                while( time > time_count )
                {
                    // 1フレーム待機
                    yield return null;

                    // 色変更
                    cachedRenderer.material.color += frame * Time.deltaTime;

                    // タイマーを進める
                    time_count += Time.deltaTime;
                }
            }

            // 変化後カラーを設定
            cachedRenderer.material.color = change;
			*/
        }

        /// <summary>
        /// 無効化アクション
        /// </summary>
        void DisableAction( )
        {
            if( !ButtonManager.instance.CheckButtonState( this.m_Info.type ) )
            {
                FadeColorSetting( m_ChangeColor[(int)eColor.Disable] );

                m_IsEnable = false;
            }
        }

        /// <summary>
        /// 有効化アクション
        /// </summary>
        void EnableAction( )
        {
            if( ButtonManager.instance.CheckButtonState( this.m_Info.type ) )
            {
                FadeColorSetting( m_ChangeColor[(int)eColor.Up] );

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
            FadeColorSetting( m_ChangeColor[(int)eColor.Down] );
        }

        protected override void OnUpAction( )
        {
            FadeColorSetting( m_ChangeColor[(int)eColor.Up] );
        }

        protected override void OnExitAction( )
        {
            FadeColorSetting( m_ChangeColor[(int)eColor.Up] );
        }

        #endregion

        #endregion
    }
}