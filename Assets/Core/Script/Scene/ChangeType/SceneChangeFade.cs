using UnityEngine;
using System.Collections;

namespace Core.Scene
{

	public class SceneChangeFade : SceneChangeBase
	{
		public UnityEngine.UI.Image image;
		float alphaFadeValue = 0.0f;

		public override void Init()
		{
			alphaFadeValue = 0.0f;

			image.color = new Color(0, 0, 0, alphaFadeValue);
		}

		public override bool OutUpdate()
		{
			var dim = Mathf.Clamp01(Time.deltaTime);

			alphaFadeValue += dim * 1f;

			if (alphaFadeValue > 1.0f)
			{
				alphaFadeValue = 1.0f;

				image.color = new Color(0, 0, 0, alphaFadeValue);
				return true;
			}

			image.color = new Color(0, 0, 0, alphaFadeValue);

			return false;
		}

		public override bool InUpdate()
		{
			var dim = Mathf.Clamp01(Time.deltaTime);

			alphaFadeValue -= dim * 1f;

			if (alphaFadeValue <= 0f)
			{
				alphaFadeValue = 0f;

				image.color = new Color(0, 0, 0, alphaFadeValue);
				return true;
			}

			image.color = new Color(0, 0, 0, alphaFadeValue);

			return false;
		}

		public override void LoadingUpdate()
		{

		}

	}
}