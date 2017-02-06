using UnityEngine;
using System.Collections;

namespace Core.UI
{

	public class UIMain : MonoBehaviour
	{
		public Canvas canvas;

		// Use this for initialization
		void Awake()
		{
		//	canvas = cachedTransform.FindChild("Canvas").GetComponent<Canvas>();
		}

		// Update is called once per frame
		void Update()
		{

		}
		
		public static UIMain GetUIMain()
		{
			System.Type l_type = typeof(UIMain);

			// ヒエラルキー上のインスタンスを検索
			UIMain l_uiMain = (UIMain)FindObjectOfType(l_type);

			if (l_uiMain == null)
			{
				// なかったので、Resourceフォルダから読み込んでくる
				GameObject uiMain_Prefab = Resources.Load<GameObject>("UI/UIMain");
				GameObject uiMain_Obj = (GameObject)Instantiate(uiMain_Prefab);
				l_uiMain = uiMain_Obj.GetComponent<UIMain>();

				if( l_uiMain == null)
					l_uiMain = uiMain_Obj.AddComponent<UIMain>();

			}

			return l_uiMain;
		}
	}

}