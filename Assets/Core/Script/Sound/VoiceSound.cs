using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Core.Sound.Private
{

    /// <summary>
    /// ボイスサウンド
    /// </summary>
    public class VoiceSound : MonoBehaviour
    {
        /// <summary>
        /// オーディオソース
        /// </summary>
        AudioSource m_Source;

        /// <summary>
        /// サウンド情報
        /// </summary>
        List<VoiceInfo> m_SoundList = new List<VoiceInfo>();

        /// <summary>
        /// ボリューム
        /// </summary>
        float m_Volume = 1.0f;

        /// <summary>
        /// Voice再生数
        /// </summary>
        int m_Count = 0;

        /// <summary>
        /// ボリューム取得・設定
        /// </summary>
        public float Volume
        {
            get
            {
                return m_Volume;
            }

            set
            {
                if( 0.0f > value )
                {
                    value = 0.0f;
                }
                else if( 1.0f < value )
                {
                    value = 1.0f;
                }

                m_Volume = value;
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init( )
        {
            // オーディオソースチェック
            if( null == m_Source )
            {
                // オーディオソースコンポーネント追加
                m_Source = this.gameObject.AddComponent<AudioSource>();
            }
        }

        /// <summary>
        /// 再生
        /// </summary>
        /// <param name="id">ボイスのID</param>
        public void Play( SoundTable.eVoice id )
        {
            // 再生数チェック & IDチェック
            if( SoundManager.k_VOICE_MAX <= m_Count || SoundTable.eVoice.NoSound == id )
            {
                return;
            }

            // Voice情報取得
            VoiceInfo info = null;
            foreach( var voice in m_SoundList )
            {
                if( voice.id == id )
                {
                    info = voice;

                    break;
                }
            }

            // Voice情報チェック
            if( null == info )
            {
                info = new VoiceInfo( id );

                // Voice情報追加
                m_SoundList.Add( new VoiceInfo( id ) );
            }

            // 開始から終了まで待機
            StartCoroutine( StartToEnd( info ) );
        }

        /// <summary>
        /// 開始から終了まで待機
        /// </summary>
        /// <param name="info">Voice情報</param>
        IEnumerator StartToEnd( VoiceInfo info )
        {
            // Voice開始
            m_Source.PlayOneShot( info.clip, m_Volume );

            // 使用カウントプラス
            info.count++;
            m_Count++;

            // ボイス終了待ち
            yield return new WaitForSeconds( info.clip.length );

            // 使用カウントマイナス
            info.count--;
            m_Count--;

            // 使用チェック
            if( 0 == info.count )
            {
                // Voiceアンロード
                Resources.UnloadAsset( info.clip );

                m_SoundList.Remove( info );
            }
        }
    }

    /// <summary>
    /// ボイス情報
    /// </summary>
    public class VoiceInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public SoundTable.eVoice id;

        /// <summary>
        /// クリップ
        /// </summary>
        public AudioClip clip;

        /// <summary>
        /// 使用カウント
        /// </summary>
        public int count;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="clip">クリップ</param>
        public VoiceInfo( SoundTable.eVoice voice )
        {
            id = voice;
            clip = SoundManager.instance.GetVoiceClip( id );
            count = 0;

            if( clip == null )
            {
                Debug.LogError( "ID：" + id + "のボイスが見つかりません" );
            }
        }
    }
}