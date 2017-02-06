using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core.UI
{
	public class WindowManager : Base.MonoSingleton<WindowManager>
	{
		/// <summary>
		/// 生成されるウィンドウキュー
		/// </summary>
		private Queue<Window> m_WindowCueue;

		/// <summary>
		/// 現在開かれているウィンドウ
		/// </summary>
		private Window m_CurrentWindow;

		private UIMain m_UIMain = null;

		/// <summary>
		/// 初期化時
		/// </summary>
		protected override void OnInitialize()
		{
			// UIMainを取得
			m_UIMain = UIMain.GetUIMain();

			m_WindowCueue = new Queue<Window>();
			m_CurrentWindow = null;
		}

		void Update()
		{
			// 全面のウィンドウが存在している時
			if (m_CurrentWindow != null)
			{
				// ウィンドウの状態を取得
				switch (m_CurrentWindow.WindowState)
				{
					case Window.eWindowState.Close:
						{
							// ウインドウが閉じられた
							Destroy(m_CurrentWindow.gameObject);

							// まだ表示しきれていないウィンドウがある
							if (m_WindowCueue.Count >= 1)
							{
								// 次のウィンドウへ
								NextWindow();
							}
							else
							{
								// 終わり
								m_CurrentWindow = null;
							}
						}
						break;
				}

				return;
			}
		}

		public void Open()
		{
			string path = Core.ResourcesCore.ResourcesTable.GetResourcesPath(Core.ResourcesCore.ResourcesTable.eResourcePath.Core_Prefab_Resources_UI_Window);

			GameObject l_wnd = (GameObject)Resources.Load(path);

			GameObject l_newWndObj = (GameObject)Instantiate(l_wnd);

			l_newWndObj.SetActive(false);

			l_newWndObj.transform.parent = m_UIMain.canvas.transform;
			l_newWndObj.transform.localPosition = Vector3.zero;

			Window l_Wnd = l_newWndObj.AddComponent<Window>();

			// ウィンドウを作ってキューに入れる
			m_WindowCueue.Enqueue(l_Wnd);

			// 今表示しているウインドウが無ければ
			// 今登録したのを開くようにする
			if (m_CurrentWindow == null)
				NextWindow();
		}

		/// <summary>
		/// 次のウィンドウへ移動
		/// </summary>
		private void NextWindow()
		{
			m_CurrentWindow = m_WindowCueue.Dequeue();
			m_CurrentWindow.gameObject.SetActive(true);
			m_CurrentWindow.Open();
		}

	}
}