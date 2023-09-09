using System;

namespace LibNoise.Unity.Operator
{
	public class Translate : ModuleBase
	{
		public Translate() : base(1)
		{
		}

		public Translate(double x, double y, double z, ModuleBase input) : base(1)
		{
			this.m_modules[0] = input;
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public double X
		{
			get
			{
				return this.m_x;
			}
			set
			{
				this.m_x = value;
			}
		}

		public double Y
		{
			get
			{
				return this.m_y;
			}
			set
			{
				this.m_y = value;
			}
		}

		public double Z
		{
			get
			{
				return this.m_z;
			}
			set
			{
				this.m_z = value;
			}
		}

		public override double GetValue(double x, double y, double z)
		{
			return this.m_modules[0].GetValue(x + this.m_x, y + this.m_y, z + this.m_z);
		}

		private double m_x = 1.0;

		private double m_y = 1.0;

		private double m_z = 1.0;
	}
}
