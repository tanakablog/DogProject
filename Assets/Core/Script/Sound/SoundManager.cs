using UnityEngine;
using System;
using System.Collections;

namespace Core.Sound
{

    /// <summary>
    /// 音管理クラス
    /// </summary>
    public class SoundManager : Base.MonoSingleton<SoundManager>
    {
        /// <summary>
        /// SE最大数
        /// </summary>
        public const int k_SE_MAX = 10;

        /// <summary>
        /// ボイス最大数
        /// </summary>
        public const int k_VOICE_MAX = 5;

        /// <summary>
        /// BGMサウンド
        /// </summary>
        private Private.BgmSound m_Bgm;

        /// <summary>
        /// SEサウンド
        /// </summary>
        private Private.SeSound m_Se;

        /// <summary>
        /// ボイスサウンド
        /// </summary>
        private Private.VoiceSound m_Voice;

        /// <summary>
        /// イニシャライズ
        /// </summary>
        protected override void OnInitialize( )
        {
            // シーン遷移後も削除しない
            DontDestroyOnLoad( this.gameObject );

            // BGM
            {
                // 取得
                m_Bgm = this.gameObject.GetComponent<Private.BgmSound>();

                // 存在チェック
                if( null == m_Bgm )
                {
                    // なければ追加
                    m_Bgm = this.gameObject.AddComponent<Private.BgmSound>();
                }

                // 初期化
                m_Bgm.Init();
            }

            // SE
            {
                // 取得
                m_Se = this.gameObject.GetComponent<Private.SeSound>();

                // 存在チェック
                if( null == m_Se )
                {
                    // なければ追加
                    m_Se = this.gameObject.AddComponent<Private.SeSound>();
                }

                // 初期化
                m_Se.Init();
            }

            // ボイス
            {
                // 取得
                m_Voice = this.gameObject.GetComponent<Private.VoiceSound>();

                // 存在をチェック
                if( null == m_Voice )
                {
                    // なければ追加
                    m_Voice = this.gameObject.AddComponent<Private.VoiceSound>();
                }

                // 初期化
                m_Voice.Init();
            }
        }

        /// <summary>
        /// BGM取得
        /// </summary>
        /// <returns>BGMクリップ</returns>
        /// <param name="id">BGM名</param>
        public AudioClip GetBgmClip( SoundTable.eBGM id )
        {
            if( id == SoundTable.eBGM.NoSound )
                return null;

            return Resources.Load<AudioClip>( SoundTable.BGM_PATH[(int)id] );
        }

        /// <summary>
        /// SE取得
        /// </summary>
        /// <returns>SEクリップ</returns>
        /// <param name="id">SE名</param>
        public AudioClip GetSeClip( SoundTable.eSE id )
        {
            if( id == SoundTable.eSE.NoSound )
                return null;

            return Resources.Load<AudioClip>( SoundTable.SE_PATH[(int)id] );
        }

        /// <summary>
        /// ボイス取得
        /// </summary>
        /// <returns>ボイスクリップ</returns>
        /// <param name="id">ボイス名</param>
        public AudioClip GetVoiceClip( SoundTable.eVoice id )
        {
            if( id == SoundTable.eVoice.NoSound )
                return null;

            return Resources.Load<AudioClip>( SoundTable.VOICE_PATH[(int)id] );
        }

        /// <summary>
        /// BGMボリューム設定
        /// </summary>
        /// <param name="volume">Volume.</param>
        public void SetBgmVolume( float volume )
        {
            m_Bgm.Volume = volume;
        }

        /// <summary>
        /// SE設定
        /// </summary>
        /// <param name="volume">Volume.</param>
        public void SetSeVolume( float volume )
        {
            m_Se.Volume = volume;
        }  
        
        /// <summary>
        /// ボイスボリューム設定
        /// </summary>
        /// <param name="volume">Volume.</param>
        public void SetVoiceVolume( float volume )
        {
            m_Bgm.Volume = volume;
        }

        /// <summary>
        /// BGM停止
        /// </summary>
        /// <param name="index">BGM名</param>
        /// <param name="time">切替時間</param>
        /// <param name="restart">リスタート</param>
        public void PlayBGM( SoundTable.eBGM index, float time = 0.0f, bool restart = false )
        {
            m_Bgm.Play( index, time, restart );
        }

        /// <summary>
        /// BGM停止
        /// </summary>
        /// <param name="time">停止時間</param>
        public void StopBGM( float time = 0.0f )
        {
            m_Bgm.Stop( time );
        }

        /// <summary>
        /// SE再生
        /// </summary>
        /// <param name="index">SE名</param>
        public void PlaySE( SoundTable.eSE index )
        {
            m_Se.Play( index );
        }

        /// <summary>
        /// ボイス再生
        /// </summary>
        /// <param name="index">ボイス名</param>
        public void PlayVoice( SoundTable.eVoice index )
        {
            m_Voice.Play( index );
        }
    }
}