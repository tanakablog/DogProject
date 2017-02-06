using UnityEngine;
using System.Collections;

namespace Core.Scene
{

	public class SceneChangeNone : SceneChangeBase
	{
		public override void Init()
		{
		}

		public override bool OutUpdate()
		{
			return true;
		}

		public override bool InUpdate()
		{
			return true;
		}

		public override void LoadingUpdate()
		{
		}
	}
}