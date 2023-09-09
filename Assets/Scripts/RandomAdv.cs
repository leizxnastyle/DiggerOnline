using System;

public class RandomAdv
{
	public RandomAdv()
	{
		ulong[] init_key = new ulong[]
		{
			291UL,
			564UL,
			837UL,
			1110UL
		};
		ulong key_length = 4UL;
		this.init_by_array(init_key, key_length);
	}

	private void init_genrand(ulong s)
	{
		RandomAdv.mt[0] = (s & (ulong)-1);
		RandomAdv.mti = 1UL;
		while (RandomAdv.mti < 624UL)
		{
			RandomAdv.mt[(int)(checked((IntPtr)RandomAdv.mti))] = 1812433253UL * checked(RandomAdv.mt[(int)((IntPtr)(unchecked(RandomAdv.mti - 1UL)))] ^ RandomAdv.mt[(int)((IntPtr)(unchecked(RandomAdv.mti - 1UL)))] >> 30) + RandomAdv.mti;
			RandomAdv.mt[(int)(checked((IntPtr)RandomAdv.mti))] &= (ulong)-1;
			RandomAdv.mti += 1UL;
		}
	}

	public void init_by_array(ulong[] init_key, ulong key_length)
	{
		this.init_genrand(19650218UL);
		ulong num = 1UL;
		ulong num2 = 0UL;
		for (ulong num3 = (624UL <= key_length) ? key_length : 624UL; num3 > 0UL; num3 -= 1UL)
		{
			RandomAdv.mt[(int)(checked((IntPtr)num))] = (RandomAdv.mt[(int)(checked((IntPtr)num))] ^ checked(RandomAdv.mt[(int)((IntPtr)(unchecked(num - 1UL)))] ^ RandomAdv.mt[(int)((IntPtr)(unchecked(num - 1UL)))] >> 30) * 1664525UL) + init_key[(int)(checked((IntPtr)num2))] + num2;
			RandomAdv.mt[(int)(checked((IntPtr)num))] &= (ulong)-1;
			num += 1UL;
			num2 += 1UL;
			if (num >= 624UL)
			{
				RandomAdv.mt[0] = RandomAdv.mt[(int)(checked((IntPtr)623L))];
				num = 1UL;
			}
			if (num2 >= key_length)
			{
				num2 = 0UL;
			}
		}
		for (ulong num3 = 623UL; num3 > 0UL; num3 -= 1UL)
		{
			RandomAdv.mt[(int)(checked((IntPtr)num))] = (RandomAdv.mt[(int)(checked((IntPtr)num))] ^ checked(RandomAdv.mt[(int)((IntPtr)(unchecked(num - 1UL)))] ^ RandomAdv.mt[(int)((IntPtr)(unchecked(num - 1UL)))] >> 30) * 1566083941UL) - num;
			RandomAdv.mt[(int)(checked((IntPtr)num))] &= (ulong)-1;
			num += 1UL;
			if (num >= 624UL)
			{
				RandomAdv.mt[0] = RandomAdv.mt[(int)(checked((IntPtr)623L))];
				num = 1UL;
			}
		}
		RandomAdv.mt[0] = (ulong)int.MinValue;
	}

	public long genrand_int31()
	{
		return (long)(this.genrand_int32() >> 1);
	}

	public double genrand_real1()
	{
		return this.genrand_int32() * 2.3283064370807974E-10;
	}

	public double genrand_real2()
	{
		return this.genrand_int32() * 2.3283064365386963E-10;
	}

	public double genrand_real3()
	{
		return (this.genrand_int32() + 0.5) * 2.3283064365386963E-10;
	}

	public double genrand_res53()
	{
		ulong num = this.genrand_int32() >> 5;
		ulong num2 = this.genrand_int32() >> 6;
		return (num * 67108864.0 + num2) * 1.1102230246251565E-16;
	}

	public ulong genrand_int32()
	{
		ulong[] array = new ulong[]
		{
			0UL,
			(ulong)-1727483681
		};
		ulong num2;
		if (RandomAdv.mti >= 624UL)
		{
			if (RandomAdv.mti == 625UL)
			{
				this.init_genrand(5489UL);
			}
			ulong num;
			for (num = 0UL; num < 227UL; num += 1UL)
			{
				checked
				{
					num2 = ((RandomAdv.mt[(int)((IntPtr)num)] & unchecked((ulong)int.MinValue)) | (RandomAdv.mt[(int)((IntPtr)(unchecked(num + 1UL)))] & 2147483647UL));
					RandomAdv.mt[(int)((IntPtr)num)] = (RandomAdv.mt[(int)((IntPtr)(unchecked(num + 397UL)))] ^ num2 >> 1 ^ array[(int)((IntPtr)(num2 & 1UL))]);
				}
			}
			while (num < 623UL)
			{
				checked
				{
					num2 = ((RandomAdv.mt[(int)((IntPtr)num)] & unchecked((ulong)int.MinValue)) | (RandomAdv.mt[(int)((IntPtr)(unchecked(num + 1UL)))] & 2147483647UL));
					RandomAdv.mt[(int)((IntPtr)num)] = (RandomAdv.mt[(int)((IntPtr)(unchecked(num - 227UL)))] ^ num2 >> 1 ^ array[(int)((IntPtr)(num2 & 1UL))]);
				}
				num += 1UL;
			}
			num2 = ((RandomAdv.mt[(int)(checked((IntPtr)623L))] & (ulong)int.MinValue) | (RandomAdv.mt[0] & 2147483647UL));
			checked
			{
				RandomAdv.mt[(int)((IntPtr)623L)] = (RandomAdv.mt[(int)((IntPtr)396L)] ^ num2 >> 1 ^ array[(int)((IntPtr)(num2 & 1UL))]);
				RandomAdv.mti = 0UL;
			}
		}
		ulong[] array2 = RandomAdv.mt;
		ulong num3 = RandomAdv.mti;
		RandomAdv.mti = num3 + 1UL;
		num2 = array2[(int)(checked((IntPtr)num3))];
		num2 ^= num2 >> 11;
		num2 ^= (num2 << 7 & (ulong)-1658038656);
		num2 ^= (num2 << 15 & (ulong)-272236544);
		return num2 ^ num2 >> 18;
	}

	public int RandomRange(int lo, int hi)
	{
		return Math.Abs((int)this.genrand_int32() % (hi - lo + 1)) + lo;
	}

	private const ulong N = 624UL;

	private const ulong M = 397UL;

	private const ulong MATRIX_A = 2567483615UL;

	private const ulong UPPER_MASK = 2147483648UL;

	private const ulong LOWER_MASK = 2147483647UL;

	private const uint DEFAULT_SEED = 4357u;

	private static ulong[] mt = new ulong[625L];

	private static ulong mti = 625UL;
}
