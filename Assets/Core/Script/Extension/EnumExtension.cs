using UnityEngine;
using System.Collections;
using System;

namespace Core.Extension
{
    /// <summary>
    /// Enum拡張クラス
    /// </summary>
    public class EnumExtension
    {
        /// <summary>
        /// 列挙型の要素数取得
        /// </summary>
        /// <typeparam name="TEnum">列挙型を指定してください</typeparam>  
        /// <returns>要素数</returns>
        public static int GetEnumNum<TEnum>( ) where TEnum : struct
        {
            return Enum.GetValues( typeof( TEnum ) ).Length;
        }

        /// <summary>
        /// 列挙型の要素名取得
        /// </summary>
        /// <typeparam name="TEnum">列挙型を指定してください</typeparam> 
        /// <returns>列挙型の名前配列</returns>
        public static string[] GetEnumNames<TEnum>( ) where TEnum : struct
        {
            return Enum.GetNames( typeof( TEnum ) );
        }

        /// <summary>
        /// 文字列から列挙型に変換
        /// </summary>
        /// <typeparam name="TEnum">列挙型を指定してください</typeparam>
        /// <param name="name">文字列</param>
        /// <returns>列挙型の要素</returns>
        public static TEnum StringToEnum<TEnum>( string name ) where TEnum : struct
        {
            return (TEnum)Enum.Parse( typeof( TEnum ), name );
        }

        /// <summary>
        /// 数値から列挙型に変換
        /// </summary>
        /// <typeparam name="TEnum">列挙型を指定してください</typeparam>
        /// <param name="index">数値</param>
        /// <returns>列挙型の要素</returns>
        public static TEnum IndexToEnum<TEnum>( int index ) where TEnum : struct
        {
            return (TEnum)Enum.ToObject( typeof( TEnum ), index );
        }
    }
}
