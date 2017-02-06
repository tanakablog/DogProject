// =====================================================
// [iPhone用スクリプト]ローカル保存ファイルの下記分はiCloudの対象外とするための追加・監視クラス
// -----------------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Utility
{
	public class iPhoneNoBackup:MonoBehaviour
	{
		protected static Queue< string > list = new Queue< string >();

		void Start()
		{
			#if !UNITY_IPHONE
			Destroy(this.gameObject.GetComponent<iPhoneNoBackup>());
			#endif
		}

		#if UNITY_IPHONE
		public void Update(){
			if( list.Count == 0 ) return;

			while( list.Count > 0 )
			{
				string path = list.Dequeue();
				Debug.Log("iPhone.SetNoBackupFlag:"+path);
				iPhone.SetNoBackupFlag( path );
			}
		}
		#endif


		public static void add( string path )
		{
			#if UNITY_IPHONE
			list.Enqueue( path );
			#endif
		}
	}
}