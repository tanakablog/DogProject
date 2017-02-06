using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class ResourcesTableCreator
{
    /// <summary>
    /// コマンド名
    /// </summary>
	private const string m_ITEM_NAME = "Core/Create/Resources Table";

    /// <summary>
    /// 作成ファイルパス
    /// </summary>
	private const string m_PATH = "Assets/CreateTable/ResourcesTable.cs";

    /// <summary>
    /// ファイル名
    /// </summary>
    private static readonly string m_FILE_NAME = Path.GetFileName( m_PATH );

    /// <summary>
    /// ファイル名（拡張子なし）
    /// </summary>
    private static readonly string m_CLASS_NAME = Path.GetFileNameWithoutExtension( m_PATH );

    /// <summary>
    /// ネームスペース
    /// </summary>
    const string m_NAME_SPACE = "Core.ResourcesCore";

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
		var builder = new StringBuilder();

		// ネームスペース定義
		{
			builder.AppendFormat("namespace {0}\n", m_NAME_SPACE);
			builder.AppendLine("{");
		}

		// クラス定義
		{
			builder.AppendFormat("public static class {0}", m_CLASS_NAME).AppendLine();
			builder.AppendLine("{");
		}

		// Assetsに入っているすべて列挙
		string[] files = System.IO.Directory.GetFiles(Application.dataPath, "*", System.IO.SearchOption.AllDirectories);

		List<string> fileList = new List<string>();

		// ファイルのパスからResourcesを含まれているものだけを探す
		foreach (var f_path in files)
		{
			// Assetのパスまで削除
			string path = f_path.Replace(Application.dataPath + "\\", "").Replace("\\", "/");

			if (path.IndexOf("Editor") != -1)
				continue;

			if (path.IndexOf("Resources") == -1)
				continue;

			if (path.IndexOf(".meta") != -1)
				continue;

			if (path.IndexOf(".cs") != -1)
				continue;

			fileList.Add(path);
		}

		// Enumを定義させる
		{
			builder.Append("\t").AppendFormat(@"public enum eResourcePath").AppendLine();
			builder.AppendLine("\t{");

			foreach (var path in fileList)
			{
				string newPath = Path.GetFileNameWithoutExtension(path.Replace("/", "_"));
				builder.Append("\t\t").AppendFormat(@"{0},", newPath).AppendLine();
			}

			//builder.AppendLine("\n\t\tNum");
			builder.AppendLine("\t}\n");
		}

		// テーブル定義
		{
			builder.Append("\t").AppendFormat(@"public static string[] ResourcePathTable = new string[]").AppendLine();
			builder.AppendLine("\t{");

			for (int i = 0; i < fileList.Count; ++i)
			{
				// Resourcesのフォルダまで削除する
				int index = fileList[i].LastIndexOf("Resources/");
				string path = fileList[i].Substring(index + "Resources/".Length);

				int lastindex = path.LastIndexOf(".");
				string name = path.Substring(0, lastindex);
				bool last = (i == (fileList.Count - 1));
				builder.Append("\t\t").AppendFormat("\"" + name + "\"" + (last == true ? "" : ",")).AppendLine();

			}

			builder.AppendLine("\t};\n");
		}

		// ゲッター定義
		{
			builder.Append("\t").AppendFormat(@"public static string GetResourcesPath(eResourcePath resource)").AppendLine();
			builder.AppendLine("\t{");
			builder.AppendLine("\t\treturn ResourcePathTable[(int)resource];");
			builder.AppendLine("\t}");
		}

		builder.AppendLine("}");
		builder.AppendLine("}");

		var directoryName = Path.GetDirectoryName(m_PATH);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}

		File.WriteAllText(m_PATH, builder.ToString(), Encoding.UTF8);
		AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

		return true;
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