using System.Text.RegularExpressions;

namespace UPG_semestralka
{
	internal static class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			ApplicationConfiguration.Initialize();

			int scenario = 0; // V�choz� hodnota pro sc�n��.
			int gridSpacingX = 50; // V�choz� rozte� v ose x
			int gridSpacingY = 50; // V�choz� rozte� v ose y

			// Kontrola argument�
			foreach (var arg in args)
			{
				if (int.TryParse(arg, out int parsedScenario))
				{
					scenario = parsedScenario; // Prvn� argument je sc�n��.
					if (scenario < 0 || scenario > 3)
					{
						scenario = 0; // Pokud je mimo rozsah, vr�t� se k v�choz� hodnot�.
					}
				}
				else
				{
					// Rozpozn�n� argumentu -g<X>x<Y> pro rozte� m��ky
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
