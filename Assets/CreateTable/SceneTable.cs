namespace Core.Scene
{
public static class SceneTable
{
	public enum eScene
	{
		_PrefabPool,
		_Title,
		_Game,
		Sample,
		ExampleScene,
	}

	public static string[] ScenePathTable = new string[]
	{
		"Scene/00_PrefabPool.unity",
		"Scene/01_Title.unity",
		"Scene/02_Game.unity",
		"Scene/Sample.unity",
		"StoreAsset/CielaSpike/Thread Ninja/Example/ExampleScene.unity"
	};

	public static string GetSceneName(eScene scene)
	{
		return ScenePathTable[(int)scene];
	}
}
}
