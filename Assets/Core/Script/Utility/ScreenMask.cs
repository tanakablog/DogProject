using UnityEngine;
using System.Collections;

public class ScreenMask : MonoBehaviour
{

	// このアプリの画面サイズ
	private const float APP_WIDTH = 640;
	private const float APP_HEIGHT = 960;

	// カメラの正投影視野(orthographicSize)の初期値
//	private float default_ortho_size;

	// 開始時のスクリーンサイズ
	private int screenW = -1;
	private int screenH = -1;

	// Use this for initialization
	void Start()
	{
//		default_ortho_size = Camera.main.orthographicSize;
	}

	// Update is called once per frame
	void Update()
	{
		// スクリーンサイズ変更時処理
		if ((screenW != Screen.width) || (screenH != Screen.height))
			adjustMask();

		// 2014.01.31 大久保
		// カメラの視野が変わった場合に、画面外マスクに逆のスケールをかけて補正
		// この処理はフレームの最後に行いたいので、このスクリプト実行順を変えてます
//		float ortho_scale = Camera.main.orthographicSize / default_ortho_size;
//		transform.localScale = Vector3.one * ortho_scale;
	}

	// 2014.06.20 大久保
	// マスクサイズの調整
	void
	adjustMask()
	{
		screenW = Screen.width;
		screenH = Screen.height;

		// 短辺を640とした場合の長辺の長さ
		float appH;
		if (screenW <= screenH)
			appH = (float)screenH * APP_WIDTH / (float)screenW;
		else
			appH = (float)screenW * APP_WIDTH / (float)screenH;

		// マスクで塞ぐべき高さ(+4 はマージン)
		float maskH = (appH - APP_HEIGHT) * 0.5f + 4;

		// マスクが小さければ拡大する
		SpriteRenderer mask = transform.Find("Mask").GetComponent<SpriteRenderer>();
		float originalH =mask.sprite.texture.height;

		float scaleY = maskH / originalH;
		if (scaleY < 1.0f) scaleY = 1.0f;
		Vector3 scale = Vector3.one * scaleY;
		foreach (Transform child in transform)
		{
			if (child.name == "Mask")
			{
				scale.y = (child.transform.localPosition.y < 0) ? -scaleY : scaleY;
				child.transform.localScale = scale;

				// 位置をずらす
				float pos = APP_HEIGHT / 2 + ((originalH * scaleY) / 2f);
				Vector3 nextpos = child.transform.localPosition;
				nextpos.y = (child.transform.localPosition.y < 0) ? -pos : pos;

				child.transform.localPosition = nextpos;

			}
		}
	}
}
