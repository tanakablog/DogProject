﻿using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// ビルダー補助クラス
/// </summary>
public class StringBuilderSupport
{
    /// <summary>
    /// 無効文字配列
    /// </summary>
    private static readonly string[] m_INVALID_CHARS =
    {
	    " ", "!", "\"", "#", "$", "%", "&", "\'", "(", ")", 
        "-", "=", "^", "~", "\\", "|", "[", "{",  "@", "`", 
        "]", "}", ":", "*", ";", "+", "/", "?", ".", ">", 
        ",", "<"
    };

    /// <summary>
    /// 列挙型最初無効文字配列
    /// </summary>
    private static readonly string[] m_ENUM_FIRST_INVALID_CHARS =
    {
        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
    };

    /// <summary>
    /// 名前空間定義 ※「}」あり
    /// </summary>
    /// <param name="name_space">名前空間名</param>
    /// <param name="tab_num">タブ数</param>  
    /// <returns>名前空間が定義された文字列</returns>
    public static string EditNameSpace( string name_space, string content, int tab_num = 0 )
    {
        // ビルダー生成
        StringBuilder builder = new StringBuilder();

        // 名前空間定義
        builder.AppendLine( EditNameSpace( name_space, tab_num ) );

        // 名前空間内容定義
        builder.AppendLine( content );
        builder.AppendLine( SetTab( tab_num ) + "}" );

        return builder.ToString();
    }

    /// <summary>
    /// 名前空間定義 ※「}」なし
    /// </summary>
    /// <param name="name_space">名前空間名</param>
    /// <param name="tab_num">タブ数</param>   
    /// <returns>名前空間が定義された文字列</returns>
    public static string EditNameSpace(  string name_space, int tab_num = 0 )
    {
        // ビルダー生成
        StringBuilder builder = new StringBuilder();

        // タブ設定
        string tab = SetTab( tab_num );

        // 名前空間定義
        builder.AppendLine( tab + "namespace " + name_space );
        builder.AppendLine( tab + "{" );

        return builder.ToString();
    }

    /// <summary>
    /// クラス定義 ※「}」あり
    /// </summary>
    /// <param name="class_name">クラス名</param> 
    /// <param name="content">内容</param>
    /// <param name="tab_num">タブ数</param>
    /// <param name="summary">説明</param>
    /// <param name="is_static">静的クラスか</param> 
    /// <returns>クラスが定義された文字列</returns>
    public static string EditClass( string class_name, string content, int tab_num = 0, bool is_static = false, string summary = "" )
    {
        // ビルダー生成
        StringBuilder builder = new StringBuilder();

        // クラス定義
        builder.AppendLine( EditClass( class_name, tab_num, is_static, summary ) );

        // クラス内容定義
        builder.AppendLine( content );
        builder.AppendLine( SetTab( tab_num ) + "}" );

        return builder.ToString();
    }

    /// <summary>
    /// クラス定義 ※「}」なし
    /// </summary>
    /// <param name="class_name">クラス名</param>
    /// <param name="tab_num">タブ数</param>
    /// <param name="summary">説明</param>
    /// <param name="is_static">静的クラスか</param> 
    /// <returns>クラスが定義された文字列</returns>
    public static string EditClass( string class_name, int tab_num = 0, bool is_static = false, string summary = "" )
    {                                              
        // ビルダー生成
        StringBuilder builder = new StringBuilder();

        // タブ設定
        string tab = SetTab( tab_num );

        // 静的クラス
        string static_class = "";

        // 静的クラス設定
        if( is_static )
        {
            static_class = "static ";
        }

        // 説明定義
        builder.Append( EditSummary( summary, tab_num ) );

        // クラス定義
        builder.AppendLine( tab + "public " + static_class + "class " + class_name );
        builder.AppendLine( tab + "{" );

        return builder.ToString();
    }

