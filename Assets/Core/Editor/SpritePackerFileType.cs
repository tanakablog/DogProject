using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

public class SpritePackerFileType
{

	// 保存先
	private const string m_PATH = "Assets/Resource/Atlas";


	[MenuItem("Tools/CreateAllAtlas")]
	static void Create()
	{

		// Assetsに入っているすべて列挙
		string[] files = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);

		List<string> fileList = new List<string>();

		// ファイルのパスからResourcesを含まれているものだけを探す
		foreach (var f_path in files)
		{
			// Assetのパスまで削除
			string path = f_path.Replace(Application.dataPath + "\\", "").Replace("\\", "/");

			if (path.IndexOf("Editor") != -1)
				continue;

			if (path.IndexOf("Texture") == -1)
				continue;

			if (path.IndexOf(".meta") != -1)
				continue;

			if (path.IndexOf(".cs") != -1)
				continue;

			fileList.Add(path);
		}

		Dictionary<string, AtlasClass> l_atlas = new Dictionary<string, AtlasClass>();

		foreach (var st in fileList)
		{
			// Resourcesのフォルダまで削除する
			int index = st.LastIndexOf("Texture/");
			string path = st.Substring(index + ("Texture/".Length));
			string FoldierName = "NotFolder";

			int folderIndex = path.IndexOf("/");
			if (folderIndex != -1)
			{
				FoldierName = path.Substring(0, folderIndex);
			}

			AtlasClass atlassClass = null;

			if (l_atlas.ContainsKey(FoldierName) == true)
			{
				// 既に追加されているフォルダ
				atlassClass = l_atlas[FoldierName];
			}
			else
			{
				atlassClass = new AtlasClass(FoldierName);
				// 追加されていないので追加
				l_atlas.Add(FoldierName, atlassClass);
			}
			
			SpriteElement element = new SpriteElement();
			element.tex = AssetDatabase.LoadAssetAtPath("Assets/" + st, typeof(Texture2D)) as Texture2D;
			element.meta = new SpriteMetaData();
			element.meta.name = Path.GetFileNameWithoutExtension(path);

			atlassClass.spriteElement.Add(element);

		}

		// アトラスごとのファイルを生成
		foreach (var st in l_atlas)
		{
			string atlasPath = m_PATH + "/" + st.Key + ".png";
			if (System.IO.File.Exists(atlasPath) == false)
				continue;

			TextureImporter textureImporter = AssetImporter.GetAtPath(atlasPath) as TextureImporter;

			foreach (SpriteMetaData sprite in textureImporter.spritesheet)
			{
				string name = sprite.name;

				SpriteElement spriteElement = st.Value.spriteElement.Find((a) => a.tex.name == name);


				// 同じ奴が居た！！
				if (spriteElement != null)
				{
					// metaのコピ～
					spriteElement.meta = sprite;
				}
				else
				{
					// 居なかったの
				}

			}
		}


		// 保存先にあるファイルをすべて削除
		{
			// 存在するなら削除
			if (Directory.Exists(m_PATH))
			{
				// 作る（変更）予定となっていないファイルを削除
				foreach (var filepath in Directory.GetFiles(m_PATH))
				{
					string name = Path.GetFileNameWithoutExtension(filepath);
					if (l_atlas.ContainsKey(name) == false)
					{
						File.Delete(filepath);
					}
				}
			}
			else
			{
				Directory.CreateDirectory(m_PATH);
			}
		}

		// アトラス毎にファイルを生成
		foreach (var st in l_atlas)
		{
			Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);

			List<Texture2D> spriteTexs = new List<Texture2D>();
			List<SpriteMetaData> spritesheet = new List<SpriteMetaData>();

			foreach (var sprite in st.Value.spriteElement)
			{
				spritesheet.Add(sprite.meta);

				string path = AssetDatabase.GetAssetPath(sprite.tex.GetInstanceID());

				SetTextureSetting(path, true);

				sprite.tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;

				spriteTexs.Add(sprite.tex);

			}

			int maxSize = Mathf.Min(SystemInfo.maxTextureSize, 2048);

			Rect[] rects = tex.PackTextures(spriteTexs.ToArray(), 2, maxSize);

			string newPath = m_PATH + "/" + st.Key + ".png";

			byte[] bytes = tex.EncodeToPNG();
			System.IO.File.WriteAllBytes(newPath, bytes);
			bytes = null;

			TextureImporter textureImporter = AssetImporter.GetAtPath(newPath) as TextureImporter;

			textureImporter.textureType = TextureImporterType.Sprite;
			textureImporter.spriteImportMode = SpriteImportMode.Multiple;

			for (int i = 0; i < spriteTexs.Count; ++i)
			{
				SpriteMetaData metaData = spritesheet[i];

				Rect rect = rects[i];
				rect.x *= tex.width;
				rect.y *= tex.height;
				rect.width *= tex.width;
				rect.height *= tex.height;

				metaData.rect = rect;
				spritesheet[i] = metaData;
			}

			textureImporter.spritesheet = spritesheet.ToArray();

			SetTextureSetting(textureImporter, false);

			AssetDatabase.ImportAsset(newPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
			
			UnityEditor.EditorUtility.UnloadUnusedAssets();

		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();



	}


	static void SetTextureSetting(TextureImporter textureImporter, bool readable)
	{
		TextureImporterSettings settings = new TextureImporterSettings();
		textureImporter.ReadTextureSettings(settings);

		settings.readable = readable;
		settings.textureFormat = TextureImporterFormat.ARGB32;
		settings.npotScale = TextureImporterNPOTScale.None;

		settings.mipmapEnabled = false;

		textureImporter.SetTextureSettings(settings);
	}

	static void SetTextureSetting(string path, bool readable)
	{
		TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

		SetTextureSetting(textureImporter, readable);

		AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
	}

	public class SpriteElement
	{
		public Texture2D tex;
		public SpriteMetaData meta;
	};

	class AtlasClass
	{
		public string name;
		public List<SpriteElement> spriteElement;

		public AtlasClass(string name)
		{
			this.name = name;
			spriteElement = new List<SpriteElement>();
		}
	}



}