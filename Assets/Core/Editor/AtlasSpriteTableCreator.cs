using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public static class AtlasSpriteTableCreator
{
    /// <summary>
    /// コマンド名
    /// </summary>
    private const string m_ITEM_NAME = "Core/Create/Atlas Sprite Table";

    /// <summary>
    /// 作成ファイルパス
    /// </summary>
    private const string m_PATH = "Assets/CreateTable/SpriteTable.cs";

    /// <summary>
    /// ファイル名
    /// </summary>
    private static readonly string m_FILE_NAME = Path.GetFileName( m_PATH );

    /// <summary>
    /// ファイル名（拡張子なし）
    /// </summary>
    private static readonly string m_CLASS_NAME = Path.GetFileNameWithoutExtension( m_PATH );

    /// <summary>
    /// アトラスフォルダパス
    /// </summary>
    const string ATLAS_FOLDER_PATH = "Assets/Resources/Atlas";

    /// <summary>
    /// ネームスペース
    /// </summary>
    const string m_NAME_SPACE = "Core.SpriteCore";

    /// <summary>
    /// 無効文字が入った名前
    /// </summary>
    static string m_InvalidName;

    /// <summary>
    /// 無効文字
    /// </summary>
    static string m_InvalidChar;

    /// <summary>
    /// サウンドテーブルを作成します
    /// </summary>
    [MenuItem(m_ITEM_NAME)]
    public static void Create()
    {
        if (!CanCreate())
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
    public static bool CreateScript()
    {
        // ビルダー生成
        var builder = new StringBuilder();

        // 名前空間定義開始
        builder.AppendLine( StringBuilderSupport.EditNameSpace( m_NAME_SPACE ) );

        // クラス定義開始
        builder.AppendLine( StringBuilderSupport.EditClass( m_CLASS_NAME, 1, true, "スプライトテーブルクラス ※自動生成されるクラスです" ) );

        // アトラスフォルダ内のアトラスパス取得
        string[] atlas_path = System.IO.Directory.GetFiles(ATLAS_FOLDER_PATH, "*.png", System.IO.SearchOption.AllDirectories);

        // スプライト情報取得
        List<SpriteMetaData>[] sprite_data = GetSpriteMetaData( atlas_path );
                
        // アトラスパス変更
        for (int i = 0; i < atlas_path.Length; ++i)
        {
            // アトラスフォルダまでのパスを削除
            atlas_path[i] = atlas_path[i].Substring("Assets/Resources/".Length);

            // 拡張子削除
            atlas_path[i] = atlas_path[i].Substring( 0, atlas_path[i].Length - 4 );

            // 階層文字を変更
            atlas_path[i] = atlas_path[i].Replace("\\", "/");
        }

        // パスからアトラス名取得
        List<string> atlas_name = new List<string>();
        atlas_name.Add( "NoAtlas" );
        atlas_name.AddRange( atlas_path.Select( x => Path.GetFileName( x ) ).ToArray() );

        // アトラス列挙型作成                                    
        {     
            // 無効文字が入った文字列検索＆取得
            m_InvalidName = atlas_name.FirstOrDefault( x =>
            {
                // 無効文字取得
                m_InvalidChar = StringBuilderSupport.FindInvaild( x );

                // 無効文字があるかチェック
                return !string.IsNullOrEmpty( m_InvalidChar );
            } );

            // 無効文字が入った文字列があったかチェック
            if( !string.IsNullOrEmpty( m_InvalidName ) )
            {
                // スクリプト作成失敗
                return false;
            }

            // 列挙型定義
            builder.AppendLine( StringBuilderSupport.EditEnum( "eAtlas", StringBuilderSupport.JoinStrings( atlas_name.ToArray(), 3 ), 2, "アトラス名" ) );
        }

        // スプライトインデックス
        int[] sprite_index = new int[atlas_path.Length];

        // 個別スプライト名保存リスト
        List<string>[] sprite_name_list = new List<string>[sprite_data.Length];

        // 全スプライト列挙型作成
        {
            // スプライト名保存リスト生成                 
            var all_sprite_name_hash = new HashSet<string>();

            // タブ数設定
            string tab = StringBuilderSupport.SetTab( 3 );

            // 列挙型定義開始
            builder.Append( StringBuilderSupport.EditEnum( "eSprite", 2, "スプライト名" ) );

            // アトラス名表示
            builder.AppendLine( tab + "NoSprite = 0," );

            // アトラス毎に処理
            foreach( var list in sprite_data.Select( ( data, index ) => new { data, index } ) )
            {
                // アトラス名表示
                builder.AppendLine( tab + "// アトラス名：" + atlas_name[list.index+1] );

                // 無効文字が入った文字列検索＆取得
                m_InvalidName = list.data.FirstOrDefault( x =>
                {
                    // 無効文字取得
                    m_InvalidChar = StringBuilderSupport.FindInvaild( x.name );

                    // 無効文字があるかチェック
                    return !string.IsNullOrEmpty( m_InvalidChar );
                } ).name;

                // 無効文字が入った文字列があったかチェック
                if( !string.IsNullOrEmpty( m_InvalidName ) )
                {
                    // スクリプト作成失敗
                    return false;
                }

                // スプライトリスト生成
                sprite_name_list[list.index] = new List<string>();

                // スプライト名取得
                sprite_name_list[list.index].AddRange( list.data.Select( x => x.name ) );

                // スプライト名ソート
                sprite_name_list[list.index].Sort();

                // スプライト名毎に処理&インデックス数取得
                sprite_index[list.index] = sprite_name_list[list.index].Count( x =>
                {
                    // 登録済みでなければ追加
                    if( all_sprite_name_hash.Add( x ) )
                    {
                        // 登録済みでなかったので名前表示
                        builder.AppendLine( tab + x + "," );

                        // カウンター増加
                        return true;
                    }

                    // 登録済み
                    builder.AppendLine( tab + "// " + x + " はすでに使われています" );

                    return false;
                } );
            }

            // 列挙型定義終了
            builder.AppendLine( StringBuilderSupport.SetTab( 2 ) + "}");
        }

        // 改行
        builder.AppendLine();

        // 個別スプライト列挙体作成
        {
            // スプライト列挙型文字列
            List<string> sprite_enum_string = new List<string>();

            // アトラス毎に処理
            foreach( var list in sprite_name_list.Select( ( names, index ) => new { names, index } ) )
            {
                // 列挙型内容生成
                sprite_enum_string.AddRange( list.names.Select( x => x + " = eSprite." + x ));

                // 列挙型定義
                builder.AppendLine( StringBuilderSupport.EditEnum( 
                    "e" + atlas_name[list.index],                                           // 列挙型名
                    StringBuilderSupport.JoinStrings( sprite_enum_string.ToArray(), 3 ),    // 内容
                    2,                                                                      // タブ数
                    atlas_name[list.index] + "のスプライト名" ) );                         // 説明

                // リストクリア
                sprite_enum_string.Clear();                
            }
        }

        // アトラスパス配列作成
        {
            // 説明定義
            builder.Append( StringBuilderSupport.EditSummary( "アトラスパス配列", 2 ) );

            // 定義開始
            builder.AppendLine( StringBuilderSupport.SetTab( 2 ) + "public static readonly string[] ATLAS_PATH = {" );

            // ダミー定義
            builder.AppendLine( StringBuilderSupport.SetTab( 3 ) + "\"dummy\"," );

            // 内容定義
            builder.AppendLine( StringBuilderSupport.JoinStrings( atlas_path, 3, ",\n", "\"", "\"" ) );

            // 定義終了
            builder.AppendLine( StringBuilderSupport.SetTab( 2 ) + "};" );
        }

        // 改行
        builder.AppendLine();

        // スプライトインデックス配列作成
        {
            // 説明定義
            builder.Append( StringBuilderSupport.EditSummary( "スプライトインデックス配列", 2 ) );

            // 定義開始
            builder.AppendLine( StringBuilderSupport.SetTab( 2 ) + "public static readonly int[] SPRITE_INDEX = {" );

            // ダミー定義
            builder.AppendLine( StringBuilderSupport.SetTab( 3 ) + "1," );

            // 内容定義
            builder.AppendLine( StringBuilderSupport.JoinStrings( sprite_index.Select( x => x.ToString() ).ToArray(), 3 ) );

            // 定義終了
            builder.AppendLine( StringBuilderSupport.SetTab( 2 ) + "};" );
        }

        // クラス定義終了
        builder.AppendLine( "\t}" );

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
    /// スプライトメタデータ取得
    /// </summary>
    /// <param name="atlas_path">アトラスパス</param>
    /// <returns>スプライトメタデータリスト</returns>
    private static List<SpriteMetaData>[] GetSpriteMetaData( string[] atlas_path )
    {
        // スプライトメタデータリスト生成
        List<SpriteMetaData>[] list = new List<SpriteMetaData>[atlas_path.Length];

        // テクスチャインポーター生成
        TextureImporter[] importer = new TextureImporter[atlas_path.Length];

        // アトラス内のスプライト情報取得
        for( int i = 0; i < atlas_path.Length; ++i )
        {
            // アトラスパスから情報取得
            importer[i] = AssetImporter.GetAtPath( atlas_path[i] ) as TextureImporter;

            // リスト生成
            list[i] = new List<SpriteMetaData>();

            // スプライトメタデータ取得
            list[i].AddRange( importer[i].spritesheet );
        }

        return list;
    }

    /// <summary>
    /// 作成できるかどうかを取得します
    /// </summary>
    [MenuItem(m_ITEM_NAME, true)]
    private static bool CanCreate()
    {
        return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
    }

    
}