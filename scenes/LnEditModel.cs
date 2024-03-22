using Godot;
using System;

public partial class LnEditModel : LineEdit
{
	private void _on_model_file_dialog_file_selected(string path)
	{
		Text = path;
	}
}
