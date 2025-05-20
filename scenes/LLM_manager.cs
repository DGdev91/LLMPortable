using Godot;
using System;
using LLama.Common;
using LLama;
using System.Threading.Tasks;
using System.Xml.Xsl;
using System.Linq;

public partial class LLM_manager : Node
{
	string modelUserPath = "user://tinyllama-1.1b-chat-v1.0.Q4_K_M.gguf";
	
	private LLamaContext context;
	public ChatSession session { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void DownloadFile(String url, String savePath)
	{
		RichTextLabel llmMsgLabel = GetNode<RichTextLabel>("RichTextLabel");
		HttpRequest httpRequest = GetNode<HttpRequest>("HTTPRequest");
		httpRequest.DownloadFile = savePath;
		llmMsgLabel.AppendText("Downloading model...\n");
		Error error = httpRequest.Request(url);
		if (error != Error.Ok)
		{
			llmMsgLabel.AppendText("Error Downloading Model, please retry.\n");
		}
	}

	public void InitNewSession (string systemPrompt)
	{
		if (context is null)
		{
			RichTextLabel llmMsgLabel = GetNode<RichTextLabel>("RichTextLabel");
			SettingsManager.LLMPortableSettings settings = GetParent().GetNode<SettingsManager>("SettingsManager").llm_portable_settings;

			if (FileAccess.FileExists(modelUserPath))
			{
				try
				{
					// Load a model
					llmMsgLabel.AppendText("Loading Parameters...\n");
					//ModelParams parameters = new ModelParams(settings.Model_Path)
					ModelParams parameters = new(ProjectSettings.GlobalizePath(modelUserPath))
					{
						ContextSize = (uint)settings.Context_Size,
						//Seed = (uint)settings.Seed,
						Threads = (int)settings.Threads,
						BatchThreads = (int)settings.Batch_Threads
					};
					if (OS.GetName().CompareTo("Windows") == 0 || OS.GetName().CompareTo("Linux") == 0) 
					{
						parameters.GpuLayerCount = (int)settings.Gpu_Layers;
					}
					llmMsgLabel.AppendText("Loading Model..\n");
					using LLamaWeights model = LLamaWeights.LoadFromFile(parameters);

					// Initialize a chat session
					llmMsgLabel.AppendText("Initializing Context...\n");
					context = model.CreateContext(parameters);
					llmMsgLabel.AppendText("Session Ok\n");
				}
				catch (Exception ex)
				{
					llmMsgLabel.AppendText("ERROR: failed to initialize model - " + ex.Message);
				}
			}
			else
			{
				llmMsgLabel.AppendText("Error: Model not found!\n");
			}
		}

		InteractiveExecutor iex = new(context);
		session = new ChatSession(iex);
		session.AddSystemMessage(systemPrompt);
	}

	public async void ExecuteChat(string prompt, string[] antiPrompts)
	{
		RichTextLabel llmMsgLabel = GetNode<RichTextLabel>("RichTextLabel");
		llmMsgLabel.AppendText(prompt);

		await foreach (var text in session.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), new InferenceParams { AntiPrompts = antiPrompts.ToList() }))
		{
			llmMsgLabel.AppendText(text);
		}
		if (!llmMsgLabel.Text.EndsWith('\n')) llmMsgLabel.AppendText("\n");
	}

	private void _on_enter_button_pressed()
	{
		LineEdit lineEdit = GetNode<LineEdit>("LLMManager_LineEdit");
		SettingsManager.LLMPortableSettings settings = GetParent().GetNode<SettingsManager>("SettingsManager").llm_portable_settings;
		string prompt = lineEdit.Text + "\n";
		lineEdit.Clear();
		ExecuteChat(prompt, settings.Stop_Words.Split(','));
	}
	
	private void _on_start_timer_timeout()
	{
		if (!FileAccess.FileExists(modelUserPath))
		{
			GetNode<ConfirmationDialog>("ConfirmationDialog").Visible = true;
		}
		else
		{
			RichTextLabel llmMsgLabel = GetNode<RichTextLabel>("RichTextLabel");
			SettingsManager.LLMPortableSettings settings = GetParent().GetNode<SettingsManager>("SettingsManager").llm_portable_settings;
			
			llmMsgLabel.AppendText("Model Found, Loading...\n");

			InitNewSession(settings.System_Prompt);
			llmMsgLabel.AppendText(settings.Start_Text);
		}
	}

	private void _on_http_request_request_completed(long result, long response_code, string[] headers, byte[] body)
	{
		RichTextLabel llmMsgLabel = GetNode<RichTextLabel>("RichTextLabel");
		llmMsgLabel.AppendText("Model Downloaded\n");
		GetParent().GetNode<Timer>("StartTimer").Start();
	}

	private void _on_confirmation_dialog_confirmed ()
	{
		DownloadFile("https://huggingface.co/TheBloke/TinyLlama-1.1B-Chat-v1.0-GGUF/resolve/main/tinyllama-1.1b-chat-v1.0.Q4_K_M.gguf?download=true", 
				modelUserPath);
	}

	private void _on_confirmation_dialog_canceled ()
	{
		GetTree().Quit();
	}

}
