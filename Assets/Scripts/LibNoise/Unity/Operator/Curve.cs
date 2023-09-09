using System;
using System.Collections.Generic;
using UnityEngine;

namespace LibNoise.Unity.Operator
{
	public class Curve : ModuleBase
	{
		public Curve() : base(1)
		{
		}

		public Curve(ModuleBase input) : base(1)
		{
			this.m_modules[0] = input;
		}

		public int ControlPointCount
		{
			get
			{
				return this.m_data.Count;
			}
		}

		public List<KeyValuePair<double, double>> ControlPoints
		{
			get
			{
				return this.m_data;
			}
		}

		public void Add(double input, double output)
		{
			KeyValuePair<double, double> item = new KeyValuePair<double, double>(input, output);
			if (!this.m_data.Contains(item))
			{
				this.m_data.Add(item);
			}
			this.m_data.Sort((KeyValuePair<double, double> lhs, KeyValuePair<double, double> rhs) => lhs.Key.CompareTo(rhs.Key));
		}

		public void Clear()
		{
			this.m_data.Clear();
		}

		public override double GetValue(double x, double y, double z)
		{
			double value = this.m_modules[0].GetValue(x, y, z);
			int i;
			for (i = 0; i < this.m_data.Count; i++)
			{
				if (value < this.m_data[i].Key)
				{
					break;
				}
			}
			int index = Mathf.Clamp(i - 2, 0, this.m_data.Count - 1);
			int num = Mathf.Clamp(i - 1, 0, this.m_data.Count - 1);
			int num2 = Mathf.Clamp(i, 0, this.m_data.Count - 1);
			int index2 = Mathf.Clamp(i + 1, 0, this.m_data.Count - 1);
			if (num == num2)
			{
				return this.m_data[num].Value;
			}
			double value2 = this.m_data[num].Value;
			double value3 = this.m_data[num2].Value;
			double position = (value - value2) / (value3 - value2);
			return LibNoizeUtils.InterpolateCubic(this.m_data[index].Value, this.m_data[num].Value, this.m_data[num2].Value, this.m_data[index2].Value, position);
		}

		private List<KeyValuePair<double, double>> m_data = new List<KeyValuePair<double, double>>();
	}
}
