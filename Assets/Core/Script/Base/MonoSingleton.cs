using UnityEngine;

namespace Core.Base
{
	/// <summary>
	/// シングルトン基底クラス
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		/// <summary>
		/// インスタンス
		/// </summary>
		private static T m_Inst = null;

		/// <summary>
		/// インスタンスを取得
		/// </summary>
		public static T instance
		{
			get
			{
				// インスタンスが既にあるのでそれを返す
				if (m_Inst != null)
					return m_Inst;

				// 継承されたクラスのタイプ
				System.Type l_type = typeof(T);

				// ヒエラルキー上のインスタンスを検索
				T l_inst = (T)FindObjectOfType(l_type);

				// 存在を確認
				if (l_inst == null)
				{
					// 存在していなかったので、新たに生成
					GameObject l_object = new GameObject(l_type.ToString(), l_type);

					// コンポーネントを取得
					l_inst = l_object.GetComponent<T>();

					if (l_inst != null)
					{
						Initialize(l_inst);
					}

				}
				else
				{
					// 初期化を行う
					Initialize(l_inst);
				}

				return m_Inst;
			}
		}

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="instance"></param>
		static void Initialize(T instance)
		{
			// 未登録であれば登録とか
			if (m_Inst == null)
			{
				m_Inst = instance;
				m_Inst.OnInitialize();
			}
			else if (m_Inst != instance)
			{
				// 既に登録済みとなっていて、違うインスタンスであれば
				// 即座に破棄
				DestroyImmediate(instance.gameObject);
			}
		}

		/// <summary>
		/// 破棄
		/// </summary>
		/// <param name="instance"></param>
		static void Destoryed(T instance)
		{
			if (m_Inst == instance)
			{
				m_Inst.OnFinalize();
				m_Inst = null;
			}
		}

		/// <summary>
		/// 初期化時に呼びたいものをここに書く
		/// </summary>
		protected virtual void OnInitialize()
		{
		}

		/// <summary>
		/// 終了時に呼びたいものをここに書く
		/// </summary>
		protected virtual void OnFinalize()
		{
		}

		/// <summary>
		/// 開始時
		/// </summary>
		void Awake()
		{
			// 初期化を開始させる
			Initialize((T)this);
		}

		/// <summary>
		/// 破棄された時
		/// </summary>
		void OnDestroy()
		{
			Destoryed((T)this);
		}

		/// <summary>
		/// ゲームが終了された時
		/// </summary>
		void OnApplicationQuit()
		{
			Destoryed((T)this);
		}
	}
}