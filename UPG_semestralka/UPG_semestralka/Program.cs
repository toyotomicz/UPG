using System.Text.RegularExpressions;

namespace UPG_semestralka
{
	internal static class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			// Inicializace konfigurace aplikace.
			ApplicationConfiguration.Initialize();

			// Výchozí hodnoty.
			int scenario = 0;
			int? gridSpacingX = null; // Použijeme nullable typ pro indikaci nezadané hodnoty.
			int? gridSpacingY = null;

			// Zpracování argumentù pøíkazové øádky.
			foreach (var arg in args)
			{
				if (int.TryParse(arg, out int parsedScenario))
				{
					scenario = parsedScenario;
					if (scenario < 0 || scenario > 3)
					{
						scenario = 0;
					}
				}
				else
				{
					var match = Regex.Match(arg, @"^-g(\d+)x(\d+)$");
					if (match.Success)
					{
						gridSpacingX = int.Parse(match.Groups[1].Value);
						gridSpacingY = int.Parse(match.Groups[2].Value);
					}
				}
			}

			// Spuštìní hlavní formy s parametry.
			Application.Run(new MainForm(scenario, gridSpacingX, gridSpacingY));
		}
	}
}