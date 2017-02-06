using UnityEngine;
using System.Collections;

#pragma warning disable 0414

namespace Core.Private.Sample    // ネームスペースを「Core.~」の形式で書く
{

    public class TemplateSample : MonoBehaviour    // クラス名は単語毎に1文字目を大文字にする
    {
        private enum eSampleEnum    // enum名は初めにeを付け、単語毎に1文字目を大文字にする
        {
            SampleOne,      // 要素名は単語毎に1文字目を大文字にする
            SampleTwo,
        }

        private int m_MemberSample;   // メンバ変数は初めにm_を付け、単語毎に1文字目を大文字にする
        private static int m_StaticSample;  // static もメンバ変数と同じ扱い
        //private readonly int m_ReadOnlySample; // readonlyは使用禁止 

        private const int k_CONST_SAMPLE = 0;  // メンバ定数は初めにk_を付け、全て大文字で書く。単語と単語の間は_を入れる
        private static readonly int k_STATIC_READONLY_SAMPLE = 0;   // static readonly は定数と同じ扱い

        private delegate void DelegateSample( ); // デリゲートは関数と同じ命名規則

        private int memberSample        // プロパティは値の取得と設定のみ行う
        {                               // プロパティ名はメンバのm_を取り最初の文字を小文字にした名前
            get
            {
                return m_MemberSample;
            }

            set
            {
                m_MemberSample = value;
            }
        }

        private void SetAndRoundMemberSample( int member )      // 特別な処理を挟んで値を設定や取得をしたい場合は関数をつくる
        {                                                       // ここでは設定された値を0~100に丸めてm_MemberSampleにいれている
            m_MemberSample = Mathf.Clamp( member, 0, 100 );
        }

        private void MethodSample( int method_sample )          // 関数名は単語毎に最初の1文字を大文字にする
        {                                                       // 引数は全て小文字で単語と単語の間に_を入れる
        }
    }
}