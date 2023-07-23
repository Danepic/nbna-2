using Godot;
using System;
using System.Collections.Generic;
using static Godot.FileAccess;


public partial class ObjDataReader : Node
{
	[Export(PropertyHint.File)]
	private string dataFile;

	private ObjEntity objEntity;

	[Export(PropertyHint.Dir)]
	private string spritePath;
	private Texture2D[] sprites;

	public override void _EnterTree()
	{
		this.objEntity = new();
		this.objEntity.sprites = new();
		this.objEntity.header = new();
		this.objEntity.stats = new();

		using DirAccess spriteDir = DirAccess.Open(spritePath);

		if (spriteDir != null)
		{
			int numberOfSprite = 1;
			spriteDir.ListDirBegin();
			string fileName = spriteDir.GetNext();
			while (fileName != "")
			{
				if (!spriteDir.CurrentIsDir() && fileName.EndsWith(".png"))
				{
					SpriteAtlasEntity spriteAtlas = new();
					spriteAtlas.sprite = GD.Load<Texture2D>(spritePath + "/" + fileName);
					spriteAtlas.fileName = fileName;

					objEntity.sprites.Add(numberOfSprite, spriteAtlas);
				}
				fileName = spriteDir.GetNext();
			}
		}

		FileAccess dataFileAccess = Open(dataFile, ModeFlags.Read);
		string[] dataContentLines = dataFileAccess.GetAsText().Split("\n");

		foreach (string line in dataContentLines)
		{
			if (line.StartsWith("type"))
			{
				string type = this.GetValueFromKey(line, "type", "value");
				if (type != null)
					objEntity.type = Enum.Parse<ObjTypeEnum>(type);
			}

			if (line.StartsWith("header"))
			{
				string name = this.GetValueFromKey(line, "header", "name");
				if (name != null)
					objEntity.header.name = name;

				string startHp = this.GetValueFromKey(line, "header", "startHp");
				if (startHp != null)
					objEntity.header.startHp = int.Parse(startHp);

				string startMp = this.GetValueFromKey(line, "header", "startMp");
				if (startMp != null)
					objEntity.header.startMp = int.Parse(startMp);
			}

			if (line.StartsWith("stats"))
			{
				string aggressive = this.GetValueFromKey(line, "stats", "aggressive");
				if (aggressive != null)
					objEntity.stats.aggressive = int.Parse(aggressive);

				string technique = this.GetValueFromKey(line, "stats", "technique");
				if (technique != null)
					objEntity.stats.technique = int.Parse(technique);

				string intelligent = this.GetValueFromKey(line, "stats", "intelligent");
				if (intelligent != null)
					objEntity.stats.intelligent = int.Parse(intelligent);

				string speed = this.GetValueFromKey(line, "stats", "speed");
				if (speed != null)
					objEntity.stats.speed = int.Parse(speed);

				string resistance = this.GetValueFromKey(line, "stats", "resistance");
				if (resistance != null)
					objEntity.stats.resistance = int.Parse(resistance);

				string stamina = this.GetValueFromKey(line, "stats", "stamina");
				if (stamina != null)
					objEntity.stats.stamina = int.Parse(stamina);
			}

			if (line.StartsWith("sprite"))
			{
				SpriteDimensionEntity spriteDimension = this.GetSpriteDimensionFromKey(line, "sprite", "1");
				if (spriteDimension != null)
					objEntity.sprites[1].quantityOfSprites = (spriteDimension.pixelSize * spriteDimension.pixelSize) / (spriteDimension.pixelSize / spriteDimension.quantityPerRowAndColumn) * 256;

				string second = this.GetValueFromKey(line, "stats", "technique");
				if (technique != null)
					objEntity.stats.technique = int.Parse(technique);

				string third = this.GetValueFromKey(line, "stats", "intelligent");
				if (intelligent != null)
					objEntity.stats.intelligent = int.Parse(intelligent);

				string fourth = this.GetValueFromKey(line, "stats", "speed");
				if (speed != null)
					objEntity.stats.speed = int.Parse(speed);

				string fifth = this.GetValueFromKey(line, "stats", "resistance");
				if (resistance != null)
					objEntity.stats.resistance = int.Parse(resistance);

				string sixth = this.GetValueFromKey(line, "stats", "stamina");
				if (stamina != null)
					objEntity.stats.stamina = int.Parse(stamina);
			}
		}

		foreach (Texture2D sprite in sprites)
		{
			GD.Print(sprite.GetHeight() + " - " + sprite.GetWidth());
		}
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	private string GetValueFromKey(string line, string keyLine, string key)
	{
		string[] separateContent = line.Replace(keyLine + ":", "").Split(" ");
		foreach (string content in separateContent)
		{
			if (content != "")
			{
				return content.Replace(key + "=", "").Replace("\"", "");
			}
		}
		return null;
	}

	private SpriteDimensionEntity GetSpriteDimensionFromKey(string line, string keyLine, string key)
	{
		SpriteDimensionEntity spriteDimension = new();
		string dimension = this.GetValueFromKey(line, keyLine, key);
		GD.Print(dimension);
		return spriteDimension;
	}
}
