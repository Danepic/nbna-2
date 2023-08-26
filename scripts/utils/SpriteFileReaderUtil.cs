using Godot;
using System;
using System.Collections.Generic;


public class SpriteFileReaderUtil
{
	public static Dictionary<int, SpriteAtlasEntity> Get(string spritePath)
	{
		Dictionary<int, SpriteAtlasEntity> result = new ();

		using DirAccess spriteDir = DirAccess.Open(spritePath);
		if (spriteDir != null)
		{
			int numberOfSprite = 0;
			spriteDir.ListDirBegin();
			string fileName = spriteDir.GetNext();
			while (fileName != "")
			{
				if (!spriteDir.CurrentIsDir() && fileName.EndsWith(".png"))
				{
					SpriteAtlasEntity spriteAtlas = new();
					spriteAtlas.sprite = GD.Load<Texture2D>(spritePath + "/" + fileName);
					spriteAtlas.fileName = fileName;
					spriteAtlas.textureIndex = numberOfSprite;

					result.Add(numberOfSprite, spriteAtlas);
					numberOfSprite++;
				}
				fileName = spriteDir.GetNext();
			}
		}

		return result;
	}
}
