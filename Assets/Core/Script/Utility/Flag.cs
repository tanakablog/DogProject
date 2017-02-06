using UnityEngine;
using System.Collections;
using System;

namespace Core.Utility
{
    /// <summary>
    /// ビット演算を利用したフラグ管理クラスです.
    /// </summary>
    /// <example>
    /// <code>
    /// 
    /// // Enum
    /// [FlagsAttribute]
    /// enum eFlag
    /// {
    ///     None = 0,
    ///     Read = 1,
    ///     Write = 1 << 1,
    ///     Create = 1 << 2,
    ///     Delete = 1 << 3,
    ///     All = eFlag.Read | eFlag.Write | eFlag.Create | eFlag.Delete
    /// }
    /// 
    /// // Create
    /// Flag<eFlag> flag = new Flag<eFlag>();
    /// 
    /// </code>
    /// </example>
    public sealed class Flag<TEnum> where TEnum : struct
    {
        #region 変数宣言領域
        /// <summary>
        /// フラグ管理用変数.
        /// </summary>
        private ulong setFlags;
        #endregion

        #region 関数宣言領域
        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public Flag( )
        {
            Clear();
        }

        /// <summary>
        /// フラグの初期化.
        /// </summary>
        public void Clear( )
        {
            setFlags = 0;
        }

        /// <summary>
        /// フラグの追加.
        /// </summary>
        /// <param name="f">追加したいフラグ</param>
        public void Add( TEnum f )
        {
            setFlags |= Convert.ToUInt64( f );
        }

        /// <summary>
        /// フラグの設定.
        /// </summary>
        /// <param name="f">設定したいフラグ.</param>
        public void Set( TEnum f )
        {
            setFlags = Convert.ToUInt64( f );
        }

        /// <summary>
        /// フラグの削除. 
        /// </summary>
        /// <param name="f">削除したいクラス</param>
        public void Delete( TEnum f )
        {
            setFlags &= ~Convert.ToUInt64( f );
        }

        /// <summary>
        /// フラグの変更
        /// </summary>
        /// <param name="f">変更したいフラグ</param>
        public void Change( TEnum f )
        {
            setFlags ^= Convert.ToUInt64( f );
        }

        /// <summary>
        /// フラグの確認（一部一致）
        /// </summary>
        /// <param name="f">確認したいフラグ</param>
        public bool Check( TEnum f )
        {
            // Convert UInt
            ulong f_value = Convert.ToUInt64( f );

            // Check 0
            if( f_value == 0 && setFlags == 0 )
            {
                return true;
            }

            // Check Flag
            if( ( setFlags & f_value ) != 0 )
                return true;
            else
                return false;
        }

        /// <summary>
        /// フラグの確認（完全一致）
        /// </summary>
        /// <param name="f">確認したいフラグ</param>
        public bool CheckSame(TEnum f)
        {
            // Convert UInt
            ulong f_value = Convert.ToUInt64(f);

            // Check Flag
            if ((setFlags & f_value) == f_value)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 値取得
        /// </summary>
        /// <returns>フラグ値.</returns>
        public UInt64 GetValue( )
        {
            return setFlags;
        }
        #endregion
    }
}