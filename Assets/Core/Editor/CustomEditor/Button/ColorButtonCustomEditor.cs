using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// カラーボタンカスタムエディタ
/// </summary>
[CustomEditor( typeof( Core.UI.Button.ColorButton ) )]
public class ColorButtonCustomEditor : Editor
{
    /// <summary>
    /// 色名
    /// </summary>
    private readonly string[] m_ColorName = 
    {
        "押された時の色",
        "有効時の色",
        "無効時の色"
    };

    /// <summary>
    /// フェードタイムのシリアライズプロパティ
    /// </summary>
    private SerializedProperty m_FadeTime;

    /// <summary>
    /// 色変更のシリアライズプロパティ
    /// </summary>
    private SerializedProperty[] m_ChangeColor = new SerializedProperty[Core.Extension.EnumExtension.GetEnumNum<Core.UI.Button.ColorButton.eColor>()];

    /// <summary>
    /// ターゲット
    /// </summary>
    private Core.UI.Button.ColorButton m_Target;

    private void OnEnable( )
    {
        // ターゲット取得
        m_Target = target as Core.UI.Button.ColorButton;

        // 最初の取得時には何故か null が返って来るので1度適当な値で読む
        serializedObject.FindProperty( "__dummy__" );

        // フェードタイム取得
        m_FadeTime = serializedObject.FindProperty( "m_FadeTime" );

        // 要素ごとに処理
        for( int i = 0; i < m_ChangeColor.Length; ++i )
        {
            // シリアライズプロパティ取得
            m_ChangeColor[i] = serializedObject.FindProperty( string.Format( "{0}.Array.data[{1}]", "m_ChangeColor", i ) );
        }
    }
    
    public override void OnInspectorGUI( )
    {
        // 変更チェック開始
        EditorGUI.BeginChangeCheck();

        // フェード時間設定
        EditorGUILayout.PropertyField( m_FadeTime, new GUIContent( "色のフェード時間" ) );

        // 変更色設定
        for( int i = 0; i < m_ColorName.Length; ++i )
        {
           EditorGUILayout.PropertyField( m_ChangeColor[i], new GUIContent( m_ColorName[i] ) );
        }
        
        // 変更チェック終了
        if( EditorGUI.EndChangeCheck() )
        {
            // プレイ中チェック
            if( EditorApplication.isPlaying || Application.isPlaying )
            {
                // リフレッシュカラー
                m_Target.RefreshColor();
            }
        }

        // シリアライズオブジェクトの更新反映
        serializedObject.ApplyModifiedProperties();

        // インスペクター更新
        EditorUtility.SetDirty( m_Target );
    }
}