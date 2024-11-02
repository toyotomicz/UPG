using System.Text.RegularExpressions;

namespace UPG_semestralka
{
	internal static class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			ApplicationConfiguration.Initialize();

			int scenario = 0; // Výchozí hodnota pro scénáø.
			int gridSpacingX = 50; // Výchozí rozteè v ose x
			int gridSpacingY = 50; // Výchozí rozteè v ose y

			// Kontrola argumentù
			foreach (var arg in args)
			{
				if (int.TryParse(arg, out int parsedScenario))
				{
					scenario = parsedScenario; // První argument je scénáø.
					if (scenario < 0 || scenario > 3)
					{
						scenario = 0; // Pokud je mimo rozsah, vrátí se k výchozí hodnotì.
					}
				}
				else
				{
					// Rozpoznání argumentu -g<X>x<Y> pro rozteè møížky
					var match = Regex.Match(arg, @"^-g(\d+)x(\d+)$");
					if (match.Success)
					{
						gridSpacingX = int.Parse(match.Groups[1].Value);
						gridSpacingY = int.Parse(match.Groups[2].Value);
					}
				}
			}

			Application.Run(new MainForm(scenario, gridSpacingX, gridSpacingY));
		}
	}
}
