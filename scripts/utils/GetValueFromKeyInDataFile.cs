using Godot;
using System;

public static class GetValueFromKeyInDataFile
{
    public static string Apply(string line, string keyLine, string key)
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

    public static string ApplyV2(string line, string keyLine)
    {
        return line.Replace(keyLine + ":", "").Split(" ")[1];
    }
}
