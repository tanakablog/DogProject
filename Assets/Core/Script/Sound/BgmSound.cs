using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Core.Sound.Private
{

    /// <summary>
    /// BGMサウンド
    /// </summary>
    public class BgmSound : MonoBehaviour
    {

        // BGMタイプ
        enum eBgm
        {
            Main,
            Sub
        }

        /// <summary>
        /// BGMソース
        /// </summary>
        Bgmsource[] m_source = new Bgmsource[Extension.EnumExtension.GetEnumNum<eBgm>()];

        /// <summary>
        /// BGM情報リスト
        /// </summary>
        List<BgmInfo> m_SoundList = new List<BgmInfo>();

        /// <summary>
        /// ボリューム
        /// </summary>
        float m_Volume = 1.0f;

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
                m_source[(int)eBgm.Main].source.volume = m_Volume = Mathf.Clamp( value, 0.0f, 1.0f );
            }
        }

        private bool m_IsStop;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init( )
        {
            // オーディオソースチェック
            if( null == m_source[0] )
            {
                for( int i = 0; i < m_source.Length; ++i )
                {
                    m_source[i] = new Bgmsource();

                    m_source[i].source = this.gameObject.AddComponent<AudioSource>();

                    m_source[i].source.loop = true;
                }
            }
        }

        /// <summary>
        /// 再生
        /// </summary>
        /// <param name="id">BGMのID</param>
        /// <param name="time">切替時間</param>
        /// <param name="restart">trueの場合、同じBGMでも初めから再生します</param>
        public bool Play( SoundTable.eBGM id, float time, bool restart = false )
        {
            if( SoundTable.eBGM.NoSound == id )
            {
                return false;
            }
                                
            // BGM情報存在チェック
            if( m_source[(int)eBgm.Main].info != null )
            {
                // 同じBGMの場合は何もしない & idチェック
                if( m_source[(int)eBgm.Main].info.Id == id && !restart )
                {
                    bool is_success = false;

                    if( m_IsStop )
                    {
                        m_source[(int)eBgm.Main].Play();
                        m_source[(int)eBgm.Main].fade = FadeBgmVolume( eBgm.Main, m_Volume, time, null );
                        StartCoroutine( m_source[(int)eBgm.Main].fade );
                        is_success = true;
                    }

                    m_IsStop = false;
                      
                    return is_success;
                }
            }

            // フェードチェック
            foreach( var bgm in m_source )
            {
                if( bgm.fade != null )
                {
                    StopCoroutine( bgm.fade );

                    bgm.fade = null;
                }
            }

            // 再生チェック
            if( m_source[(int)eBgm.Sub].source.isPlaying )
            {
                // 再生停止
                m_source[(int)eBgm.Sub].source.Stop();
                m_source[(int)eBgm.Sub].source.clip = null;
            }

            // メインからサブへ 
            Bgmsource source = m_source[(int)eBgm.Sub];
            m_source[(int)eBgm.Sub] = m_source[(int)eBgm.Main];

            // サブフェード開始
            if( m_source[(int)eBgm.Main].info != null )
            {
                StartCoroutine( FadeSubBgm( time ) );
            }

            // メイン情報設定
            m_source[(int)eBgm.Main] = source;
            m_source[(int)eBgm.Main].info = GetBgmInfo( id );
            m_source[(int)eBgm.Main].info.Count++;
            m_source[(int)eBgm.Main].Play();

            // メインフェード開始
            m_source[(int)eBgm.Main].fade = FadeBgmVolume( eBgm.Main, m_Volume, time, null );
            StartCoroutine( m_source[(int)eBgm.Main].fade );

            m_IsStop = false;

            return true;
        }

        /// <summary>
        /// BGM停止
        /// </summary>
        /// <param name="time">停止までの時間</param>
        /// <returns>true = 停止開始, false = 停止中のため失敗</returns>
        public bool Stop( float time = 0.0f )
        {
            m_IsStop = true;

            // フェードチェック
            if( m_source[(int)eBgm.Main].fade != null )
            {
                // フェード停止
                StopCoroutine( m_source[(int)eBgm.Main].fade );

                // フェード初期化
                m_source[(int)eBgm.Main].fade = null;
            }

            // フェード設定
            m_source[(int)eBgm.Main].fade = FadeBgmVolume( eBgm.Main, 0.0f, time, m_source[(int)eBgm.Main].source.Stop );

            // フェード開始
            StartCoroutine( m_source[(int)eBgm.Main].fade );

            return true;
        }

        /// <summary>
        /// サブBGM停止
        /// </summary>
        /// <param name="time">停止までの時間</param>
        IEnumerator FadeSubBgm( float time )
        {
            // カウント減少
            m_source[(int)eBgm.Sub].info.Count--;

            // フェード設定                                             
            m_source[(int)eBgm.Sub].fade = FadeBgmVolume( eBgm.Sub, 0.0f, time, m_source[(int)eBgm.Sub].source.Stop );

            // フェード開始
            yield return StartCoroutine( m_source[(int)eBgm.Sub].fade );

            // アンロードチェック
            if( m_source[(int)eBgm.Main].info.isUnLoad && m_source[(int)eBgm.Main].info.Count <= 0 )
            {
                // リストから削除
                m_SoundList.Remove( m_source[(int)eBgm.Main].info );

                // BGMクリップアンロード
                Resources.UnloadAsset( m_source[(int)eBgm.Main].info.Clip );

                m_source[(int)eBgm.Main].info = null;
            }
        }

        /// <summary>
        /// BGM情報取得
        /// </summary>
        /// <param name="id">BGMのID</param>
        /// <returns>BGM情報</returns>
        public BgmInfo GetBgmInfo( SoundTable.eBGM id )
        {
            BgmInfo info = null;

            foreach( var bgm in m_SoundList )
            {
                if( bgm.Id == id )
                {
                    info = bgm;

                    break;
                }
            }

            if( info == null )
            {
                info = new BgmInfo( id );

                m_SoundList.Add( info );
            }

            return info;
        }

        /// <summary>
        /// フェードBGMボリューム
        /// </summary>
        /// <param name="type">BGMタイプ</param>
        /// <param name="volume">フェード後ボリューム</param>
        /// <param name="time">フェードタイム</param>
        /// <param name="call_back">コールバック関数</param>
        IEnumerator FadeBgmVolume( eBgm type, float volume, float time, Action call_back = null )
        {
            // タイムチェック
            if( time <= 0.0f )
            {
                // １フレームは確保
                time = 0.01f;
            }

            // 変化量算出
            float change = ( volume - m_source[(int)type].source.volume ) / time;

            // タイマー
            float timer = 0.0f;

            // フェード処理
            while( time > timer )
            {
                // 1フレーム待機
                yield return null;

                // ボリューム変更
                m_source[(int)type].source.volume += change * Time.deltaTime;

                // タイマー加算
                timer += Time.deltaTime;
            }

            // ボリューム設定
            m_source[(int)type].source.volume = volume;

            // コールバック関数呼び出し
            if( call_back != null )
            {
                call_back();
            }
        }
    }

    /// <summary>
    /// BGM情報クラス
    /// </summary>
    public class BgmInfo
    {
        /// <summary>
        /// BGMID
        /// </summary>
        public SoundTable.eBGM Id;

        /// <summary>
        /// クリップ
        /// </summary>
        public AudioClip Clip;

        /// <summary>
        /// 使用カウント
        /// </summary>
        public int Count;

        /// <summary>
        /// アンロードフラグ
        /// </summary>
        public bool isUnLoad;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="clip">クリップ</param>
        public BgmInfo( SoundTable.eBGM id, bool un_load = true )
        {
            Id = id;
            Clip = SoundManager.instance.GetBgmClip( id );
            Count = 0;
            isUnLoad = un_load;

            if( Clip == null )
            {        
                Debug.LogError( "ID：" + id + "のBGMが見つかりません" );
            }
        }
    }

    /// <summary>
    /// BGMソース
    /// </summary>
    public class Bgmsource
    {
        public AudioSource source;

        public BgmInfo info;

        public IEnumerator fade;

        public void Play( )
        {
            if( info == null )
                return;

            source.clip = info.Clip;

            source.Play();
        }
    }
}