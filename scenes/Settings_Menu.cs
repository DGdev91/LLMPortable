using Godot;
using System;

public partial class Settings_Menu : Control
{
	bool IsMenuShown { get; set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		IsMenuShown = false;
		SettingsManager.LLMPortableSettings settngs = GetNode<SettingsManager>("../SettingsManager").llm_portable_settings;
		GetNode<LineEdit>("Panel/LblModel/LnEditModel").Text = settngs.Model_Path;
		GetNode<TextEdit>("Panel/LblSystemPrompt/TxtSystemPrompt").Text = settngs.System_Prompt;
		GetNode<TextEdit>("Panel/LblStartText/TxtStartText").Text = settngs.Start_Text;
		GetNode<LineEdit>("Panel/LblStopWords/LnStopWords").Text = settngs.Stop_Words;
		GetNode<SpinBox>("Panel/LblGpuLayers/SBGpuLayers").Value = Convert.ToDouble(settngs.Gpu_Layers);
		GetNode<SpinBox>("Panel/LblContextSize/SBContextSize").Value = Convert.ToDouble(settngs.Context_Size);
		GetNode<SpinBox>("Panel/LblSeed/SBSeed").Value = Convert.ToDouble(settngs.Seed);
		GetNode<SpinBox>("Panel/LblThreads/SBThreads").Value = Convert.ToDouble(settngs.Threads);
		GetNode<SpinBox>("Panel/LblBatchThreads/SBBatchThreads").Value = Convert.ToDouble(settngs.Batch_Threads);
		if (OS.GetName().CompareTo("Windows") == 0 || OS.GetName().CompareTo("Linux") == 0)
		{
			GetNode<Label>("Panel/LblGpuLayers").Visible = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _on_material_button_button_up()
	{
		if (IsMenuShown == false)
		{
			AnimationPlayer ap = new AnimationPlayer();
			GetNode<AnimationPlayer>("AnimationPlayer").Play("slide_in");
			IsMenuShown = true;
			ShowBehindParent = true;
		}
		else
		{
			GetNode<AnimationPlayer>("AnimationPlayer").Play("slide_out");
			IsMenuShown = false;
			ShowBehindParent = false;
		}
	}

	private void _on_save_button_pressed()
	{
        SettingsManager.LLMPortableSettings settngs = new SettingsManager.LLMPortableSettings
        {
            Model_Path = GetNode<LineEdit>("Panel/LblModel/LnEditModel").Text,
            System_Prompt = GetNode<TextEdit>("Panel/LblSystemPrompt/TxtSystemPrompt").Text,
            Start_Text = GetNode<TextEdit>("Panel/LblStartText/TxtStartText").Text,
            Stop_Words = GetNode<LineEdit>("Panel/LblStopWords/LnStopWords").Text,
            Gpu_Layers = Convert.ToInt64(GetNode<SpinBox>("Panel/LblGpuLayers/SBGpuLayers").Value),
            Context_Size = Convert.ToInt64(GetNode<SpinBox>("Panel/LblContextSize/SBContextSize").Value),
            Seed = Convert.ToInt64(GetNode<SpinBox>("Panel/LblSeed/SBSeed").Value),
            Threads = Convert.ToInt64(GetNode<SpinBox>("Panel/LblThreads/SBThreads").Value),
            Batch_Threads = Convert.ToInt64(GetNode<SpinBox>("Panel/LblBatchThreads/SBBatchThreads").Value)
        };
        string jsonSettings = Newtonsoft.Json.JsonConvert.SerializeObject(settngs);
		GetNode<SettingsManager>("../SettingsManager").SaveSettings(jsonSettings);
	}
}
