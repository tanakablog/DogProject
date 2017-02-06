using UnityEngine;
using System.Collections;
using Core.Base;

namespace Core.Scene
{

    public class ChangeSceneManager : Base.MonoSingleton<ChangeSceneManager>
    {
		[System.Serializable]
		class PrefabList
		{
			public GameObject sceneChangeNone = null;
			public GameObject sceneChangeFade = null;
		};
		[SerializeField]
		private PrefabList prefab;

        public enum eChangeType : int
        {
			None = 0,		// 演出なし
			Fade,			// フェードInOut
			
            Num				// 最大数
        }

        public enum eChangeState : int
        {
            Wait = 0,	// 使用待機中
            SceneOut,	// アウト
            LoadStart,	// 読み込み開始
            Loading,	// 読み込み中（開始処理待ち）
            SceneIn		// イン
        }

		eChangeState		m_State			= eChangeState.Wait;			// 現在の状態
		SceneChangeBase		m_ChangeClass	= null;							// 演出クラス
		System.Action		m_LoadAction	= null;							// 読み込み開始時の処理

		public RectTransform rectTransform { get; private set; }
		public Canvas canvas { get; private set; }
		public UnityEngine.UI.CanvasScaler canvasScaler { get; private set; }
		public UnityEngine.UI.GraphicRaycaster graphicRaycaster { get; private set; }

		// 使用中か確認
		public bool IsUse { get { return (m_State != eChangeState.Wait); } }

		protected override void OnInitialize()
		{
			DontDestroyOnLoad(this);

			rectTransform = this.gameObject.AddComponent<RectTransform>();
			canvas = this.gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 999;
			canvasScaler = this.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
			graphicRaycaster = this.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

		}

        protected override void OnFinalize( )
        {
        }

        /// <summary>
        /// シーンチェンジを行う
        /// </summary>
        /// <param name="scene_name"></param>
        /// <param name="change_type"></param>
		public void ChangeScene(eChangeType change_type, System.Action LoadStartAction)
        {
            if( IsUse ) return;

            // シーンアウトへ
            m_State = eChangeState.SceneOut;

			// 演出を取得
			m_ChangeClass = CreateChangeEffect(change_type);
			if (m_ChangeClass == null)
			{
				Debug.LogError("SceneEffectClass is Null");
			}

			m_LoadAction = LoadStartAction;

			// 初期化
			m_ChangeClass.Init();
        }

        /// <summary>
        /// シーンチェンジを行う
        /// </summary>
        /// <param name="scene_name"></param>
        /// <param name="change_type"></param>
		public void ChangeScene(Core.Scene.SceneTable.eScene scene_name, eChangeType change_type)
		{
			ChangeScene(Core.Scene.SceneTable.GetSceneName(scene_name), change_type);
		}

		/// <summary>
		/// シーンチェンジを行う
		/// </summary>
		/// <param name="scene_name"></param>
		/// <param name="change_type"></param>
        public void ChangeScene(string scene_name, eChangeType change_type )
        {
            if( IsUse )
                return;

            // シーンアウトへ
            m_State = eChangeState.SceneOut;

            // 演出を取得
            m_ChangeClass = CreateChangeEffect( change_type );
            if( m_ChangeClass == null )
            {
                Debug.LogError( "SceneEffectClass is Null" );
            }

			m_LoadAction = () => {
				Application.LoadLevel(scene_name);
				Core.Scene.ChangeSceneManager.instance.StartFadeIn();
			};

            // 初期化
            m_ChangeClass.Init();
        }

        void Update( )
        {
            switch( m_State )
            {
                case eChangeState.SceneIn:
                    if( m_ChangeClass != null )
                    {
                        if( m_ChangeClass.InUpdate() )
						{
							m_State = eChangeState.Wait;

							// 破棄
							Destroy(m_ChangeClass);
							m_ChangeClass = null;

							// 裏で使われたものも破棄
							for (int i = this.transform.childCount - 1; i >= 0; --i)
							{
								Destroy(this.transform.GetChild(i).gameObject);
							}

                        }
                    }
                    break;

                case eChangeState.SceneOut:

                    if( m_ChangeClass != null )
                    {
                        if( m_ChangeClass.OutUpdate() )
                        {
                            m_State = eChangeState.LoadStart;
                        }
                    }
                    break;

                case eChangeState.LoadStart:
					{
						m_State = eChangeState.Loading;
						m_LoadAction();
                    }
                    break;

				case eChangeState.Loading:
					{
						m_ChangeClass.LoadingUpdate();
					}
					break;
            }
        }

        // フェードインを許可する
        public void StartFadeIn( )
        {
            // 読み込み待ち以外では処理しない
            if( m_State != eChangeState.Loading )
                return;

            // 状態変更
            m_State = eChangeState.SceneIn;
        }

        /// <summary>
        /// シーン読み込み用演出クラスを作成
        /// </summary>
        /// <param name="type">演出タイプ</param>
        /// <returns>クラス</returns>
        SceneChangeBase CreateChangeEffect( eChangeType type )
        {
            SceneChangeBase l_effect = null;

			GameObject obj = null;

			switch (type)
			{
				case eChangeType.None:
					obj = (GameObject)Instantiate(prefab.sceneChangeNone);
					l_effect = obj.GetComponent<SceneChangeNone>();
					break;

				case eChangeType.Fade:
					obj = (GameObject)Instantiate(prefab.sceneChangeFade);
					l_effect = obj.GetComponent<SceneChangeFade>();
					break;
			}

			obj.transform.SetParent(this.transform);


			RectTransform rect = obj.gameObject.GetComponent<RectTransform>();
			rect.anchoredPosition = Vector3.zero;
			rect.sizeDelta = new Vector2(0, 0);

            return l_effect;
        }
    }
}