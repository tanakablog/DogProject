using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(AssetBundleManager))]
public class AssetBundleManagerEditor : Editor 
{
    private AssetBundleManager m_Target;

    private SerializedProperty m_IsDebugLog;

    private SerializedProperty m_SeverUrl;

    private void OnEnable()
    {
        // ターゲット取得
        m_Target = target as AssetBundleManager;

        // シリアライズプロパティ
        {
            // 最初の取得時には何故か null が返って来るので1度適当な値で読む
            serializedObject.FindProperty( "__dummy__" );

			m_IsDebugLog = serializedObject.FindProperty ("m_IsDebugLog");
            m_SeverUrl = serializedObject.FindProperty ("m_SeverUrl");
        }
    }

    public override void OnInspectorGUI ()
    {
        if (!IsPlay ()) {
            EditorGUILayout.PropertyField (m_SeverUrl, new GUIContent ("サーバー接続先"));
			EditorGUILayout.PropertyField (m_IsDebugLog, new GUIContent ("デバッグログ表示"));
		} else {
			EditorGUILayout.PropertyField (m_IsDebugLog, new GUIContent ("デバッグログ表示"));
            foreach (var loaded in m_Target.GetLoadedAssetPath()) {
                EditorGUILayout.LabelField (loaded);
            }
        }

        // シリアライズオブジェクトの更新反映
        serializedObject.ApplyModifiedProperties();

        // インスペクター更新
        EditorUtility.SetDirty( m_Target );
    }

    private bool IsPlay ()
    {
        return EditorApplication.isPaused || EditorApplication.isPlaying;
    }
}