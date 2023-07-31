using Godot;
using System;
using System.Collections.Generic;
using static Godot.FileAccess;


public partial class ObjDataReader : Node
{
	[Export(PropertyHint.File)]
	private string dataFile;

	private ObjEntity objEntity;
	private FrameEntity currentFrame;

	[Export(PropertyHint.Dir)]
	private string spritePath;
	private Dictionary<int, Dictionary<int, Rect2>> spritePosition;

	private Sprite3D sprite3D;

	public override void _EnterTree()
	{
		this.objEntity = new();
		this.objEntity.sprites = new();
		this.objEntity.header = new();
		this.objEntity.stats = new();
		this.objEntity.frames = new();

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

					objEntity.sprites.Add(numberOfSprite, spriteAtlas);
					numberOfSprite++;
				}
				fileName = spriteDir.GetNext();
			}
		}

		GD.Print("1: " + objEntity.sprites[0].fileName);

		FileAccess dataFileAccess = Open(dataFile, ModeFlags.Read);
		if (dataFileAccess == null)
		{
			return;
		}
		string[] dataContentLines = dataFileAccess.GetAsText().Split("\n");

		FrameEntity currentFrameToMap = null;
		foreach (string line in dataContentLines)
		{
			string trimLine = line.Trim();
			if (trimLine.StartsWith("type"))
			{
				string type = this.GetValueFromKey(trimLine, "type", "value");
				if (type != null)
					objEntity.type = Enum.Parse<ObjTypeEnum>(type);

				continue;
			}

			if (trimLine.StartsWith("header"))
			{
				string name = this.GetValueFromKey(trimLine, "header", "name");
				if (name != null)
					objEntity.header.name = name;

				string startHp = this.GetValueFromKey(trimLine, "header", "startHp");
				if (startHp != null)
					objEntity.header.startHp = int.Parse(startHp);

				string startMp = this.GetValueFromKey(trimLine, "header", "startMp");
				if (startMp != null)
					objEntity.header.startMp = int.Parse(startMp);

				continue;
			}

			if (trimLine.StartsWith("stats"))
			{
				string aggressive = this.GetValueFromKey(trimLine, "stats", "aggressive");
				if (aggressive != null)
					objEntity.stats.aggressive = int.Parse(aggressive);

				string technique = this.GetValueFromKey(trimLine, "stats", "technique");
				if (technique != null)
					objEntity.stats.technique = int.Parse(technique);

				string intelligent = this.GetValueFromKey(trimLine, "stats", "intelligent");
				if (intelligent != null)
					objEntity.stats.intelligent = int.Parse(intelligent);

				string speed = this.GetValueFromKey(trimLine, "stats", "speed");
				if (speed != null)
					objEntity.stats.speed = int.Parse(speed);

				string resistance = this.GetValueFromKey(trimLine, "stats", "resistance");
				if (resistance != null)
					objEntity.stats.resistance = int.Parse(resistance);

				string stamina = this.GetValueFromKey(trimLine, "stats", "stamina");
				if (stamina != null)
					objEntity.stats.stamina = int.Parse(stamina);

				continue;
			}

			if (trimLine.StartsWith("sprite"))
			{
				for (int i = 0; i < objEntity.sprites.Count; i++)
				{
					SpriteAtlasEntity spriteAtlasEnriched = this.GetSpriteAtlasFromKey(trimLine, "sprite", (i + 1).ToString());

					if (spriteAtlasEnriched != null)
					{
						objEntity.sprites[i].textureIndex = i;
						objEntity.sprites[i].pixelSize = spriteAtlasEnriched.pixelSize;
						objEntity.sprites[i].quantityPerRowAndColumn = spriteAtlasEnriched.quantityPerRowAndColumn;
						objEntity.sprites[i].spriteSize = spriteAtlasEnriched.spriteSize;
						objEntity.sprites[i].quantityOfSprites = spriteAtlasEnriched.quantityOfSprites;
					}
				}
			}

			spritePosition = new();
			foreach (KeyValuePair<int, SpriteAtlasEntity> spriteAtlasMap in objEntity.sprites)
			{
				Dictionary<int, Rect2> position = new();
				int x = 0;
				int y = 0;
				int w = spriteAtlasMap.Value.spriteSize;
				int h = spriteAtlasMap.Value.spriteSize;

				int index = 0;
				for (int y_row = 0; y_row < spriteAtlasMap.Value.quantityPerRowAndColumn; y_row++)
				{
					for (int x_row = 0; x_row < spriteAtlasMap.Value.quantityPerRowAndColumn; x_row++)
					{
						Rect2 rect2 = new(new Vector2(x, y), new Vector2(w, h));
						position.Add(index, rect2);

						x += spriteAtlasMap.Value.spriteSize;
						index++;
					}

					x = 0;
					y += spriteAtlasMap.Value.spriteSize;
					index++;
				}

				spritePosition.Add(spriteAtlasMap.Value.textureIndex, position);
			}

			if (trimLine.StartsWith("frame"))
			{
				currentFrameToMap = new();

				string id = this.GetValueFromKey(trimLine, "frame", "id");
				if (id != null)
					currentFrameToMap.id = int.Parse(id);

				string name = this.GetValueFromKey(trimLine, "frame", "name");
				if (name != null)
					currentFrameToMap.name = name;

				string state = this.GetValueFromKey(trimLine, "frame", "state");
				if (state != null)
					Enum.TryParse<StateFrameEnum>(state, true, out currentFrameToMap.state);

				string wait = this.GetValueFromKey(trimLine, "frame", "wait");
				if (wait != null)
					currentFrameToMap.wait = float.Parse(wait);

				continue;
			}

			if (trimLine.StartsWith("img"))
			{
				string pic = this.GetValueFromKey(trimLine, "img", "pic");
				if (pic != null)
					currentFrameToMap.pic = int.Parse(pic);

				string x = this.GetValueFromKey(trimLine, "img", "x");
				if (x != null)
					currentFrameToMap.x_offset = float.Parse(x);

				string y = this.GetValueFromKey(trimLine, "img", "y");
				if (y != null)
					currentFrameToMap.y_offset = float.Parse(y);

				continue;
			}

			if (trimLine.StartsWith("next"))
			{
				string id = this.GetValueFromKey(trimLine, "next", "id");
				if (id != null)
					currentFrameToMap.next = int.Parse(id);
			}

			if (trimLine.StartsWith("movement"))
			{
				string dvx = this.GetValueFromKey(trimLine, "movement", "dvx");
				if (dvx != null)
					currentFrameToMap.dvx = float.Parse(dvx);

				string dvy = this.GetValueFromKey(trimLine, "movement", "dvy");
				if (dvy != null)
					currentFrameToMap.dvy = float.Parse(dvy);

				string dvz = this.GetValueFromKey(trimLine, "movement", "dvz");
				if (dvz != null)
					currentFrameToMap.dvz = float.Parse(dvz);

				continue;
			}

			if (trimLine.StartsWith("hit"))
			{
				string hitTaunt = this.GetValueFromKey(trimLine, "hit", "hit_taunt");
				if (hitTaunt != null)
					currentFrameToMap.hitTaunt = int.Parse(hitTaunt);

				string hitJump = this.GetValueFromKey(trimLine, "hit", "hitJump");
				if (hitJump != null)
					currentFrameToMap.hitJump = int.Parse(hitJump);

				string hitDefense = this.GetValueFromKey(trimLine, "hit", "hitDefense");
				if (hitDefense != null)
					currentFrameToMap.hitDefense = int.Parse(hitDefense);

				string hitAttack = this.GetValueFromKey(trimLine, "hit", "hitAttack");
				if (hitAttack != null)
					currentFrameToMap.hitAttack = int.Parse(hitAttack);

				string hitJumpDefense = this.GetValueFromKey(trimLine, "hit", "hitJumpDefense");
				if (hitJumpDefense != null)
					currentFrameToMap.hitJumpDefense = int.Parse(hitJumpDefense);

				string hitDefensePower = this.GetValueFromKey(trimLine, "hit", "hitDefensePower");
				if (hitDefensePower != null)
					currentFrameToMap.hitDefensePower = int.Parse(hitDefensePower);

				string hitDefenseAttack = this.GetValueFromKey(trimLine, "hit", "hitDefenseAttack");
				if (hitDefenseAttack != null)
					currentFrameToMap.hitDefenseAttack = int.Parse(hitDefenseAttack);

				continue;
			}

			if (trimLine.StartsWith("bdy"))
			{
				// bdy: kind="NORMAL" x="1" y="1" z="1" w="1" h="1" zwidth="1" wallCheck="false"
				string kind = this.GetValueFromKey(trimLine, "bdy", "kind");
				if (kind != null)
					Enum.TryParse<BdyKindEnum>(kind, true, out currentFrameToMap.kind);

				string x = this.GetValueFromKey(trimLine, "bdy", "x");
				if (x != null)
					currentFrameToMap.x_body = float.Parse(x);

				string y = this.GetValueFromKey(trimLine, "bdy", "y");
				if (y != null)
					currentFrameToMap.y_body = float.Parse(y);

				string z = this.GetValueFromKey(trimLine, "bdy", "z");
				if (z != null)
					currentFrameToMap.z_body = float.Parse(z);

				string h = this.GetValueFromKey(trimLine, "bdy", "h");
				if (h != null)
					currentFrameToMap.h_body = float.Parse(h);

				string w = this.GetValueFromKey(trimLine, "bdy", "w");
				if (w != null)
					currentFrameToMap.w_body = float.Parse(w);

				string zwidth = this.GetValueFromKey(trimLine, "bdy", "zwidth");
				if (zwidth != null)
					currentFrameToMap.zwidth_body = float.Parse(zwidth);

				continue;
			}

			if (trimLine == "" && currentFrameToMap != null)
			{
				objEntity.frames.Add(currentFrameToMap.id, currentFrameToMap);
			}
		}
	}

	public override void _Ready()
	{

		if (objEntity.type == ObjTypeEnum.CHARACTER)
			currentFrame = objEntity.frames[0];

		sprite3D = GetNode<Sprite3D>("sprite");
		GD.Print("2: " + objEntity.sprites[0].fileName);
	}

	public override void _Process(double delta)
	{
		// GD.Print(currentFrame.id);
		sprite3D.Texture = objEntity.sprites[currentFrame.textureIndex].sprite;
		sprite3D.RegionRect = spritePosition[currentFrame.textureIndex][currentFrame.pic];
		GD.Print(objEntity.sprites[0].fileName + "-" + spritePosition[currentFrame.textureIndex][currentFrame.pic]);
	}

	private string GetValueFromKey(string line, string keyLine, string key)
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

	private SpriteAtlasEntity GetSpriteAtlasFromKey(string line, string keyLine, string key)
	{
		SpriteAtlasEntity spriteAtlas = new();
		string dimension = this.GetValueFromKey(line, keyLine, key);
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
