// エディタ or アイフォン or アンドロイド
#if UNITY_EDITOR
#define EDITOR
#elif UNITY_IPHONE
#define IPHONE
#elif UNITY_ANDROID
#define ANDROID
#endif

// エディタ&アイフォン or エディタ&アンドロイド
#if UNITY_IPHONE
#define EDITOR_IPHONE
#elif UNITY_ANDROID
#define EDITOR_ANDROID
#endif

using System;

namespace Core.Platform
{
    /// <summary>
    /// プラットフォームアクション
    /// </summary>
    public class PlatformAction
    {
        /// <summary>
        /// エディタ
        /// </summary>
        const string m_EDITOR = "EDITOR";

        /// <summary>
        /// アイフォン
        /// </summary>
        const string m_IPHONE = "IPHONE";

        /// <summary>
        /// アンドロイド
        /// </summary>
        const string m_ANDROID = "ANDROID";

        /// <summary>
        /// エディタ&アイフォン
        /// </summary>
        const string m_EDITOR_IPHONE = m_EDITOR + "_" + m_IPHONE;

        /// <summary>
        /// エディタ&アンドロイド
        /// </summary>
        const string m_EDITOR_ANDROID = m_EDITOR + "_" + m_ANDROID;

        /// <summary>
        /// エディタのみ実行
        /// </summary>
        /// <param name="action">実行するアクション</param>
        [System.Diagnostics.Conditional( m_EDITOR )]
        public static void EditorOnlyAction( Action action )
        {
            if( action != null )
            {
                action();
            }
        }

        /// <summary>
        /// アイフォンのみ実行
        /// </summary>
        /// <param name="action">実行するアクション</param>
        [System.Diagnostics.Conditional( m_IPHONE )]
        public static void iPhoneOnlyAction( Action action )
        {
            if( action != null )
            {
                action();
            }
        }

        /// <summary>
        /// アンドロイドのみ実行
        /// </summary>
        /// <param name="action">実行するアクション</param>
        [System.Diagnostics.Conditional( m_ANDROID )]
        public static void AndroidOnlyAction( Action action )
        {
            if( action != null )
            {
                action();
            }
        }

        /// <summary>
        /// エディタとアイフォンのみ実行
        /// </summary>
        /// <param name="action">実行するアクション</param>
        [System.Diagnostics.Conditional( m_EDITOR_IPHONE )]
        public static void EditorAndiPhoneAction( Action action )
        {
            if( action != null )
            {
                action();
            }
        }

        /// <summary>
        /// エディタとアンドロイドのみ実行
        /// </summary>
        /// <param name="action">実行するアクション</param>
        [System.Diagnostics.Conditional( m_EDITOR_ANDROID )]
        public static void EditorAndAndroidAction( Action action )
        {
            if( action != null )
            {
                action();
            }
        }
    }
}
