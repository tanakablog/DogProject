using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ScalableCamera : MonoBehaviour
{
	public static int manualWidth = 640;
	public static int manualHeight = 960;

	Camera ex_camera;

	//public GUISkin gSkin;

	void
	Awake()
	{
		ex_camera = GetComponent<Camera>();
		ex_camera.backgroundColor = Color.black;
	}

	void Start()
	{

		// gameObject.transform.position = new Vector3(manualWidth / 2, manualHeight / 2, -2500);
		gameObject.transform.localScale = new Vector3(1, 1, 1);

		ex_camera.orthographic = true;
		ex_camera.nearClipPlane = 0.3f;
		ex_camera.farClipPlane = 5000.0f;

		if ((float)Screen.width / (float)Screen.height < (float)manualWidth / (float)manualHeight)
		{
			//横に突っ張る
			ex_camera.orthographicSize = manualWidth / 2.0f * (float)Screen.height / (float)Screen.width;
		}
		else
		{
			//縦に突っ張る
			ex_camera.orthographicSize = manualHeight / 2.0f;
		}
	}
	void Update()
	{
	}

	public void ScalableOnGui()
	{

	//	if (gSkin)
	//	{
	//		GUI.skin = gSkin;
	//	}

		GUI.matrix = Matrix4x4.TRS(
			Vector2.zero,
			Quaternion.identity,
			//new Vector3((float)Screen.width/640f,(float)Screen.height/960f,1)
			new Vector3((float)Screen.width / (float)manualWidth, (float)Screen.height / (float)manualHeight, 1)
			//new Vector3((float)Screen.width/(float)manualWidth,1,1)
			);
	}

	// スケールに合わせたRectを取得
	public static Rect
	ScalableRect(
		float left,
		float top,
		float width,
		float height)
	{
		return new Rect(left,
			(top + ((float)manualWidth * Screen.height / Screen.width - (float)manualHeight) / 2f)
				* (float)manualHeight / ((float)manualWidth * Screen.height / Screen.width)
			, width, height);
	}

}
