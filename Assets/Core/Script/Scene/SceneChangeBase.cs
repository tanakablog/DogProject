using UnityEngine;
using System.Collections;

namespace Core.Scene
{

    /// <summary>
    /// シーンチェンジベースクラス
    /// </summary>
    public abstract class SceneChangeBase : MonoBehaviour
    {
        /// <summary>
        /// 初期化
        /// </summary>
        public virtual void Init( )
        {
        }

        /// <summary>
        /// シーンアウト
        /// </summary>
        /// <returns>処理完了</returns>
        public abstract bool OutUpdate( );

        /// <summary>
        /// シーンイン
        /// </summary>
        /// <returns>処理完了</returns>
		public abstract bool InUpdate();

		/// <summary>
		/// 読み込み中
		/// </summary>
		public abstract void LoadingUpdate();

    }
}
