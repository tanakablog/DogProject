using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// ボタン情報カスタムエディタ
/// </summary>
[CustomEditor( typeof( Core.UI.Button.ButtonInfo ) )]
public class ButtonInfoCustomEditor : Editor
{                                    
    /// <summary>
    /// ボタンタイプ
    /// </summary>
    enum eButtonType
    {
        Color,
        Scale,
        Event,
        Image,
        Sound,
    }

    /// <summary>
    /// ボタンタイプ名
    /// </summary>
    readonly string[] buttonTypeName = 
    {
        "色変更",
        "拡大率変更",
        "イベント送信",
        "画像変更",
        "サウンド"
    };

    /// <summary>
    /// ターゲット
    /// </summary>
    private Core.UI.Button.ButtonInfo m_Target;

    /// <summary>
    /// スプライト保存
    /// </summary>
    private Core.SpriteCore.SpriteTable.eSprite m_SpriteType;

    /// <summary>
    /// ボタンタイプシリアライズプロパティ
    /// </summary>
    private SerializedProperty m_Type;

    /// <summary>
    /// 長押し移行時間シリアライズプロパティ
    /// </summary>
    private SerializedProperty m_PressTime;

    void OnEnable( )
    {
        // ターゲット取得
        m_Target = target as Core.UI.Button.ButtonInfo;

        // シリアライズプロパティ
        {
            // 最初の取得時には何故か null が返って来るので1度適当な値で読む
            serializedObject.FindProperty( "__dummy__" );

            m_Type = serializedObject.FindProperty( "m_Type" );
            m_PressTime = serializedObject.FindProperty( "m_PressTime" );
        }

        Image image = m_Target.GetComponent<Image>();

        if( image )
        {
            m_SpriteType = Core.SpriteCore.SpriteManager.instance.SpriteToSpriteEnum( image.sprite );
        }
    }

    public override void OnInspectorGUI( )
    {
        // ボタンタイプ設定            
        EditorGUILayout.PropertyField( m_Type, new GUIContent( "ボタンタイプ" ) );

        // 長押し移行時間設定            
        EditorGUILayout.PropertyField( m_PressTime, new GUIContent( "長押し移行時間" ) );

        // コライダー設定
        ColliderSetting();

        // イメージ取得
        Image image = m_Target.GetComponent<Image>();

        if( image )
        {
            Core.UI.Button.ImageChangeButton change = m_Target.GetComponent<Core.UI.Button.ImageChangeButton>();

            if( change )
            {
                m_SpriteType = change.spriteType[(int)change.useSprite];
            }
            else
            {
                m_SpriteType = (Core.SpriteCore.SpriteTable.eSprite)EditorGUILayout.EnumPopup( "Image画像設定", m_SpriteType );
            }

            image.sprite = Core.SpriteCore.SpriteManager.instance.GetSprite( m_SpriteType );
        }

        // 使用ボタン設定
        UseButtonSetting();

        // シリアライズオブジェクトの更新反映
        serializedObject.ApplyModifiedProperties();

        // インスペクター更新
        EditorUtility.SetDirty( m_Target );
    }

    /// <summary>
    /// コライダー設定
    /// </summary>
    void ColliderSetting()
    {
		/*
        // コライダーがあるかチェック
        if( m_Target.cachedBoxCollider || m_Target.cachedBoxCollider2D )
        {                                  
            // コライダー2Dチェック
            if( m_Target.cachedBoxCollider2D && m_Target.cachedRectTransform )
            {
                if( GUILayout.Button( "コライダーリサイズ" ) )
                {
                    m_Target.ResizeBoxCollider2DBasisImage();
                }
            }
        }
        else
        {                                       
            // ラベル
            EditorGUILayout.LabelField( "※コライダーを設定してください" );

            EditorGUILayout.BeginHorizontal();

            // コライダー設定ボタン
            if( GUILayout.Button( "3D", GUILayout.Width( 50f ) ) )
            {
                Undo.AddComponent<BoxCollider>( m_Target.gameObject );
            }

            // コライダー2D設定ボタン
            if( GUILayout.Button( "2D", GUILayout.Width( 50f ) ) )
            {
                Undo.AddComponent<BoxCollider2D>( m_Target.gameObject );
                m_Target.ResizeBoxCollider2DBasisImage();
            }

            EditorGUILayout.EndHorizontal();
        }
		*/
    }

    /// <summary>
    /// 使用ボタン設定
    /// </summary>
    void UseButtonSetting()
    {
        // ラベル
        EditorGUILayout.LabelField( "使用するボタン設定" );

        // 存在フラグ
        bool[] use_type = new bool[buttonTypeName.Length];

        // コンポーネントチェック
        CheckComponent( ref use_type );

        // 変更チェック開始
        EditorGUI.BeginChangeCheck();

        // 追加・削除判定
        for( int i = 0; i < buttonTypeName.Length; ++i )
        {
            use_type[i] = EditorGUILayout.Toggle( buttonTypeName[i], use_type[i] );
        }

        // コンポーネント追加・削除  
        AddOrDeleteComponent( use_type );

        // 変更チェック終了
        if( EditorGUI.EndChangeCheck() )
        {
            // 無理やり更新
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }    
    }

