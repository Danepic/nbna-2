using Godot;
using System;

public static class GetStatsFromDataFile
{
	public static StatsEntity Apply(FileAccess dataFileAccess)
	{
		foreach (string line in dataFileAccess.GetAsText().Split("\n"))
		{
			string trimLine = line.Trim();

			if (trimLine.StartsWith("stats"))
			{
				StatsEntity stats = new();

				string aggressive = GetValueFromKeyInDataFile.Apply(trimLine, "stats", "aggressive");
				if (aggressive != null)
					stats.aggressive = int.Parse(aggressive);

				string technique = GetValueFromKeyInDataFile.Apply(trimLine, "stats", "technique");
				if (technique != null)
					stats.technique = int.Parse(technique);

				string intelligent = GetValueFromKeyInDataFile.Apply(trimLine, "stats", "intelligent");
				if (intelligent != null)
					stats.intelligent = int.Parse(intelligent);

				string speed = GetValueFromKeyInDataFile.Apply(trimLine, "stats", "speed");
				if (speed != null)
					stats.speed = int.Parse(speed);

				string resistance = GetValueFromKeyInDataFile.Apply(trimLine, "stats", "resistance");
				if (resistance != null)
					stats.resistance = int.Parse(resistance);

				string stamina = GetValueFromKeyInDataFile.Apply(trimLine, "stats", "stamina");
				if (stamina != null)
					stats.stamina = int.Parse(stamina);

				return stats;
			}
		}
		throw new Exception("Stats not found for Object Entity!");
	}
}
