using Godot;
using System;

public partial class SBSeed : SpinBox
{
	private void _on_material_button_pressed()
	{
		Value = new Random().Next();
	}
}