    /// <summary>
    /// 列挙型定義 ※「}」あり
    /// </summary>
    /// <param name="enum_name">列挙型名</param> 
    /// <param name="content">内容</param>
    /// <param name="tab_num">タブ数</param>
    /// <param name="summary">説明</param>   
    /// <returns>列挙型が定義された文字列</returns>
    public static string EditEnum( string enum_name, string content, int tab_num = 0, string summary = "" )
    {
        // ビルダー生成
        StringBuilder builder = new StringBuilder();

        // 列挙型定義
        builder.Append( EditEnum( enum_name, tab_num, summary ) );

        // 列挙型内容定義
        builder.AppendLine( content );
        builder.AppendLine( SetTab( tab_num ) + "}" );

        return builder.ToString();
    }

    /// <summary>
    /// 列挙型定義 ※「}」なし
    /// </summary>
    /// <param name="enum_name">列挙型名</param>
    /// <param name="tab_num">タブ数</param>
    /// <param name="summary">説明</param>   
    /// <returns>列挙型が定義された文字列</returns>
    public static string EditEnum( string enum_name, int tab_num = 0, string summary = "" )
    {
        // ビルダー生成
        StringBuilder builder = new StringBuilder();

        // タブ設定
        string tab = SetTab( tab_num );

        // 説明定義
        builder.Append( EditSummary( summary, tab_num ) );

        // 列挙型定義
        builder.AppendLine( tab + "public enum " + enum_name );
        builder.AppendLine( tab + "{" );

        return builder.ToString();        
    }

    /// <summary>
    /// 説明定義
    /// </summary>
    /// <param name="summary">説明</param>
    /// <param name="tab_num">タブ数</param>
    /// <returns>説明が設定された文字列</returns>
    public static string EditSummary( string summary, int tab_num = 0 )
    {
        // ビルダー生成
        StringBuilder builder = new StringBuilder();

        // タブ設定
        string tab = SetTab( tab_num ); 

        // 説明定義
        builder.AppendLine( tab + "/// <summary>" );
        builder.AppendLine( tab + "/// " + summary );
        builder.AppendLine( tab + "/// </summary>" );

        return builder.ToString();
    }

    /// <summary>
    /// 文字列結合
    /// </summary>
    /// <param name="string_array">文字列配列</param>
    /// <param name="tab_num">タブ数</param>
    /// <param name="delimit">結合時に挿入する文字</param>
    /// <returns>結合された文字列</returns>
    public static string JoinStrings( string[] string_array, int tab_num, string delimit = ",\n", string start = "", string end = "" )
    {
        // タブ設定
        string tab = SetTab( tab_num );

        // 結合文字列
        string join_string = "";

        // 配列全てを結合
        for( int i = 0; i < string_array.Length; ++i )
        {
            // 終了チェック
            if( string_array.Length - 1 == i )
            {
                // 最後はなにもつけない
                delimit = "";
            }

            // 結合
            join_string += tab + start + string_array[i] + end + delimit;           
        }

        return join_string;
    }

    /// <summary>
    /// 列挙型最初無効文字検索
    /// </summary>
    /// <param name="str">検索する文字列</param>
    /// <returns>見つかった無効文字</returns>
    public static string FindEnumFirstInvaild( string str )
    {
        return FindInvaild( str, m_ENUM_FIRST_INVALID_CHARS );
    }

    /// <summary>
    /// 無効文字検索
    /// </summary>
    /// <param name="str">検索する文字列</param>
    /// <returns>見つかった無効文字</returns>
    public static string FindInvaild( string str )
    {
        return FindInvaild( str, m_INVALID_CHARS );
    }

    /// <summary>
    /// 無効文字検索
    /// </summary>
    /// <param name="str">検索する文字列</param>
    /// <param name="invaild_chars">無効文字配列</param>
    /// <returns>見つかった無効文字</returns>
    public static string FindInvaild( string str, string[] invaild_chars )
    {
        // 無効文字があるか検索
        foreach( var c in invaild_chars )
        {
            // 無効文字チェック
            if( 0 <= str.IndexOf( c ) )
            {
                // 無効文字あり
                return c;
            }
        }

        // 無効文字なし
        return null;
    }

    /// <summary>
    /// タブ設定
    /// </summary>
    /// <param name="num">タブ数</param>
    /// <returns>タブが設定された文字列</returns>
    public static string SetTab( int num )
    {
        // タブ文字列
        string tab = "";

        // タブ設定
        for( int i = 0; i < num; ++i )
        {
            tab += "\t";
        }

        return tab;
    }
}
