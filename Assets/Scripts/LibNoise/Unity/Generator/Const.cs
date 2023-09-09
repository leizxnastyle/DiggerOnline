using System;

namespace LibNoise.Unity.Generator
{
	public class Const : ModuleBase
	{
		public Const() : base(0)
		{
		}

		public Const(double value) : base(0)
		{
			this.Value = value;
		}

		public double Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		public override double GetValue(double x, double y, double z)
		{
			return this.m_value;
		}

		private double m_value;
	}
}
