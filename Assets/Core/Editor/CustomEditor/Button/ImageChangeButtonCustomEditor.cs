using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

/// <summary>
/// イメージ変更ボタンカスタムエディタ
/// </summary>
[CustomEditor( typeof( Core.UI.Button.ImageChangeButton ) )]
public class ImageChangeButtonCustomEditor : Editor
{
    /// <summary>
    /// 表示テキスト
    /// </summary>
    readonly string[] text = 
    {    
        "有効時",
        "押された時",
        "無効時"
    };

    /// <summary>
    /// 検索文字
    /// </summary>
    string[] search = { "", "", "" };

    /// <summary>
    /// 以前の検索文字
    /// </summary>
    string[] before = { "", "", "" };

    /// <summary>
    /// 検索後の指定スプライトインデックス
    /// </summary>
    int[] select = { 0, 0, 0 };

    /// <summary>
    /// 検索結果保存リスト
    /// </summary>
    List<string>[] list = new List<string>[Core.Extension.EnumExtension.GetEnumNum<Core.UI.Button.ImageChangeButton.eSprite>()];

    /// <summary>
    /// ターゲット
    /// </summary>
    Core.UI.Button.ImageChangeButton m_Target;          

    /// <summary>
    /// アトラスを表示するか
    /// </summary>                                                                                                                                 
    bool m_IsAtlasCheck;

    /// <summary>
    /// Enum使用フラグのシリアライズプロパティ
    /// </summary>
    private SerializedProperty useEnum;

    void OnEnable()
    {
        m_Target = target as Core.UI.Button.ImageChangeButton;

        for( int i = 0; i < list.Length; ++i )
        {
            if( list[i] == null )
            {
                list[i] = new List<string>();
            }
        }

        foreach( var sprite_type in m_Target.spriteType )
        {
            if( sprite_type != Core.SpriteCore.SpriteTable.eSprite.NoSprite )
            {
                return;
            }
        }               

        Image image = m_Target.GetComponent<Image>();

        if( image )
        {
            var type = Core.SpriteCore.SpriteManager.instance.SpriteToSpriteEnum( image.sprite );

            for( int i = 0; i < m_Target.spriteType.Length; ++i )
            {
                m_Target.spriteType[i] = type;
            }
        }
    }

    public override void OnInspectorGUI( )
    {
        m_Target.useEnum = EditorGUILayout.Toggle( "列挙型で指定する", m_Target.useEnum );

        // 列挙体使用チェック
        if( m_Target.useEnum )
        {
            // スプライト列挙型指定処理
            SetSpriteEnum();
        }
        else
        {
            // インスペクターでスプライトを設定
            for( int i = 0; i < Core.Extension.EnumExtension.GetEnumNum<Core.UI.Button.ImageChangeButton.eSprite>(); ++i )
            {
                m_Target.changeSprite[i] = (Sprite)EditorGUILayout.ObjectField( text[i] + "のスプライト", m_Target.changeSprite[i], typeof( Sprite ), true );
            }
        }

        // 使用アトラス表示
        UseAtlasDraw();

        EditorUtility.SetDirty( target );
    }

    /// <summary>
    /// スプライト列挙型設定
    /// </summary>
    void SetSpriteEnum()
    {

        // 列挙体でスプライト指定
        for( int i = 0; i < Core.Extension.EnumExtension.GetEnumNum<Core.UI.Button.ImageChangeButton.eSprite>(); ++i )
        {
            // 検索値設定
            search[i] = EditorGUILayout.TextField( "検索用", search[i] );

            if( before[i] != search[i] )
            // 検索値変更
            {
                list[i].Clear();

                select[i] = 0;

                before[i] = search[i];

                foreach( var name in Core.Extension.EnumExtension.GetEnumNames<Core.SpriteCore.SpriteTable.eSprite>() )
                {
                    if( 0 <= name.IndexOf( search[i], StringComparison.OrdinalIgnoreCase ) )
                    {
                        list[i].Add( name );
                    }
                }
            }
            // 検索値あり
            else if( "" != search[i] )
            {
                // 検索結果チェック
                if( list[i].Count != 0 )
                // 検索結果あり
                {
                    // 結果保存配列
                    string[] result = new string[list[i].Count];

                    // 結果取得
                    list[i].CopyTo( result );

                    // スプライト指定
                    select[i] = EditorGUILayout.Popup( text[i] + "のスプライト", select[i], result );

                    // スプライト設定
                    m_Target.spriteType[i] = Core.Extension.EnumExtension.StringToEnum<Core.SpriteCore.SpriteTable.eSprite>( result[select[i]] );
                }
                // 検索結果なし
                else
                {
                    string[] dummy = { "検索結果無し", "" };

                    select[i] = EditorGUILayout.Popup( text[i] + "のスプライト", 0, dummy );
                }
            }
            // 検索値なし
            else
            {
                // 列挙体でスプライト指定
                m_Target.spriteType[i] = (Core.SpriteCore.SpriteTable.eSprite)EditorGUILayout.EnumPopup( text[i] + "のスプライト", m_Target.spriteType[i] );
            }
        }
    }

    /// <summary>
    /// 使用アトラス表示
    /// </summary>
    void UseAtlasDraw()
    {
        // 文言表示
        m_IsAtlasCheck = EditorGUILayout.Foldout( m_IsAtlasCheck, "使用アトラス確認" );

        // アトラス確認フラグチェック
        if( m_IsAtlasCheck )
        {
            // 範囲生成
            Rect rect = new Rect();

            // スプライト毎にアトラスを表示
            for( int i = 0; i < Core.Extension.EnumExtension.GetEnumNum<Core.UI.Button.ImageChangeButton.eSprite>(); ++i )
            {
                // 範囲取得
                rect = EditorGUILayout.GetControlRect();

                // アトラスを使用しているスプライト表示
                EditorGUI.LabelField( rect, text[i] + "のアトラス" );

                // ずらす
                rect.x += 130;
                rect.width += 130;

                // 列挙体を使用するかチェック
                if( m_Target.useEnum )
                {
                    // 使用アトラス表示
                    EditorGUI.SelectableLabel( rect, Core.SpriteCore.SpriteManager.instance.SpriteEnumToAtlasEnum( m_Target.spriteType[i] ).ToString() );
                }
                else
                {
                    if( m_Target.changeSprite[i] != null )
                    {
                        // 使用アトラス表示
                        EditorGUI.SelectableLabel( rect, Core.SpriteCore.SpriteManager.instance.SpriteToAtlasName( m_Target.changeSprite[i] ) );
                    }
                }
            }
        }
    }
}
