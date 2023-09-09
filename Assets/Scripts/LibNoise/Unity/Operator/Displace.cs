using System;

namespace LibNoise.Unity.Operator
{
	public class Displace : ModuleBase
	{
		public Displace() : base(4)
		{
		}

		public Displace(ModuleBase input, ModuleBase x, ModuleBase y, ModuleBase z) : base(4)
		{
			this.m_modules[0] = input;
			this.m_modules[1] = x;
			this.m_modules[2] = y;
			this.m_modules[3] = z;
		}

		public ModuleBase X
		{
			get
			{
				return this.m_modules[1];
			}
			set
			{
				this.m_modules[1] = value;
			}
		}

		public ModuleBase Y
		{
			get
			{
				return this.m_modules[2];
			}
			set
			{
				this.m_modules[2] = value;
			}
		}

		public ModuleBase Z
		{
			get
			{
				return this.m_modules[3];
			}
			set
			{
				this.m_modules[3] = value;
			}
		}

		public override double GetValue(double x, double y, double z)
		{
			double x2 = x + this.m_modules[1].GetValue(x, y, z);
			double y2 = y + this.m_modules[1].GetValue(x, y, z);
			double z2 = z + this.m_modules[1].GetValue(x, y, z);
			return this.m_modules[0].GetValue(x2, y2, z2);
		}
	}
}
