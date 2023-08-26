using Godot;
using System;

public class DataReaderUtil
{
	public static string GetValueFromKey(string line, string keyLine, string key)
	{
		string[] separateContent = line.Replace(keyLine + ":", "").Split(" ");
		foreach (string content in separateContent)
		{
			if (content != "" && content.StartsWith(key))
			{
				return content.Replace(key + "=", "").Replace("\"", "");
			}
		}
		return null;
	}

		public static SpriteAtlasEntity GetSpriteAtlasFromKey(string line, string keyLine, string key)
	{
		SpriteAtlasEntity spriteAtlas = new();
		string dimension = GetValueFromKey(line, keyLine, key);
		if (dimension == null)
		{
			return null;
		}

		string[] values = dimension.Split("x");
		spriteAtlas.pixelSize = int.Parse(values[0]);
		spriteAtlas.quantityPerRowAndColumn = int.Parse(values[1]);
		spriteAtlas.spriteSize = int.Parse(values[2]);
		spriteAtlas.quantityOfSprites = int.Parse(values[3]);
		return spriteAtlas;
	}
}
