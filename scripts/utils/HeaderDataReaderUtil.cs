using Godot;
using System;

public class HeaderDataReaderUtil
{
	public static HeaderDataEntity Get(string trimLine)
	{
		HeaderDataEntity header = new ();
		if (trimLine.StartsWith("header"))
		{
			string name = DataReaderUtil.GetValueFromKey(trimLine, "header", "name");
			if (name != null)
				header.name = name;

			string startHp = DataReaderUtil.GetValueFromKey(trimLine, "header", "startHp");
			if (startHp != null)
				header.startHp = int.Parse(startHp);

			string startMp = DataReaderUtil.GetValueFromKey(trimLine, "header", "startMp");
			if (startMp != null)
				header.startMp = int.Parse(startMp);
		}

		return header;
	}
}
