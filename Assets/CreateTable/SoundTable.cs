namespace Core.Sound
{

	/// <summary>
	/// サウンドテーブルクラス ※自動生成されるクラスです
	/// </summary>
	public static class SoundTable
	{

		/// <summary>
		/// サウンドタイプ
		/// </summary>
		public enum eType
		{
			BGM,
			SE,
			Voice
		}

		/// <summary>
		/// BGMタイプ
		/// </summary>
		public enum eBGM
		{

			NoSound = -1, // 音なし


		}

		/// <summary>
		/// SEタイプ
		/// </summary>
		public enum eSE
		{

			NoSound = -1, // 音なし


		}

		/// <summary>
		/// Voiceタイプ
		/// </summary>
		public enum eVoice
		{

			NoSound = -1, // 音なし


		}

		/// <summary>
		/// BGMパス
		/// </summary>
		public static readonly string[] BGM_PATH = {
			"NoSound(dummy)"
		};

		/// <summary>
		/// SEパス
		/// </summary>
		public static readonly string[] SE_PATH = {
			"NoSound(dummy)"
		};

		/// <summary>
		/// Voiceパス
		/// </summary>
		public static readonly string[] VOICE_PATH = {
			"NoSound(dummy)"
		};

	}
}
