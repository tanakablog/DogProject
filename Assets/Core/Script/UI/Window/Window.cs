using UnityEngine;
using System.Collections;

namespace Core.UI
{

	public class Window : MonoBehaviour
	{
		public enum eWindowState
		{
			Wait,
			Opening,
			Open,
			Closeing,
			Close
		}

		eWindowState mWndState;

		public eWindowState WindowState
		{
			get { return mWndState; }
			private set { mWndState = value; }
		}

		void Start()
		{
			WindowState = eWindowState.Wait;
		}

		// Update is called once per frame
		void Update()
		{

		}

		public void Open()
		{
			if (WindowState != eWindowState.Wait)
				return;

			WindowState = eWindowState.Opening;

			StartCoroutine(YieldFunction.WaitYieldFunction(1.0f, () => {
				WindowState = eWindowState.Open;
			}));



			StartCoroutine(YieldFunction.WaitYieldFunction(3.0f, () =>
			{
				Close();
			}));
		}

		public void Close()
		{
			if (WindowState != eWindowState.Open)
				return;

			WindowState = eWindowState.Closeing;

			StartCoroutine(YieldFunction.WaitYieldFunction(1.0f, () =>
			{
				WindowState = eWindowState.Close;
			}));
		}
	}

}