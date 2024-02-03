using Godot;
using System;
using System.Collections.Generic;


public class ObjEntity
{
    public ObjTypeEnum type;

    public HeaderDataEntity header;

    public StatsEntity stats;

    public Dictionary<int, SpriteAtlasEntity> sprites;

    public Dictionary<int, FrameEntity> frames;

    public override string ToString()
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
