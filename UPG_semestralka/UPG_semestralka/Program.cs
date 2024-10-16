namespace UPG_semestralka
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			ApplicationConfiguration.Initialize();

			int scenario = 0; // Výchozí hodnota.

			if (args.Length > 0)
			{
				int.TryParse(args[0], out scenario); // Pokud neplatný vstup, zùstane 0.

				if (scenario < 0 || scenario > 3)
				{
					scenario = 0; // Pokud je mimo rozsah, vrátí se k výchozí hodnotì.
				}
			}

			Application.Run(new MainForm(scenario));
		}
	}
}