namespace Core.ResourcesCore
{
public static class ResourcesTable
{
	public enum eResourcePath
	{
		Core_Prefab_Resources_UI_MiniButton,
		Core_Prefab_Resources_UI_UIMain,
		Core_Prefab_Resources_UI_Window,
		Resources_suruyatu,
		Resources_Atlas_a_02_Game,
		Resources_Atlas_SampleAtlas,
		Resources_material_00_PrefabPool_black,
		Resources_material_00_PrefabPool_blue,
		Resources_material_00_PrefabPool_green,
		Resources_material_00_PrefabPool_red,
		Resources_material_00_PrefabPool_white,
		Resources_Sound_BGM_bgm_main_loop,
		Resources_Sound_BGM_bgm_world_intro,
		Resources_Sound_BGM_GameBgm,
		Resources_Sound_BGM_memories,
		Resources_Sound_BGM_thinking,
		Resources_Sound_SE_Bullet,
		Resources_Sound_SE_Cursor,
		Resources_Sound_SE_Explosion,
		Resources_Sound_SE_HallHit01,
		Resources_Sound_SE_HallHit02,
		Resources_Sound_SE_HallHit03,
		Resources_Sound_SE_HallHit04,
		Resources_Sound_SE_HallHit05,
		Resources_Sound_SE_HallHit06,
		Resources_Sound_SE_HallHit07,
		Resources_Sound_SE_HallOut,
		Resources_Sound_SE_kiran,
		Resources_Sound_SE_sasara_go,
		Resources_Sound_SE_sasara_ready,
		Resources_Sound_SE_sasara_title,
		Resources_Sound_SE_sasara_touch,
		Resources_Sound_SE_Select,
		Resources_Sound_SE_SYS_SE_01,
		Resources_Sound_SE_whistle,
	}

	public static string[] ResourcePathTable = new string[]
	{
		"UI/MiniButton",
		"UI/UIMain",
		"UI/Window",
		"suruyatu",
		"Atlas/a_02_Game",
		"Atlas/SampleAtlas",
		"material/00_PrefabPool/black",
		"material/00_PrefabPool/blue",
		"material/00_PrefabPool/green",
		"material/00_PrefabPool/red",
		"material/00_PrefabPool/white",
		"Sound/BGM/bgm_main_loop",
		"Sound/BGM/bgm_world_intro",
		"Sound/BGM/GameBgm",
		"Sound/BGM/memories",
		"Sound/BGM/thinking",
		"Sound/SE/Bullet",
		"Sound/SE/Cursor",
		"Sound/SE/Explosion",
		"Sound/SE/HallHit01",
		"Sound/SE/HallHit02",
		"Sound/SE/HallHit03",
		"Sound/SE/HallHit04",
		"Sound/SE/HallHit05",
		"Sound/SE/HallHit06",
		"Sound/SE/HallHit07",
		"Sound/SE/HallOut",
		"Sound/SE/kiran",
		"Sound/SE/sasara_go",
		"Sound/SE/sasara_ready",
		"Sound/SE/sasara_title",
		"Sound/SE/sasara_touch",
		"Sound/SE/Select",
		"Sound/SE/SYS_SE_01",
		"Sound/SE/whistle"
	};

	public static string GetResourcesPath(eResourcePath resource)
	{
		return ResourcePathTable[(int)resource];
	}
}
}
