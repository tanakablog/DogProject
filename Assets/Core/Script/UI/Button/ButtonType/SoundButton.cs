using UnityEngine;
using System.Collections;
using System.Linq;

namespace Core.UI.Button
{
    /// <summary>
    /// サウンドボタン
    /// </summary>
    public class SoundButton : Private.ButtonBase
    {
        /// <summary>
        /// 行うアクション
        /// </summary>
        public enum eAction
        {
            Down,
            Up,
            PressStart,
        }

        /// <summary>
        /// 有効時のSE
        /// </summary>
        public Sound.SoundTable.eSE[] enableSe =
        {
            Sound.SoundTable.eSE.NoSound, Sound.SoundTable.eSE.NoSound, Sound.SoundTable.eSE.NoSound 
        };

        /// <summary>
        /// 無効時のSE
        /// </summary>
        public Sound.SoundTable.eSE[] disableSe =
        {
            Sound.SoundTable.eSE.NoSound, Sound.SoundTable.eSE.NoSound, Sound.SoundTable.eSE.NoSound 
        };

        #region 有効アクション

        protected override void OnDownAction( )
        {
            Sound.SoundManager.instance.PlaySE( enableSe[(int)eAction.Down] );
        }

        protected override void OnUpAction( )
        {
            Sound.SoundManager.instance.PlaySE( enableSe[(int)eAction.Up] );
        }

        protected override void OnPressStartAction( )
        {
            Sound.SoundManager.instance.PlaySE( enableSe[(int)eAction.PressStart] );
        }

        #endregion

        #region 無効アクション

        protected override void OnNotDownAction( )
        {
            Sound.SoundManager.instance.PlaySE( disableSe[(int)eAction.Down] );
        }

        protected override void OnNotUpAction( )
        {
            Sound.SoundManager.instance.PlaySE( disableSe[(int)eAction.Up] );
        }

        protected override void OnNotPressStartAction( )
        {
            Sound.SoundManager.instance.PlaySE( disableSe[(int)eAction.PressStart] );
        }

        #endregion
    }
}