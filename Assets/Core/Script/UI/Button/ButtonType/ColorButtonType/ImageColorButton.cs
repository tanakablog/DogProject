using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Core.UI.Button
{
    /// <summary>
    /// イメージカラーボタンクラス
    /// </summary>
    [RequireComponent( typeof( Image ) )]
    public class ImageColorButton : ColorButton
    {
        /// <summary>
        /// ボタンイメージ
        /// </summary>
        private Image m_ButtonImage;

        /// <summary>
        /// リフレッシュカラー
        /// </summary>
        public override void RefreshColor( )
        {
            m_IsEnable = ButtonManager.instance.CheckButtonState( this.m_Info.type );

            if( m_IsEnable )
            {
                m_ButtonImage.color = changeColor[(int)eColor.Up];
            }
            else
            {
                m_ButtonImage.color = changeColor[(int)eColor.Disable];
            }
        }

        /// <summary>
        /// Unity Start
        /// </summary>
        private void Start( )
        {
            // イメージ取得
            m_ButtonImage = this.GetComponent<Image>();
        }

        /// <summary>
        /// フェードカラー
        /// </summary>
        /// <param name="change">変更色</param>
        /// <param name="time">変更時間</param>
        protected override IEnumerator FadeColor( Color change, float time )
        {
            if( !this.enabled )
            {
                yield break;
            }

            // 変更時間チェック
            if( time > 0 && m_ButtonImage.color != change )
            {
                // 1秒間の変化量
                Color frame = ( change - m_ButtonImage.color ) / time;

                // タイマー
                float time_count = 0.0f;

                while( time > time_count )
                {
                    // 1フレーム待機
                    yield return null;

                    // 色変更
                    m_ButtonImage.color += frame * Time.deltaTime;

                    // タイマーを進める
                    time_count += Time.deltaTime;
                }
            }

            // 変化後カラーを設定
            m_ButtonImage.color = change;
        }
    }
}

