using Godot;
using System;

public partial class ModelFileDialog : FileDialog
{
	private void _on_btn_model_pressed()
	{
		if(OS.GetName().CompareTo("Android") == 0)
		{
			CurrentPath = "/storage/emulated/0/";
		}
		Visible = true;
	}
}
