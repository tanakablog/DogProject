using System.IO;
using System.Linq;
using System.Reflection;
using System.Text; using UnityEditor; 
using UnityEngine;

public class MonoBehaviourExtensionCreater
{
    /// <summary>
    /// ファイルパス
    /// </summary>
    private const string m_PATH = "Assets/Core/Script/Extension/MonoBehaviourExtension.cs";

    /// <summary>
    /// ファイル名
    /// </summary>
    private static readonly string m_FILE_NAME = Path.GetFileNameWithoutExtension( m_PATH );

    /// <summary>
    /// スクリプト作成
    /// </summary>
    [MenuItem( "Tools/Create MonoBehaviourExtension" )]
    static void CreateScript( )
    {
        StringBuilder builder = new StringBuilder();

        // インポート
        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( );

        // クラス定義
        builder.AppendFormat( "public class {0} : MonoBehaviour\n", m_FILE_NAME );
        builder.AppendLine( "{" );

        // UnityEngineの全タイプ取得
        foreach( var type in Assembly.Load( "UnityEngine.dll" ).GetTypes() )
        {
            // キャッシュ化するか判定
            if( type.IsSubclassOf( typeof( Component ) ) && 
                type.IsPublic && 
                IsObsolete( type ) == false && 
                type != typeof( MonoBehaviour ) )
            {
                // タイプ名取得
                var name = type.Name;

                // キャッシュ変数宣言
                {
                    builder.AppendFormat( "\tprivate {0} m_{1};\n\n", name, name );
                }

                // プロパティ宣言
                {
                    // プロパティ名
                    builder.AppendFormat( "\tpublic {0} cached{1}\n", name, name );
                    builder.AppendLine( "\t{" );

                    // ゲッター
                    builder.AppendLine( "\t\tget" );
                    builder.AppendLine( "\t\t{" );

                    // 取得チェック
                    builder.AppendFormat( "\t\t\tif( m_{0} == null )\n", name );
                    builder.AppendLine( "\t\t\t{" );

                    // 取得
                    builder.AppendFormat( "\t\t\t\tm_{0} = GetComponent<{1}> ();\n", name, name );
                    builder.AppendLine( "\t\t\t}" );
                    builder.AppendLine();

                    // リターン
                    builder.AppendFormat( "\t\t\treturn m_{0};\n", name );
                    builder.AppendLine( "\t\t}" );
                    builder.AppendLine( "\t}" );
                }

                builder.AppendLine();
            }
        }

        // クラス定義終了
        builder.Append( "}" );

        // パス設定
        string path = m_PATH;

        // すでに同名のファイルがあるか検索
        var importer = MonoImporter.GetAllRuntimeMonoScripts().Where( i => i.GetClass() != null && i.GetClass().Name == m_FILE_NAME ).FirstOrDefault();

        // 同名ファイルチェック
        if( importer )
        {
            // あればそのファイルパスへ変更
            path = AssetDatabase.GetAssetPath( importer );
        }

        File.WriteAllText( path, builder.ToString() );
        AssetDatabase.ImportAsset( path );
    }

    static bool IsObsolete( System.Type type )
    {
        return type.GetCustomAttributes( typeof( System.ObsoleteAttribute ), true ).Length != 0;
    }
}