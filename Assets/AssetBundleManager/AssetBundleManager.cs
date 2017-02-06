using System.IO;
using UnityEngine;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// アセットバンドル管理クラス
/// </summary>
public class AssetBundleManager : MonoBehaviour
{
	#region シングルトン処理

	private static AssetBundleManager instance;

	public static AssetBundleManager Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType <AssetBundleManager> ();
				if (instance == null) {
					var obj = new GameObject ("AssetBundleManager", typeof(AssetBundleManager));
					instance = obj.GetComponent<AssetBundleManager> ();
				}
			}
			return instance;
		}
	}

	public static bool m_IsDestroy = false;

	#endregion

	#if UNITY_EDITOR
	/// <summary>
	/// デバックログ
	/// </summary>
	[SerializeField]
	private bool m_IsDebugLog = false;
	#endif

	/// <summary>
	/// サーバーURL
	/// </summary>
	private string m_SeverUrl;

	/// <summary>
	// ルートからのアセットバンドル設置パス
	/// </summary>
	public const string kASSET_BUNDLE_OUTPUT_PATH = "AssetBundles";

	/// <summary>
	/// アセットバンドルテキスト名
	/// </summary>
	public const string kASSET_BUNDLE_TEXT_NAME = "AssetBundleText";

	/// <summary>
	/// アセットバンドルに含めるアセットのベースパス
	/// </summary>
	public const string kASSET_BUNDLE_BASE_PATH = "Assets/MB/AssetBundle/";

	/// <summary>
	/// 最大ダウンロード中数
	/// </summary>
	private const int kMAX_DONWLOADING_NUM = 3;

	/// <summary>
	/// 最大キャッシュ値
	/// </summary>
	private readonly long kMAX_CACHE = (long)2000 * (long)1024 * (long)1024;

	/// <summary>
	/// データベースへのURL保存
	/// </summary>
	private string m_DatabaseURL = "";

	/// <summary>
	/// アセットバンドル情報リスト
	/// </summary>
	private List<AssetBundleInfo> m_AssetBundleInfo;

	/// <summary>
	/// 読み込み後アセットバンドルリスト
	/// </summary>
	private Dictionary<string,LoadedAsset> m_LoadedAssetList = new Dictionary<string, LoadedAsset> ();

	/// <summary>
	/// 読み込み中アセットバンドルリスト
	/// </summary>
	private List<LoadingAsset> m_LoadingAssetList = new List<LoadingAsset> ();

	/// <summary>
	/// ダウンロード中カウント
	/// </summary>
	private int m_DownloadingCount = 0;

	/// <summary>
	/// 準備完了フラグ
	/// </summary>
	public bool IsReady {
		get { return m_AssetBundleInfo != null && Caching.ready; }
	}

	/// <summary>
	/// アンロードチェックフラグ
	/// </summary>
	private bool m_IsUnload;

	#region パス設定項目

	#if UNITY_EDITOR
	public string GetDatabaseURL (BuildTarget target)
	{

		string url = m_SeverUrl;

		switch (target) {
		case BuildTarget.Android:
			return url + "Android/";
		case BuildTarget.iOS:
			return url + "iOS/";
		case BuildTarget.WebPlayer:
			return url + "WebPlayer/";
		case BuildTarget.StandaloneWindows:
		case BuildTarget.StandaloneWindows64:
			return url + "Windows/";
		case BuildTarget.StandaloneOSXIntel:
		case BuildTarget.StandaloneOSXIntel64:
		case BuildTarget.StandaloneOSXUniversal:
			return url + "OSX/";
		case BuildTarget.WebGL:
			return url + "WebGL/";
		default:
			return null;
		}
	}

	public static string GetBuildTargetName (BuildTarget target)
	{
		switch (target) {
		case BuildTarget.Android:
			return "Android";
		case BuildTarget.iOS:
			return "iOS";
		case BuildTarget.WebPlayer:
			return "WebPlayer";
		case BuildTarget.StandaloneWindows:
		case BuildTarget.StandaloneWindows64:
			return "Windows";
		case BuildTarget.StandaloneOSXIntel:
		case BuildTarget.StandaloneOSXIntel64:
		case BuildTarget.StandaloneOSXUniversal:
			return "OSX";
		case BuildTarget.WebGL:
			return "WebGL";
		default:
			return null;
		}
	}
	#endif

	private string GetDatabaseURL (RuntimePlatform platform)
	{
		string url = m_SeverUrl;

		switch (platform) {
		case RuntimePlatform.Android:
			return url + "Android/";
		case RuntimePlatform.IPhonePlayer:
			return url + "iOS/";
		case RuntimePlatform.WindowsWebPlayer:
		case RuntimePlatform.OSXWebPlayer:
			return url + "WebPlayer/";
		case RuntimePlatform.WindowsPlayer:
			return url + "Windows/";
		case RuntimePlatform.OSXPlayer:
			return url + "OSX/";
		case RuntimePlatform.WebGLPlayer:
			return url + "WebGL/";
		default:
			return null;
		}
	}

	private static string GetBuildTargetName (RuntimePlatform platform)
	{
		switch (platform) {
		case RuntimePlatform.Android:
			return "Android";
		case RuntimePlatform.IPhonePlayer:
			return "iOS";
		case RuntimePlatform.WindowsWebPlayer:
		case RuntimePlatform.OSXWebPlayer:
			return "WebPlayer";
		case RuntimePlatform.WindowsPlayer:
			return "Windows";
		case RuntimePlatform.OSXPlayer:
			return "OSX";
		case RuntimePlatform.WebGLPlayer:
			return "WebGL";
		default:
			return null;
		}
	}

	#endregion

	/// <summary>
	/// 読み込み済みアセットパスリスト取得
	/// </summary>
	public string[] GetLoadedAssetPath ()
	{
		return m_LoadedAssetList.Select ((x) => x.Key).ToArray ();
	}

	/// <summary>
	/// アセット情報リスト取得
	/// </summary>
	/// <returns>The asset info list.</returns>
	public List<AssetBundleInfo> GetAssetInfoList ()
	{
		return m_AssetBundleInfo;
	}

	private void Awake ()
	{
		// シーン遷移時破棄しない
		DontDestroyOnLoad (this.gameObject);

		// データベースURL取得
		#if UNITY_EDITOR
		m_DatabaseURL = GetDatabaseURL (EditorUserBuildSettings.activeBuildTarget);
		#else 
			m_DatabaseURL = GetDatabaseURL (Application.platform);
		#endif

		// アセットバンドルテキスト読み込み
		StartCoroutine (LoadAssetBundleTextCol ());

		// キャッシュ領域制限
		Caching.maximumAvailableDiskSpace = kMAX_CACHE;
	}

	/// <summary>
	/// アセットバンドルテキストコルーチン
	/// </summary>
	private IEnumerator LoadAssetBundleTextCol (int error = 0)
	{
		string path = m_DatabaseURL + kASSET_BUNDLE_TEXT_NAME + "?t=" + DateTime.Now.Ticks;

		bool is_complete = true;

		#if UNITY_EDITOR
		// デバッグログ
		if (m_IsDebugLog) {
			Debug.Log ("AssetBundleTextURL:" + path);
		}
		#endif

		// テキストダウンロード
		using (var www = new WWW (path)) {
			// ダウンロード待機
			yield return www;

			// エラーチェック
			if (www.error != null) {
				// TODO : とりあえず３回リトライ
				error++;
				if (error > 3) {
					Debug.LogWarning ("アセットバンドルテキストダウンロード失敗：リトライ処理");

					StartCoroutine (LoadAssetBundleTextCol ());
				} else {
					Debug.LogWarning ("アセットバンドルテキストダウンロード失敗：" + error + "回目");
				}

				is_complete = false;
			} else {
				// テキスト取得
				string text = www.text;

				// アセット情報リスト生成
				m_AssetBundleInfo = new List<AssetBundleInfo> ();

				// アセット毎に分ける
				string[] assets = text.Split ('\n');

				foreach (var asset in assets) {
					if (string.IsNullOrEmpty (asset)) {
						continue;
					}

					// アセット名毎に分ける
					string[] names = asset.Split (',');

					// アセット情報生成
					var info = new AssetBundleInfo ();

					// タグ名設定
					info.tagName = names [0];

					// アセットバンドル名設定
					info.assetBundleName = names [1];

					// ハッシュ設定
					info.hash = Hash128.Parse (names [2]);

					// 依存アセットバンドル列挙
					for (int i = 3; i < names.Length; ++i) {
						if (string.IsNullOrEmpty (names [i])) {
							continue;
						}

						// アセット名設定
						info.dependenciesNameList.Add (names [i]);
					}

					// アセット情報リストに追加
					m_AssetBundleInfo.Add (info);
				}
			}
		}

		if (is_complete) {
			StartCoroutine (ExcuteDownloadCol ());
		} else {
			yield return new WaitForSeconds (error);
			StartCoroutine (LoadAssetBundleTextCol (error));
		}
	}

	/// <summary>
	/// アセットバンドルオブジェクト読み込み　※読み込まれたアセットバンドルは即アンロードされます
	/// Editor:m_IsLocalLoadがtrueの場合、ローカルから直接ロードを行います
	/// </summary>
	/// <param name="path">アセットパス（アセットバンドルパス）</param>
	/// <param name="argument">コールバック引数</param>
	/// <param name="action">コールバック</param>
	/// <typeparam name="T">設定した引数</typeparam>
	/// <typeparam name="T2">読み込んだアセット</typeparam>
	public void LoadAssetBundleObject<T1,T2> (string path, T2 argument, UnityAction<T1,T2> action) where T1 : UnityEngine.Object
	{
		LoadAssetBundleObject<T1,T2> (path, Path.GetFileName (path), argument, action);
	}

	/// <summary>
	/// アセットバンドルオブジェクト読み込み　※読み込まれたアセットバンドルは即アンロードされます
	/// Editor:m_IsLocalLoadがtrueの場合、ローカルから直接ロードを行います
	/// </summary>
	/// <param name="path">アセットバンドルパス</param>
	/// <param name="asset_name">アセット名</param>
	/// <param name="argument">コールバック引数</param>
	/// <param name="action">コールバック</param>
	/// <typeparam name="T">設定した引数</typeparam>
	/// <typeparam name="T2">読み込んだアセット</typeparam>
	public void LoadAssetBundleObject<T1,T2> (string path, string asset_name, T2 argument, UnityAction<T1,T2> action) where T1 : UnityEngine.Object
	{
		// アセット読み込み
		LoadAssetBundleObject<T1> (path, asset_name, (x) => {
			if (action != null) {
				action (x, argument);
			}
		});
	}

	/// <summary>
	/// アセットバンドルオブジェクト読み込み　※読み込まれたアセットバンドルは即アンロードされます
	/// Editor:m_IsLocalLoadがtrueの場合、ローカルから直接ロードを行います
	/// </summary>
	/// <param name="path">アセットバンドルパス</param>
	/// <param name="action">コールバック</param>
	/// <typeparam name="T">読み込んだアセット</typeparam>
	public void LoadAssetBundleObject<T> (string path, UnityAction<T> action) where T : UnityEngine.Object
	{
		LoadAssetBundleObject<T> (path, Path.GetFileName (path), action);
	}

	/// <summary>
	/// アセットバンドルオブジェクト読み込み　※読み込まれたアセットバンドルは即アンロードされます
	/// Editor:m_IsLocalLoadがtrueの場合、ローカルから直接ロードを行います
	/// </summary>
	/// <param name="path">アアセットバンドルパス</param>
	/// <param name="asset_name">アセット名</param>
	/// <param name="action">コールバック</param>
	/// <typeparam name="T">読み込んだアセット</typeparam>
	public void LoadAssetBundleObject<T> (string path, string asset_name, UnityAction<T> action) where T : UnityEngine.Object
	{
		// アセットバンドル読み込み
		LoadAssetBundle (path, (x) => {
			// 存在チェック
			if (x == null) {
				Debug.LogWarning ("アセットバンドルの読み込みに失敗しました：" + path);

				if (action != null) {
					action (null);
				}

				return;
			}

			// アセット読み込み
			StartCoroutine (LoadAssetAsync <T> (x, asset_name, action));
		});
	}

	/// <summary>
	/// アセットバンドルからアセットを非同期読み込み
	/// </summary>
	/// <param name="asset_bundle">アセットバンドル</param>
	/// <param name="asset_name">アセット名</param>
	/// <param name="action">アクション</param>
	public IEnumerator LoadAssetAsync<T> (AssetBundle asset_bundle, string asset_name, UnityAction<T> action, bool is_unload = true) where T : UnityEngine.Object
	{
		// 読み込みリクエスト生成
		var request = asset_bundle.LoadAssetAsync <T> (asset_name);

		// 読み込み待機
		yield return request;

		// 読み込んだアセットを変換
		var obj = request.asset as T;

		// アクションチェック
		if (action != null) {
			action (obj);
		}

		// アンロードチェック
		if (is_unload) {
			UnloadAssetBundle (asset_bundle);
		}
	}

	/// <summary>
	/// タグ毎のアセットバンドルパス取得
	/// </summary>
	/// <returns>The asset bundles each tag.</returns>
	/// <param name="tag">Tag.</param>
	public string[] GetAssetBundlePathsEachTag (string tag)
	{
		var infos = GetAssetBundleInfosEachTag (tag);

		if (infos != null) {
			return infos.Select ((x) => x.assetBundleName).ToArray ();
		}

		return null;
	}

	/// <summary>
	/// タグ毎のアセットバンドル情報取得
	/// </summary>
	/// <returns>The asset bundles each tag.</returns>
	/// <param name="tag">Tag.</param>
	public AssetBundleInfo[] GetAssetBundleInfosEachTag (string tag)
	{
		if (m_AssetBundleInfo != null) {
			return m_AssetBundleInfo.Where ((x) => x.tagName == tag).ToArray ();
		}

		return null;
	}

	/// <summary>
	/// アセットバンドル情報取得
	/// </summary>
	/// <returns>The asset budnle info.</returns>
	/// <param name="path">Path.</param>
	public AssetBundleInfo GetAssetBudnleInfo (string path)
	{
		if (m_AssetBundleInfo != null) {
			return m_AssetBundleInfo.FirstOrDefault ((x) => x.assetBundleName == path);
		}

		return null;
	}

	/// <summary>
	/// アセットバンドルをストレージにキャッシュ
	/// </summary>
	/// <param name="path">アセットバンドルパス（アセットパスでも可）</param>
	public void CacheAssetBundle (string path, UnityAction action = null)
	{
		LoadAssetBundle (path, (x) => {
			UnloadAssetBundle (x);

			if (action != null) {
				action ();
			}
		});
	}

	/// <summary>
	/// ロードアセットバンドル ※必ず「UnloadAssetBundle( AssetBundle ) 」関数でアンロードを行ってください
	/// </summary>
	/// <param name="path">アセットバンドルパス（アセットパスでも可）.</param>
	/// <param name="argument">コールバック引数</param>
	/// <param name="action">コールバック</param>
	/// <typeparam name="T">コールバック引数型</typeparam>
	public void LoadAssetBundle<T> (string path, T argument, UnityAction<AssetBundle,T> action)
	{
		LoadAssetBundle (path, (x) => {
			if (action != null) {
				action (x, argument);
			}
		});
	}

	/// <summary>
	/// ロードアセットバンドル ※必ず「UnloadAssetBundle( AssetBundle ) 」関数でアンロードを行ってください
	/// </summary>
	/// <param name="path">アセットバンドルパス（アセットパスでも可）</param>
	/// <param name="action">読み込みコールバック</param>
	public void LoadAssetBundle (string path, UnityAction<AssetBundle> action)
	{
		if (!IsReady) {
			StartCoroutine (WaitReady (() => {
				LoadAssetBundle (path, action);
			}));

			return;
		}

		// リソースパスからアセット名取得
		string asset_name = ResourcePathToAssetBundleName (path);

		// 依存アセットバンドル読み込み開始
		LoadDependents (asset_name, action);
	}

	/// <summary>
	/// 依存バンドル読み込みコルーチン
	/// </summary>
	/// <param name="path">読み込みたいアセットバンドル名</param>
	/// <param name="action">コールバック</param>
	private void LoadDependents (string path, UnityAction<AssetBundle> action)
	{
		if (action == null) {
			return;
		}

		// 依存アセットバンドル名配列取得
		var asset_bundle_info = m_AssetBundleInfo.FirstOrDefault ((x) => x.assetBundleName == path);

		int count = 0;

		if (asset_bundle_info != null) {
			count = asset_bundle_info.dependenciesNameList.Count;
		}

		if (count <= 0) {
			DownloadAssetBundle (path, action);
		} else {
			// 依存アセットバンドルパス毎に処理
			foreach (var dependent in asset_bundle_info.dependenciesNameList) {
				// アセットバンドル読み込み
				DownloadAssetBundle (dependent, (x) => {
					// 読み込み残数カウンタ減少
					count--; 

					// 依存アセットバンドルが読み込み終わったかチェック
					if (count <= 0) {
						DownloadAssetBundle (path, action);
					}
				});
			}
		}
	}

	/// <summary>
	/// ダウンロードアセットバンドル
	/// </summary>
	/// <param name="path">アセットバンドル名</param>
	/// <param name="action">コールバック</param>
	private void DownloadAssetBundle (string path, UnityAction<AssetBundle> action)
	{
		// すでに読み込み済みか検索
		var loaded = m_LoadedAssetList.FirstOrDefault ((x) => x.Key == path);

		// 読み込み済みかチェック
		if (loaded.Value != null) {
			// アクションコールバックチェック
			if (action != null) {
				// 参照カウンタ増加
				loaded.Value.referencedCount++;

				// コールバック呼び出し
				action (loaded.Value.assetBundle);
			}

			return;
		}

		// 読み込み中か検索
		var loading = m_LoadingAssetList.FirstOrDefault ((x) => x.path == path);

		// 読み込み中かチェック
		if (loading != null) {
			// 読み込み完了イベントに追加
			loading.AddAction ((x) => {
				if (action != null)
					action (x);
			});

			return;
		} else {
			loading = new LoadingAsset ();
			// 読み込み完了イベント設定
			loading.AddAction ((x) => {
				if (action != null)
					action (x);
			});
		}

		var info = m_AssetBundleInfo.FirstOrDefault ((x) => x.assetBundleName.Equals (path));

		if (info == null) {
			Debug.LogWarning ("エラー：アセットバンドルテキストに存在しないパスです\nPATH:" + path);

			if (loading != null) {
				loading.InvokeAction (null);
			}

			return;
		}

		// ハッシュ値保存
		loading.hash = info.hash;

		// アセットバンドルパス保存
		loading.path = path;

		// 読み込み中リストに追加
		m_LoadingAssetList.Add (loading);
	}

	/// <summary>
	/// ダウンロード実行コルーチン
	/// </summary>
	/// <returns>The download col.</returns>
	private IEnumerator ExcuteDownloadCol ()
	{
		// 待機フラグ
		bool is_wait = false;

		// ダウンロード実行ループ
		while (true) {
			while (m_LoadingAssetList.Count - m_DownloadingCount > 0 && // 読み込み対象があるか
			       is_wait == false && // 待機中ではないか
			       m_DownloadingCount < kMAX_DONWLOADING_NUM) { // 読み込み中数は上限でないか
				StartCoroutine (DownloadAssetBundleCol (m_LoadingAssetList.FirstOrDefault (x => x.isLoading == false), (x, y) => {
					is_wait = true;

					y.errorCount = 1;
					is_wait = false;
				}));

				yield return null;
			}

			yield return null;
		}
	}

	/// <summary>
	/// ダウンロードアセットバンドル
	/// </summary>
	/// <returns>The asset bundle.</returns>
	/// <param name="path">Path.</param>
	private IEnumerator DownloadAssetBundleCol (LoadingAsset loading, UnityAction<string,LoadingAsset> error_action)
	{
		// 読み込み中数加算
		m_DownloadingCount++;
		loading.isLoading = true;

		// 準備チェック
		if (!IsReady) {
			Debug.LogWarning ("アセットバンドルダウンロードの準備が整っていません");
			yield return null;

			// 読み込み中数減算
			m_DownloadingCount--;
			loading.isLoading = false;
			yield break;
		}

		// パスをURLに変換
		string url = PathToURL (loading.path);

		// リストから削除フラグ
		bool is_remove = true;

		// アセットバンドルダウンロード
		using (var download = WWW.LoadFromCacheOrDownload (url, loading.hash, 0)) {
			// アセットバンドルダウンロード待機
			yield return download;

			#if UNITY_EDITOR
			// デバッグログ
			if (m_IsDebugLog) {
				Debug.Log ("AssetBundleDownloadURL:" + url);
			}
			#endif

			// エラーチェック
			if (!string.IsNullOrEmpty (download.error)) {

				// とりあえず３回リトライ
				loading.errorCount++;

				// リストから削除フラグオフ
				is_remove = false;

				// エラーカウントチェック
				if (loading.errorCount > 3) {
					Debug.LogWarning ("アセットバンドルダウンロードエラー:リトライ処理\nURL:" + url);

					// 読み込み中数減算
					m_DownloadingCount--;
					loading.isLoading = false;

					// エラーアクション
					error_action (download.error, loading);
					yield break;
				} else {
					Debug.LogWarning ("アセットバンドルダウンロードエラー:" + download.error + "\nURL:" + url);
				}
			}
                // アセットバンドル存在チェック
                else if (download.assetBundle != null) {
				// アクション数チェック
				if (loading.assetBundleEvent != null) {
					// 読み込み済みアセットバンドルクラス生成
					LoadedAsset loaded_asset = new LoadedAsset (download.assetBundle);

					// 参照数設定
					loaded_asset.referencedCount = loading.referencedCount;

					// リストに追加
					m_LoadedAssetList.Add (loading.path, loaded_asset);

					// アクション実行
					loading.InvokeAction (download.assetBundle);
				} else {
					// 参照を受け取る方法がないはずなのでアンロード
					UnloadAssetBundle (download.assetBundle);
				}
			} else {
				Debug.LogWarning ("通信に成功しましたがアセットバンドルがありませんでした：" + url);
				if (loading != null) {
					loading.InvokeAction (null);
				}
			}
		}

		if (is_remove) {
			yield return null;
			// 読み込み中リストからアンロード
			m_LoadingAssetList.Remove (loading); 
			loading = null;
			m_DownloadingCount--;
		} else {
			// 数秒待機
			yield return new WaitForSeconds (loading.errorCount);
			m_DownloadingCount--;
			loading.isLoading = false;
		}
	}

	/// <summary>
	/// アセットバンドルパスからURLへ
	/// </summary>
	public string PathToURL (string path)
	{
		return m_DatabaseURL + path;
	}

	/// <summary>
	/// アセットバンドルアンロード
	/// </summary>
	/// <param name="asset">アンロードするアセットバンドル</param>
	public void UnloadAssetBundle (AssetBundle asset)
	{
		if (asset == null) {
			return;
		}
			
		// 読み込み後アセットバンドルリスト検索
		foreach (var loaded_asset in m_LoadedAssetList) {
			// 対象アセットバンドルかチェック
			if (loaded_asset.Value.Check (asset)) {
				// 対象アセットアンロード
				UnloadCheck (loaded_asset.Key);

				var info = m_AssetBundleInfo.FirstOrDefault ((x) => x.assetBundleName == loaded_asset.Key);

				// 依存アセットバンドル毎に処理
				foreach (var dependen in info.dependenciesNameList) {
					UnloadCheck (dependen);
				}

				return;
			}
		}

		if (asset != null) {
			asset.Unload (false);
		}
	}

	/// <summary>
	/// アンロードチェック
	/// </summary>
	private void UnloadCheck (string bundle_name)
	{
		// リストから取り出し
		var loaded_dependen = m_LoadedAssetList.FirstOrDefault ((x) => x.Key == bundle_name);

		// nullチェック
		if (loaded_dependen.Value != null) {
			// 参照カウンタ減少
			loaded_dependen.Value.referencedCount--;

			m_IsUnload = true;
		}
	}

	private void LateUpdate ()
	{
		if (m_IsUnload) {
			m_IsUnload = false;

			var key_list = new List<string> (m_LoadedAssetList.Keys);

			foreach (var key in key_list) {

				var info = m_LoadedAssetList [key];

				// 参照カウンタチェック
				if (info.referencedCount <= 0) {
					// リストから削除
					m_LoadedAssetList.Remove (key);

					// アセットバンドルをアンロード
					info.assetBundle.Unload (false);

					#if UNITY_EDITOR
					// デバッグログ
					if (m_IsDebugLog) {
						Debug.Log ("UnLoadAsset:" + key);
					}
					#endif
				}
			}
		}
	}

	/// <summary>
	/// すべてのアセットバンドルをアンロード
	/// </summary>
	public void AllUnloadAssetBundle ()
	{
		foreach (var loaded in m_LoadedAssetList) {
			loaded.Value.assetBundle.Unload (false);
		}

		m_LoadedAssetList.Clear ();
	}

	/// <summary>
	/// リソースパスからアセットバンドル名へ
	/// 引数がアセットバンドルパスの場合はそのまま返却
	/// </summary>
	/// <returns>アセットバンドル名</returns>
	/// <param name="path">リソースパス（アセットバンドルパスでも可）</param>
	private string ResourcePathToAssetBundleName (string path)
	{
		return path.ToLower ();
	}

	/// <summary>
	/// 準備完了待機
	/// </summary>
	/// <param name="action">準備完了後コールバック</param>
	public IEnumerator WaitReady (UnityAction action)
	{
		while (!IsReady) {
			yield return null;
		}

		if (action != null) {
			action ();
		}
	}

	void OnDestroy ()
	{
		m_IsDestroy = true;
	}
}

