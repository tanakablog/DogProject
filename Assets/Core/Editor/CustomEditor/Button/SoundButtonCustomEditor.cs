using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// サウンドボタンカスタムエディタ
/// </summary>
[CustomEditor( typeof( Core.UI.Button.SoundButton ) )]
public class SoundButtonCustomEditor : Editor
{
    /// <summary>
    /// ボタン状態
    /// </summary>
    public enum eState
    {
        Enable,
        Disable
    }
                
    /// <summary>
    /// 状態名
    /// </summary>
    readonly string[] m_StateName = 
    {
        "有効時",
        "無効時"
    };

    /// <summary>
    /// 項目名
    /// </summary>
    readonly string[] m_ItemName =  
    {
        "押された時",
        "離された時",
        "長押し時"
    };

    /// <summary>
    /// 検索文字
    /// </summary>
    string[,] m_Search = { { "", "", "" }, { "", "", "" } };

    /// <summary>
    /// 以前の検索文字
    /// </summary>
    string[,] m_BeforeSearch = { { "", "", "" }, { "", "", "" } };

    /// <summary>
    /// 検索後の指定SEインデックス
    /// </summary>
    int[,] m_Select = { { 0, 0, 0 }, { 0, 0, 0 } };

    /// <summary>
    /// 検索結果保存リスト
    /// </summary>
    List<string>[,] m_List = new List<string>[Core.Extension.EnumExtension.GetEnumNum<eState>(), Core.Extension.EnumExtension.GetEnumNum<Core.UI.Button.SoundButton.eAction>()];

    /// <summary>
    /// ターゲット
    /// </summary>
    Core.UI.Button.SoundButton m_Target;       

    void OnEnable( )
    {
        if( m_Target == null )
        {
            m_Target = target as Core.UI.Button.SoundButton;
        }

        for( int i = 0; i < Core.Extension.EnumExtension.GetEnumNum<eState>(); ++i )
        {
            for( int j = 0; j < Core.Extension.EnumExtension.GetEnumNum<Core.UI.Button.SoundButton.eAction>(); ++j )
            {
                if( m_List[i,j] == null )
                {
                    m_List[i,j] = new List<string>();
                }
            }
        }
    }

    public override void OnInspectorGUI( )
    {                                                  
        // 状態ごとに表示
        for( int i = 0; i < Core.Extension.EnumExtension.GetEnumNum<eState>(); ++i )
        {
            // 状態名
            EditorGUILayout.LabelField( m_StateName[i] );

            // 項目ごとに表示
            for( int j = 0; j < Core.Extension.EnumExtension.GetEnumNum<Core.UI.Button.SoundButton.eAction>(); ++j )
            {
                // 検索値設定
                m_Search[i,j] = EditorGUILayout.TextField( "検索用", m_Search[i,j] );

                // 前回の検索値と比較
                if( m_BeforeSearch[i,j] != m_Search[i,j] )
                // 検索値変更
                {
                    // 検索結果クリア
                    m_List[i,j].Clear();

                    // SEインデックスクリア
                    m_Select[i,j] = 0;

                    // 前回検索値更新
                    m_BeforeSearch[i,j] = m_Search[i,j];

                    // 検索
                    foreach( var name in Core.Extension.EnumExtension.GetEnumNames<Core.Sound.SoundTable.eSE>() )
                    {
                        if( 0 <= name.IndexOf( m_Search[i,j], StringComparison.OrdinalIgnoreCase ) )
                        {
                            m_List[i,j].Add( name );
                        }
                    }
                }
                // 検索値あり
                else if( "" != m_Search[i,j] )
                {
                    // 検索結果チェック
                    if( m_List[i,j].Count != 0 )
                    // 検索結果あり
                    {
                        // 結果保存配列
                        string[] result = new string[m_List[i,j].Count];

                        // 結果取得
                        m_List[i,j].CopyTo( result );

                        // SE指定
                        m_Select[i,j] = EditorGUILayout.Popup( m_ItemName[j], m_Select[i,j], result );

                        // SE設定
                        if( i == (int)eState.Enable )
                        {
                            m_Target.enableSe[j] = Core.Extension.EnumExtension.StringToEnum<Core.Sound.SoundTable.eSE>( result[m_Select[i, j]] );
                        }
                        else
                        {
                            m_Target.disableSe[j] = Core.Extension.EnumExtension.StringToEnum<Core.Sound.SoundTable.eSE>( result[m_Select[i, j]] );
                        }
                    }
                    // 検索結果なし
                    else
                    {
                        string[] dummy = { "検索結果無し", "" };

                        m_Select[i,j] = EditorGUILayout.Popup( m_ItemName[j], 0, dummy );
                    }
                }
                // 検索値なし
                else
                {
                    // 列挙体でのSE指定  
                    if( i == (int)eState.Enable )
                    {
                        m_Target.enableSe[j] = (Core.Sound.SoundTable.eSE)EditorGUILayout.EnumPopup( m_ItemName[j], m_Target.enableSe[j] );
                    }
                    else
                    {
                        m_Target.disableSe[j] = (Core.Sound.SoundTable.eSE)EditorGUILayout.EnumPopup( m_ItemName[j], m_Target.disableSe[j] );                    
                    }
                }
            }
        }
        EditorUtility.SetDirty( m_Target );
    }
}