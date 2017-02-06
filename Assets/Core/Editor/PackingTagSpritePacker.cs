using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
   
public class PackingTagSpritePacker : EditorWindow
{
    private const string k_ITEM_NAME = "Core/PackingTagSpritePacker";

    [MenuItem( k_ITEM_NAME )]
    public static void CreateWindow()
    {
        PackingTagSpritePacker window = EditorWindow.GetWindow<PackingTagSpritePacker>();
        window.title = "PackingTagSpritePacker";
         
    }


    private const string k_TEXTURE_FOLDER_PATH_FIXED = "Assets/";
    private const string k_ATLAS_FOLDER_PATH_FIXED = "Assets/Resources/Atlas/"; 
    private string m_TextureFolderPath = "Textures";
    private string m_AtlasFolderPath = "";
    private List<TextureImporter> m_SpriteList;
    private Dictionary<string, List<TextureImporter>> m_SpriteEachPackingTag;
    private TextureFormat m_AtlasFormat = TextureFormat.ARGB32;

    void OnGUI()
    {
        GUILayout.BeginHorizontal( GUI.skin.box );
        GUILayout.Label( "テクスチャフォルダ", GUILayout.Width( 105.0f ) );
        GUILayout.Label( k_TEXTURE_FOLDER_PATH_FIXED, GUILayout.Width( 48.0f ) );
        m_TextureFolderPath = GUILayout.TextField( m_TextureFolderPath );
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal( GUI.skin.box );
        GUILayout.Label( "アトラスフォルダ", GUILayout.Width( 105.0f ) );
        GUILayout.Label( k_ATLAS_FOLDER_PATH_FIXED, GUILayout.Width( 145.0f ) );
        m_AtlasFolderPath = GUILayout.TextField( m_AtlasFolderPath );
        GUILayout.EndHorizontal();

        if( GUILayout.Button( "アトラス生成" ) )
        {
            GetPackingTagAndSprites();

            m_SpriteEachPackingTag.Keys.ToList().ForEach( x => CreateAtlas( x ) );
        }
    }


    private void GetPackingTagAndSprites()
    {
        if( m_TextureFolderPath.Last() == '/' )
        {
            m_TextureFolderPath = m_TextureFolderPath.Remove( m_AtlasFolderPath.Length - 1, 1 );
        }

        // 指定フォルダ内のスプライトパス取得
        string[] sprite_path = System.IO.Directory.GetFiles( k_TEXTURE_FOLDER_PATH_FIXED + m_TextureFolderPath, "*.png", System.IO.SearchOption.AllDirectories );

        m_SpriteList = sprite_path.Select( x =>
        {
            // テクスチャ情報取得
            TextureImporter sprite = AssetImporter.GetAtPath( x ) as TextureImporter;

            // スプライト名取得
            sprite.name = Path.GetFileNameWithoutExtension( x ); 

            return sprite;

        } ).ToList();

        m_SpriteEachPackingTag = new Dictionary<string, List<TextureImporter>>();

        m_SpriteList.ForEach( sprite =>
        {
            var list = m_SpriteEachPackingTag.FirstOrDefault( tag => { return tag.Key == sprite.spritePackingTag ? true : false; } );

            if( string.IsNullOrEmpty( list.Key ) )
            {
                list = new KeyValuePair<string, List<TextureImporter>>( sprite.spritePackingTag, new List<TextureImporter>() );

                m_SpriteEachPackingTag.Add( list.Key, list.Value );
            }

            list.Value.Add( sprite );
        } );

    }

    private void CreateAtlas( string tag )
    {
        if( string.IsNullOrEmpty( tag ) )
        {
            return;
        }

        // アトラステクスチャ生成
        Texture2D atlas = new Texture2D( 1, 1, m_AtlasFormat, false );

        // テクスチャメタ情報取得
        var textures_meta = m_SpriteEachPackingTag.FirstOrDefault( x =>
        {
            return tag == x.Key ? true : false;
        } ).Value;
        
        // テクスチャ取得
        var textures = textures_meta.Select( x => 
        {
            Texture2D tex = new Texture2D( 1, 1 );
            tex.LoadImage( File.ReadAllBytes( x.assetPath ) );
            return tex;
        }).ToArray();

        // アトラスにテクスチャをパック
        Rect[] rects = atlas.PackTextures( textures, 2, 2048 );

        if( m_AtlasFolderPath.Last() != '/' )
        {
            m_AtlasFolderPath += '/';  
        }

        // アトラスパス取得
        string atlas_path = k_ATLAS_FOLDER_PATH_FIXED + m_AtlasFolderPath + tag + ".png";

        // アトラスを書き出し
        System.IO.File.WriteAllBytes( atlas_path, atlas.EncodeToPNG() );

        // 変更を保存
        AssetDatabase.SaveAssets();

        // 更新
        AssetDatabase.Refresh();

        // アトラスメタ情報
        TextureImporter atlas_meta = AssetImporter.GetAtPath( atlas_path ) as TextureImporter;

        atlas_meta.textureType = TextureImporterType.Advanced;
        atlas_meta.spriteImportMode = SpriteImportMode.Multiple;
        atlas_meta.alphaIsTransparency = true;
        atlas_meta.mipmapEnabled = false;

        atlas_meta.spritesheet = textures_meta.Select( (x, index) =>
        {
            SpriteMetaData meta = new SpriteMetaData();
            meta.border = x.spriteBorder;
            meta.name = x.name;

            Rect rect = rects[index];
            rect.x *= atlas.width;
            rect.width *= atlas.width;
            rect.y *= atlas.height;
            rect.height *= atlas.height;
            meta.rect = rect;

            return meta;
        } ).ToArray();

        TextureImporterSettings settings = new TextureImporterSettings();
        atlas_meta.ReadTextureSettings( settings );

        settings.readable = false;
        settings.textureFormat = TextureImporterFormat.ARGB32;
        settings.npotScale = TextureImporterNPOTScale.None;

        atlas_meta.SetTextureSettings( settings );
        AssetDatabase.ImportAsset( atlas_path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }                
}