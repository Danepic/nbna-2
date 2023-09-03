using Godot;
using System;
using System.Collections.Generic;


public static class GetSpritesForCacheUtil
{
    public static Dictionary<int, SpriteAtlasEntity> Apply(DirAccess spriteDir, string spritePath)
    {
        Dictionary<int, SpriteAtlasEntity> result = new();
        
        if (spriteDir == null)
        {
            return result;
        }

        int numberOfSprite = 0;
        spriteDir.ListDirBegin();
        string fileName = spriteDir.GetNext();
        while (fileName != "")
        {
            if (!spriteDir.CurrentIsDir() && fileName.EndsWith(".png"))
            {
                SpriteAtlasEntity spriteAtlas = new()
                {
                    sprite = GD.Load<Texture2D>(spritePath + "/" + fileName),
                    fileName = fileName,
                    textureIndex = numberOfSprite
                };

                result.Add(numberOfSprite, spriteAtlas);
                numberOfSprite++;
            }
            fileName = spriteDir.GetNext();
        }
        return result;
    }
}
