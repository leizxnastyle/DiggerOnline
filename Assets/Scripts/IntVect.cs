using System;
using UnityEngine;

public struct IntVect
{
	public IntVect(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public IntVect(float x, float y, float z)
	{
		this.x = (int)x;
		this.y = (int)y;
		this.z = (int)z;
	}

	public int X
	{
		get
		{
			return this.x;
		}
	}

	public int Y
	{
		get
		{
			return this.y;
		}
	}

	public int Z
	{
		get
		{
			return this.z;
		}
	}

	public override bool Equals(object obj)
	{
		IntVect intVect = (IntVect)obj;
		return intVect.X == this.x && intVect.Y == this.y && intVect.Z == this.z;
	}

	public override int GetHashCode()
	{
		return this.x ^ this.y ^ this.z;
	}

	public override string ToString()
	{
		return string.Format("({0}, {1}, {2})", this.x, this.y, this.z);
	}

	public IntVect MultiplyComponent(IntVect right)
	{
		return new IntVect(this.x * right.x, this.y * right.y, this.z * right.z);
	}

	public bool CompareOr(IntVect right)
	{
		return this.x == right.x || this.y == right.y || this.z == right.z;
	}

	public static bool operator ==(IntVect left, IntVect right)
	{
		return left.x == right.x && left.y == right.y && left.z == right.z;
	}

	public static bool operator !=(IntVect left, IntVect right)
	{
		return !(left == right);
	}

	public static IntVect operator -(IntVect left, IntVect right)
	{
		return new IntVect(left.x - right.x, left.y - right.y, left.z - right.z);
	}

	public static IntVect operator -(IntVect a)
	{
		return new IntVect(-a.x, -a.y, -a.z);
	}

	public static IntVect operator +(IntVect left, IntVect right)
	{
		return new IntVect(left.x + right.x, left.y + right.y, left.z + right.z);
	}

	public static IntVect operator *(IntVect a, int d)
	{
		return new IntVect(a.x * d, a.y * d, a.z * d);
	}

	public static implicit operator Vector3(IntVect v)
	{
		return new Vector3((float)v.x, (float)v.y, (float)v.z);
	}

	private readonly int x;

	private readonly int y;

	private readonly int z;

	public static IntVect Zero = new IntVect(0, 0, 0);

	public static IntVect One = new IntVect(1, 1, 1);

	public static IntVect Right = new IntVect(1, 0, 0);

	public static IntVect Forward = new IntVect(0, 1, 0);

	public static IntVect Up = new IntVect(0, 0, 1);
}
