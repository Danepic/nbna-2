using Godot;
using System;

public static class GetHeaderFromDataFile
{
    public static HeaderDataEntity Apply(FileAccess dataFileAccess)
    {
        foreach (string line in dataFileAccess.GetAsText().Split("\n"))
        {
            string trimLine = line.Trim();

            if (trimLine.StartsWith("header"))
            {
                HeaderDataEntity header = new();
                string name = GetValueFromKeyInDataFile.Apply(trimLine, "header", "name");
                if (name != null)
                    header.name = name;

                string startHp = GetValueFromKeyInDataFile.Apply(trimLine, "header", "startHp");
                if (startHp != null)
                    header.startHp = int.Parse(startHp);

                string startMp = GetValueFromKeyInDataFile.Apply(trimLine, "header", "startMp");
                if (startMp != null)
                    header.startMp = int.Parse(startMp);

                return header;
            }
        }

        throw new Exception("Header not found for Object Entity!");
    }
}
