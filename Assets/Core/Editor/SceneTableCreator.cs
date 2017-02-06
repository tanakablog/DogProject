using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// シーン名を定数で管理するクラスを作成するスクリプト
/// </summary>
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public static class SceneTableCreator
{
    /// <summary>
    /// 無効文字配列
    /// </summary>
    private static readonly string[] INVALID_CHARS =
	{
		" ", "!", "\"", "#", "$",
		"%", "&", "\'", "(", ")",
		"-", "=", "^",  "~", "\\",
		"|", "[", "{",  "@", "`",
		"]", "}", ":",  "*", ";",
		"+", "/", "?",  ".", ">",
		",", "<"
	};

    /// <summary>
    /// コマンド名
    /// </summary>
    private const string ITEM_NAME = "Core/Create/Scene Table";

    /// <summary>
    /// 作成ファイルパス
    /// </summary>
    private const string PATH = "Assets/CreateTable/SceneTable.cs";

    /// <summary>
    /// ファイル名
    /// </summary>
    private static readonly string FILENAME = Path.GetFileName( PATH );

    /// <summary>
    /// ファイル名（拡張子なし）
    /// </summary>
    private static readonly string FILENAME_WITHOUT_EXTENSION = Path.GetFileNameWithoutExtension( PATH );

    /// <summary>
    /// ネームスペース
    /// </summary>
    private const string NAME_SPACE = "Core.Scene";

    /// <summary>
    /// シーン名を定数で管理するクラスを作成します
    /// </summary>
    [MenuItem( ITEM_NAME )]
    public static void Create( )
    {
        if( !CanCreate() )
        {
            return;
        }

        CreateScript();

        EditorUtility.DisplayDialog( FILENAME, "Creation Completion", "OK" );
    }

    /// <summary>
    /// スクリプトを作成します
    /// </summary>
    public static void CreateScript( )
	{
		var builder = new StringBuilder();

		// ネームスペース定義
		{
			builder.AppendFormat("namespace {0}\n", NAME_SPACE);
			builder.AppendLine("{");
		}

		// クラス定義
		{
			builder.AppendFormat("public static class {0}", FILENAME_WITHOUT_EXTENSION).AppendLine();
			builder.AppendLine("{");
		}

		// Assetsに入っているシーンをすべて列挙
		string[] scenes = System.IO.Directory.GetFiles(Application.dataPath, "*.unity", System.IO.SearchOption.AllDirectories);

		string[] ScenePath = new string[scenes.Length];
		for (int i = 0; i < scenes.Length; ++i)
		{
			string nn = scenes[i].Replace(Application.dataPath + "\\", "").Replace("\\", "/");
			ScenePath[i] = nn;
		}

		// ビルドセッティングにシーンを登録
		{
			List<EditorBuildSettingsScene> addScenes = new List<EditorBuildSettingsScene>();

			// まず既に追加されているシーンを追加
			for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
			{
				addScenes.Add(new EditorBuildSettingsScene(EditorBuildSettings.scenes[i].path, EditorBuildSettings.scenes[i].enabled));
			}

			// 追加されていないシーンが有るか確認
			for (int i = 0; i < ScenePath.Length; ++i)
			{
				string scenePath = "Assets/" + ScenePath[i];

				if (addScenes.Exists(
					(a) =>
					{
						return (a.path == scenePath);
					})
					)
				{
					// 存在している

				}
				else
				{
					// 存在していない
					addScenes.Add(new EditorBuildSettingsScene(scenePath, true));
				}
			}

			// すべての結果を元にシーン一覧を再構成
			EditorBuildSettingsScene[] editor_scenes = new EditorBuildSettingsScene[addScenes.Count];

			for (int i = 0; i < addScenes.Count; ++i)
			{
				editor_scenes[i] = addScenes[i];
			}

			EditorBuildSettings.scenes = editor_scenes;
		}


		// enum定義
		{
			builder.Append("\t").AppendFormat(@"public enum eScene").AppendLine();
			builder.AppendLine("\t{");

			// 正規表現による先頭文字削除
			Regex reg = new Regex("^[0-9]{0,}");
			foreach (var n in scenes)
			{
				// ファイル名
				string fileName = Path.GetFileNameWithoutExtension(n);

				// 最初の数字を削除
				fileName = reg.Replace(fileName, "");

				builder.Append("\t\t").AppendFormat(@"{0},", fileName).AppendLine();
			}

			//builder.AppendLine("\n\t\tNum");
			builder.AppendLine("\t}\n");
		}

		// テーブル定義
		{
			builder.Append("\t").AppendFormat(@"public static string[] ScenePathTable = new string[]").AppendLine();
			builder.AppendLine("\t{");

			bool isOverlap = false;
			List<string> addSceneName = new List<string>();

			for (int i = 0; i < ScenePath.Length; ++i)
			{
				string name = Path.GetFileNameWithoutExtension(ScenePath[i]);

				if (isOverlap == false)
				{
					if (addSceneName.Exists((a) => { return (a == name); }))
					{
						isOverlap = true;
					}
					else
					{
						addSceneName.Add(name);
					}
				}
				bool last = (i == (scenes.Length - 1));
				builder.Append("\t\t").AppendFormat("\"" + name + "\"" + (last == true ? "" : ",")).AppendLine();

			}

			builder.AppendLine("\t};\n");

			if (isOverlap)
			{
				// 違うフォルダ階層に同じシーン名が存在します
				Debug.LogWarning("There are the same scene name to a different folder hierarchy !!!");
			}
		}

		// ゲッター定義
		{
			builder.Append("\t").AppendFormat(@"public static string GetSceneName(eScene scene)").AppendLine();
			builder.AppendLine("\t{");
			builder.AppendLine("\t\treturn ScenePathTable[(int)scene];");
			builder.AppendLine("\t}");
		}

		builder.AppendLine("}");
		builder.AppendLine("}");

		var directoryName = Path.GetDirectoryName(PATH);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}

		File.WriteAllText(PATH, builder.ToString(), Encoding.UTF8);
		AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
	}

	/// <summary>
	/// クラスを作成できるかどうかを取得します
	/// </summary>
	[MenuItem(ITEM_NAME, true)]
    public static bool CanCreate( )
    {
        return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
    }

    /// <summary>
    /// 無効な文字を削除します
    /// </summary>
    public static string RemoveInvalidChars( string str )
    {
        Array.ForEach( INVALID_CHARS, c => str = str.Replace( c, string.Empty ) );
        return str;
    }
}