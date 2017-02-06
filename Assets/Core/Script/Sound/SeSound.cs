using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core.Sound.Private
{
    /// <summary>
    /// SEサウンド
    /// </summary>
    public class SeSound : MonoBehaviour
    {
        /// <summary>
        /// オーディオソース
        /// </summary>
        private AudioSource m_Source;

        /// <summary>
        /// サウンド情報
        /// </summary>
        public List<SeInfo> m_SoundList = new List<SeInfo>();

        /// <summary>
        /// ボリューム
        /// </summary>
        public float m_Volume = 1.0f;

        /// <summary>
        /// SE再生数
        /// </summary>
        public int m_Count = 0;

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
        /// <param name="id">SEのID</param>
        public void Play( SoundTable.eSE id )
        {
            // 再生数チェック & IDチェック
            if( SoundManager.k_SE_MAX <= m_Count || SoundTable.eSE.NoSound == id )
            {
                return;
            }

            SeInfo info = null;

            // SE情報取得
            foreach( var se in m_SoundList )
            {
                if( se.id == id )
                {
                    info = se;

                    break;
                }
            }

            // SE情報チェック
            if( null == info )
            {
                info = new SeInfo( id );

                // SE情報追加
                m_SoundList.Add( info );
            }

            // 開始から終了まで待機
            StartCoroutine( StartToEnd( info ) );
        }

        /// <summary>
        /// 開始から終了まで待機
        /// </summary>
        /// <param name="info">SE情報</param>
        IEnumerator StartToEnd( SeInfo info )
        {
            // SE開始
            m_Source.PlayOneShot( info.clip, m_Volume );

            // 使用カウントプラス
            info.count++;
            m_Count++;

            // SE終了待ち
            yield return new WaitForSeconds( info.clip.length );

            // 使用カウントマイナス
            info.count--;
            m_Count--;

            // 使用チェック
            if( 0 == info.count )
            {
                // SEアンロード
                Resources.UnloadAsset( info.clip );

                m_SoundList.Remove( info );
            }
        }
    }

    /// <summary>
    /// SE情報
    /// </summary>
    public class SeInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public SoundTable.eSE id;

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
        public SeInfo( SoundTable.eSE se )
        {
            id = se;
            clip = SoundManager.instance.GetSeClip( id );
            count = 0;

            if( clip == null )
            {
                Debug.LogError( "ID：" + id + "のSEが見つかりません" );
            }
        }
    }
}