using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Core.UI.Button
{

    /// <summary>
    /// イメージ変更ボタンクラス
    /// </summary>
    [RequireComponent( typeof( Image ) )]
    public class ImageChangeButton : Private.ButtonBase
    {
        // スプライト名
        public enum eSprite
        {
            Up,
            Down,
            Disable
        }

        /// <summary>
        /// スプライト配列
        /// </summary>
        public SpriteCore.SpriteTable.eSprite[] spriteType = 
        {
            SpriteCore.SpriteTable.eSprite.NoSprite,
            SpriteCore.SpriteTable.eSprite.NoSprite,
            SpriteCore.SpriteTable.eSprite.NoSprite
        };

        /// <summary>
        /// スプライト変更
        /// </summary>
        private Sprite[] m_ChangeSprite = new Sprite[Extension.EnumExtension.GetEnumNum<eSprite>()];

        /// <summary>
        /// 使用スプライト
        /// </summary>
        private eSprite m_UseSprite;

        /// <summary>
        /// イメージ
        /// </summary>
        private Image m_ButtonImage;

        /// <summary>
        /// ボタンが有効かどうか
        /// </summary>
        private bool m_IsEnable;

        /// <summary>
        /// 列挙体指定を行うか
        /// </summary>
        /// <remarks>エディターで使用</remarks>
        private bool m_UseEnum = true;

        /// <summary>
        /// 変更スプライトプロパティ
        /// </summary>
        public Sprite[] changeSprite
        {
            get
            {
                return m_ChangeSprite;
            }

            set
            {
                m_ChangeSprite = value;
            }
        }

        /// <summary>
        /// 使用スプライトプロパティ
        /// </summary>
        public eSprite useSprite
        {
            get
            {
                return m_UseSprite;
            }

            set
            {
                m_UseSprite = value;
            }
        }

        /// <summary>
        /// 列挙体指定フラグプロパティ
        /// </summary>
        public bool useEnum
        {
            get
            {
                return m_UseEnum;
            }

            set
            {
                m_UseEnum = value;
            }
        }

        /// <summary>
        /// 列挙体で指定されたスプライトを設定
        /// </summary>
        public void SetEnumSprite( )
        {
            for( int i = 0; i < Extension.EnumExtension.GetEnumNum<eSprite>(); ++i )
            {
                m_ChangeSprite[i] = SpriteCore.SpriteManager.instance.GetSprite( spriteType[i] );
            }       
        }

        /// <summary>
        /// リセットスプライト
        /// </summary>
        public void ResetSprite( )
        {
            // イメージチェック
            if( m_ButtonImage == null )
            {
                // イメージ取得
                m_ButtonImage = this.GetComponent<Image>();
            }

            // ボタン状態チェック
            m_IsEnable = ButtonManager.instance.CheckButtonState( this.m_Info.type );

            if( m_IsEnable )
            {
                SetSprite( eSprite.Up );
            }
            else
            {
                SetSprite( eSprite.Disable );
            }
        }

        #region 非公開メソッド

        /// <summary>
        /// Unity Start
        /// </summary>
        void Start( )
        {
            if( m_UseEnum )
            {
                // スプライト設定
                SetEnumSprite();
            }

            // ボタンイメージリセット
            ResetSprite();
        }

        /// <summary>
        /// スプライトを設定
        /// </summary>
        /// <param name="type">スプライト名</param>
        void SetSprite( eSprite type )
        {
            if( !this.enabled )
            {
                return;
            }

            if( type == eSprite.Disable )
            {
                m_IsEnable = false;
            }
            else
            {
                m_IsEnable = true;
            }

            m_ButtonImage.sprite = m_ChangeSprite[(int)type];

            // タイプ保存
            m_UseSprite = type;
        }

        #region ボタン状態変更コールバック

        protected override void SetButtonStateChange( ButtonManager.eButton state )
        {
            if( ButtonManager.instance.CheckButtonState( this.m_Info.type ) && !m_IsEnable )
            {
                SetSprite( eSprite.Up );
            }
            else if( !ButtonManager.instance.CheckButtonState( this.m_Info.type ) && m_IsEnable )
            {
                SetSprite( eSprite.Disable );
            }
        }

        protected override void AddButtonStateChange( ButtonManager.eButton state )
        {
            if( ButtonManager.instance.CheckButtonState( this.m_Info.type ) && !m_IsEnable )
            {
                SetSprite( eSprite.Up );
            }
        }

        protected override void DeleteButtonStateChange( ButtonManager.eButton state )
        {
            if( !ButtonManager.instance.CheckButtonState( this.m_Info.type ) && m_IsEnable )
            {
                SetSprite( eSprite.Disable );
            }
        }

        #endregion

        #region 有効アクション

        protected override void OnDownAction( )
        {
            SetSprite( eSprite.Down );
        }

        protected override void OnUpAction( )
        {
            SetSprite( eSprite.Up );
        }

        protected override void OnExitAction( )
        {
            SetSprite( eSprite.Up );
        }

        #endregion

        #endregion
    }
}