using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public static class GetObjectEntityFromDataFile
{
    public static readonly string BMP_TAG_BEGIN = "<bmp_begin>";
    public static readonly string BMP_TAG_END = "<bmp_end>";

    public static ObjEntity Apply(FileAccess dataFileAccess)
    {
        string textFile = dataFileAccess.GetAsText();

        string[] bmpSplit = textFile.Split(BMP_TAG_BEGIN)[1].Split(BMP_TAG_END);

        string bmpContent = bmpSplit[0];

        string textFileWithoutBmp = bmpSplit[1];

		return new()
		{
			type = GetTypeFromDataFile.ApplyV2(bmpContent),
			sprites = GetSpritesForCacheUtil.ApplyV2(bmpContent),
			// header = GetHeaderFromDataFile.Apply(dataFileAccess),
			// stats = GetStatsFromDataFile.Apply(dataFileAccess)
		};
    }
}
