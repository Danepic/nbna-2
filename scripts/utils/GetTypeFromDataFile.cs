using Godot;
using System;
using System.Collections.Generic;


public static class GetTypeFromDataFile
{
    public static ObjTypeEnum Apply(FileAccess dataFileAccess)
    {
        foreach (string line in dataFileAccess.GetAsText().Split("\n"))
        {
            string trimLine = line.Trim();
            if (trimLine.StartsWith("type"))
            {
                string type = GetValueFromKeyInDataFile.Apply(trimLine, "type", "value");
                if (type != null)
                    return Enum.Parse<ObjTypeEnum>(type);
            }

        }

        throw new Exception("Type not found for Object Entity!");
    }
}
