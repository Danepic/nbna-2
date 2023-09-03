using Godot;
using System;

public class TypeDataReaderUtil
{
	public static Nullable<ObjTypeEnum> Get(string trimLine)
	{
		if (trimLine.StartsWith("type"))
		{
			string type = DataReaderUtil.GetValueFromKey(trimLine, "type", "value");
			if (type != null)
				return Enum.Parse<ObjTypeEnum>(type);
		}

		return null;
	}
}
