using UnityEngine;
using System.Collections;
using System;

public class GestureManager : MonoBehaviour
{
    /// <summary>
    /// ジェスチャー情報
    /// </summary>
    public class GestureInfo
    {
        /// <summary>
        /// ジェスチャー開始位置
        /// </summary>
        public Vector2 startPos;

        /// <summary>
        /// ジェスチャーイベント位置
        /// </summary>
        public Vector2 eventPos;

        /// <summary>
        /// 移動方向
        /// </summary>
        public eDirection moveDirection;

        /// <summary>
        /// ジェスチャー経過時間
        /// </summary>
        public float gestureTime;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GestureInfo()
        { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="start">開始位置</param>
        /// <param name="end">イベント位置</param>
        /// <param name="dir">移動方向</param>
        /// <param name="time">時間</param>
        public GestureInfo( Vector2 start, Vector2 end, eDirection dir, float time )
        {
            startPos = start;
            eventPos = end;
            moveDirection = dir;
            gestureTime = time; 
        }

    }

    /// <summary>
    /// 方向名
    /// </summary>
    [Flags]
    public enum eDirection
    {
        // 移動なし
        None            = 0,

        // 移動方向
        Up              = 1,                                // 上
        Down            = 1 << 1,                           // 下
        Right           = 1 << 2,                           // 右
        Left            = 1 << 3,                           // 左
        UpRight         = 1 << 4,                           // 右上
        UpLeft          = 1 << 5,                           // 左上
        DownRight       = 1 << 6,                           // 右下
        DownLeft        = 1 << 7,                           // 左下

        AboutUp         = Up | UpRight | UpLeft,            // 大体上 
        AboutDown       = Down | DownRight | DownLeft,      // 大体下
        AboutRight      = Right | UpRight | DownRight,      // 大体右
        AboutLeft       = Left | UpLeft | DownLeft,         // 大体左

        // 移動あり
        AnyMove         = int.MaxValue
    }

    /// <summary>
    /// ジェスチャ名
    /// </summary>
    public enum eGesture
    {
        Tap,
        DoubleTap,
        PressStart,
        PressEnd,
        Flick,
        Drag,
        Move
    }

    /// <summary>
    /// ジェスチャーデリゲート
    /// </summary>
    /// <param name="info"></param>
    public delegate void GestureDelegate(GestureInfo info);

    /// <summary>
    /// タップ判定時間
    /// </summary>
    [SerializeField]
    private float m_TapTime = 0.2f;

    /// <summary>
    /// ダブルタップ判定時間
    /// </summary>
    [SerializeField]
    private float m_DoubleTapTime = 0.3f;

    /// <summary>
    /// 長押し開始時間
    /// </summary>
    [SerializeField]
    private float m_PressTime = 0.7f;

    /// <summary>
    /// フリック判定時間
    /// </summary>
    [SerializeField]
    private float m_FlickTime = 0.3f;

    [SerializeField]
    private float m_MoveStartLength = 20.0f;

    /*
    /// <summary>
    /// フリック開始距離
    /// </summary>
    [SerializeField]
    private float m_FlickLength = 20.0f;

    /// <summary>
    /// ドラッグ開始距離
    /// </summary>
    [SerializeField]
    private float m_DragLength = 10.0f;
    */

    /// <summary>
    /// ジェスチャイベント
    /// </summary>
    private GestureDelegate[] m_GestureEvent = new GestureDelegate[Core.Extension.EnumExtension.GetEnumNum<eGesture>()];

    private float m_DownTime;

    private float m_PrevDownTime;

    private Vector2 m_DownPos;

    private Vector2 m_PrevDownPos;

    private Core.Utility.Flag<eDirection> m_Direction = new Core.Utility.Flag<eDirection>();


    void Awake()
    {
        for (int i = 0; i < m_GestureEvent.Length; ++i )
        {
            m_GestureEvent[i] += (GestureInfo info) => { Debug.Log( " start:" + info.startPos + " end:" + info.eventPos + " dir:" + info.moveDirection + " time:" + info.gestureTime); };
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if( Input.GetMouseButtonDown(0) )
        {
            Down();
        }
        else if( Input.GetMouseButton(0) )
        {
            Press();
        }

        if( Input.GetMouseButtonUp(0) )
        {
            Up();
        }
	}

    private void Down()
    {
        m_Direction.Set(eDirection.None);

        m_PrevDownTime = m_DownTime;

        m_PrevDownPos = m_DownPos;

        m_DownTime = Time.time;

        m_DownPos = Input.mousePosition;

        StartCoroutine(CheckPress());

    }

    private void Up()
    {
        float time = Time.time - m_DownTime;
        float prev_time = Time.time - m_PrevDownTime;

        if ( m_Direction.Check( eDirection.AnyMove ) )
        {
            
        }
        else
        {
            if( time <= m_TapTime )
            {
                Debug.Log("Tap");
                m_GestureEvent[(int)eGesture.Tap](new GestureInfo(m_DownPos, Input.mousePosition, eDirection.None, time));

                if( prev_time <= m_DoubleTapTime )
                {
                    Debug.Log("DoubleTap");
                    m_GestureEvent[(int)eGesture.DoubleTap](new GestureInfo(m_DownPos, Input.mousePosition, eDirection.None, time));
                }
            }
        }
    }

    private void Press()
    {
    }

    private IEnumerator CheckPress()
    {  
        float time = Time.time;
        bool press = false;

        while(!Input.GetMouseButtonUp(0))
        {
            yield return null;

            if (Time.time - time >= m_PressTime)
            {
                Debug.Log("PressStart");
                m_GestureEvent[(int)eGesture.PressStart](new GestureInfo(m_DownPos, Input.mousePosition, m_Direction.Check( eDirection.AnyMove ) ? eDirection.AnyMove : eDirection.None, Time.time - m_DownTime));

                press = true;

                break;
            }
        }

        if (press)
        {
            time = Time.time;

            while (!Input.GetMouseButtonUp(0))
            {
                yield return null;
            }

            Debug.Log("PressEnd");
            m_GestureEvent[(int)eGesture.PressEnd](new GestureInfo(m_DownPos, Input.mousePosition, m_Direction.Check(eDirection.AnyMove) ? eDirection.AnyMove : eDirection.None, Time.time - time));
        }
    }

    private IEnumerator CheckMove()
    {
        Vector2 gap;
        Vector2 start = m_DownPos;
        float length;

        while (!Input.GetMouseButtonUp(0))
        {
            yield return null;

            gap = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - start;
            length = gap.magnitude;

            if (length >= m_MoveStartLength)
            {
                start = Input.mousePosition;

                float dir = Vector2.Dot(Vector2.up, gap);

                if (0.75 <= dir)
                {
                    m_Direction.Set(eDirection.Up);
                }
                else if (-0.75 >= dir)
                {
                    m_Direction.Set(eDirection.Down);
                }
                else if (0.25 < dir)
                { }

                      
            }
        }

    }
}
