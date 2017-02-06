using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// スケールボタンカスタムエディタ
/// </summary>
[CustomEditor( typeof( Core.UI.Button.ScaleButton ) )]
public class ScaleButtonCustomEditor : Editor
{
    /// <summary>
    /// 色名
    /// </summary>
    private readonly string[] m_ScaleName = 
    {
        "押された時の拡大率",
        "有効時の拡大率",
        "無効時の拡大率"
    };

    /// <summary>
    /// 変更時間のシリアライズプロパティ
    /// </summary>
    private SerializedProperty m_ChangeTime;

    /// <summary>
    /// 拡大率変更のシリアライズプロパティ
    /// </summary>
    private SerializedProperty[] m_ChangeScale = new SerializedProperty[Core.Extension.EnumExtension.GetEnumNum<Core.UI.Button.ScaleButton.eScale>()];

    /// <summary>
    /// ターゲット
    /// </summary>
    Core.UI.Button.ScaleButton m_Target;

    void OnEnable( )
    {
        // ターゲット取得
        m_Target = target as Core.UI.Button.ScaleButton;

        // シリアライズプロパティ
        {
            // 最初の取得時には何故か null が返って来るので1度適当な値で読む
            serializedObject.FindProperty( "__dummy__" );

            m_ChangeTime = serializedObject.FindProperty( "changeTime" );

            // 要素ごとに処理
            for( int i = 0; i < m_ChangeScale.Length; ++i )
            {
                // シリアライズプロパティ取得
                m_ChangeScale[i] = serializedObject.FindProperty( string.Format( "{0}.Array.data[{1}]", "changeScale", i ) );
            }
        }
    }

    public override void OnInspectorGUI( )
    {
        // 変更チェック開始
        EditorGUI.BeginChangeCheck();

        // フェード時間設定
        EditorGUILayout.PropertyField( m_ChangeTime, new GUIContent( "変更時間" ) );

        // 変更色設定
        for( int i = 0; i < m_ScaleName.Length; ++i )
        {
            EditorGUILayout.PropertyField( m_ChangeScale[i], new GUIContent( m_ScaleName[i] ) );
        }
         // 変更チェック終了
        if( EditorGUI.EndChangeCheck() )
        {
            // プレイ中チェック
            if( EditorApplication.isPlaying || Application.isPlaying )
            {
                // 拡大率リフレッシュ
                m_Target.RefreshScale();
            }
            else
            {
                // 有効時の拡大率に変更
                m_Target.transform.localScale = m_Target.changeScale[(int)Core.UI.Button.ScaleButton.eScale.Up];
            }
        }

        // シリアライズオブジェクトの更新反映
        serializedObject.ApplyModifiedProperties();


        EditorUtility.SetDirty( target );
    }
}