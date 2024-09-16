int m;

if(args.Length > 0)
{
	m = Convert.ToInt32(args[0]);
}
else
{
	Console.Write("Zadej sirku m: ");
	m = Convert.ToInt32(Console.ReadLine());
}

int n = 5 * m / 8;

Console.WriteLine($"Velikost m x n = {m} x {n}");


for (int y = 0; y < n; y++)
{
	for (int x = 0; x < m; x++)
	{
		int krX = 7 * m / 16;
		int krY = n / 2;
		int r = m * 3 / 16;

		int z = (x - krX) * (x - krX) + (y- krY) * (y - krY) - r*r;

		if (z >= 0)
		{
			Console.Write("-");
		}
		else
		{
			Console.Write("+");
		}
		
	}
	Console.WriteLine();
}