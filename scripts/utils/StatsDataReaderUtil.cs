using Godot;
using System;

public class StatsDataReaderUtil
{
	public static StatsEntity Get(string trimLine)
	{
		StatsEntity stats = new ();
		if (trimLine.StartsWith("stats"))
		{
			string aggressive = DataReaderUtil.GetValueFromKey(trimLine, "stats", "aggressive");
			if (aggressive != null)
				stats.aggressive = int.Parse(aggressive);

			string technique = DataReaderUtil.GetValueFromKey(trimLine, "stats", "technique");
			if (technique != null)
				stats.technique = int.Parse(technique);

			string intelligent = DataReaderUtil.GetValueFromKey(trimLine, "stats", "intelligent");
			if (intelligent != null)
				stats.intelligent = int.Parse(intelligent);

			string speed = DataReaderUtil.GetValueFromKey(trimLine, "stats", "speed");
			if (speed != null)
				stats.speed = int.Parse(speed);

			string resistance = DataReaderUtil.GetValueFromKey(trimLine, "stats", "resistance");
			if (resistance != null)
				stats.resistance = int.Parse(resistance);

			string stamina = DataReaderUtil.GetValueFromKey(trimLine, "stats", "stamina");
			if (stamina != null)
				stats.stamina = int.Parse(stamina);
		}
		return stats;
	}
}
