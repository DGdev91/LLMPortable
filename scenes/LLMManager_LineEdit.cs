using Godot;
using System;
using System.Threading.Tasks;

public partial class LLMManager_LineEdit : LineEdit
{
	float original_Y;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		original_Y = Position.Y;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (DisplayServer.HasFeature(DisplayServer.Feature.VirtualKeyboard))
		{
			if (DisplayServer.VirtualKeyboardGetHeight() > 0)
			{
				SetPosition(new Vector2(Position.X, original_Y - (Size.Y / 2) - (DisplayServer.VirtualKeyboardGetHeight() / 2)));
			}
			else
			{
				SetPosition(new Vector2(Position.X, original_Y));
			}
		}
	}
}