/// <summary>
/// 読み込み後アセットバンドルクラス
/// </summary>
public class LoadedAsset
{
	/// <summary>
	/// アセットバンドル
	/// </summary>
	public AssetBundle assetBundle;

	/// <summary>
	/// 参照カウンタ
	/// </summary>
	public int referencedCount;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="asset">Asset.</param>
	public LoadedAsset (AssetBundle asset)
	{
		assetBundle = asset;
		referencedCount = 0;
	}

	/// <summary>
	/// 同じアセットバンドルかチェック
	/// </summary>
	/// <param name="asset">アセットバンドル</param>
	public bool Check (AssetBundle asset)
	{
		return assetBundle == asset;
	}
}

/// <summary>
/// 読み込み中アセットバンドル
/// </summary>
public class LoadingAsset
{
	/// <summary>
	/// アセットバンドルイベント
	/// </summary>
	public AssetBundleEvent assetBundleEvent = new AssetBundleEvent ();

	/// <summary>
	/// 参照カウンタ
	/// </summary>
	public int referencedCount { get; private set; }

	/// <summary>
	/// ハッシュ値
	/// </summary>
	public Hash128 hash;

	/// <summary>
	/// アセットバンドルパス
	/// </summary>
	public string path;

	/// <summary>
	/// エラーカウンタ
	/// </summary>
	public int errorCount;

	/// <summary>
	/// 読み込み中フラグ
	/// </summary>
	public bool isLoading = false;

	/// <summary>
	/// アクション追加
	/// </summary>
	/// <param name="action">Action.</param>
	public void AddAction (UnityAction<AssetBundle> action)
	{
		assetBundleEvent.AddListener (action);
		referencedCount++;
	}

	/// <summary>
	/// アクション実行
	/// </summary>
	/// <param name="bundle">バンドル</param>
	public void InvokeAction (AssetBundle bundle)
	{
		assetBundleEvent.Invoke (bundle);
	}
}

/// <summary>
/// アセット情報
/// </summary>
public class AssetBundleInfo
{
	/// <summary>
	/// アセットバンドル名
	/// </summary>
	public string assetBundleName;

	/// <summary>
	/// ハッシュ値
	/// </summary>
	public Hash128 hash;

	/// <summary>
	/// アセットバンドル名
	/// </summary>
	public List<string> dependenciesNameList = new List<string> ();

	/// <summary>
	/// タグ名
	/// </summary>
	public string tagName;
}

/// <summary>
/// アセットバンドルイベント
/// </summary>
public class AssetBundleEvent : UnityEvent<AssetBundle>
{
}
