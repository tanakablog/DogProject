using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public static class SoundTableCreator
{
    /// <summary>
    /// 使用できる拡張子
    /// </summary>
    static readonly string[] m_EXTENSION = 
    {
        "alf","wav","mp3","ogg"
    };

    /// <summary>
    /// コマンド名
    /// </summary>
    private const string m_ITEM_NAME = "Core/Create/Sound Table";

    /// <summary>
    /// 作成ファイルパス
    /// </summary>
    private const string m_PATH = "Assets/CreateTable/SoundTable.cs";

    /// <summary>
    /// ファイル名
    /// </summary>
    private static readonly string m_FILE_NAME = Path.GetFileName( m_PATH );

    /// <summary>
    /// ファイル名（拡張子なし）
    /// </summary>
    private static readonly string m_CLASS_NAME = Path.GetFileNameWithoutExtension( m_PATH );

    /// <summary>
    /// サウンドタイプ配列
    /// </summary>
    private static readonly string[] m_SOUND_TYPE = 
    {
        "BGM", "SE", "Voice"
    };

    /// <summary>
    /// サウンドフォルダパス
    /// </summary>
    private const string SOUND_FOLDER_PATH = "Assets/Resources/Sound";

    /// <summary>
    /// ネームスペース
    /// </summary>
    private const string m_NAME_SPACE = "Core.Sound";

    /// <summary>
    /// 音なし名
    /// </summary>
    private const string m_NO_SOUND_NAME = "NoSound";

    /// <summary>
    /// 無効文字が入った名前
    /// </summary>
    private static string m_InvalidName;

    /// <summary>
    /// 無効文字
    /// </summary>
    private static string m_InvalidChar;

    /// <summary>
    /// サウンドテーブルを作成します
    /// </summary>
    [MenuItem( m_ITEM_NAME )]
    public static void Create( )
    {
        if( !CanCreate() )
        {
            return;
        }

        if( CreateScript() )
        {
            EditorUtility.DisplayDialog( m_FILE_NAME, "生成完了！", "OK" );
        }
        else
        {
            EditorUtility.DisplayDialog( m_FILE_NAME, "「" + m_InvalidName + "」に「" + m_InvalidChar + "」が含まれているため、失敗しました。", "OK" );
        }
    }

    /// <summary>
    /// スクリプトを作成します
    /// </summary>
    public static bool CreateScript( )
    {
        // ビルダー生成
        var builder = new StringBuilder();

        // 名前空間定義開始
        builder.AppendLine( StringBuilderSupport.EditNameSpace( m_NAME_SPACE ) );

        // クラス定義開始
        builder.AppendLine( StringBuilderSupport.EditClass( m_CLASS_NAME, 1, true, "サウンドテーブルクラス ※自動生成されるクラスです" ) );

        // サウンドパスリスト配列生成
        List<string>[] sound_path = new List<string>[m_SOUND_TYPE.Length];

        // サウンドフォルダパスごとに処理
        for( int i = 0; i < sound_path.Length; ++i )
        {
            // サウンドパスリスト生成
            sound_path[i] = new List<string>();

            // 拡張子ごとに処理
            foreach( var extension in m_EXTENSION )
            {
                // サウンドパスリストに追加
                sound_path[i].AddRange( System.IO.Directory.GetFiles( SOUND_FOLDER_PATH + "/" + ( m_SOUND_TYPE[i] ).ToString(), "*." + extension, System.IO.SearchOption.AllDirectories ) );
            }

            // サウンドフォルダまでのパス削除
            sound_path[i] = sound_path[i].Select( x => x.Substring( "Assets/Resources/".Length ).Replace( "\\", "/" ) ).ToList();
        }

        // サウンド列挙型作成
        {
            // サウンドタイプ列挙型定義
            builder.AppendLine( StringBuilderSupport.EditEnum(
                "eType",                                                // 列挙型名
                StringBuilderSupport.JoinStrings( m_SOUND_TYPE, 3 ),    // 内容
                2,                                                      // タブ数
                "サウンドタイプ" ) );                                   // 説明

            // サウンド名リスト生成
            List<string> sound_name = new List<string>();

            // タブ数設定
            string tab = StringBuilderSupport.SetTab( 3 );

            // サウンドタイプごとに処理
            for( int i = 0; i < m_SOUND_TYPE.Length; ++i )
            {     
                // サウンド名取得
                sound_name.AddRange( sound_path[i].Select( x => Path.GetFileNameWithoutExtension( x ) ) );

                // 無効文字検索
                m_InvalidName = sound_name.FirstOrDefault( x =>
                {
                    m_InvalidChar = StringBuilderSupport.FindInvaild( x.ToString() );

                    return !string.IsNullOrEmpty( m_InvalidChar );
                } );

                // 無効文字が入った文字列があったかチェック
                if( !string.IsNullOrEmpty( m_InvalidName ) )
                {
                    // スクリプト作成失敗
                    return false;
                }

                // 列挙型定義開始
                builder.AppendLine( StringBuilderSupport.EditEnum(
                    "e" + m_SOUND_TYPE[i],              // 列挙型名
                    2,                                  // タブ数
                    m_SOUND_TYPE[i] + "タイプ" ) );     // 説明

                // 内容定義
                builder.AppendLine( tab + m_NO_SOUND_NAME + " = -1, // 音なし\n" );
                builder.AppendLine( StringBuilderSupport.JoinStrings( sound_name.ToArray(), 3 ) );

                // 列挙型定義終了
                builder.AppendLine( StringBuilderSupport.SetTab( 2 ) + "}" );

                // 改行
                builder.AppendLine();

                // サウンド名リスト初期化
                sound_name.Clear();
            }
        }

        // サウンドパス配列作成
        {
            // タブ設定
            string tab = StringBuilderSupport.SetTab( 2 );
            string content_tab = StringBuilderSupport.SetTab( 3 );

            // サウンドタイプごとに処理
            for( int i = 0; i < m_SOUND_TYPE.Length; ++i )
            {
                // 説明定義
                builder.Append( StringBuilderSupport.EditSummary( m_SOUND_TYPE[i] + "パス", 2 ) );
                
                // 配列定義開始
                builder.AppendLine( tab + "public static readonly string[] " + m_SOUND_TYPE[i].ToUpper() + "_PATH = {" );

                // 数チェック
                if( sound_path[i].Count == 0 )
                {
                    // サウンドなし
                    builder.AppendLine( content_tab + "\"" + m_NO_SOUND_NAME + "(dummy)\"" );

                    // 配列定義終了
                    builder.AppendLine( tab + "};" );

                    // 改行
                    builder.AppendLine();

                    continue;
                }

                // サウンドパスから拡張子排除
                sound_path[i] = sound_path[i].Select( x => x.Substring( 0, x.Length - Path.GetExtension( x ).Length ) ).ToList();

                // 配列内容定義
                builder.AppendLine( StringBuilderSupport.JoinStrings( sound_path[i].ToArray(), 3, ",\n", "\"", "\"" ) );

                // 配列定義終了
                builder.AppendLine( tab + "};" );

                // 改行
                builder.AppendLine();
            }
        }

        // クラス定義終了
        builder.AppendLine( StringBuilderSupport.SetTab( 1 ) + "}" );

        // 名前空間定義終了
        builder.AppendLine( "}" );

        // ディレクトリパス取得
        var directory = Path.GetDirectoryName( m_PATH );

        // ディレクトリがあるかチェック
        if( !Directory.Exists( directory ) )
        {
            // ディレクトリを作成
            Directory.CreateDirectory( directory );
        }

        // スプライト生成
        File.WriteAllText( m_PATH, builder.ToString(), Encoding.UTF8 );

        AssetDatabase.Refresh( ImportAssetOptions.ImportRecursive );

        // スプライト生成成功
        return true;
    }

    /// <summary>
    /// 作成できるかどうかを取得します
    /// </summary>
    [MenuItem( m_ITEM_NAME, true )]
    public static bool CanCreate( )
    {
        return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
    }
}