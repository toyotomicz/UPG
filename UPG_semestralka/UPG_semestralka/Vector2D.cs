using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPG_semestralka
{
	public class Vector2D
	{
		public double X { get; }
		public double Y { get; }

		public Vector2D(double x, double y)
		{
			X = x;
			Y = y;
		}

		public double Magnitude()
		{
			return Math.Sqrt(X * X + Y * Y);
		}

		public Vector2D Normalize()
		{
			double magnitude = Magnitude();
			return new Vector2D(X / magnitude, Y / magnitude);
		}

		public static Vector2D operator +(Vector2D v1, Vector2D v2)
		{
			return new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
		}

		public static Vector2D operator *(Vector2D v, double scalar)
		{
			return new Vector2D(v.X * scalar, v.Y * scalar);
		}
		public static Vector2D operator /(Vector2D v, double scalar)
		{
			return new Vector2D(v.X / scalar, v.Y / scalar);
		}
	}
}
