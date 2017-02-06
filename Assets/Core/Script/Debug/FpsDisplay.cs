using UnityEngine;
using System.Collections;
using System.Text;

[ExecuteInEditMode ()]
public class FpsDisplay : MonoBehaviour
{
	public Rect drawRect;

	/// <summary>
	/// インターバル
	/// </summary>
    const float INTERVAL = 0.5f;

	string drawText;

	/// <summary>
	/// Unity Start
	/// </summary>
    void Start()
    {
		StartCoroutine( DrawFps() );
    }

	/// <summary>
	/// FPS表示
	/// </summary>
    IEnumerator DrawFps()
    {
		// フレーム
		int frame = 0;

		// フレームレート
		float rate = 0.0f;

		// 時間
		float time = 0.0f;

		// 過去時間
		float old_time = 0.0f;

		// FPS表示ループ
		while( true )
		{
			// フレーム更新
			frame++;

			// タイム更新
			time += Time.deltaTime;

			// 時間チェック
			if( Time.realtimeSinceStartup - old_time >= INTERVAL )
			{
				// フレームレート算出
				rate = frame / time;

				// FPS表示 / メモリ表示
				drawText = rate.ToString( "0.0" ) + " fps, " + ( System.GC.GetTotalMemory( false ) / 1000000F ).ToString( "0.0" ) + " mem";

				// 過去時間時更新
				old_time = Time.realtimeSinceStartup;

				// フレーム初期化
				frame = 0;

				// タイム初期化
				time = 0.0f;
			}

			// 1フレーム待機
			yield return null;
		}
    }

	void OnGUI()
	{
		GUI.Label( drawRect ,drawText );
	}
}
