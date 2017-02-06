using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// スプライトテーブル作成
/// </summary>
public static class SpriteTableCreator
{
    /// <summary>
    /// コマンド名
    /// </summary>
    private const string m_ITEM_NAME = "Core/Create/Sprite Table";

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
    const string m_SPRITE_FOLDER_PATH = "Assets/Resources/Sprite";

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
    [MenuItem( m_ITEM_NAME, true )]
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
        builder.AppendLine( StringBuilderSupport.EditClass( m_CLASS_NAME, 1, true, "スプライトテーブルクラス ※自動生成されるクラスです" ) );

        // スプライトフォルダ内のスプライトパス取得
        string[] sprite_path = System.IO.Directory.GetFiles( m_SPRITE_FOLDER_PATH, "*.png", System.IO.SearchOption.AllDirectories );

        // スプライト情報取得
        TextureImporter[] sprite_data = GetSpriteInfo( sprite_path );

        // 無効文字チェック
        {
            // 無効文字が入った文字列検索＆取得
            TextureImporter sprite  = sprite_data.FirstOrDefault( x =>
            {
                // 無効文字取得
                m_InvalidChar = StringBuilderSupport.FindInvaild( x.name );

                // 無効文字があるかチェック
                return !string.IsNullOrEmpty( m_InvalidChar );
            } );

            // 無効文字が入った文字列があったかチェック
            if( sprite != null )
            {
                // 無効文字が入った文字列設定
                m_InvalidName = sprite.name;

                // スクリプト作成失敗
                return false;
            }
        }

        // スプライト情報並べ替え( パッキングタグ名 & スプライト名 )
        sprite_data.OrderBy( x => x.name ).ThenBy( x => x.spritePackingTag.ToString() );

        // タグ名保存リスト生成
        var tag_name = new HashSet<string>();

        // パッキングタグ列挙型作成                                    
        {
            // スプライト情報ごとに処理
            foreach( var sprite in sprite_data )
            {
                // パッキングタグ名リストに追加
                tag_name.Add( sprite.spritePackingTag );
            }

            // 無効文字チェック
            {
                // 無効文字が入った文字列検索＆取得
                m_InvalidName = tag_name.FirstOrDefault( x =>
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
            }

            // 列挙型定義
            builder.AppendLine( StringBuilderSupport.EditEnum( "eTag", StringBuilderSupport.JoinStrings( tag_name.ToArray(), 3 ), 2, "パッキングタグ名" ) );
        }

        // スプライトインデックス
        int[] sprite_index = new int[tag_name.Count()];

        // 個別スプライト名保存リスト
        List<string>[] sprite_name_list = new List<string>[tag_name.Count];
        for( int i = 0; i < sprite_name_list.Length; ++i )
        {
            sprite_name_list[i] = new List<string>();
        }

        // 全スプライト列挙型作成
        {
            // タブ数設定
            string tab = StringBuilderSupport.SetTab( 3 );

            // 列挙型定義開始
            builder.Append( StringBuilderSupport.EditEnum( "eSprite", 2, "スプライト名" ) );

            int index = 0;
            int count = 0;

            // スプライト情報ごとに処理
            foreach( var sprite in sprite_data )
            {
                // タグ名取得
                string tag = tag_name.ToArray()[index];

                // パッキングタグ名チェック
                if( sprite.spritePackingTag != tag )
                {
                    // スプライトインデックス数保存
                    sprite_index[index] = count;

                    // インデックス数増加
                    index++;

                    // カウンタ初期化
                    count = 0;
                }

                // スプライト名保存
                sprite_name_list[index].Add( sprite.name );

                // 列挙型内容定義
                builder.AppendLine( tab + tag + "_" + sprite.name + "," );

                // カウンタ増加
                count++;
            }

            // スプライトインデックス数保存
            sprite_index[index] = count;

            // 列挙型定義終了
            builder.AppendLine( StringBuilderSupport.SetTab( 2 ) + "}" );
        }

        // 改行
        builder.AppendLine();

        // 個別スプライト列挙体作成
        {
            // スプライト列挙型文字列
            List<string> sprite_enum_string = new List<string>();

            // スプライト名ごとに処理
            for( int i = 0; i < sprite_name_list.Length; ++i )
            {
                Debug.Log( sprite_name_list[i] );                       

                // タグ名取得
                string tag = tag_name.ToArray()[i];

                // 列挙型内容生成
                sprite_enum_string.AddRange( sprite_name_list[i].Select( x => x + " = eSprite." + tag + "_" + x ) );

                // 列挙型定義
                builder.AppendLine( StringBuilderSupport.EditEnum(
                    "e" + tag,                                                              // 列挙型名
                    StringBuilderSupport.JoinStrings( sprite_enum_string.ToArray(), 3 ),    // 内容
                    2,                                                                      // タブ数
                    tag + "のスプライト名" ) );                                             // 説明

                // リストクリア
                sprite_enum_string.Clear();
            }
        }

        // スプライト配列作成
        {
            // 説明定義
            builder.Append( StringBuilderSupport.EditSummary( "スプライトパス配列", 2 ) );

            // 定義開始
            builder.AppendLine( StringBuilderSupport.SetTab( 2 ) + "public static readonly string[] m_SPRITE_PATH = {" );

            // 内容定義
            builder.AppendLine( StringBuilderSupport.JoinStrings(
                sprite_data.Select( x => 
                {
                    string name = x.assetPath.Replace( "\\", "/" ).Substring( "Assets/Resources/".Length );

                    return name.Substring( 0, name.Length - ".png".Length );
                } ).ToArray(), 
                3, 
                ",\n", 
                "\"", 
                "\"" ) );

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
            builder.AppendLine( StringBuilderSupport.SetTab( 2 ) + "public static readonly int[] m_SPRITE_INDEX = {" );

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
    /// スプライト情報取得
    /// </summary>
    /// <param name="sprite_path">スプライトパス</param>
    /// <returns>テクスチャインポーター配列</returns>
    private static TextureImporter[] GetSpriteInfo( string[] sprite_path )
    {
        return sprite_path.Select( x =>
        {
            // テクスチャ情報取得
            TextureImporter sprite = AssetImporter.GetAtPath( x ) as TextureImporter;

            // スプライト名取得
            sprite.name = Path.GetFileNameWithoutExtension( x ); 

            return sprite;

        } ).ToArray();
    }

    /// <summary>
    /// 作成できるかどうかを取得します
    /// </summary>
    [MenuItem( m_ITEM_NAME, true )]
    private static bool CanCreate( )
    {
        return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
    }


}