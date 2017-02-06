using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Core.SpriteCore
{
    /// <summary>
    /// スプライト管理クラス
    /// </summary>
    public class SpriteManager : Base.MonoSingleton<SpriteManager>
    {
        /// <summary>
        /// スプライトリスト配列
        /// </summary>
        List<Sprite>[] spriteList = new List<UnityEngine.Sprite>[SpriteTable.ATLAS_PATH.Length];

        /// <summary>
        /// スプライト取得
        /// </summary>
        /// <param name="sprite">スプライト列挙型の要素</param>
        /// <returns>スプライト</returns>
        public UnityEngine.Sprite GetSprite( SpriteTable.eSprite sprite )
        {
            return SpriteEnumToSprite( sprite );
        }

        /// <summary>
        /// スプライト取得
        /// </summary>
        /// <param name="sprite">スプライト列挙型の要素</param>
        /// <returns>スプライト</returns>
        public UnityEngine.Sprite SpriteEnumToSprite( SpriteTable.eSprite sprite )
        {   
            // アトラス番号取得
            int atlas = SpriteEnumToAtlasIndex( sprite );

            // アトラス番号チェック
            if( (int)SpriteTable.eAtlas.NoAtlas >= atlas )
            {
                return null;
            }

            // 読み込み済みかチェック
            if( spriteList[atlas] == null )
            {
                // スプライトリスト生成
                spriteList[atlas] = new List<UnityEngine.Sprite>();

                // スプライト読み込み
                spriteList[atlas].AddRange( Resources.LoadAll<UnityEngine.Sprite>( SpriteTable.ATLAS_PATH[atlas] ) );
            }

            // スプライト検索
            UnityEngine.Sprite back = spriteList[atlas].Find( find => find.name.Equals( sprite.ToString() ) );

            if( !back )
            {
                Debug.LogError( sprite.ToString() + " が見つかりませんでした" );
            }

            return back;
        }

        /// <summary>
        /// スプライト列挙型からアトラス番号へ変換
        /// </summary>
        /// <param name="sprite">スプライト列挙型</param>
        /// <returns>アトラス番号</returns>
        public int SpriteEnumToAtlasIndex( SpriteTable.eSprite sprite )
        {   
            int counter = 0;

            // 使用アトラス検索
            for( int i = 0; i < SpriteTable.SPRITE_INDEX.Length; ++i )
            {
                counter += SpriteTable.SPRITE_INDEX[i];

                if( (int)sprite < counter )
                {
                    return i;
                }
            }

            Debug.LogError( sprite.ToString() + "の使用しているアトラスが見つかりません" );

            // 見つからなかった場合
            return -1;
        }

        /// <summary>
        /// スプライト列挙型からアトラス列挙型へ変換
        /// </summary>
        /// <param name="sprite">スプライト列挙型</param>
        /// <returns>アトラス列挙型</returns>
        public SpriteTable.eAtlas SpriteEnumToAtlasEnum( SpriteTable.eSprite sprite )
        {   
            return (SpriteTable.eAtlas)SpriteEnumToAtlasIndex( sprite );
        }

        /// <summary>
        /// スプライト名からスプライト列挙型へ変換
        /// </summary>
        /// <param name="name">スプライト名</param>
        /// <returns>スプライト列挙型</returns>
        public SpriteTable.eSprite SpriteNameToSpriteEnum( string name )
        {
            return Extension.EnumExtension.StringToEnum<SpriteTable.eSprite>( name );
        }

        /// <summary>
        /// スプライトからアトラス名へ変換
        /// </summary>
        /// <param name="sprite">スプライト</param>
        /// <returns>アトラス名</returns>
        public string SpriteToAtlasName( UnityEngine.Sprite sprite )
        {
            if( sprite == null )
            {
                return null;
            }

            SpriteTable.eSprite e_sprite = Extension.EnumExtension.StringToEnum<SpriteTable.eSprite>( sprite.name );

            return SpriteEnumToAtlasEnum( e_sprite ).ToString();
        }

        /// <summary>
        /// スプライトからアトラス列挙体へ変換
        /// </summary>
        /// <param name="sprite">スプライト</param>
        /// <returns>アトラス名</returns>
        public SpriteTable.eSprite SpriteToSpriteEnum( UnityEngine.Sprite sprite )
        {
            if( sprite == null )
            {
                return SpriteTable.eSprite.NoSprite;
            }

            return Extension.EnumExtension.StringToEnum<SpriteTable.eSprite>( sprite.name );
        }
    }
}
