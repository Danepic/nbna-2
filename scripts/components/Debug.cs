using Godot;
using System;

public partial class Debug : Node
{

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("pause_break"))
		{
			GetTree().Paused = !GetTree().Paused;
		}
	}
}
