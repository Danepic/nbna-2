using Godot;
using System;
using System.Collections.Generic;
using static Godot.FileAccess;


public partial class ObjDataReader : Node
{
	[Export(PropertyHint.File)]
	private string dataFile;

	[Export(PropertyHint.Dir)]
	private string spritePath;
	private Texture2D[] sprites;

	public override void _EnterTree()
	{
		using DirAccess dir = DirAccess.Open(spritePath);

		List<Texture2D> texture2DList = new();
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "")
			{
				if (!dir.CurrentIsDir() && fileName.EndsWith(".png"))
				{
					Texture2D texture2D = GD.Load<Texture2D>(spritePath + "/" + fileName);
					texture2DList.Add(texture2D);
				}
				fileName = dir.GetNext();
			}
		}

		sprites = texture2DList.ToArray();

		foreach(Texture2D sprite in sprites) {
			GD.Print(sprite.ResourceName);
			GD.Print(sprite.GetHeight() + " - " + sprite.GetWidth());
		}
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
}
