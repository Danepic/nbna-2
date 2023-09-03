using Godot;
using System;
using System.Collections.Generic;


public static class GetSpritesFromDataFile
{
    public static Dictionary<int, Dictionary<int, Rect2>> Apply(FileAccess dataFileAccess, ref Dictionary<int, SpriteAtlasEntity> sprites)
    {
        foreach (string line in dataFileAccess.GetAsText().Split("\n"))
        {
            string trimLine = line.Trim();

            if (trimLine.StartsWith("sprite"))
            {
                for (int i = 0; i < sprites.Count; i++)
                {
                    SpriteAtlasEntity spriteAtlasEnriched = GetSpriteAtlasFromKey(trimLine, "sprite", (i + 1).ToString());

                    if (spriteAtlasEnriched != null)
                    {
                        sprites[i].textureIndex = i;
                        sprites[i].pixelSize = spriteAtlasEnriched.pixelSize;
                        sprites[i].quantityPerRowAndColumn = spriteAtlasEnriched.quantityPerRowAndColumn;
                        sprites[i].spriteSize = spriteAtlasEnriched.spriteSize;
                        sprites[i].quantityOfSprites = spriteAtlasEnriched.quantityOfSprites;
                    }
                }
            }

            Dictionary<int, Dictionary<int, Rect2>> spritePosition = new();
            foreach (KeyValuePair<int, SpriteAtlasEntity> spriteAtlasMap in sprites)
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

            return spritePosition;
        }

        throw new Exception("Sprites not found for Object Entity!");
    }

    private static SpriteAtlasEntity GetSpriteAtlasFromKey(string line, string keyLine, string key)
    {
        SpriteAtlasEntity spriteAtlas = new();
        string dimension = GetValueFromKeyInDataFile.Apply(line, keyLine, key);
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
