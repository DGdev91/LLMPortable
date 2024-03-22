using Godot;
using System;

public partial class SettingsManager : Node
{
	public partial class LLMPortableSettings
	{
		public string Model_Path { get; set; }
		public string System_Prompt { get; set; }
		public string Start_Text { get; set; }
		public string Stop_Words { get; set; }
		public long Gpu_Layers { get; set; }
		public long Context_Size { get; set; }
		public long Seed { get; set; }
		public long Threads { get; set; }
		public long Batch_Threads { get; set; }
	}

	public LLMPortableSettings llm_portable_settings { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LoadSettings();
		if (llm_portable_settings == null)
		{
			//Default Settings
            llm_portable_settings = new LLMPortableSettings
            {
				System_Prompt = "Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.\n",
				Start_Text = "User: Hello, Bob.\n" +
					"Bob: Hello. How may I help you today?\n" +
					"User: Please tell me the largest city in Europe.\n" +
					"Bob: Sure. The largest city in Europe is Moscow, the capital of Russia.\n" +
					"User:",
				Stop_Words = "User:,You:",
				Context_Size = 1024,
				Gpu_Layers = 33,
                Seed = (uint)new Random().Next(),
				Threads = (uint)System.Environment.ProcessorCount,
				Batch_Threads = (uint)System.Environment.ProcessorCount
            };
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void LoadSettings()
	{
		if (!FileAccess.FileExists("user://LLMSettings.json"))
		{
			return; // We don't have a save to load.
		}

		using (FileAccess saveGame = FileAccess.Open("user://LLMSettings.json", FileAccess.ModeFlags.Read))
		{
			string jsonString = saveGame.GetLine();
			try
			{
				llm_portable_settings = Newtonsoft.Json.JsonConvert.DeserializeObject<LLMPortableSettings>(jsonString);
			}
			catch (Exception ex)
			{
				GD.PushError("Error loading settings file: " + ex.Message);
			}


		}
	}

	public void SaveSettings(string jsonSettings)
	{
		using (FileAccess saveGame = FileAccess.Open("user://LLMSettings.json", FileAccess.ModeFlags.Write))
		{
			saveGame.StoreLine(jsonSettings);
		}
	}
}