    /// <summary>
    /// コンポーネントチェック
    /// </summary>
    /// <param name="use_type">存在フラグ</param>
    void CheckComponent( ref bool[] use_type )
    {
        // 色変更
        if( m_Target.GetComponent<Image>() )
        {
            if( m_Target.GetComponent<Core.UI.Button.ImageColorButton>() )
            {
                use_type[(int)eButtonType.Color] = true;
            }
            else
            {
                use_type[(int)eButtonType.Color] = false;
            }
        }
        else
        {
            if( m_Target.GetComponent<Core.UI.Button.ColorButton>() )
            {
                use_type[(int)eButtonType.Color] = true;
            }
            else
            {
                use_type[(int)eButtonType.Color] = false;
            }
        }

        // 拡大率変更
        if( m_Target.GetComponent<Core.UI.Button.ScaleButton>() )
        {
            use_type[(int)eButtonType.Scale] = true;

        }
        else
        {
            use_type[(int)eButtonType.Scale] = false;
        }

        // イベント送信
        if( m_Target.GetComponent<Core.UI.Button.EventButton>() )
        {
            use_type[(int)eButtonType.Event] = true;

        }
        else
        {
            use_type[(int)eButtonType.Event] = false;
        }

        // 画像変更
        if( m_Target.GetComponent<Core.UI.Button.ImageChangeButton>() )
        {
            use_type[(int)eButtonType.Image] = true;
        }
        else
        {
            use_type[(int)eButtonType.Image] = false;
        }

        // サウンド
        if( m_Target.GetComponent<Core.UI.Button.SoundButton>() )
        {
            use_type[(int)eButtonType.Sound] = true;
        }
        else
        {
            use_type[(int)eButtonType.Sound] = false;
        }
    }

    /// <summary>
    /// コンポーネント追加・削除
    /// </summary>
    /// <param name="use_type">ｓ</param>
    void AddOrDeleteComponent( bool[] use_type )
    {
        // 色変更
        if( m_Target.GetComponent<Image>() )
        {
            // imageの色変更
            if( m_Target.GetComponent<Core.UI.Button.ImageColorButton>() )
            {
                if( !use_type[(int)eButtonType.Color] )
                {
                    Undo.DestroyObjectImmediate( m_Target.GetComponent<Core.UI.Button.ImageColorButton>() );
                }
            }
            else
            {
                if( use_type[(int)eButtonType.Color] )
                {
                    Undo.AddComponent<Core.UI.Button.ImageColorButton>( m_Target.gameObject );
                }
            }
        }
        else
        {
            // マテリアルの色変更
            if( m_Target.GetComponent<Core.UI.Button.ColorButton>() )
            {
                if( !use_type[(int)eButtonType.Color] )
                {
                    Undo.DestroyObjectImmediate( m_Target.GetComponent<Core.UI.Button.ColorButton>() );
                }
            }
            else
            {
                if( use_type[(int)eButtonType.Color] )
                {
                    Undo.AddComponent<Core.UI.Button.ColorButton>( m_Target.gameObject );
                }
            }
        }

        // 拡大率変更
        if( m_Target.GetComponent<Core.UI.Button.ScaleButton>() )
        {
            if( !use_type[(int)eButtonType.Scale] )
            {
                Undo.DestroyObjectImmediate( m_Target.GetComponent<Core.UI.Button.ScaleButton>() );
            }
        }
        else
        {
            if( use_type[(int)eButtonType.Scale] )
            {
                Undo.AddComponent<Core.UI.Button.ScaleButton>( m_Target.gameObject );
            }
        }

        // イベント送信
        if( m_Target.GetComponent<Core.UI.Button.EventButton>() )
        {
            if( !use_type[(int)eButtonType.Event] )
            {
                Undo.DestroyObjectImmediate( m_Target.GetComponent<Core.UI.Button.EventButton>() );
            }
        }
        else
        {
            if( use_type[(int)eButtonType.Event] )
            {
                Undo.AddComponent<Core.UI.Button.EventButton>( m_Target.gameObject );
            }
        }

        // 画像変更
        if( m_Target.GetComponent<Core.UI.Button.ImageChangeButton>() )
        {
            if( !use_type[(int)eButtonType.Image] )
            {
                Undo.DestroyObjectImmediate( m_Target.GetComponent<Core.UI.Button.ImageChangeButton>() );
            }
        }
        else
        {
            if( use_type[(int)eButtonType.Image] )
            {
                Undo.AddComponent<Core.UI.Button.ImageChangeButton>( m_Target.gameObject );
            }
        }

        // サウンド
        if( m_Target.GetComponent<Core.UI.Button.SoundButton>() )
        {
            if( !use_type[(int)eButtonType.Sound] )
            {
                Undo.DestroyObjectImmediate( m_Target.GetComponent<Core.UI.Button.SoundButton>() );
            }
        }
        else
        {
            if( use_type[(int)eButtonType.Sound] )
            {
                Undo.AddComponent<Core.UI.Button.SoundButton>( m_Target.gameObject );
            }
        }
    }
}
