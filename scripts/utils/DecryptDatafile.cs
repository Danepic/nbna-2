using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public static class DecryptDatafile
{
	static IEnumerator<char> EncryptionKey = "odBearBecauseHeIsVeryGoodSiuHungIsAGo".GetEnumerator();

	public static string Apply(Godot.FileAccess dataFileAccess)
	{
        string path = dataFileAccess.GetPathAbsolute();
        string text = File.ReadAllText(path, Encoding.ASCII);
		var clearText = new string(text.ToCharArray().Skip(123).ToArray());
        GD.Print(clearText);
        return clearText;
	}
	
	static string DecryptByteSequence(IEnumerable<byte> byteStream)
    {
        EncryptionKey.Reset();
        return new string(byteStream.Select(DecryptByte).ToArray());
    }

    static Func<byte, char> DecryptByte = b => (char)(b - NextEncryptionByte());

    static byte NextEncryptionByte()
    {
        if (!EncryptionKey.MoveNext())
        {
            EncryptionKey.Reset();
            EncryptionKey.MoveNext();
        }
        return (byte)EncryptionKey.Current;
    }
}