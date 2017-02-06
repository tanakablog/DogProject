using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Reflection;
using System.Linq;

/// <summary>
/// イベントボタンカスタムエディタ
/// </summary>
[CustomEditor( typeof( Core.UI.Button.EventButton ) )]
public class EventButtonCustomEditor : Editor
{
    /// <summary>
    /// ターゲット
    /// </summary>
    Core.UI.Button.EventButton m_Target;

    /// <summary>
    /// イベントシリアライズプロパティ
    /// </summary>
    private SerializedProperty[] m_Events = new SerializedProperty[Core.Extension.EnumExtension.GetEnumNum<Core.UI.Button.ButtonInfo.eAction>()];
            
    void OnEnable()
    {
        // ターゲット取得
        m_Target = target as Core.UI.Button.EventButton;

        // 最初の取得時には何故か null が返って来るので1度適当な値で読む
        serializedObject.FindProperty( "__dummy__" );

        // シリアライズプロパティ取得
        for( int i = 0; i < m_Events.Length; ++i )
        {
            m_Events[i] = serializedObject.FindProperty( string.Format( "{0}.Array.data[{1}]", "m_Events", i ) ); 
        }

    }

    public override void OnInspectorGUI( )
    {
        // イベントタイプリスト生成
        List<Core.UI.Button.ButtonInfo.eAction> type_list = new List<Core.UI.Button.ButtonInfo.eAction>();

        // イベントタイプをリストに追加
        for( int i = 0; i < m_Target.events.Count(); ++i )
        {
            // 描画されているかチェック
            if( !m_Target.isDraw[i] )
            {
                type_list.Add( (Core.UI.Button.ButtonInfo.eAction)i );
            }
        }

        // イベント追加設定
        int select = -1;
        select = EditorGUILayout.Popup( "追加イベントタイプ", select, type_list.Select( x => x.ToString() ).ToArray() );

        // イベントタイプ追加チェック
        if( select >= 0 )
        {
            // イベント追加 
            m_Target.isDraw[(int)type_list[select]] = true;
        }

        for( int i = 0; i < m_Events.Length; ++i )
        {
            // 描画するかチェック
            if( m_Target.isDraw[i] )
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( "イベントタイプ：" + (Core.UI.Button.ButtonInfo.eAction)i );    
                
                // イベント削除ボタン
                if( GUILayout.Button( "×", GUILayout.Width( 50f ) ) )
                {
                    m_Target.isDraw[i] = false;
                }

                EditorGUILayout.EndHorizontal();

                serializedObject.Update();
                EditorGUILayout.PropertyField( m_Events[i] );
                serializedObject.ApplyModifiedProperties();
            }
        }

        // Gui更新？
        EditorUtility.SetDirty( m_Target );
    }
}